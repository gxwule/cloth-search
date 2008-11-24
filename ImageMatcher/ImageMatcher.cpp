// This is the main DLL file.

#include "stdafx.h"
#include <highgui.h>
#include <vcclr.h>
#include "ImageMatcher.h"
#include "GetFeature.h"

#pragma comment(lib, "cxcore.lib")
#pragma comment(lib, "highgui.lib")
#pragma comment(lib, "libmmfile.lib")
#pragma comment(lib, "libmatlb.lib")
#pragma comment(lib, "libmx.lib")
#pragma comment(lib, "libmat.lib")
#pragma comment(lib, "libmatpm.lib")
#pragma comment(lib, "sgl.lib")
#pragma comment(lib, "libmwsglm.lib")
#pragma comment(lib, "libmwservices.lib")
#pragma comment(lib, "gaborfilterdll.lib")
#pragma comment(lib, "gaborkerneldll.lib")

namespace Zju
{
	namespace Image
	{
		ImageMatcher::ImageMatcher() : isLuvInited(false)
		{
			pCoocc = new Cooccurrence();
			pGabor = new Gabor();
		}

		ImageMatcher::~ImageMatcher()
		{
			delete pCoocc;
			delete pGabor;
		}

		bool ImageMatcher::LuvInit(String^ luvFileName)
		{
			char* fileName = nullptr;
			if (!to_CharStar(luvFileName, fileName))
			{
				// error, exception should be thrown out here.
				return false;
			}

			bool re = luv_init(fileName);
			delete[] fileName;

			isLuvInited = true;
			return re;
		}
/*
		bool ImageMatcher::GaborKernelInit(String^ gaborKernelFileName)
		{
			char* fileName = nullptr;
			if (!to_CharStar(gaborKernelFileName, fileName))
			{
				// error, exception should be thrown out here.
				return false;
			}

			bool re = luv_init(fileName);
			delete[] fileName;

			isLuvInited = true;
			return re;
		}
*/
		array<int>^ ImageMatcher::ExtractColorVector(String^ imageFileName, array<int>^ ignoreColors)
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

			if (imgRgb == NULL)
			{
				return nullptr;
			}
			
			System::Collections::Generic::HashSet<int> ignoreColorSet;
			if (ignoreColors != nullptr && ignoreColors->Length > 0)
			{
				for (int i=0; i<ignoreColors->Length; ++i)
				{
					ignoreColorSet.Add(ignoreColors[i]);
				}
			}

			BYTE b, g, r;
			for( int h = 0; h < imgRgb->height; ++h ) {
				for ( int w = 0; w < imgRgb->width * 3; w += 3 ) {
					b  = ((PUCHAR)(imgRgb->imageData + imgRgb->widthStep * h))[w+0];
					g = ((PUCHAR)(imgRgb->imageData + imgRgb->widthStep * h))[w+1];
					r = ((PUCHAR)(imgRgb->imageData + imgRgb->widthStep * h))[w+2];
					if (!ignoreColorSet.Contains((((int)r) << 16) + (((int)g) << 8) + b))
					{
						++v[r/32];
						++v[g/32+8];
						++v[b/32+16];
					}
				}
			}

			cvReleaseImage( &imgRgb );

			return v;
		}

		array<float>^ ImageMatcher::ExtractTextureVector(String^ imageFileName)
		{
			if (!isLuvInited)
			{
				return nullptr;
			}

			char* fileName = nullptr;
			if (!to_CharStar(imageFileName, fileName))
			{
				// error, exception should be thrown out here.
				return nullptr;
			}

			int n = DIM * DIM;
			float* pVector = new float[n];
			//memset(pVector, 0.0f, sizeof(float) * n);
			bool re = get_waveletfeature(fileName, pVector);
			delete[] fileName;

			if (!re)
			{
				return nullptr;
			}

			array<float>^ textureVector = to_array(pVector, n);
			delete[] pVector;

			return textureVector;
		}

		array<float>^ ImageMatcher::ExtractGaborVector(String^ imageFileName)
		{
			char* fileName = nullptr;
			if (!to_CharStar(imageFileName, fileName))
			{
				// error, exception should be thrown out here.
				return nullptr;
			}

			Gabor::Pic_GaborWL* picwl = new Gabor::Pic_GaborWL;
			int re = pGabor->OnWenLi(fileName, picwl);
			delete[] fileName;

			array<float>^ textureVector = nullptr;
			if (re == 0)
			{	// success
				textureVector = to_array(picwl);
			}
			delete picwl;

			return textureVector;
		}

		array<float>^ ImageMatcher::ExtractCooccurrenceVector(String^ imageFileName)
		{
			char* fileName = nullptr;
			if (!to_CharStar(imageFileName, fileName))
			{
				// error, exception should be thrown out here.
				return nullptr;
			}

			Cooccurrence::Pic_WLType* picwl = new Cooccurrence::Pic_WLType;
			int re = pCoocc->OnWenLi(fileName, picwl);
			delete[] fileName;

			array<float>^ textureVector = nullptr;
			if (re == 0)
			{	// success
				textureVector = to_array(picwl);
			}
			delete picwl;

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
		/*
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
		*/

		array<float>^ ImageMatcher::to_array(float* pf, int n)
		{
			array<float>^ textureVector = gcnew array<float>(n);
			for (int i=0; i<n; ++i)
			{
				textureVector[i] = pf[i];
			}

			return textureVector;
		}

		array<float>^ ImageMatcher::to_array(Gabor::Pic_GaborWL* picwl)
		{
			int n = GABOR_TEXTURE_SIZE;
			array<float>^ textureVector = gcnew array<float>(n);
			for (int i=0; i<n; ++i)
			{
				textureVector[i] = (float)picwl->wenli[i];
			}

			return textureVector;
		}

		array<float>^ ImageMatcher::to_array(Cooccurrence::Pic_WLType* picwl)
		{
			array<float>^ textureVector = gcnew array<float>(BLOCK_TEXTURE_SIZE * Block_Total);
			for (int i=0; i<Block_Total; ++i)
			{
				for (int j=0; j<BLOCK_TEXTURE_SIZE; ++j)
				{
					textureVector[i*BLOCK_TEXTURE_SIZE+j] = picwl->wenli[i][j];
				}
			}

			return textureVector;
		}
	}
}
