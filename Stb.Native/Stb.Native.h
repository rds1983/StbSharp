// Stb.Native.h

#pragma once

using namespace System;
using namespace System::IO;
using namespace System::Runtime::InteropServices;

#include <stdio.h>
#include <vector>

#define STBI_NO_STDIO
#define STB_IMAGE_IMPLEMENTATION
#include "../StbSharp.Generator/StbSource/stb_image.h"

#define STBI_WRITE_NO_STDIO
#define STB_IMAGE_WRITE_IMPLEMENTATION
#include "../StbSharp.Generator/StbSource/stb_image_write.h"

#define STB_DXT_IMPLEMENTATION
#include "../StbSharp.Generator/StbSource/stb_dxt.h"

namespace StbNative {
	int read_callback(void *user, char *data, int size);
	void skip_callback(void *user, int size);
	int eof_callback(void *user);
	void write_func(void *context, void *data, int size);

	public ref class Native
	{
	public:
		static array<unsigned char>^ buffer;
		static Stream^ stream;

		// TODO: Add your methods for this class here.
		static array<unsigned char> ^ load_from_memory(array<unsigned char> ^bytes, [Out] int %x, [Out] int %y, [Out] int %comp, int req_comp)
		{
			pin_ptr<unsigned char> p = &bytes[0];

			int xx, yy, ccomp;
			const unsigned char *ptr = (const unsigned char *)p;
			void *res = stbi_load_from_memory(ptr, bytes->Length, &xx, &yy, &ccomp, req_comp);

			x = xx;
			y = yy;
			comp = ccomp;

			int c = req_comp != 0 ? req_comp : comp;
			array<unsigned char> ^result = gcnew array<unsigned char>(x * y * c);

			Marshal::Copy(IntPtr((void *)res), result, 0, result->Length);
			free(res);

			return result;
		}

		static array<unsigned char> ^ load_from_stream(Stream ^input, [Out] int %x, [Out] int %y, [Out] int %comp, int req_comp)
		{
			stream = input;
			buffer = gcnew array<unsigned char>(1024);

			stbi_io_callbacks callbacks;
			callbacks.read = read_callback;
			callbacks.skip = skip_callback;
			callbacks.eof = eof_callback;

			int xx, yy, ccomp;

			void *res = stbi_load_from_callbacks(&callbacks, nullptr, &xx, &yy, &ccomp, req_comp);

			x = xx;
			y = yy;
			comp = ccomp;

			int c = req_comp != 0 ? req_comp : comp;
			array<unsigned char> ^result = gcnew array<unsigned char>(x * y * c);

			Marshal::Copy(IntPtr((void *)res), result, 0, result->Length);
			free(res);

			stream = nullptr;
			buffer = nullptr;

			return result;
		}

		// TODO: Add your methods for this class here.
		static void save_to_stream(array<unsigned char> ^bytes, int x, int y, int comp, int type, Stream ^output)
		{
			stream = output;

			pin_ptr<unsigned char> p = &bytes[0];
			unsigned char *ptr = (unsigned char *)p;

			std::vector<float> ff;
			switch (type)
			{
				case 0:
					stbi_write_bmp_to_func(write_func, nullptr, x, y, comp, ptr);
					break;
				case 1:
					stbi_write_tga_to_func(write_func, nullptr, x, y, comp, ptr);
					break;
				case 2:
				{
					ff.resize(bytes->Length);
					for (int i = 0; i < bytes->Length; ++i)
					{
						ff[i] = (float)(bytes[i] / 255.0f);
					}

					stbi_write_hdr_to_func(write_func, nullptr, x, y, comp, &ff[0]);
					break;
				}
				case 3:
					stbi_write_png_to_func(write_func, nullptr, x, y, comp, ptr, x * comp);
					break;
			}

			stream = nullptr;
		}

		static void save_to_jpg(array<unsigned char> ^bytes, int x, int y, int comp, Stream ^output, int quality)
		{
			stream = output;

			pin_ptr<unsigned char> p = &bytes[0];
			unsigned char *ptr = (unsigned char *)p;

			stbi_write_jpg_to_func(write_func, nullptr, x, y, comp, ptr, quality);

			stream = nullptr;
		}

		static array<unsigned char> ^ compress_dxt(array<unsigned char> ^input, int w, int h, bool hasAlpha)
		{
			int osize = hasAlpha ? 16 : 8;

			array<unsigned char> ^result = gcnew array<unsigned char>((w + 3)*(h + 3) / 16 * osize);

			pin_ptr<unsigned char> ip = &input[0];
			unsigned char *rgba = ip;

			pin_ptr<unsigned char> op = &result[0];
			unsigned char *p = op;

			unsigned char block[16 * 4];
			for (int j = 0; j < w; j += 4)
			{
				int x = 4;
				for (int i = 0; i < h; i += 4)
				{
					if (j + 3 >= w) x = w - j;
					int y;
					for (y = 0; y < 4; ++y)
					{
						if (j + y >= h) break;
						memcpy(block + y * 16, rgba + w * 4 * (j + y) + i * 4, x * 4);
					}
					int y2;
					if (x < 4)
					{
						switch (x)
						{
						case 0:
							throw gcnew Exception("Unknown error");
						case 1:
							for (y2 = 0; y2 < y; ++y2)
							{
								memcpy(block + y2 * 16 + 1 * 4, block + y2 * 16 + 0 * 4, 4);
								memcpy(block + y2 * 16 + 2 * 4, block + y2 * 16 + 0 * 4, 8);
							}
							break;
						case 2:
							for (y2 = 0; y2 < y; ++y2)
								memcpy(block + y2 * 16 + 2 * 4, block + y2 * 16 + 0 * 4, 8);
							break;
						case 3:
							for (y2 = 0; y2 < y; ++y2)
								memcpy(block + y2 * 16 + 3 * 4, block + y2 * 16 + 1 * 4, 4);
							break;
						}
					}
					y2 = 0;
					for (; y < 4; ++y, ++y2)
						memcpy(block + y * 16, block + y2 * 16, 4 * 4);
					stb_compress_dxt_block(p, block, hasAlpha ? 1 : 0, 10);
					p += hasAlpha ? 16 : 8;
				}
			}

			return result;
		}
	};

	int read_callback(void *user, char *data, int size)
	{
		if (size > Native::buffer->Length) {
			Native::buffer = gcnew array<unsigned char>(size * 2);
		}

		int res = Native::stream->Read(Native::buffer, 0, size);

		Marshal::Copy(Native::buffer, 0, IntPtr(data), res);

		return res;
	}

	void skip_callback(void *user, int size)
	{
		Native::stream->Seek(size, SeekOrigin::Current);
	}

	int eof_callback(void *user)
	{
		return Native::stream->CanRead ? 1 : 0;
	}

	void write_func(void *context, void *data, int size)
	{
		unsigned char *bptr = (unsigned char *)data;
		for (int i = 0; i < size; ++i)
		{
			Native::stream->WriteByte(*bptr);
			++bptr;
		}
	}
}
