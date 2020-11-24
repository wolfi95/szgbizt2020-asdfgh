﻿/* 
CAFF parser 
1) Reading CAFF file from file system
2) Extracting first CIFF section from CAFF file
3) Converint CIFF to BMP
*/

#define _CRT_SECURE_NO_DEPRECATE
#include "main.h"

#include <iostream>
#include <fstream>
#include <stdio.h>
#include <stdlib.h>
#include <math.h>

#define CAFF_BLOCK_LENGTH_OFFSET 1
#define CAFF_BLOCK_LENGTH_LENGTH 8
#define CAFF_BLOCK_DATA_OFFSET 9
#define CAFF_HEADER_SIZE_OFFSET 4
#define CAFF_HEADER_NUM_ANIM_OFFSET 12

#define CIFF_DURATION_FIELD_LENGTH 8
#define CIFF_HEADER_MAGIC_LENGTH 4
#define CIFF_HEADER_SIZE_LENGTH 8
#define CIFF_HEADER_CONTENT_SIZE_LENGTH 8
#define CIFF_HEADER_WIDTH_LENGTH 8
#define CIFF_HEADER_HEIGHT_LENGTH 8
#define CIFF_HEADER_CAPTION_OFFSET 36
#define BYTES_PER_PIXEL 3

#define HEADER_BLOCK 1
#define CREDITS_BLOCK 2
#define CIFF_BLOCK 3

#define BMP_FILE_HEADER_SIZE 14
#define BMP_INFO_HEADER_SIZE 40
#define NO_COMPRESION 0
#define PIXEL_RESOLUTION_BASE 11811
#define MAX_NUMBER_OF_COLORS 0
#define IMPORTANT_COLORS_ALL 0

typedef unsigned int int32;
typedef short int16;
typedef unsigned char byte;

using std::cout;
using std::endl;
using std::ifstream;
using std::ofstream;
using std::ios;

/*
    Returns the number of bytes needed for padding.
    BMP images can only handly horizontal rows when their size is the multiple of 4 bytes, so a padding might be necessary

    PROBLEM: - width * 3 > MAX_UNSIGNED_INT
*/
int getPadding(int32 width)
{
    return (width * BYTES_PER_PIXEL) % 4;
}

/*
    Returns the size of a row in the BME file in bytes.

    PROBLEM: - width * 3 + 3 > MAX_INT
*/
int getRowSizeWithPadding(int32 width)
{
    return BYTES_PER_PIXEL * width + getPadding(width);
}

/*
    Creates the File Header of the BMP file. [Signature, File size, Reserved, Data offset]

    PROBLEM: 
        - bmpFile ?
        - rowSizeWithPadding * height + BMP_FILE_HEADER_SIZE + BMP_INFO_HEADER_SIZE > MAX_UNSIGNED_INT
            - ==> (3 * width + 3) * height + 54 > MAX_UNSIGNED_INT
*/
void writeBmpFileHeader(FILE* bmpFile, int32 width, int32 height)
{
    // BM signature
    const char* BM = "BM";
    fwrite(&BM[0], 1, 1, bmpFile);
    fwrite(&BM[1], 1, 1, bmpFile);

    // file size
    int rowSizeWithPadding = getRowSizeWithPadding(width);
    int32 fileSize = rowSizeWithPadding * height + BMP_FILE_HEADER_SIZE + BMP_INFO_HEADER_SIZE;
    fwrite(&fileSize, 4, 1, bmpFile);

    // reserved
    int32 reserved = 0x0000;
    fwrite(&reserved, 4, 1, bmpFile);

    // data offset
    int32 dataOffset = BMP_FILE_HEADER_SIZE + BMP_INFO_HEADER_SIZE;
    fwrite(&dataOffset, 4, 1, bmpFile);
}

