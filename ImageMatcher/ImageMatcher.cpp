// This is the main DLL file.

#include "stdafx.h"
#include <cv.h>
#include <highgui.h>
//#include <vcclr.h>
#include "ImageMatcher.h"
#include "GetFeature.h"
#include <cmath>

#pragma comment(lib, "cxcore.lib")
#pragma comment(lib, "cv.lib")
#pragma comment(lib, "highgui.lib")
#pragma comment(lib, "libmmfile.lib")
#pragma comment(lib, "libmatlb.lib")
#pragma comment(lib, "libmx.lib")
#pragma comment(lib, "libmat.lib")
#pragma comment(lib, "libmatpm.lib")
#pragma comment(lib, "sgl.lib")
#pragma comment(lib, "libmwsglm.lib")
#pragma comment(lib, "libmwservices.lib")
//#pragma comment(lib, "gaborfilterdll.lib")
//#pragma comment(lib, "gaborkerneldll.lib")

using namespace System::Runtime::InteropServices;

namespace Zju
{
	namespace Image
	{
		ImageMatcher::ImageMatcher() : isLuvInited(false)
		{
			pCoocc = new Cooccurrence();
			//pGabor = new Gabor();
		}

		ImageMatcher::~ImageMatcher()
		{
			delete pCoocc;
			//delete pGabor;
		}

