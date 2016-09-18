namespace StbSharp
{
	partial class Image
	{
		private const int FAST_BITS = 9;
		private const int STBI__ZFAST_BITS = 9;

		private unsafe delegate int ReadCallback(void* user, Pointer<byte> data, long size);

		private unsafe delegate int SkipCallback(void* user, int n);

		private unsafe delegate int EofCallback(void* user);

		private unsafe delegate void idct_block_kernel(Pointer<byte> output, int out_stride, short[] data);

		private unsafe delegate void YCbCr_to_RGB_kernel(
			Pointer<byte> output, Pointer<byte> y, Pointer<byte> pcb, Pointer<byte> pcr, int count, int step);

		private unsafe delegate Pointer<byte> resample_row_hv_2_kernel(
			Pointer<byte> output, Pointer<byte> in_near, Pointer<byte> in_far, int w, int hs);

		private delegate Pointer<byte> Resampler(Pointer<byte> a, Pointer<byte> b, Pointer<byte> c, int d, int e);


		private static Pointer<sbyte> stbi__g_failure_reason;
		private static int stbi__vertically_flip_on_load;

		private static float stbi__h2l_gamma_i = 1.0f/2.2f, stbi__h2l_scale_i = 1.0f;

		private enum STBI
		{
			STBI_default = 0,
			STBI_grey = 1,
			STBI_grey_alpha = 2,
			STBI_rgb = 3,
			STBI_rgb_alpha = 4
		};

		private class stbi_io_callbacks
		{
			public ReadCallback read;
			public SkipCallback skip;
			public EofCallback eof;
		}

		private unsafe class img_comp
		{
			public int id;
			public int h, v;
			public int tq;
			public int hd, ha;
			public int dc_pred;

			public int x, y, w2, h2;
			public byte* data;
			public void* raw_data;
			private void* raw_coeff;
			public byte* linebuf;
			public short* coeff; // progressive only
			public int coeff_w, coeff_h; // number of 8x8 coefficient blocks
		}

		private class stbi__jpeg
		{
			public stbi__context s;
			public stbi__huffman[] huff_dc = new stbi__huffman[4];
			public stbi__huffman[] huff_ac = new stbi__huffman[4];
			public byte[,] dequant = new byte[4, 64];

			public ushort[,] fast_ac = new ushort[4, 1 << FAST_BITS];

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
			public int[] order = new int[4];
			public int restart_interval, todo;

// kernels
			public idct_block_kernel idct_block_kernel;
			public YCbCr_to_RGB_kernel YCbCr_to_RGB_kernel;
			public resample_row_hv_2_kernel resample_row_hv_2_kernel;

			public stbi__jpeg()
			{
				for (var i = 0; i < img_comp.Length; ++i)
				{
					img_comp[i] = new img_comp();
				}
			}
		};

		private class stbi__resample
		{
			Resampler resample;
			Pointer<byte> line0;
			Pointer<byte> line1;
			int hs;
			int vs;
			int w_lores;
			int ystep;
			int ypos;
		}

		private static Pointer<byte> stbi__malloc(long size)
		{
			return new Pointer<byte>(size);
		}

		private static void stbi_image_free(Pointer<byte> retval_from_stbi_load)
		{
		}

		private static void memcpy(Pointer<byte> a, Pointer<byte> b, long size)
		{
			for (var i = 0; i < size; ++i)
			{
				a[i] = b[i];
			}
		}

		private static void free(Pointer<byte> a)
		{
			a.Reset();
		}

		private static void memset<T>(Pointer<T> ptr, T value, long size) where T : struct
		{
			for (var i = 0; i < size; ++i)
			{
				ptr[i] = value;
			}
		}

		private static void assert(bool expr)
		{
		}
	}
}