/*
    Creates the Info Header of the BMP file.

    PROBLEM:
        - bmpFile ?
        - height > MAX_INT
        - width > MAX_UNSIGNED_INT
*/
void writeBmpInfoHeader(FILE* bmpFile, int32 width, int32 height)
{
    // Info header size
    int32 infoHeaderSize = BMP_INFO_HEADER_SIZE;
    fwrite(&infoHeaderSize, 4, 1, bmpFile);

    // width and height
    //
    // Setting negated height in the header allows us to write the image pixels from TOP to BOTTOM.
    // Otherwise a BMP file needs to be written from BOTTOM to TOP
    int negativeHeight = -(int)height;
    fwrite(&width, 4, 1, bmpFile);
    fwrite(&negativeHeight, 4, 1, bmpFile);

    int16 numberOfPlanes = 1;
    fwrite(&numberOfPlanes, 2, 1, bmpFile);

    int16 bitsPerPixel = BYTES_PER_PIXEL * 8;
    fwrite(&bitsPerPixel, 2, 1, bmpFile);

    // write compression
    int32 compression = NO_COMPRESION;
    fwrite(&compression, 4, 1, bmpFile);

    // image size in bytes without padding
    int32 imageSize = width * height * BYTES_PER_PIXEL;
    fwrite(&imageSize, 4, 1, bmpFile);

    // Horizontal and vertical resolution (in pixels per meter). 11811 ~= 300 dpi
    int32 resolutionX = PIXEL_RESOLUTION_BASE;
    int32 resolutionY = PIXEL_RESOLUTION_BASE;
    fwrite(&resolutionX, 4, 1, bmpFile);
    fwrite(&resolutionY, 4, 1, bmpFile);

    // number of colors in the color palette. Default: 0
    int32 colorsUsed = MAX_NUMBER_OF_COLORS;
    fwrite(&colorsUsed, 4, 1, bmpFile);

    // the number of important colors used. 0 when every color is important
    int32 importantColors = IMPORTANT_COLORS_ALL;
    fwrite(&importantColors, 4, 1, bmpFile);
}

// Writes BMP header part to target file
void writeBmpHeader(FILE* bmpFile, int32 width, int32 height)
{
    writeBmpFileHeader(bmpFile, width, height);
    writeBmpInfoHeader(bmpFile, width, height);
}

// Appends 'padding' number of 'NULL' bytes to the end of a row.
void appendPadding(FILE* file, int padding_length)
{
    int null_padding = 0;
    fwrite(&null_padding, 1, padding_length, file);
}


/*
    Reads pixel data from a CIFF block of the CAFF file, converts it to BMP compatible format and writes it to the BMP file.

    PROBLEM: 
        - height > MAX_INT
        - width > MAX_INT
*/
void writeBmpData(FILE* bmpFile, FILE* caffFile, int32 width, int32 height)
{
    int padding = getPadding(width);
    for (int currentRow = 0; currentRow < height; currentRow++)
    {
        for (int currentPixel = 0; currentPixel < width; currentPixel++)
        {
            uint8_t red, green, blue;
            fread(&red, 1, 1, caffFile);
            fread(&green, 1, 1, caffFile);
            fread(&blue, 1, 1, caffFile);

            fwrite(&blue, 1, 1, bmpFile);
            fwrite(&green, 1, 1, bmpFile);
            fwrite(&red, 1, 1, bmpFile);
        }

        appendPadding(bmpFile, padding);
    }
}

/*  Gets CIFF file dimensions and moves to the data part in the CIFF file.

    Inputs:
        caffFile: the CAFF file we are parsing from 
    Outputs:
        width: An int64 pointer to store the width of the image in pixels
        height: An int64 pointer to store the height of the image in pixels

    PROBLEM:
        - 
*/
void getCiffDimensions(FILE* caffFile, int64_t* width, int64_t* height)
{
    int64_t ciffHeaderLength;

    fseek(caffFile, CIFF_HEADER_MAGIC_LENGTH, SEEK_CUR);
    fread(&ciffHeaderLength, sizeof(int64_t), 1, caffFile);
    int64_t contentSize;
    fread(&contentSize, sizeof(int64_t), 1, caffFile);
    fread(width, sizeof(int64_t), 1, caffFile);
    fread(height, sizeof(int64_t), 1, caffFile);

    cout << "width: " << *width << ", height: " << *height << ", content size: " << contentSize << endl; // remove

    // move to the END of CIFF header
    cout << "ciffHeaderLength - CIFF_HEADER_CAPTION_OFFSET: " << ciffHeaderLength - CIFF_HEADER_CAPTION_OFFSET << endl; // remove
    fseek(caffFile, ciffHeaderLength - CIFF_HEADER_CAPTION_OFFSET, SEEK_CUR);
}

// Reads next byte from caffFile assuming its a blocktype field and returns its value.
char readBlockType(FILE* caffFile)
{
    char blockType;
    fread(&blockType, 1, 1, caffFile);
    int numericBlockType = (int)blockType;
    if (numericBlockType != HEADER_BLOCK && numericBlockType != CREDITS_BLOCK && numericBlockType != CIFF_BLOCK)
    {
        throw "Invalid block type";
    }
    cout << "blocktype: " << numericBlockType << endl;

    return blockType;
}

