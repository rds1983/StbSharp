using System;
using System.IO;
using System.Runtime.InteropServices;

namespace StbSharp
{
	public enum ImageWriterFormat
	{
		Bmp,
		Tga,
		Hdr,
		Png
	}

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
				_buffer = new byte[size * 2];
			}

			var bptr = (byte*)data;

			Marshal.Copy(new IntPtr(bptr), _buffer, 0, size);

			_stream.Write(_buffer, 0, size);

			return size;
		}

		public void Write(Image image, ImageWriterFormat format, Stream dest)
		{
			if (image == null || image.Data == null)
			{
				throw new ArgumentNullException("image");
			}

			try
			{
				_stream = dest;
				fixed (byte* b = &image.Data[0])
				{
					switch (format)
					{
						case ImageWriterFormat.Bmp:
							Stb.stbi_write_bmp_to_func(WriteCallback, null, image.Width, image.Height, image.Comp, b);
							break;
						case ImageWriterFormat.Tga:
							Stb.stbi_write_tga_to_func(WriteCallback, null, image.Width, image.Height, image.Comp, b);
							break;
						case ImageWriterFormat.Hdr:
						{
							var f = new float[image.Data.Length];
							for (var i = 0; i < image.Data.Length; ++i)
							{
								f[i] = image.Data[i] / 255.0f;
							}

							fixed (float* fptr = f)
							{
								Stb.stbi_write_hdr_to_func(WriteCallback, null, image.Width, image.Height, image.Comp, fptr);
							}
						}
							break;
						case ImageWriterFormat.Png:
							Stb.stbi_write_png_to_func(WriteCallback, null, image.Width, image.Height, image.Comp, b, image.Width * image.Comp);
							break;
						default:
							throw new ArgumentOutOfRangeException("format", format, null);
					}
				}
			}
			finally
			{
				_stream = null;
			}
		}
	}
}