		bool ImageMatcher::LuvInit(String^ luvFileName)
		{
			/*
			char* fileName = nullptr;
			if (!to_CharStar(luvFileName, fileName))
			{
				// error, exception should be thrown out here.
				return false;
			}
			*/
			IntPtr ip = Marshal::StringToHGlobalAnsi(luvFileName);
			const char* fileName = static_cast<const char*>(ip.ToPointer());

			bool re = luv_init(fileName);
			//delete[] fileName;
			Marshal::FreeHGlobal(ip);

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
		array<float>^ ImageMatcher::ExtractRGBSeparateColorVector(String^ imageFileName, int n, array<int>^ ignoreColors)
		{
			int n2 = n * 2;
			int n3 = n * 3;
			array<int>^ v = gcnew array<int>(n3) {0};

			IplImage* imgRgb = NULL;

			IntPtr ip = Marshal::StringToHGlobalAnsi(imageFileName);
			const char* fileName = static_cast<const char*>(ip.ToPointer());

			imgRgb = cvLoadImage(fileName, CV_LOAD_IMAGE_COLOR);
			Marshal::FreeHGlobal(ip);

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

			int interval = (256 + n -1) / n;
			BYTE b, g, r;
			for( int h = 0; h < imgRgb->height; ++h ) {
				for ( int w = 0; w < imgRgb->width * 3; w += 3 ) {
					b  = ((PUCHAR)(imgRgb->imageData + imgRgb->widthStep * h))[w+0];
					g = ((PUCHAR)(imgRgb->imageData + imgRgb->widthStep * h))[w+1];
					r = ((PUCHAR)(imgRgb->imageData + imgRgb->widthStep * h))[w+2];
					if (!ignoreColorSet.Contains((((int)r) << 16) + (((int)g) << 8) + b))
					{
						++v[r/interval];
						++v[g/interval+n];
						++v[b/interval+n2];
					}
				}
			}

			float total = imgRgb->height * imgRgb->width;
			cvReleaseImage( &imgRgb );

			array<float>^ re = gcnew array<float>(n3);
			for (int i=0; i<n3; ++i)
			{
				re[i] = v[i] / total;
			}

			return re;
		}

		array<float>^ ImageMatcher::ExtractRGBColorVector(String^ imageFileName, int n, array<int>^ ignoreColors)
		{
			int n2 = n * n;
			int n3 = n2 * n;

			array<int>^ v = gcnew array<int>(n3) {0};

			IplImage* imgRgb = NULL;

			IntPtr ip = Marshal::StringToHGlobalAnsi(imageFileName);
			const char* fileName = static_cast<const char*>(ip.ToPointer());

			imgRgb = cvLoadImage(fileName, CV_LOAD_IMAGE_COLOR);
			Marshal::FreeHGlobal(ip);

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

			int interval = (256 + n -1) / n;
			BYTE b, g, r;
			for( int h = 0; h < imgRgb->height; ++h ) {
				for ( int w = 0; w < imgRgb->width * 3; w += 3 ) {
					b  = ((PUCHAR)(imgRgb->imageData + imgRgb->widthStep * h))[w+0];
					g = ((PUCHAR)(imgRgb->imageData + imgRgb->widthStep * h))[w+1];
					r = ((PUCHAR)(imgRgb->imageData + imgRgb->widthStep * h))[w+2];
					if (!ignoreColorSet.Contains((((int)r) << 16) + (((int)g) << 8) + b))
					{
						++v[r/interval*n2+g/interval*n+b/interval];
					}
				}
			}

			float total = imgRgb->width * imgRgb->height;
			cvReleaseImage( &imgRgb );

			array<float>^ re = gcnew array<float>(n3);
			for (int i=0; i<n3; ++i)
			{
				re[i] = v[i] / total;
			}

			return re;
		}

		// Extract n*n*n-v HSV color vector for a image, H-[0,180], S-[0,255], V-[0,255]
		array<float>^ ImageMatcher::ExtractHSVColorVector(String^ imageFileName, int n, array<int>^ ignoreColors)
		{
			int n2 = n * n;
			int n3 = n2 * n;

			array<int>^ v = gcnew array<int>(n3) {0};

			IplImage* imgRgb = NULL;
			IplImage* imgHsv = NULL;
			CvSize imgSize;

			IntPtr ip = Marshal::StringToHGlobalAnsi(imageFileName);
			const char* fileName = static_cast<const char*>(ip.ToPointer());

			imgRgb = cvLoadImage(fileName, CV_LOAD_IMAGE_COLOR);
			Marshal::FreeHGlobal(ip);

			if (imgRgb == NULL)
			{
				return nullptr;
			}

			imgSize.width = imgRgb->width;
			imgSize.height = imgRgb->height;
			imgHsv = cvCreateImage(imgSize, imgRgb->depth, imgRgb->nChannels);
			cvCvtColor(imgRgb, imgHsv, CV_BGR2HSV);
			cvReleaseImage(&imgRgb);

			System::Collections::Generic::HashSet<int> ignoreColorSet;
			if (ignoreColors != nullptr && ignoreColors->Length > 0)
			{
				for (int i=0; i<ignoreColors->Length; ++i)
				{
					ignoreColorSet.Add(ignoreColors[i]);
				}
			}

			int ih = (181 + n - 1) / n;
			int isv = (256 + n -1) / n;
			BYTE H, L, V;
			for( int h = 0; h < imgSize.height; ++h ) {
				for ( int w = 0; w < imgSize.width * 3; w += 3 ) {
					H  = ((PUCHAR)(imgHsv->imageData + imgHsv->widthStep * h))[w+0];
					L = ((PUCHAR)(imgHsv->imageData + imgHsv->widthStep * h))[w+1];
					V = ((PUCHAR)(imgHsv->imageData + imgHsv->widthStep * h))[w+2];
					if (!ignoreColorSet.Contains((((int)H) << 16) + (((int)L) << 8) + V))
					{
						++v[H/ih*n2+L/isv*n+V/isv];
					}
				}
			}

			cvReleaseImage( &imgHsv );

			float total = imgSize.width * imgSize.height;
			array<float>^ re = gcnew array<float>(n3);
			for (int i=0; i<n3; ++i)
			{
				re[i] = v[i] / total;
			}

			return re;
		}

		array<float>^ ImageMatcher::ExtractHSVAynsColorVector(String^ imageFileName, int n, array<int>^ ignoreColors)
		{
			int nh = 11;
			int ns = 4;
			int nv = 4;
			int na = nh * ns * nv;

			array<int>^ v = gcnew array<int>(na) {0};

			IplImage* imgRgb = NULL;

			IntPtr ip = Marshal::StringToHGlobalAnsi(imageFileName);
			const char* fileName = static_cast<const char*>(ip.ToPointer());

			imgRgb = cvLoadImage(fileName, CV_LOAD_IMAGE_COLOR);
			Marshal::FreeHGlobal(ip);

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
						//++v[r/interval*n2+g/interval*n+b/interval];
						float tr = r / 255.0f;
						float tg = g / 255.0f;
						float tb = b / 255.0f;
						float tmax = max(tr, tg, tb);
						float tmin = min(tr, tg, tb);

						float tv = tmax;
						float ts = (tv - tmin) / tv;
						float th = 0;
						float tr1 = (tv - tr) / (tv - tmin);
						float tg1 = (tv - tg) / (tv - tmin);
						float tb1 = (tv - tb) / (tv - tmin);
						if (tr == tmax && tg == tmin)
						{
							th = 5 + tb1;
						}
						else if (tr == tmax && tg != tmin)
						{
							th = 1 - tg1;
						}
						else if (tg == tmax && tb == tmin)
						{
							th = 1 + tr1;
						}
						else if (tg == tmax && tb != tmin)
						{
							th = 3 - tb1;
						}
						else if (tb == tmax && tr == tmin)
						{
							th = 3 + tg1;
						}
						else
						{
							th = 5 - tr1;
						}

						++v[calcHSVIndex(th, ts, tv)];
					}
				}
			}

