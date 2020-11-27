/* 
CAFF parser 
1) Reads CAFF file from file system
2) Finds first CIFF block
3) Converts CIFF block to BMP file
*/

#define _CRT_SECURE_NO_DEPRECATE
#include "main.h"

#include <iostream>
#include <fstream>
#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include <ctime>


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

#define LOGFILE "parser_log.txt"

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
*/
int getPadding(int32 width)
{
    return (width * BYTES_PER_PIXEL) % 4;
}

/*
    Returns the size of a row in the BME file in bytes.
*/
int getRowSizeWithPadding(int32 width)
{
    return BYTES_PER_PIXEL * width + getPadding(width);
}

/*
    calls fread() and checks if the given amount of bytes were read successfully.
*/
size_t fread_s(void* ptr, size_t size, size_t count, FILE* stream) {
    size_t bytesRead = fread(ptr, size, count, stream);
    if (bytesRead != count) {
        throw "File reading error. Probably reached End Of File";
    }
    return bytesRead;
}

int fseek_s(FILE* stream, long int offset, int origin, int fileSize) {
    switch (origin)
    {
    case SEEK_CUR: {
        int currentPosition = ftell(stream);
        if (offset > fileSize - currentPosition)
            throw "Error: moving file pointer over the end of the file";
    }
        break;
    case SEEK_SET:
        if (offset > fileSize)
            throw "Error: moving file pointer over the end of the file";
        break;
    default:
        break;
    }
    return fseek(stream, offset, origin);
}

