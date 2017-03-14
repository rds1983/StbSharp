// Stb.Native.h

#pragma once

using namespace System;
using namespace System::Runtime::InteropServices;

#define STB_IMAGE_IMPLEMENTATION

#include "stb_image.h"

#define STBI_WRITE_NO_STDIO
#define STB_IMAGE_WRITE_IMPLEMENTATION

#include "stb_image_write.h"

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

		// TODO: Add your methods for this class here.
		static array<unsigned char> ^ save_to_memory(array<unsigned char> ^bytes, int x, int y, int comp)
		{
			pin_ptr<unsigned char> p = &bytes[0];
			unsigned char *ptr = (unsigned char *)p;

			int len;
			unsigned char *png = stbi_write_png_to_mem(ptr, x * comp, x, y, comp, &len);

			array<unsigned char> ^result = gcnew array<unsigned char>(len);
			for (int i = 0; i < result->Length; ++i)
			{
				result[i] = png[i];
			}

			return result;
		}
	};
}
