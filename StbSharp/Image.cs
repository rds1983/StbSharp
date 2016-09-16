namespace StbSharp
{
	partial class Image
	{
		public unsafe delegate int ReadCallback(void* user, char* data, int size);

		public unsafe delegate int SkipCallback(void* user, int n);

		public unsafe delegate int EofCallback(void* user);

		private static string stbi__g_failure_reason;
		private static int stbi__vertically_flip_on_load;

		enum STBI
		{
			STBI_default = 0,
			STBI_grey = 1,
			STBI_grey_alpha = 2,
			STBI_rgb = 3,
			STBI_rgb_alpha = 4
		};

		internal class stbi_io_callbacks
		{
			public ReadCallback read;
			public SkipCallback skip;
			public EofCallback eof;
		}

		internal unsafe class stbi__context
		{
			public uint img_x, img_y;
			public int img_n, img_out_n;
			public stbi_io_callbacks io;
			public void* io_user_data;
			public int read_from_callbacks;
			public int buflen;
			public Pointer<byte> buffer_start = new Pointer<byte>(128);
			public Pointer<byte> img_buffer, img_buffer_end;
			public Pointer<byte> img_buffer_original, img_buffer_original_end;
		}

		private static Pointer<byte> stbi__malloc(int size)
		{
			return new Pointer<byte>(size);
		}

		private static void stbi_image_free(Pointer<byte> retval_from_stbi_load)
		{
		}
	}
}