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
			array<int>^ ExtractColorVector(String^ imageFileName);

			// Extract 64-v texture vector for a image.
			array<float>^ ExtractTextureVector(String^ imageFileName);

			// It should be called before get_waveletfeature.
			// It can be called just once before several "get_waveletfeature"
			int luvInit();

			ImageMatcher();
		private:
			bool ImageMatcher::to_CharStar(String^ source, char*& target);
			bool ImageMatcher::to_string(String^ source, std::string &target);
			array<float>^ to_array(float* pf, int n);
		};
	}
}
