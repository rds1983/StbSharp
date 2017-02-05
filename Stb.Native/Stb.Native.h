// Stb.Native.h

#pragma once

using namespace System;
using namespace System::Runtime::InteropServices;

#define STB_IMAGE_IMPLEMENTATION

#include "stb_image.h"

namespace StbNative {

	public ref class Loader
	{
	public:
		// TODO: Add your methods for this class here.
		static array<unsigned char> ^ load_from_memory(array<unsigned char> ^bytes, [Out] int %x, [Out] int %y, [Out] int %comp, int req_comp)
		{
			pin_ptr<unsigned char> p = &bytes[0];

			int xx, yy, ccomp;
			const unsigned char *ptr = (const unsigned char *)p;
			const unsigned char *res = stbi_load_from_memory(ptr, bytes->Length, &xx, &yy, &ccomp, req_comp);

			x = xx;
			y = yy;
			comp = ccomp;

			array<unsigned char> ^result = gcnew array<unsigned char>(x * y * req_comp);
			for (int i = 0; i < result->Length; ++i)
			{
				result[i] = res[i];
			}

			return result;
		}
	};
}