			float total = imgRgb->width * imgRgb->height;
			cvReleaseImage( &imgRgb );

			array<float>^ re = gcnew array<float>(na);
			for (int i=0; i<na; ++i)
			{
				re[i] = v[i] / total;
			}

			return re;
		}

		int ImageMatcher::calcHSVIndex(float h, float s, float v)
		{
			int S = 0;
			if (s >= 0 && s < 0.08)
			{
				S = 0;
			}
			else if (s >= 0.08 && s < 0.4)
			{
				S = 1;
			}
			else if (s >= 0.4 && s < 0.75)
			{
				S = 2;
			}
			else if (s >= 0.75 && s <= 1)
			{
				S = 3;
			}
			
			int V = 0;
			if (v >= 0 && v < 0.06)
			{
				V = 0;
			}
			else if (v >= 0.06 && v < 0.4)
			{
				V = 1;
			}
			else if (v >= 0.4 && v < 0.82)
			{
				V = 2;
			}
			else if (v >= 0.82 && v <= 1)
			{
				V = 3;
			}

			int H = 0;
			if ((h >= 0 && h < 0.38) || (h >= 5.25 && h < 6))
			{
				H = 0;
			}
			else if (h >= 0.38 && h < 0.83)
			{
				H = 1;
			}
			else if (h >= 0.83 && h < 1.25)
			{
				H = 2;
			}
			else if (h >= 1.25 && h < 2.58)
			{
				H = 3;
			}
			else if (h >= 2.58 && h < 3.25)
			{
				H = 4;
			}
			else if (h >= 3.25 && h < 4.58)
			{
				H = 5;
			}
			else if (h >= 4.58 && h < 4.83)
			{
				H = 6;
			}
			else if (h >= 4.83 && h < 5.25)
			{
				H = 7;
			}
			else if (V == 0 || (S == 0 && v < 0.2))
			{
				H = 8;
			}
			else if (V == 3)
			{
				H = 9;
			}
			else if (S == 0)
			{
				H = 10;
			}

			return H * 16 + S * 4 + V;
		}

		// Extract n*n*n-v HLS color vector for a image.
		array<float>^ ImageMatcher::ExtractHLSColorVector(String^ imageFileName, int n, array<int>^ ignoreColors)
		{
			int n2 = n * n;
			int n3 = n2 * n;

			array<int>^ v = gcnew array<int>(n3) {0};

			IplImage* imgRgb = NULL;
			IplImage* imgHls = NULL;
			CvSize imgSize;

			IntPtr ip = Marshal::StringToHGlobalAnsi(imageFileName);
			const char* fileName = static_cast<const char*>(ip.ToPointer());

			imgRgb = cvLoadImage(fileName, CV_LOAD_IMAGE_COLOR);
			Marshal::FreeHGlobal(ip);

			if (imgRgb == NULL)
			{
				return nullptr;
			}

			imgSize.width = imgRgb->width;
			imgSize.height = imgRgb->height;
			imgHls = cvCreateImage(imgSize, imgRgb->depth, imgRgb->nChannels);
			cvCvtColor(imgRgb, imgHls, CV_BGR2HLS);
			cvReleaseImage(&imgRgb);

			System::Collections::Generic::HashSet<int> ignoreColorSet;
			if (ignoreColors != nullptr && ignoreColors->Length > 0)
			{
				for (int i=0; i<ignoreColors->Length; ++i)
				{
					ignoreColorSet.Add(ignoreColors[i]);
				}
			}

			int ih = (181 + n - 1) / n;
			int ils = (256 + n -1) / n;
			BYTE H, L, S;
			for( int h = 0; h < imgSize.height; ++h ) {
				for ( int w = 0; w < imgSize.width * 3; w += 3 ) {
					H  = ((PUCHAR)(imgHls->imageData + imgHls->widthStep * h))[w+0];
					L = ((PUCHAR)(imgHls->imageData + imgHls->widthStep * h))[w+1];
					S = ((PUCHAR)(imgHls->imageData + imgHls->widthStep * h))[w+2];
					if (!ignoreColorSet.Contains((((int)H) << 16) + (((int)L) << 8) + S))
					{
						++v[H/ih*n2+L/ils*n+S/ils];
					}
				}
			}

			cvReleaseImage( &imgHls );

			float total = imgSize.width * imgSize.height;
			array<float>^ re = gcnew array<float>(n3);
			for (int i=0; i<n3; ++i)
			{
				re[i] = v[i] / total;
			}

			return re;
		}

		array<float>^ ImageMatcher::ExtractDaubechiesWaveletVector(String^ imageFileName)
		{
			if (!isLuvInited)
			{
				return nullptr;
			}

			IntPtr ip = Marshal::StringToHGlobalAnsi(imageFileName);
			const char* fileName = static_cast<const char*>(ip.ToPointer());

			int n = DIM * DIM;
			float* pVector = new float[n];
			//memset(pVector, 0.0f, sizeof(float) * n);
			bool re = false;
			try 
			{
				re = get_waveletfeature(fileName, pVector);
			}
			catch (Exception^ e)
			{
				// TODO do some log
				Marshal::FreeHGlobal(ip);
				delete[] pVector;
				return nullptr;
			}

			Marshal::FreeHGlobal(ip);

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
			//return nullptr;
			/*IntPtr ip = Marshal::StringToHGlobalAnsi(imageFileName);
			const char* fileName = static_cast<const char*>(ip.ToPointer());

			Gabor::Pic_GaborWL* picwl = new Gabor::Pic_GaborWL;
			int re = 0;
			try 
			{
				re = pGabor->OnWenLi(fileName, picwl);
			}
			catch (Exception^ e)
			{
				// TODO do some log
				Marshal::FreeHGlobal(ip);
				delete[] picwl;
				return nullptr;
			}

			Marshal::FreeHGlobal(ip);

			array<float>^ textureVector = nullptr;
			if (re == 0)
			{	// success
				textureVector = to_array(picwl);
			}
			delete picwl;

			return textureVector;*/
			return nullptr;
		}

		array<float>^ ImageMatcher::ExtractCooccurrenceVector(String^ imageFileName)
		{
			IntPtr ip = Marshal::StringToHGlobalAnsi(imageFileName);
			const char* fileName = static_cast<const char*>(ip.ToPointer());

			Cooccurrence::Pic_WLType* picwl = new Cooccurrence::Pic_WLType;
			int re = 0;
			try 
			{
				re = pCoocc->OnWenLi(fileName, picwl);
			}
			catch (Exception^ e)
			{
				// TODO do some log
				Marshal::FreeHGlobal(ip);
				delete[] picwl;
				return nullptr;
			}

			Marshal::FreeHGlobal(ip);

			array<float>^ textureVector = nullptr;
			if (re == 0)
			{	// success
				textureVector = to_array(picwl);
			}
			delete picwl;

			return textureVector;
		}

		/*
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

		/*array<float>^ ImageMatcher::to_array(Gabor::Pic_GaborWL* picwl)
		{
			int n = GABOR_TEXTURE_SIZE;
			array<float>^ textureVector = gcnew array<float>(n);
			for (int i=0; i<n; ++i)
			{
				textureVector[i] = (float)picwl->wenli[i];
			}

			return textureVector;
		}*/

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
