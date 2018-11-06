namespace StbSharpSafe
{
	public static partial class StbTrueType
	{
		public static uint stbtt__find_table(FakePtr<byte> data, uint fontstart, string tag)
		{
			int num_tables = ttUSHORT(data + fontstart + 4);
			var tabledir = fontstart + 12;
			int i;
			for (i = 0; (i) < num_tables; ++i)
			{
				var loc = (int)(tabledir + 16 * i);
				if (((data)[loc] == (tag[0])) && ((data)[loc + 1] == tag[1]) &&
					((data)[loc + 2] == tag[2]) && ((data)[loc + 3] == (tag[3])))
					return ttULONG(data + loc + 8);
			}
			return 0;
		}

		public static void stbtt__dict_get_ints(stbtt__buf b, int key, int outcount, ref uint _out_)
		{
			int i;
			stbtt__buf operands = stbtt__dict_get(b, key);
			for (i = (int)(0); ((i) < (outcount)) && ((operands.cursor) < (operands.size)); i++)
			{
				_out_ = stbtt__cff_int(operands);
			}
		}

		public static int stbtt_GetGlyphBox(stbtt_fontinfo info, int glyph_index, ref int x0, ref int y0, ref int x1, ref int y1)
		{
			var px0 = new FakePtr<int>(1)
			{
				Value = x0
			};

			var px1 = new FakePtr<int>(1)
			{
				Value = x1
			};

			var py0 = new FakePtr<int>(1)
			{
				Value = y0
			};

			var py1 = new FakePtr<int>(1)
			{
				Value = y1
			};

			var result = stbtt_GetGlyphBox(info, glyph_index, px0, py0, px1, py1);

			x0 = px0.Value;
			x1 = px1.Value;
			y0 = py0.Value;
			y1 = py1.Value;

			return result;
		}
		public static void stbtt_GetGlyphBitmapBoxSubpixel(stbtt_fontinfo font, int glyph, float scale_x, float scale_y, float shift_x, float shift_y, ref int x0, ref int y0, ref int x1, ref int y1)
		{
			var px0 = new FakePtr<int>(1)
			{
				Value = x0
			};

			var px1 = new FakePtr<int>(1)
			{
				Value = x1
			};

			var py0 = new FakePtr<int>(1)
			{
				Value = y0
			};

			var py1 = new FakePtr<int>(1)
			{
				Value = y1
			};

			stbtt_GetGlyphBitmapBoxSubpixel(font, glyph, scale_x, scale_y, shift_x, shift_y, px0, py0, px1, py1);

			x0 = px0.Value;
			x1 = px1.Value;
			y0 = py0.Value;
			y1 = py1.Value;
		}

		public static void stbtt_GetGlyphBitmapBox(stbtt_fontinfo font, int glyph, float scale_x, float scale_y, ref int x0, ref int y0, ref int x1, ref int y1)
		{
			var px0 = new FakePtr<int>(1)
			{
				Value = x0
			};

			var px1 = new FakePtr<int>(1)
			{
				Value = x1
			};

			var py0 = new FakePtr<int>(1)
			{
				Value = y0
			};

			var py1 = new FakePtr<int>(1)
			{
				Value = y1
			};

			stbtt_GetGlyphBitmapBox(font, glyph, scale_x, scale_y, px0, py0, px1, py1);

			x0 = px0.Value;
			x1 = px1.Value;
			y0 = py0.Value;
			y1 = py1.Value;
		}

		public static bool stbtt_BakeFontBitmap(byte[] ttf, int offset, float pixel_height, byte[] pixels, int pw, int ph,
			int first_char, int num_chars, stbtt_bakedchar[] chardata)
		{
			var result = stbtt_BakeFontBitmap(new FakePtr<byte>(ttf), offset, pixel_height, new FakePtr<byte>(pixels), pw, ph, first_char, num_chars, new FakePtr<stbtt_bakedchar>(chardata));

			return result != 0;
		}
	}
}