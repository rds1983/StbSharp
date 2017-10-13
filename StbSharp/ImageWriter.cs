using System;
using System.IO;
using System.Runtime.InteropServices;

namespace StbSharp
{
	public unsafe class ImageWriter
	{
		private Stream _stream;
		private byte[] _buffer = new byte[1024];

		private int WriteCallback(void* context, void* data, int size)
		{
			if (data == null || size <= 0)
			{
				return 0;
			}

			if (_buffer.Length < size)
			{
				_buffer = new byte[size*2];
			}

			var bptr = (byte*) data;

			Marshal.Copy(new IntPtr(bptr), _buffer, 0, size);

			_stream.Write(_buffer, 0, size);

			return size;
		}

		public void WriteBmp(Image image, Stream dest)
		{
			try
			{
				_stream = dest;
				fixed (byte* b = &image.Data[0])
				{
					Stb.stbi_write_bmp_to_func(WriteCallback, null, image.Width, image.Height, image.Comp, b);
				}
			}
			finally
			{
				_stream = null;
			}
		}

		public void WriteTga(Image image, Stream dest)
		{
			try
			{
				_stream = dest;
				fixed (byte* b = &image.Data[0])
				{
					Stb.stbi_write_tga_to_func(WriteCallback, null, image.Width, image.Height, image.Comp, b);
				}
			}
			finally
			{
				_stream = null;
			}
		}

		public void WriteHdr(Image image, Stream dest)
		{
			try
			{
				_stream = dest;
				var f = new float[image.Data.Length];
				for (var i = 0; i < image.Data.Length; ++i)
				{
					f[i] = image.Data[i]/255.0f;
				}

				fixed (float* fptr = f)
				{
					Stb.stbi_write_hdr_to_func(WriteCallback, null, image.Width, image.Height, image.Comp, fptr);
				}
			}
			finally
			{
				_stream = null;
			}
		}

		public void WritePng(Image image, Stream dest)
		{
			try
			{
				_stream = dest;

				fixed (byte* b = &image.Data[0])
				{
					Stb.stbi_write_png_to_func(WriteCallback, null, image.Width, image.Height, image.Comp, b, image.Width*image.Comp);
				}
			}
			finally
			{
				_stream = null;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="image"></param>
		/// <param name="dest"></param>
		/// <param name="quality">Should be beetween 1 & 100</param>
		public void WriteJpg(Image image, Stream dest, int quality)
		{
			try
			{
				_stream = dest;

				fixed (byte* b = &image.Data[0])
				{
					Stb.stbi_write_jpg_to_func(WriteCallback, null, image.Width, image.Height, image.Comp, b, quality);
				}
			}
			finally
			{
				_stream = null;
			}
		}
	}
}