// This is the main DLL file.

#include "stdafx.h"
#include <cv.h>
#include <highgui.h>
//#include <vcclr.h>
#include "ImageMatcher.h"
#include "GetFeature.h"

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
#pragma comment(lib, "gaborfilterdll.lib")
#pragma comment(lib, "gaborkerneldll.lib")

using namespace System::Runtime::InteropServices;

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
		array<int>^ ImageMatcher::ExtractColorVector(String^ imageFileName, array<int>^ ignoreColors)
		{
			array<int>^ v = gcnew array<int>(24) {0};

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
						++v[r/32];
						++v[g/32+8];
						++v[b/32+16];
					}
				}
			}

			cvReleaseImage( &imgRgb );

			return v;
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

		array<float>^ ImageMatcher::ExtractTextureVector(String^ imageFileName)
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
			return nullptr;
			IntPtr ip = Marshal::StringToHGlobalAnsi(imageFileName);
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

			return textureVector;
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
