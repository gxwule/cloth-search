// ImageMatcher.h

#pragma once

#include <string>

using namespace System;

namespace Zju 
{
	namespace Image
	{
		public ref class ImageMatcher
		{
		public:
			// Extract 24-v color vector for a image.
			// Format of ingoreColors is as: 0xffffff.s
			array<int>^ ExtractColorVector(String^ imageFileName, array<int>^ ignoreColors);

			// Extract 64-v texture vector for a image.
			array<float>^ ExtractTextureVector(String^ imageFileName);

			// It should be called before get_waveletfeature.
			// It can be called just once before several "get_waveletfeature"
			int luvInit(String^ luvFileName);

			ImageMatcher();
		private:
			bool ImageMatcher::to_CharStar(String^ source, char*& target);
			bool ImageMatcher::to_string(String^ source, std::string &target);
			array<float>^ to_array(float* pf, int n);
		};
	}
}
