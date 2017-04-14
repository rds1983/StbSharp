using System;

namespace StbSharp
{
	public unsafe partial class StbDxt
	{
		public static void stb__DitherBlock(byte* dest, byte* block)
		{
			int* err = stackalloc int[8];
			var ep1 = err;
			var ep2 = err + 4;
			int ch;
			for (ch = 0; ch < 3; ++ch)
			{
				var bp = block + ch;
				var dp = dest + ch;
				var quantArray = ch == (1) ? stb__QuantGTab : stb__QuantRBTab;
				fixed (byte* quant = quantArray)
				{
					CRuntime.memset(err, 0, (ulong) (8*sizeof (int)));
					int y;
					for (y = 0; (y) < (4); ++y)
					{
						dp[0] = quant[bp[0] + ((3*ep2[1] + 5*ep2[0]) >> 4)];
						ep1[0] = bp[0] - dp[0];
						dp[4] = quant[bp[4] + ((7*ep1[0] + 3*ep2[2] + 5*ep2[1] + ep2[0]) >> 4)];
						ep1[1] = bp[4] - dp[4];
						dp[8] = quant[bp[8] + ((7*ep1[1] + 3*ep2[3] + 5*ep2[2] + ep2[1]) >> 4)];
						ep1[2] = bp[8] - dp[8];
						dp[12] = quant[bp[12] + ((7*ep1[2] + 5*ep2[3] + ep2[2]) >> 4)];
						ep1[3] = bp[12] - dp[12];
						bp += 16;
						dp += 16;
						var et = ep1;
						ep1 = ep2;
						ep2 = et;
					}
				}
			}
		}

		public static byte[] stb_compress_dxt(Image image, bool hasAlpha = true, int mode = 10)
		{
			if (image.Comp != 4)
			{
				throw new Exception("This method supports only rgba images");
			}

			var osize = hasAlpha ? 16 : 8;
			var result = new byte[(image.Width + 3)*(image.Height + 3)/16*osize];

			fixed (byte* rgba = image.Data)
			{
				fixed (byte* resultPtr = result)
				{
					var p = resultPtr;

					byte* block = stackalloc byte[16*4];
					for (var j = 0; j < image.Width; j += 4)
					{
						var x = 4;
						for (var i = 0; i < image.Height; i += 4)
						{
							if (j + 3 >= image.Width) x = image.Width - j;
							int y;
							for (y = 0; y < 4; ++y)
							{
								if (j + y >= image.Height) break;
								CRuntime.memcpy(block + y*16, rgba + image.Width*4*(j + y) + i*4, x*4);
							}
							int y2;
							if (x < 4)
							{
								switch (x)
								{
									case 0:
										throw new Exception("Unknown error");
									case 1:
										for (y2 = 0; y2 < y; ++y2)
										{
											CRuntime.memcpy(block + y2*16 + 1*4, block + y2*16 + 0*4, 4);
											CRuntime.memcpy(block + y2*16 + 2*4, block + y2*16 + 0*4, 8);
										}
										break;
									case 2:
										for (y2 = 0; y2 < y; ++y2)
											CRuntime.memcpy(block + y2*16 + 2*4, block + y2*16 + 0*4, 8);
										break;
									case 3:
										for (y2 = 0; y2 < y; ++y2)
											CRuntime.memcpy(block + y2*16 + 3*4, block + y2*16 + 1*4, 4);
										break;
								}
							}
							y2 = 0;
							for (; y < 4; ++y,++y2)
								CRuntime.memcpy(block + y*16, block + y2*16, 4*4);
							stb_compress_dxt_block(p, block, hasAlpha ? 1 : 0, mode);
							p += hasAlpha ? 16 : 8;
						}
					}
				}
			}

			return result;
		}
	}
}