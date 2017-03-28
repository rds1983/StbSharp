using System;
using System.IO;
using System.Runtime.InteropServices;

namespace StbSharp
{
	public unsafe class ImageWriterToStream
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

		public void Write(byte[] bytes, int x, int y, int comp, ImageWriterType type, Stream dest)
		{
			try
			{
				_stream = dest;
				fixed (byte* b = &bytes[0])
				{
					switch (type)
					{
						case ImageWriterType.Bmp:
							Stb.stbi_write_bmp_to_func(WriteCallback, null, x, y, comp, b);
							break;
						case ImageWriterType.Tga:
							Stb.stbi_write_tga_to_func(WriteCallback, null, x, y, comp, b);
							break;
						case ImageWriterType.Hdr:
						{
							var f = new float[bytes.Length];
							for (var i = 0; i < bytes.Length; ++i)
							{
								f[i] = bytes[i]/255.0f;
							}

							fixed (float* fptr = f)
							{
								Stb.stbi_write_hdr_to_func(WriteCallback, null, x, y, comp, fptr);
							}
						}
							break;
						case ImageWriterType.Png:
							Stb.stbi_write_png_to_func(WriteCallback, null, x, y, comp, b, x*comp);
							break;
						default:
							throw new ArgumentOutOfRangeException("type", type, null);
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
