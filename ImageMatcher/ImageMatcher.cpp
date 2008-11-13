// This is the main DLL file.

#include "stdafx.h"
//#include <cv.h>
#include <highgui.h>
#include <vcclr.h>
#include "ImageMatcher.h"
#include "GetFeature.h"

#define LUV_FILE_NAME "E:\\projects\\ClothSearch\\codes\\trunk\\data\\luv.dat"

#pragma comment(lib, "cxcore.lib")
#pragma comment(lib, "highgui.lib")

namespace Zju
{
	namespace Image
	{
		ImageMatcher::ImageMatcher()
		{

		}

		int ImageMatcher::luvInit()
		{
			return luv_init(LUV_FILE_NAME);
		}

		array<int>^ ImageMatcher::ExtractColorVector(String^ imageFileName)
		{
			array<int>^ v = gcnew array<int>(24) {0};

			IplImage* imgRgb = NULL;

			char* fileName = nullptr;
			if (!to_CharStar(imageFileName, fileName))
			{
				// error, exception should be thrown out here.
				return nullptr;
			}

			imgRgb = cvLoadImage(fileName, CV_LOAD_IMAGE_COLOR);
			delete[] fileName;

			BYTE b, g, r;
			for( int h = 0; h < imgRgb->height; ++h ) {
				for ( int w = 0; w < imgRgb->width * 3; w += 3 ) {
					b  = ((PUCHAR)(imgRgb->imageData + imgRgb->widthStep * h))[w+0];
					g = ((PUCHAR)(imgRgb->imageData + imgRgb->widthStep * h))[w+1];
					r = ((PUCHAR)(imgRgb->imageData + imgRgb->widthStep * h))[w+2];
					
					++v[r/32];
					++v[g/32+8];
					++v[b/32+16];
				}
			}

			cvReleaseImage( &imgRgb );

			return v;
		}

		array<float>^ ImageMatcher::ExtractTextureVector(String^ imageFileName)
		{
			char* fileName = nullptr;
			if (!to_CharStar(imageFileName, fileName))
			{
				// error, exception should be thrown out here.
				return nullptr;
			}

			int n = DIM * DIM;
			float* pVector = new float[n];
			get_waveletfeature(fileName, pVector);
			delete fileName;

			array<float>^ textureVector = to_array(pVector, n);
			delete[] pVector;

			return textureVector;
		}

		// the "target" alloc in this method, the should be delete[] outside.
		bool ImageMatcher::to_CharStar(String^ source, char*& target)
		{
			pin_ptr<const wchar_t> wch = PtrToStringChars(source);
			int len = ((source->Length+1) * 2);
			target = new char[len];
			return wcstombs( target, wch, len ) != -1;
		}

		// no memory delete need outside the method.
		bool ImageMatcher::to_string(String^ source, std::string &target)
		{    
			pin_ptr<const wchar_t> wch = PtrToStringChars(source);    
			int len = ((source->Length+1) * 2);    
			char *ch = new char[len];    
			bool result = wcstombs( ch, wch, len ) != -1;    
			target = ch;    
			delete[] ch;    
			return result;
		}

		array<float>^ ImageMatcher::to_array(float* pf, int n)
		{
			array<float>^ textureVector = gcnew array<float>(n);
			for (int i=0; i<n; ++i)
			{
				textureVector[i] = pf[i];
			}

			return textureVector;
		}
	}
}