/*
    Creates the File Header of the BMP file. [Signature, File size, Reserved, Data offset]
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
*/
void writeBmpData(FILE* bmpFile, FILE* caffFile, int32 width, int32 height)
{
    int padding = getPadding(width);
    for (int currentRow = 0; currentRow < height; currentRow++)
    {
        for (int currentPixel = 0; currentPixel < width; currentPixel++)
        {
            uint8_t red, green, blue;
            fread_s(&red, 1, 1, caffFile);
            fread_s(&green, 1, 1, caffFile);
            fread_s(&blue, 1, 1, caffFile);

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
*/
void getCiffDimensions(FILE* caffFile, int64_t* width, int64_t* height, int fileSize)
{
    int64_t ciffHeaderLength;

    fseek_s(caffFile, CIFF_HEADER_MAGIC_LENGTH, SEEK_CUR, fileSize);
    fread_s(&ciffHeaderLength, sizeof(int64_t), 1, caffFile);
    int64_t contentSize;
    fread_s(&contentSize, sizeof(int64_t), 1, caffFile);
    fread_s(width, sizeof(int64_t), 1, caffFile);
    fread_s(height, sizeof(int64_t), 1, caffFile);

    cout << "width: " << *width << ", height: " << *height << ", content size: " << contentSize << endl; // remove

    // move to the END of CIFF header
    cout << "ciffHeaderLength - CIFF_HEADER_CAPTION_OFFSET: " << ciffHeaderLength - CIFF_HEADER_CAPTION_OFFSET << endl; // remove
    fseek_s(caffFile, ciffHeaderLength - CIFF_HEADER_CAPTION_OFFSET, SEEK_CUR, fileSize);
}

// Reads next byte from caffFile assuming its a blocktype field and returns its value.
char readBlockType(FILE* caffFile)
{
    char blockType;
    fread_s(&blockType, 1, 1, caffFile);
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
*/
void parseCaff(const char* caffFileName, const char* bmpFileName)
{
    //Open the file for reading in binary mode
    FILE* caffFile = fopen(caffFileName, "rb");
    if (caffFile == NULL)
    {
        throw "Couldn't open CAFF file.";
    }

    // caff file size
    fseek(caffFile, 0L, SEEK_END);
    int caffFileSize = ftell(caffFile);
    fseek(caffFile, 0L, SEEK_SET);

    // read blocktype
    char blockType = readBlockType(caffFile);

    // Read file untill first CIFF block
    while ((int)blockType != CIFF_BLOCK)
    {
        // read length
        int64_t blockLength;
        fread_s(&blockLength, sizeof(int64_t), 1, caffFile);
        cout << "block length: " << blockLength << endl;

        //move to next block
        fseek_s(caffFile, blockLength, SEEK_CUR, caffFileSize);

        // read next blocktype
        blockType = readBlockType(caffFile);
    }
    
    // read CIFF block length
    int64_t ciffBlockLength;
    fread_s(&ciffBlockLength, sizeof(int64_t), 1, caffFile);
    cout << "CIFF block length: " << ciffBlockLength << endl;

    // move to CIFF file
    fseek_s(caffFile, CIFF_DURATION_FIELD_LENGTH, SEEK_CUR, caffFileSize);

    int64_t width;
    int64_t height;
    getCiffDimensions(caffFile, &width, &height, caffFileSize);
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
    if (inputFileNameLength > 200 || outputFileNameLength > 200) {
        throw "file names must be at most 200 characters long.";
    }
}

char* readFileBytes(const char* fileName)
{
    FILE* f = fopen(fileName, "rb");
    fseek(f, 0, SEEK_END);
    long length = ftell(f);
    char* fileBytes = (char*) malloc(length);   //TODO: free - lehet kell egy freePointer dll endpoint is
    fseek(f, 0, SEEK_SET);
    fread(fileBytes, 1, length, f);
    fclose(f);

    return fileBytes;
}

void checkCaffName(const char* caffName) {
    if (!endsWith(caffName, ".caff")) {
        throw "input has to be a .caff file.";
    }
}

std::string getDateString() {
    time_t rawtime;
    struct tm* timeinfo;
    char buffer[80];

    time(&rawtime);
    timeinfo = localtime(&rawtime);

    strftime(buffer, sizeof(buffer), "%d-%m-%Y %H:%M:%S", timeinfo);
    std::string datetimeString(buffer);

    return datetimeString;
}

void logMessage(const char* msg) {
    FILE* logFile;
    logFile = fopen(LOGFILE, "a");

    std::string datetimeString = getDateString();

    fprintf(logFile, "%s\t%s\n", datetimeString.c_str(), msg);
    fclose(logFile);
}

void logMessage(const char* msg1, const char* msg2) {
    FILE* logFile;
    logFile = fopen(LOGFILE, "a");

    std::string datetimeString = getDateString();

    fprintf(logFile, "%s\t%s%s\n", datetimeString.c_str(), msg1, msg2);
    fclose(logFile);
}

void logMessage(const char* msg1, const char* msg2, const char* msg3, const char* msg4) {
    FILE* logFile;
    logFile = fopen(LOGFILE, "a");

    std::string datetimeString = getDateString();

    fprintf(logFile, "%s\t%s%s%s%s\n", datetimeString.c_str(), msg1, msg2, msg3, msg4);
    fclose(logFile);
}

/*  Returns BMP file in a char*

    Inputs:
        caffName: absolute or relative path to the CAFF file we are parsing from
    Outputs:
        fileLength: length of the returned BMP file in bytes
        retuns: the BMP file (char* ~ byte[])
*/
char* parseCaffToBmpStreamV1(const char* caffName) {
    logMessage("Logging started\n");
    logMessage("File name= ");
    logMessage(caffName);
    try {
        checkCaffName(caffName);
        const char* outputFileName = "preview2.bmp";

        parseCaff(caffName, outputFileName);

        long fileLength;    //TODO: remove
        char* returnValue = readFileBytes(outputFileName, fileLength);   //TODO: remove fileLength parameter?
        logMessage(returnValue);
        return returnValue;
    }
    catch (const char* msg) {
        logMessage("ERROR: ", msg);
    }
    catch (std::exception const& e) {
        logMessage("ERROR: ", e.what());
    }
    catch (...) {
        logMessage("ERROR: default exception: unknown problem occured during parsing.");
    }
}

int main(int argc, char* argv[])
{    
    try {
        checkArguments(argc, argv);
        const char* inputFileName = argv[1];
        const char* outputFileName = argv[2];
        logMessage("Start parsing ", inputFileName, " to ", outputFileName);

        parseCaff(inputFileName, outputFileName);
    }
    catch (const char* msg) {
        std::cerr << msg << endl;
        logMessage("ERROR: ", msg);
        return 1;
    }
    catch (std::exception const& e) {
        logMessage("ERROR: ", e.what());
        return 1;
    }
    catch (...) {
        const char* msg = "ERROR: default exception: unknown problem occured during parsing.";
        std::cerr << msg << endl;
        logMessage(msg);
        return 1;
    }

	return 0;
}