// ImageMatcher.h

#pragma once

//#include <string>
#include "Gabor.h"
#include "Cooccurrence.h"

using namespace System;

namespace Zju 
{
	namespace Image
	{
		public ref class ImageMatcher
		{
		public:
			// Extract 24-v color vector for a image.
			// Format of ingoreColors is as: 0xffffff.
			// Return null if failed.
			array<int>^ ExtractColorVector(String^ imageFileName, array<int>^ ignoreColors);

			// Extract n*n*n-v color vector for a image.
			array<float>^ ExtractRGBColorVector(String^ imageFileName, int n, array<int>^ ignoreColors);

			// Extract n*n*n-v HSV color vector for a image.
			array<float>^ ExtractHSVColorVector(String^ imageFileName, int n, array<int>^ ignoreColors);

			// Extract n*n*n-v HLS color vector for a image.
			array<float>^ ExtractHLSColorVector(String^ imageFileName, int n, array<int>^ ignoreColors);

			// Extract 64-v texture vector for a image. Return null if failed.
			array<float>^ ExtractTextureVector(String^ imageFileName);

			// It should be called before get_waveletfeature.
			// It can be called just once before several "get_waveletfeature"
			// Return null if failed.
			bool LuvInit(String^ luvFileName);

			//bool GaborKernelInit(String^ gaborKernelFileName);

			array<float>^ ExtractGaborVector(String^ imageFileName);

			array<float>^ ExtractCooccurrenceVector(String^ imageFileName);

			ImageMatcher();

			~ImageMatcher();
		private:
			// mark if lvuInit method called.
			bool isLuvInited;

			Gabor* pGabor;

			Cooccurrence* pCoocc;
		private:
			//bool ImageMatcher::to_CharStar(String^ source, char*& target);
			//bool ImageMatcher::to_string(String^ source, std::string &target);
			array<float>^ to_array(float* pf, int n);

			array<float>^ to_array(Gabor::Pic_GaborWL* picwl);

			array<float>^ to_array(Cooccurrence::Pic_WLType* picwl);
		};
	}
}
