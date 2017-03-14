using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Sichem;

namespace StbSharp
{
	partial class Stb
	{
		public static int stbi_write_tga_with_rle = 1;

		public unsafe delegate int WriteCallback(void* context, void* data, long size);

		public unsafe class stbi__write_context
		{
			public WriteCallback func;
			public void* context;
		}

		public static unsafe void stbi__start_write_callbacks(stbi__write_context s, WriteCallback c, void* context)
		{
			s.func = c;
			s.context = context;
		}

		public static unsafe void stbiw__writefv(stbi__write_context s, string fmt, params object[] v)
		{
			var vindex = 0;
			for (var i = 0; i < fmt.Length; ++i)
			{
				var c = fmt[i];
				switch (c)
				{
					case ' ':
						break;
					case '1':
					{
						var x = (byte) ((int) v[vindex++] & 0xff);
						s.func(s.context, &x, 1);
						break;
					}
					case '2':
					{
						var x = (int) v[vindex++];
						var b = new ArrayPointer<byte>(2);
						((byte*) b.Pointer)[0] = (byte) ((x) & 0xff);
						((byte*) b.Pointer)[1] = (byte) ((x >> 8) & 0xff);
						s.func(s.context, ((byte*) b.Pointer), 2);
						break;
					}
					case '4':
					{
						var x = (int) v[vindex++];
						var b = new ArrayPointer<byte>(4);
						((byte*) b.Pointer)[0] = (byte) (x & 0xff);
						((byte*) b.Pointer)[1] = (byte) ((x >> 8) & 0xff);
						((byte*) b.Pointer)[2] = (byte) ((x >> 16) & 0xff);
						((byte*) b.Pointer)[3] = (byte) ((x >> 24) & 0xff);
						s.func(s.context, ((byte*) b.Pointer), 4);
						break;
					}
				}
			}
		}

		public static void stbiw__writef(stbi__write_context s, string fmt, params object[] v)
		{
			stbiw__writefv(s, fmt, v);
		}

		public static unsafe int stbiw__outfile(stbi__write_context s, int rgb_dir, int vdir, int x, int y, int comp,
			int expand_mono, void* data, int alpha, int pad, string fmt, params object[] v)
		{
			if ((y < 0) || (x < 0))
			{
				return 0;
			}

			stbiw__writefv(s, fmt, v);
			stbiw__write_pixels(s, rgb_dir, vdir, x, y, comp, data, alpha, pad, expand_mono);
			return 1;
		}

		public static unsafe int stbi_write_bmp_to_func(WriteCallback func,
			void* context,
			int x,
			int y,
			int comp,
			void* data
			)
		{
			var s = new stbi__write_context();
			stbi__start_write_callbacks(s, func, context);
			return stbi_write_bmp_core(s, x, y, comp, data);
		}

		public static unsafe int stbi_write_tga_to_func(WriteCallback func,
			void* context,
			int x,
			int y,
			int comp,
			void* data
			)
		{
			var s = new stbi__write_context();
			stbi__start_write_callbacks(s, func, context);
			return stbi_write_tga_core(s, x, y, comp, data);
		}

		public static unsafe int stbi_write_hdr_to_func(WriteCallback func,
			void* context,
			int x,
			int y,
			int comp,
			float* data
			)
		{
			stbi__write_context s = new stbi__write_context();
			stbi__start_write_callbacks(s, func, context);
			return stbi_write_hdr_core(s, x, y, comp, data);
		}

		public static unsafe int stbi_write_png_to_func(WriteCallback func,
			void* context,
			int x,
			int y,
			int comp,
			void* data,
			int stride_bytes
			)
		{
			int len;
			var png = stbi_write_png_to_mem((byte*) (data), stride_bytes, x, y, comp, &len);
			if ((png) == ((byte*) ((void*) (0)))) return 0;
			func(context, png, len);
			free(png);
			return 1;
		}

		public static unsafe int stbi_write_hdr_core(stbi__write_context s, int x, int y, int comp, float* data)
		{
			if ((y <= 0) || (x <= 0) || (data == null))
			{
				return 0;
			}

			var scratch = (byte*) (malloc((ulong) (x*4)));

			int i;
			var header = "#?RADIANCE\n# Written by stb_image_write.h\nFORMAT=32-bit_rle_rgbe\n";
			var bytes = Encoding.UTF8.GetBytes(header);
			var ptr = new ArrayPointer<byte>(bytes);
			s.func(s.context, ((sbyte*) ptr.Pointer), ptr.Size);

			var str = string.Format("EXPOSURE=          1.0000000000000\n\n-Y {0} +X {1}\n", y, x);
			bytes = Encoding.UTF8.GetBytes(str);
			ptr = new ArrayPointer<byte>(bytes);
			s.func(s.context, ((sbyte*) ptr.Pointer), ptr.Size);
			for (i = 0; i < y; i++)
			{
				stbiw__write_hdr_scanline(s, x, comp, scratch, data + comp*i*x);
			}
			free(scratch);
			return 1;
		}

		public enum ImageWriterType
		{
			Bmp,
			Tga,
			Hdr,
			Png
		}

		public static unsafe byte[] stbi_write_to_memory(byte[] bytes, int x, int y, int comp, ImageWriterType type)
		{
			byte[] result;
			using (var ms = new MemoryStream())
			{
				WriteCallback writeFunc = (context, data, size) =>
				{
					if (data == null || size <= 0)
					{
						return 0;
					}

					var b = new byte[size];
					var bptr = (byte*) data;
					for (var i = 0; i < size; ++i)
					{
						b[i] = *bptr++;
					}

					ms.Write(b, 0, b.Length);

					return (int)size;
				};

				fixed (byte* b = &bytes[0])
				{
					switch (type)
					{
						case ImageWriterType.Bmp:
							stbi_write_bmp_to_func(writeFunc, null, x, y, comp, b);
							break;
						case ImageWriterType.Tga:
							stbi_write_tga_to_func(writeFunc, null, x, y, comp, b);
							break;
						case ImageWriterType.Hdr:
						{
							var f = new ArrayPointer<float>(bytes.Length);
							var fptr = (float*) f.Pointer;
							var bptr = b;
							for (var i = 0; i < bytes.Length; ++i)
							{
								*fptr = bytes[i]/255.0f;
								fptr++;
								bptr++;
							}

							stbi_write_hdr_to_func(writeFunc, null, x, y, comp, (float*) f.Pointer);
						}
							break;
						case ImageWriterType.Png:
							stbi_write_png_to_func(writeFunc, null, x, y, comp, b, x * comp);
							break;
						default:
							throw new ArgumentOutOfRangeException("type", type, null);
					}
				}

				result = ms.ToArray();
			}

			return result;
		}
	}
}