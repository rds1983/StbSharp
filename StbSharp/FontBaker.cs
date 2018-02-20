using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace StbSharp
{
	public struct FontBakerCharacterRange
	{
		public static readonly FontBakerCharacterRange BasicLatin = new FontBakerCharacterRange((char) 0x0020, (char) 0x007F);

		public static readonly FontBakerCharacterRange Latin1Supplement =
			new FontBakerCharacterRange((char) 0x00A0, (char) 0x00FF);
		
		public static readonly FontBakerCharacterRange LatinExtendedA =
			new FontBakerCharacterRange((char) 0x0100, (char) 0x017F);
		
		public static readonly FontBakerCharacterRange LatinExtendedB =
			new FontBakerCharacterRange((char) 0x0180, (char) 0x024F);
		
		public static readonly FontBakerCharacterRange Cyrillic = new FontBakerCharacterRange((char) 0x0400, (char) 0x04FF);

		public static readonly FontBakerCharacterRange CyrillicSupplement =
			new FontBakerCharacterRange((char) 0x0500, (char) 0x052F);
		
		public static readonly FontBakerCharacterRange Hiragana =
			new FontBakerCharacterRange((char) 0x3040, (char) 0x309F);
		
		public static readonly FontBakerCharacterRange Katakana =
			new FontBakerCharacterRange((char) 0x30A0, (char) 0x30FF);

		public char Start { get; private set; }
		public char End { get; private set; }

		public FontBakerCharacterRange(char start, char end)
		{
			Start = start;
			End = end;
		}
	}

	public unsafe class FontBaker
	{
		private StbTrueType.stbtt_pack_context pc;
		private bool _beginCalled;
		private GCHandle _handle;

		private readonly Dictionary<char, StbTrueType.stbtt_packedchar> result =
			new Dictionary<char, StbTrueType.stbtt_packedchar>();

		public void Begin(byte[] pixels, int pw, int ph)
		{
			if (_beginCalled)
			{
				throw new Exception("Call End() before calling Begin again");
			}

			_beginCalled = true;
			result.Clear();

			_handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
			fixed (StbTrueType.stbtt_pack_context* pcPtr = &pc)
			{
				StbTrueType.stbtt_PackBegin(pcPtr, (byte*) _handle.AddrOfPinnedObject().ToPointer(), pw, ph, pw, 1, null);
			}
		}

		public void Add(byte[] ttf, float pixel_height, IEnumerable<FontBakerCharacterRange> ranges)
		{
			fixed (StbTrueType.stbtt_pack_context* pcPtr = &pc)
			{
				fixed (byte* ttfPtr = ttf)
				{

					foreach (var range in ranges)
					{
						if (range.Start > range.End)
						{
							continue;
						}

						var cd = new StbTrueType.stbtt_packedchar[range.End - range.Start + 1];
						fixed (StbTrueType.stbtt_packedchar* chardataPtr = cd)
						{
							StbTrueType.stbtt_PackFontRange(pcPtr, ttfPtr, 0, pixel_height, range.Start,
								range.End - range.Start + 1, chardataPtr);
						}

						for (var i = 0; i < cd.Length; ++i)
						{
							result[(char) (i + range.Start)] = cd[i];
						}
					}
				}
			}
		}

		public Dictionary<char, StbTrueType.stbtt_packedchar> End()
		{
			fixed (StbTrueType.stbtt_pack_context* pcPtr = &pc)
			{
				StbTrueType.stbtt_PackEnd(pcPtr);
			}

			return result;
		}
	}
}