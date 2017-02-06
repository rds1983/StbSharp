using System;
using Sichem;

namespace StbSharp
{
	partial class Image
	{
		public static string LastError;

		public const int STBI__ZFAST_BITS = 9;

		public unsafe delegate int ReadCallback(void* user, sbyte *data, long size);

		public unsafe delegate int SkipCallback(void* user, int n);

		public unsafe delegate int EofCallback(void *user);

		public unsafe delegate void idct_block_kernel(byte *output, int out_stride, short *data);

		public unsafe delegate void YCbCr_to_RGB_kernel(
			byte *output, byte *y, byte *pcb, byte *pcr, int count, int step);

		public unsafe delegate byte *Resampler(byte *a, byte *b, byte *c, int d, int e);

		public static string stbi__g_failure_reason;
		public static int stbi__vertically_flip_on_load;

		public class stbi_io_callbacks
		{
			public ReadCallback read;
			public SkipCallback skip;
			public EofCallback eof;
		}

		public unsafe class img_comp
		{
			public int id;
			public int h, v;
			public int tq;
			public int hd, ha;
			public int dc_pred;

			public int x, y, w2, h2;
			public byte *data;
			public void *raw_data;
			public void *raw_coeff;
			public byte *linebuf;
			public short *coeff; // progressive only
			public int coeff_w, coeff_h; // number of 8x8 coefficient blocks
		}

		public class stbi__jpeg
		{
			public stbi__context s;
			public readonly stbi__huffman[] huff_dc = new stbi__huffman[4];
			public readonly stbi__huffman[] huff_ac = new stbi__huffman[4];
			public readonly ArrayPointer<byte>[] dequant;

			public readonly ArrayPointer<short>[] fast_ac;

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
			public ArrayPointer<int> order = new ArrayPointer<int>(4);
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

				fast_ac = new ArrayPointer<short>[4];
				for (var i = 0; i < fast_ac.Length; ++i)
				{
					fast_ac[i] = new ArrayPointer<short>(1 << STBI__ZFAST_BITS);
				}

				dequant = new ArrayPointer<byte>[4];
				for (var i = 0; i < dequant.Length; ++i)
				{
					dequant[i] = new ArrayPointer<byte>(64);
				}
			}
		};

		public unsafe class stbi__resample
		{
			public Resampler resample;
			public byte *line0;
			public byte *line1;
			public int hs;
			public int vs;
			public int w_lores;
			public int ystep;
			public int ypos;
		}

		private static unsafe void* stbi__malloc(int size)
		{
			return Operations.Malloc(size);
		}

		private static unsafe void* stbi__malloc(ulong size)
		{
			return stbi__malloc((int) size);
		}

		private static unsafe void* malloc(ulong size)
		{
			return stbi__malloc(size);
		}

		private static int stbi__err(string str)
		{
			LastError = str;
			return 0;
		}

		private static unsafe void memcpy(void *a, void *b, long size)
		{
			Operations.Memcpy(a, b, size);
		}

		private static unsafe void memcpy(void* a, void* b, ulong size)
		{
			memcpy(a, b, (long)size);
		}

		private static unsafe void free(void* a)
		{
			Operations.Free(a);
		}

		private static unsafe void memset(void* ptr, int value, long size)
		{
			byte* bptr = (byte*) ptr;
			var bval = (byte) value;
			for (long i = 0; i < size; ++i)
			{
				*bptr++ = bval;
			}
		}

		private static unsafe void memset(void* ptr, int value, ulong size)
		{
			memset(ptr, value, (long)size);
		}

		private static uint _lrotl(uint x , int y)
		{
			return (x << y) | (x >> (32 - y));
		}

		private static unsafe void* realloc(void *ptr, long newSize)
		{
			return Operations.Realloc(ptr, newSize);
		}

		private static unsafe void* realloc(void* ptr, ulong newSize)
		{
			return realloc(ptr, (long)newSize);
		}

		private static int abs(int v)
		{
			return Math.Abs(v);
		}

		public static unsafe byte[] stbi_load_from_memory(byte[] bytes, out int x, out int y, out int comp, int req_comp)
		{
			byte* result;
			int xx, yy, ccomp;
			fixed (byte* b = &bytes[0])
			{
				result = stbi_load_from_memory(b, bytes.Length, &xx, &yy, &ccomp, req_comp);
			}

			x = xx;
			y = yy;
			comp = ccomp;

			if (result == null)
			{
				throw new Exception(LastError);
			}

			// Convert to array
			var bptr = result;
			var data = new byte[x*y*req_comp];
			for (var i = 0; i < x * y * req_comp; ++i)
			{
				data[i] = *bptr++;
			}

			Operations.Free(result);

			return data;
		}
	}
}