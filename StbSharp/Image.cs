using System;
using Sichem;

namespace StbSharp
{
	partial class Image
	{
		public static string LastError;

		public const int STBI__ZFAST_BITS = 9;

		public delegate int ReadCallback(object user, Pointer<sbyte> data, long size);

		public delegate int SkipCallback(object user, int n);

		public delegate int EofCallback(object user);

		public delegate void idct_block_kernel(Pointer<byte> output, int out_stride, Pointer<short> data);

		public delegate void YCbCr_to_RGB_kernel(
			Pointer<byte> output, Pointer<byte> y, Pointer<byte> pcb, Pointer<byte> pcr, int count, int step);

		public delegate Pointer<byte> Resampler(Pointer<byte> a, Pointer<byte> b, Pointer<byte> c, int d, int e);


		public static string stbi__g_failure_reason;
		public static int stbi__vertically_flip_on_load;

		public class stbi_io_callbacks
		{
			public ReadCallback read;
			public SkipCallback skip;
			public EofCallback eof;
		}

		public class img_comp
		{
			public int id;
			public int h, v;
			public int tq;
			public int hd, ha;
			public int dc_pred;

			public int x, y, w2, h2;
			public Pointer<byte> data;
			public Pointer<byte> raw_data;
			public Pointer<byte> raw_coeff;
			public Pointer<byte> linebuf;
			public Pointer<short> coeff; // progressive only
			public int coeff_w, coeff_h; // number of 8x8 coefficient blocks
		}

		public class stbi__jpeg
		{
			public stbi__context s;
			public readonly stbi__huffman[] huff_dc = new stbi__huffman[4];
			public readonly stbi__huffman[] huff_ac = new stbi__huffman[4];
			public readonly Pointer<byte>[] dequant;

			public readonly Pointer<short>[] fast_ac;

// sizes for components, interleaved MCUs
			public int img_h_max, img_v_max;
			public int img_mcu_x, img_mcu_y;
			public int img_mcu_w, img_mcu_h;

// definition of jpeg image component
			public img_comp[] img_comp = new img_comp[4];

			public uint code_buffer; // jpeg entropy-coded buffer
			public int code_bits; // number of valid bits
			public byte marker; // marker seen while filling entropy buffer
			public int nomore; // flag if we saw a marker so must stop

			public int progressive;
			public int spec_start;
			public int spec_end;
			public int succ_high;
			public int succ_low;
			public int eob_run;
			public int rgb;

			public int scan_n;
			public readonly int[] order = new int[4];
			public int restart_interval, todo;

// kernels
			public idct_block_kernel idct_block_kernel;
			public YCbCr_to_RGB_kernel YCbCr_to_RGB_kernel;
			public Resampler resample_row_hv_2_kernel;

			public stbi__jpeg()
			{
				for (var i = 0; i < 4; ++i)
				{
					huff_ac[i] = new stbi__huffman();
					huff_dc[i] = new stbi__huffman();
				}

				for (var i = 0; i < img_comp.Length; ++i)
				{
					img_comp[i] = new img_comp();
				}

				fast_ac = new Pointer<short>[4];
				for (var i = 0; i < fast_ac.Length; ++i)
				{
					fast_ac[i] = new Pointer<short>(1 << STBI__ZFAST_BITS);
				}

				dequant = new Pointer<byte>[4];
				for (var i = 0; i < dequant.Length; ++i)
				{
					dequant[i] = new Pointer<byte>(64);
				}
			}
		};

		public class stbi__resample
		{
			public Resampler resample;
			public Pointer<byte> line0;
			public Pointer<byte> line1;
			public int hs;
			public int vs;
			public int w_lores;
			public int ystep;
			public int ypos;
		}

		private static Pointer<byte> stbi__malloc(int size)
		{
			return new Pointer<byte>(size);
		}

		private static Pointer<byte> stbi__malloc(ulong size)
		{
			return stbi__malloc((int) size);
		}

		private static Pointer<byte> stbi__malloc(long size)
		{
			return stbi__malloc((int)size);
		}

		private static Pointer<byte> malloc(ulong size)
		{
			return stbi__malloc(size);
		}

		private static int stbi__err(string str)
		{
			LastError = str;
			return 0;
		}

		private static void stbi_image_free(Pointer<byte> retval_from_stbi_load)
		{
		}

		private static void memcpy(Pointer<byte> a, Pointer<byte> b, ulong size)
		{
			for (ulong i = 0; i < size; ++i)
			{
				a[i] = b[i];
			}
		}

		private static void memcpy(Pointer<sbyte> a, Pointer<byte> b, ulong size)
		{
			for (ulong i = 0; i < size; ++i)
			{
				a[i] = (sbyte)b[i];
			}
		}

		private static void memcpy(Pointer<byte> a, Pointer<byte> b, int size)
		{
			for (var i = 0; i < size; ++i)
			{
				a[i] = b[i];
			}
		}

		private static void free<T>(Pointer<T> a)
		{
			a.Reset();
		}

		private static void memset(Pointer<int> ptr, short value, int size)
		{
			for (var i = 0; i < size; ++i)
			{
				ptr[i] = value;
			}
		}

		private static void memset(Pointer<int> ptr, int value, ulong size)
		{
			for (ulong i = 0; i < size; ++i)
			{
				ptr[i] = value;
			}
		}

		private static void memset(Pointer<ushort> ptr, ushort value, int size)
		{
			for (var i = 0; i < size; ++i)
			{
				ptr[i] = value;
			}
		}

		private static void memset(Pointer<ushort> ptr, ushort value, ulong size)
		{
			for (ulong i = 0; i < size; ++i)
			{
				ptr[i] = value;
			}
		}

		private static void memset(Pointer<byte> ptr, byte value, ulong size)
		{
			for (ulong i = 0; i < size; ++i)
			{
				ptr[i] = value;
			}
		}

		private static void memset(Pointer<byte> ptr, int value, ulong size)
		{
			for (ulong i = 0; i < size; ++i)
			{
				ptr[i] = (byte)value;
			}
		}

		private static void memset(Pointer<short> ptr, short value, int size)
		{
			for (var i = 0; i < size; ++i)
			{
				ptr[i] = value;
			}
		}

		private static void memset(Pointer<short> ptr, short value, ulong size)
		{
			for (ulong i = 0; i < size; ++i)
			{
				ptr[i] = value;
			}
		}

		private static uint _lrotl(uint x , int y)
		{
			return (x << y) | (x >> (32 - y));
		}

		private static Pointer<T> realloc<T>(Pointer<T> buf, int newSize)
		{
			buf.Realloc(newSize);

			return buf;
		}

		private static Pointer<T> realloc<T>(Pointer<T> buf, ulong newSize)
		{
			buf.Realloc((long)newSize);

			return buf;
		}

		private static int abs(int v)
		{
			return Math.Abs(v);
		}

		private static void assert(bool expr)
		{
		}
	}
}