// This is the main DLL file.

#include "stdafx.h"
#include <cv.h>
#include <highgui.h>
#include <vcclr.h>
#include "ImageMatcher.h"

#pragma comment(lib, "cv.lib")
#pragma comment(lib, "cxcore.lib")
#pragma comment(lib, "highgui.lib")

namespace Zju
{
	namespace Image
	{
		array<int>^ ImageMatcher::ExtractColorVector(String^ imageFileName)
		{
			array<int>^ v = gcnew array<int>(24) {0};

			IplImage* imgRgb = NULL;

			char* fileName = nullptr;
			if (!To_CharStar(imageFileName, fileName))
			{
				// error, exception should be thrown out here.
				return nullptr;
			}

			imgRgb = cvLoadImage(fileName, CV_LOAD_IMAGE_COLOR);
			delete[] fileName;

			for( int h = 0; h < imgRgb->height; ++h ) {
				for ( int w = 0; w < imgRgb->width * 3; w += 3 ) {
					BYTE B  = ((PUCHAR)(imgRgb->imageData + imgRgb->widthStep * h))[w+0];
					BYTE G = ((PUCHAR)(imgRgb->imageData + imgRgb->widthStep * h))[w+1];
					BYTE R = ((PUCHAR)(imgRgb->imageData + imgRgb->widthStep * h))[w+2];
					
					++v[R/32];
					++v[G/32+8];
					++v[B/32+16];
				}
			}

			cvReleaseImage( &imgRgb );

			return v;
		}

		// the "target" alloc in this method, the should be delete[] outside.
		bool ImageMatcher::To_CharStar(String^ source, char*& target)
		{
			pin_ptr<const wchar_t> wch = PtrToStringChars(source);
			int len = ((source->Length+1) * 2);
			target = new char[len];
			return wcstombs( target, wch, len ) != -1;
		}

		// no memory delete need outside the method.
		bool ImageMatcher::To_string(String^ source, std::string &target)
		{    
			pin_ptr<const wchar_t> wch = PtrToStringChars(source);    
			int len = ((source->Length+1) * 2);    
			char *ch = new char[len];    
			bool result = wcstombs( ch, wch, len ) != -1;    
			target = ch;    
			delete[] ch;    
			return result;
		}
	}
}