void checkWidthAndHeight(int64_t width, int64_t height) {
    if (width == 0 || height == 0) {
        throw "Width or height is 0. Can not generate preview.";
    }

    const int64_t MAX_WIDTH = 2147483647;
    const int64_t MAX_HEIGHT = 715827881;
    const int64_t MAX_WIDTH_TIMES_HEIGTH = 1431655746;

    
    if (width > MAX_WIDTH || height > MAX_HEIGHT || width > MAX_WIDTH_TIMES_HEIGTH / height + 1) {
        std::cerr << "width: " << width << ", height: " << height << endl;
        throw "Can not generate bmp file with the given CAFF's width and height dimensions.";
    }
}

/*
    Parses a BMP image from the first CIFF block of the CAFF file.

    PROBLEM: - opening CAFF, BMP files
*/
void parseCaff(const char* caffFileName, const char* bmpFileName)
{
    //Open the file for reading in binary mode
    FILE* caffFile = fopen(caffFileName, "rb");
    if (caffFile == NULL)
    {
        throw "Couldn't open CAFF file.";
    }

    // read blocktype
    char blockType = readBlockType(caffFile);

    // Read file untill first CIFF block
    while ((int)blockType != CIFF_BLOCK)
    {
        // read length
        int64_t blockLength;
        fread(&blockLength, sizeof(int64_t), 1, caffFile);
        cout << "block length: " << blockLength << endl;

        //move to next block
        fseek(caffFile, blockLength, SEEK_CUR);

        // read next blocktype
        blockType = readBlockType(caffFile);
    }
    
    // read CIFF block length
    int64_t ciffBlockLength;
    fread(&ciffBlockLength, sizeof(int64_t), 1, caffFile);
    cout << "CIFF block length: " << ciffBlockLength << endl;

    // move to CIFF file
    fseek(caffFile, CIFF_DURATION_FIELD_LENGTH, SEEK_CUR);

    int64_t width;
    int64_t height;
    getCiffDimensions(caffFile, &width, &height);
    checkWidthAndHeight(width, height);

    FILE* bmpFile = fopen(bmpFileName, "wb");
    if (bmpFile == NULL)
    {
        throw "Couldn't open BMP file for writing.";
    }
    writeBmpHeader(bmpFile, width, height);
    writeBmpData(bmpFile, caffFile, width, height);

    fclose(bmpFile);
    fclose(caffFile);
}

bool endsWith(const char* str, const char* suffix)
{
    if (!str || !suffix)
        return false;
    size_t lenstr = strlen(str);
    size_t lensuffix = strlen(suffix);
    if (lensuffix > lenstr)
        return false;
    return strncmp(str + lenstr - lensuffix, suffix, lensuffix) == 0;
}

void checkArguments(int argc, char* argv[]) {
    if (argc != 3) {
        throw "The parser anticipates exactly 2 arguments";
    }

    char* inputFileName = argv[1];
    char* outputFileName = argv[2];
    if (!endsWith(inputFileName, ".caff")) {
        throw "input has to be a .caff file.";
    }
    if (!endsWith(outputFileName, ".bmp")) {
        throw "output has to be a .bmp file.";
    }

    size_t inputFileNameLength = std::strlen(inputFileName);
    size_t outputFileNameLength = std::strlen(outputFileName);
    cout << "Input file name length: " << inputFileNameLength << ", output File name length: " << outputFileNameLength << endl;
    if (inputFileNameLength < 6 || outputFileNameLength < 5) {
        throw "file names must be at least 1 character long (without the extension).";
    }
    if (inputFileNameLength > 100 || outputFileNameLength > 100) {
        throw "file names must be at most 100 characters long.";
    }
}

int main(int argc, char* argv[])
{
    try {
        checkArguments(argc, argv);
        const char* inputFileName = argv[1];
        const char* outputFileName = argv[2];
        
        parseCaff(inputFileName, outputFileName);
    }
    catch (const char* msg) {
        std::cerr << msg << endl;  // TODO: we should return the error to the backend later
        return 1;
    }
    catch (...) {
        std::cerr << "default exception: unknown problem occured during parsing." << endl;
        return 1;
    }

	return 0;
}


int  Test(int i) {

    return 2 * i;

}