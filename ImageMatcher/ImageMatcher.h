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
		private:
			bool ImageMatcher::To_CharStar(String^ source, char*& target);
			bool ImageMatcher::To_string(String^ source, std::string &target);
		};
	}
}
