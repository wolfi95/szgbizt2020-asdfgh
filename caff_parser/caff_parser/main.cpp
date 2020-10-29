/* 
CAFF parser 

1) Reading CAFF file from file system
2) Extracting first CIFF section from CAFF file
3) Converint CIFF to BMP
*/

#define _CRT_SECURE_NO_DEPRECATE
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

struct BmpHeader {
	char bitmapSignatureBytes[2] = { 'B', 'M' };
	uint32_t sizeOfBitmapFile = 54 + 786432; // total size of bitmap file
	uint32_t reservedBytes = 0;
	uint32_t pixelDataOffset = 54;
} bmpHeader;

struct BmpInfoHeader {
	uint32_t sizeOfThisHeader = 40;
	int32_t width = 512; // in pixels
	int32_t height = 512; // in pixels
	uint16_t numberOfColorPlanes = 1; // must be 1
	uint16_t colorDepth = 24;
	uint32_t compressionMethod = 0;
	uint32_t rawBitmapDataSize = 0; // generally ignored
	int32_t horizontalResolution = 3780; // in pixel per meter
	int32_t verticalResolution = 3780; // in pixel per meter
	uint32_t colorTableEntries = 0;
	uint32_t importantColors = 0;
} bmpInfoHeader;

struct Pixel {
	uint8_t blue = 255;
	uint8_t green = 255;
	uint8_t red = 0;
} pixel;

int getPadding(int32 width)
{
    return (width * BYTES_PER_PIXEL) % 4;
}

int getRowSizeWithPadding(int32 width)
{
    return BYTES_PER_PIXEL * width + getPadding(width);
}

int getRowSizeWithoutPadding(int32 width)
{
    return BYTES_PER_PIXEL * width;
}

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

void writeBmpHeader(FILE* bmpFile, int32 width, int32 height)
{
    writeBmpFileHeader(bmpFile, width, height);
    writeBmpInfoHeader(bmpFile, width, height);
}

void appendPadding(FILE* file, int padding_length)
{
    int null_padding = 0;
    fwrite(&null_padding, 1, padding_length, file);
}

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

// Gets CIFF file dimensions and moves to the data part in the CIFF file.
//
//***Inputs*****
//caffFile: the CAFF file we are parsing from 
//***Outputs****
//width: An int64 pointer to store the width of the image in pixels
//height: An int64 pointer to store the height of the image in pixels
//ciffHeaderLength: An int pointer to store the number of bytes per pixel that are used in the image
void getCiffDimensions(FILE* caffFile, int64_t* width, int64_t* height)
{
    int64_t ciffHeaderLength;

    fseek(caffFile, CIFF_HEADER_MAGIC_LENGTH, SEEK_CUR);
    fread(&ciffHeaderLength, sizeof(int64_t), 1, caffFile);
    int64_t contentSize;
    fread(&contentSize, sizeof(int64_t), 1, caffFile);
    //fseek(caffFile, CIFF_HEADER_CONTENT_SIZE_LENGTH, SEEK_CUR);
    fread(width, sizeof(int64_t), 1, caffFile);
    fread(height, sizeof(int64_t), 1, caffFile);

    cout << "width: " << *width << ", height: " << *height << ", content size: " << contentSize << endl;

    // move to the END of CIFF header
    cout << "ciffHeaderLength - CIFF_HEADER_CAPTION_OFFSET: " << ciffHeaderLength - CIFF_HEADER_CAPTION_OFFSET << endl;
    fseek(caffFile, ciffHeaderLength - CIFF_HEADER_CAPTION_OFFSET, SEEK_CUR);
}

void parseCaff(const char* fileName)
{
    //Open the file for reading in binary mode
    FILE* caffFile = fopen(fileName, "rb");

    // read blocktype
    char blockType;
    fread(&blockType, 1, 1, caffFile);
    cout << "blocktype: " << (int)blockType << endl;

    while ((int)blockType != CIFF_BLOCK)
    {
        // read length
        int64_t blockLength;
        fread(&blockLength, sizeof(int64_t), 1, caffFile);
        cout << "block length: " << blockLength << endl;

        //move to next block
        fseek(caffFile, blockLength, SEEK_CUR);

        // read next blocktype
        fread(&blockType, 1, 1, caffFile);
        cout << "blocktype: " << (int)blockType << endl;
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

    FILE* bmpFile = fopen("out.bmp", "wb");
    writeBmpHeader(bmpFile, width, height);
    writeBmpData(bmpFile, caffFile, width, height);

    fclose(bmpFile);
    fclose(caffFile);
}

// BMP file creation test
void writeSampleBmp()
{
    int32 width = 2;
    int32 height = 2;
    FILE* bmpFile = fopen("sample.bmp", "wb");
    writeBmpHeader(bmpFile, width, height);
    uint8_t max = 255;
    uint8_t min = 0;

    fwrite(&max, 1, 1, bmpFile);
    fwrite(&min, 1, 1, bmpFile);
    fwrite(&min, 1, 1, bmpFile);

    fwrite(&min, 1, 1, bmpFile);
    fwrite(&max, 1, 1, bmpFile);
    fwrite(&min, 1, 1, bmpFile);

    //padding
    char null_char = 0x0000;
    fwrite(&max, 1, 1, bmpFile);
    fwrite(&max, 1, 1, bmpFile);

    fwrite(&min, 1, 1, bmpFile);
    fwrite(&min, 1, 1, bmpFile);
    fwrite(&max, 1, 1, bmpFile);

    fwrite(&max, 1, 1, bmpFile);
    fwrite(&max, 1, 1, bmpFile);
    fwrite(&max, 1, 1, bmpFile);

    //padding
    fwrite(&max, 1, 1, bmpFile);
    fwrite(&max, 1, 1, bmpFile);

    fclose(bmpFile);
}

int main()
{
    parseCaff("../../caff_files/1.caff");
	return 0;
}

