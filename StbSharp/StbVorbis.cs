using System;
using System.Runtime.InteropServices;

namespace StbSharp
{
	public unsafe partial class StbVorbis
	{
		public class Residue
		{
			public uint begin;
			public uint end;
			public uint part_size;
			public byte classifications;
			public byte classbook;
			public byte** classdata;
			public short[,] residue_books;
		}

		public class stb_vorbis
		{
			public uint sample_rate;
			public int channels;
			public uint setup_memory_required;
			public uint temp_memory_required;
			public uint setup_temp_memory_required;
			public byte* stream;
			public byte* stream_start;
			public byte* stream_end;
			public uint stream_len;
			public byte push_mode;
			public uint first_audio_page_offset;
			public ProbedPage p_first = new ProbedPage();
			public ProbedPage p_last = new ProbedPage();
			public stb_vorbis_alloc alloc = new stb_vorbis_alloc();
			public int setup_offset;
			public int temp_offset;
			public int eof;

			public int error;
			public int[] blocksize = new int[2];
			public int blocksize_0;
			public int blocksize_1;
			public int codebook_count;
			public Codebook* codebooks;
			public int floor_count;
			public ushort[] floor_types = new ushort[64];
			public Floor* floor_config;
			public int residue_count;
			public ushort[] residue_types = new ushort[64];
			public Residue[] residue_config;
			public int mapping_count;
			public Mapping* mapping;
			public int mode_count;
			public PinnedArray<Mode> mode_config = new PinnedArray<Mode>(64);
			public uint total_samples;
			public float*[] channel_buffers = new float*[16];
			public float*[] outputs = new float*[16];
			public float*[] previous_window = new float*[16];
			public int previous_length;
			public short*[] finalY = new short*[16];
			public uint current_loc;
			public int current_loc_valid;
			public float*[] A = new float*[2];
			public float*[] B = new float*[2];
			public float*[] C = new float*[2];
			public float*[] window = new float*[2];
			public ushort*[] bit_reverse = new ushort*[2];
			public uint serial;
			public int last_page;
			public int segment_count;
			public PinnedArray<byte> segments = new PinnedArray<byte>(255);
			public byte page_flag;
			public byte bytes_in_seg;
			public byte first_decode;
			public int next_seg;
			public int last_seg;
			public int last_seg_which;
			public uint acc;
			public int valid_bits;
			public int packet_bytes;
			public int end_seg_with_known_loc;
			public uint known_loc_for_packet;
			public int discard_samples_deferred;
			public uint samples_output;
			public int page_crc_tests;
			public CRCscan[] scan = new CRCscan[4];
			public int channel_buffer_start;
			public int channel_buffer_end;
		}

		public static sbyte[,] channel_position =
		{
			{0, 0, 0, 0, 0, 0},
			{2 | 4 | 1, 0, 0, 0, 0, 0},
			{2 | 1, 4 | 1, 0, 0, 0, 0},
			{2 | 1, 2 | 4 | 1, 4 | 1, 0, 0, 0},
			{2 | 1, 4 | 1, 2 | 1, 4 | 1, 0, 0},
			{2 | 1, 2 | 4 | 1, 4 | 1, 2 | 1, 4 | 1, 0},
			{2 | 1, 2 | 4 | 1, 4 | 1, 2 | 1, 4 | 1, 2 | 4 | 1}
		};

		public static uint get_bits(stb_vorbis f, int n)
		{
			uint z;
			if (f.valid_bits < 0) return 0;
			if (f.valid_bits < n)
			{
				if (n > 24)
				{
					z = get_bits(f, 24);
					z += get_bits(f, n - 24) << 24;
					return z;
				}
				if (f.valid_bits == 0) f.acc = 0;
				while (f.valid_bits < n)
				{
					var z2 = get8_packet_raw(f);
					if (z2 == -1)
					{
						f.valid_bits = -1;
						return 0;
					}
					f.acc += (uint) (z2 << f.valid_bits);
					f.valid_bits += 8;
				}
			}

			if (f.valid_bits < 0) return 0;
			z = (uint) (f.acc & ((1 << n) - 1));
			f.acc >>= n;
			f.valid_bits -= n;
			return z;
		}

		public static short[] decode_vorbis_from_memory(byte[] input, out int sampleRate, out int chan)
		{
			short* result = null;
			int length = 0;
			fixed (byte* b = input)
			{
				int c, s;
				length = stb_vorbis_decode_memory(b, input.Length, &c, &s, ref result);

				chan = c;
				sampleRate = s;
			}
			var output = new short[length];
			Marshal.Copy(new IntPtr(result), output, 0, output.Length);
			Operations.Free(result);

			return output;
		}
	}
}