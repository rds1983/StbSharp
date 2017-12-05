using System;
using System.IO;
using Sichem;

namespace Generator
{
	class Program
	{
		static void ProcessImage()
		{
			using (var output = new StringWriter())
			{
				var parameters = new ConversionParameters
				{
					InputPath = @"D:\Projects\StbSharp\StbSharp.Generator\StbSource\stb_image.h",
					Output = output,
					Defines = new[]
					{
						"STBI_NO_SIMD",
						"STBI_NO_LINEAR",
						"STBI_NO_HDR",
						"STBI_NO_PIC",
						"STBI_NO_PNM",
						"STBI_NO_STDIO",
						"STB_IMAGE_IMPLEMENTATION",
					},
					Namespace = "StbSharp",
					Class = "Stb",
					SkipStructs = new[]
					{
						"stbi_io_callbacks",
						"img_comp",
						"stbi__jpeg",
						"stbi__resample",
						"stbi__gif_lzw",
						"stbi__gif"
					},
					SkipGlobalVariables = new[]
					{
						"stbi__g_failure_reason",
						"stbi__vertically_flip_on_load"
					},
					SkipFunctions = new[]
					{
						"stbi__malloc",
						"stbi_image_free",
						"stbi_failure_reason",
						"stbi__err",
						"stbi_is_hdr_from_memory",
						"stbi_is_hdr_from_callbacks",
						"stbi__pnm_isspace",
						"stbi__pnm_skip_whitespace",
						"stbi__pic_is4",
						"stbi__gif_parse_colortable"
					},
					Structs = new[]
					{
						"img_comp",
						"stbi__gif_lzw",
						"stbi__result_info",
						"stbi__pngchunk",
						"stbi__bmp_data",
						"stbi__pic_packet",
						"stbi__huffman",
						"stbi__zhuffman",
						"stbi__zbuf",
					},
					GlobalArrays = new[]
					{
						"stbi__bmask",
						"stbi__jbias",
						"stbi__jpeg_dezigzag",
						"stbi__zlength_base",
						"stbi__zlength_extra",
						"stbi__zdist_base",
						"stbi__zdist_extra",
						"first_row_filter",
						"stbi__depth_scale_table",
						"stbi__zdefault_length",
						"stbi__zdefault_distance",
						"length_dezigzag",
						"png_sig"
					}
				};

				var cp = new ClangParser();

				cp.Process(parameters);
				var data = output.ToString();

				// Post processing
				Logger.Info("Post processing...");

				data = data.Replace("(int)(a <= 2147483647 - b)", "(a <= 2147483647 - b)?1:0");
				data = data.Replace("(int)(a <= 2147483647 / b)", "(a <= 2147483647 / b)?1:0");
				data = data.Replace("(ulong)((ulong)(w) * bytes_per_pixel)", "(ulong)(w * bytes_per_pixel)");
				data = data.Replace("bytes + row * bytes_per_row", "bytes + (ulong)row * bytes_per_row");
				data = data.Replace("bytes + (h - row - 1) * bytes_per_row", "bytes + (ulong)(h - row - 1) * bytes_per_row");
				data = data.Replace("(void *)(0)", "null");
				data = data.Replace("s.img_buffer_end = s.buffer_start + 1;", "s.img_buffer_end = s.buffer_start; s.img_buffer_end++;");
				data = data.Replace("s.img_buffer_end = s.buffer_start + n;", "s.img_buffer_end = s.buffer_start; s.img_buffer_end += n;");
				data = data.Replace(" != 0?(null):(null)", " != 0?((byte *)null):(null)");
				data = data.Replace("(int)(j.code_buffer)", "j.code_buffer");
				data = data.Replace("z.huff_dc + ", "(stbi__huffman *)z.huff_dc + ");
				data = data.Replace("z.huff_ac + ", "(stbi__huffman *)z.huff_ac + ");
				data = data.Replace("z.dequant[z.img_comp[n].tq]", "(ushort *)z.dequant[z.img_comp[n].tq]");
				data = data.Replace("int sixteen = (int)(p != 0);", "int sixteen = (p != 0)?1:0;");
				data = data.Replace("(byte)('')", "(byte)('\\0')");
				data = data.Replace("coeff = 0", "coeff = null");
				data = data.Replace("if (stbi__zbuild_huffman(&a->z_length, stbi__zdefault_length, (int)(288))== 0) return (int)(0);",
					"fixed (byte* b = stbi__zdefault_length) {if (stbi__zbuild_huffman(&a->z_length, b, (int) (288)) == 0) return (int) (0);}");
				data = data.Replace("if (stbi__zbuild_huffman(&a->z_distance, stbi__zdefault_distance, (int)(32))== 0) return (int)(0);",
					"fixed (byte* b = stbi__zdefault_distance) {if (stbi__zbuild_huffman(&a->z_distance, b, (int) (32)) == 0) return (int) (0);}");
				data = data.Replace("sbyte* invalid_chunk", "string invalid_chunk");

				data = data.Replace("sizeof((s.buffer_start))",
					"s.buffer_start.Size");
				data = data.Replace("sizeof((data[0]))", "sizeof(short)");
				data = data.Replace("sizeof((sizes))", "sizeof(int)");
				data = data.Replace("memset(z->fast, (int)(0), (ulong)(sizeof((z->fast))));",
					"memset(((ushort*)(z->fast)), (int)(0), (ulong)((1 << 9) * sizeof(ushort)));");
				data = data.Replace("memset(codelength_sizes, (int)(0), (ulong)(sizeof((codelength_sizes))));",
					"memset(((byte*)(codelength_sizes)), (int)(0), (ulong)(19 * sizeof(byte)));");
				data = data.Replace("comp != 0", "comp != null");
				data = data.Replace("(int)((tga_image_type) == (3))", "(tga_image_type) == (3)?1:0");

				data = data.Replace("short* d = ((short*)data.Pointer);",
					"short* d = data;");
				data = data.Replace("byte** coutput = stackalloc byte[4];",
					"byte** coutput = stackalloc byte *[4];");
				data = data.Replace("stbi__resample res_comp = new PinnedArray<stbi__resample>(4);",
					"var res_comp = new stbi__resample[4]; for (var kkk = 0; kkk < res_comp.Length; ++kkk) res_comp[kkk] = new stbi__resample();");
				data = data.Replace("((byte**)coutput.Pointer)",
					"coutput");
				data = data.Replace("stbi__jpeg j = (stbi__jpeg)(stbi__malloc((ulong)(sizeof(stbi__jpeg)))));",
					"stbi__jpeg j = new stbi__jpeg();");
				data = data.Replace("stbi__jpeg j = (stbi__jpeg)((stbi__malloc((ulong)(sizeof(stbi__jpeg))))));",
					"stbi__jpeg j = new stbi__jpeg();");
				data = data.Replace("stbi__jpeg j = (stbi__jpeg)((stbi__malloc((ulong)(.Size))));",
					"stbi__jpeg j = new stbi__jpeg();");
				data = data.Replace("free(j);",
					string.Empty);
				data = data.Replace("z.img_comp[i].data = (byte*)(((ulong)(z.img_comp[i].raw_data) + 15) & ~15);",
					"z.img_comp[i].data = (byte*)((((long)z.img_comp[i].raw_data + 15) & ~15));");
				data = data.Replace("z.img_comp[i].coeff = (short*)(((ulong)(z.img_comp[i].raw_coeff) + 15) & ~15);",
					"z.img_comp[i].coeff = (short*)((((long)z.img_comp[i].raw_coeff + 15) & ~15));");
				data = data.Replace("(int)(!is_iphone)",
					"is_iphone!=0?0:1");
				data = data.Replace("ArrayPointer<sbyte> invalid_chunk = \"XXXX PNG chunk not known\";",
					"var invalid_chunk = \"XXXX PNG chunk not known\";");
				data = data.Replace("return (int)(stbi__err(((sbyte*)invalid_chunk.Pointer)));",
					"return (int)(stbi__err(invalid_chunk));");
				data = data.Replace("if ((p) == ((void *)(0))) return (int)(stbi__err(\"outofmem\"));",
					"if (p == null) return (int) (stbi__err(\"outofmem\"));");
				data = data.Replace("ArrayPointer<ArrayPointer<byte>> pal = new ArrayPointer<byte>(256);",
					"ArrayPointer<byte>[] pal = new ArrayPointer<byte>[256]; for (var kkk = 0; kkk < pal.Length; ++kkk) pal[kkk] = new ArrayPointer<byte>(4);");
				data = data.Replace("pal[i][", "pal[i * 4 +");
				data = data.Replace("pal[v][", "pal[v * 4 +");
				data = data.Replace("pal[g.transparent][", "pal[g.transparent * 4 +");
				data = data.Replace("uint v = (uint)((uint)((bpp) == (16)?stbi__get16le(s):stbi__get32le(s)));",
					"uint v = (uint)((uint)((bpp) == (16)?(uint)stbi__get16le(s):stbi__get32le(s)));");
				data = data.Replace("(int)(((tga_image_type) == (3)) || ((tga_image_type) == (11)))",
					"(((tga_image_type) == (3))) || (((tga_image_type) == (11)))?1:0");
				data = data.Replace("int r = (int)((stbi__get32be(s)) == (0x38425053));",
					"int r = (((stbi__get32be(s)) == (0x38425053)))?1:0;");
				data = data.Replace("(packets).Size / (packets[0]).Size",
					"packets.Size");
				data = data.Replace("stbi__gif g = (stbi__gif)(stbi__malloc((ulong)(sizeof(stbi__gif)))));",
					"stbi__gif g = new stbi__gif();");
				data = data.Replace("free(g);",
					string.Empty);
				data = data.Replace("memset(g, (int)(0), (ulong)(sizeof((g))));",
					string.Empty);
				data = data.Replace("if (((g.transparent) >= (0)) && ((g.eflags & 0x01)))",
					"if (((g.transparent) >= (0)) && ((g.eflags & 0x01) != 0))");
				data = data.Replace("&z.huff_dc[z.img_comp[n].hd]",
					"(stbi__huffman*)z.huff_dc + z.img_comp[n].hd");
				data = data.Replace("&z.huff_ac[ha]",
					"(stbi__huffman*)z.huff_ac + ha");
				data = data.Replace("g.codes[init_code]", "((stbi__gif_lzw*) (g.codes))[init_code]");
				data = data.Replace("&g.codes[avail++]", "(stbi__gif_lzw*)g.codes + avail++");
				data = data.Replace("byte* c = g.pal[g.bgindex]", "byte* c = (byte *)g.pal + g.bgindex");
				data = data.Replace("(g._out_) == (0)", "(g._out_) == null");
				data = data.Replace("((byte*)(tc))[k] = ((byte)(stbi__get16be(s) & 255) * stbi__depth_scale_table[z.depth]);",
					"((byte*)(tc))[k] = (byte)((byte)(stbi__get16be(s) & 255) * stbi__depth_scale_table[z.depth]);");
				data = data.Replace("byte** pal = stackalloc byte[256];",
					"byte* pal = stackalloc byte[256 * 4];");
				data = data.Replace("tga_data = (byte*)(stbi__malloc((ulong)(tga_width) * tga_height * tga_comp));",
					"tga_data = (byte*)(stbi__malloc(tga_width * tga_height * tga_comp));");
				data = data.Replace("case 0x3B:return (byte*)(s);",
					"case 0x3B:return null;");
				data = data.Replace("if ((u) == ((byte*)(s))) u = ((byte*)(0));",
					string.Empty);

				File.WriteAllText(@"..\..\..\..\StbSharp\Stb.Image.Generated.cs", data);
			}
		}

		static void ProcessImageWriter()
		{
			using (var output = new StringWriter())
			{
				var parameters = new ConversionParameters
				{
					InputPath = @"D:\Projects\StbSharp\StbSharp.Generator\StbSource\stb_image_write.h",
					Output = output,
					Defines = new[]
					{
						"STBI_WRITE_NO_STDIO",
						"STB_IMAGE_WRITE_IMPLEMENTATION"
					},
					Namespace = "StbSharp",
					Class = "Stb",
					SkipStructs = new[]
					{
						"stbi__write_context"
					},
					SkipGlobalVariables = new[]
					{
						"stbi_write_tga_with_rle"
					},
					SkipFunctions = new[]
					{
						"stbi__start_write_callbacks",
						"stbiw__writefv",
						"stbiw__writef",
						"stbiw__outfile",
						"stbi_write_bmp_to_func",
						"stbi_write_tga_to_func",
						"stbi_write_hdr_to_func",
						"stbi_write_png_to_func",
						"stbi_write_hdr_core",
					},
					GlobalArrays = new[]
					{
						"lengthc",
						"lengtheb",
						"distc",
						"disteb",
						"crc_table",
						"stbiw__jpg_ZigZag",
						"std_dc_luminance_nrcodes",
						"std_dc_luminance_values",
						"std_ac_luminance_nrcodes",
						"std_ac_luminance_values",
						"std_dc_chrominance_nrcodes",
						"std_dc_chrominance_values",
						"std_ac_chrominance_nrcodes",
						"std_ac_chrominance_values",
						"std_dc_chrominance_nrcodes",
						"std_dc_chrominance_values",
						"YDC_HT",
						"UVDC_HT",
						"YAC_HT",
						"UVAC_HT",
						"YQT",
						"UVQT",
						"aasf",
						"head0",
						"head2"
					}
				};

				var cp = new ClangParser();

				cp.Process(parameters);
				var data = output.ToString();

				// Post processing
				Logger.Info("Post processing...");

				data = data.Replace("int has_alpha = (int)(((comp) == (2)) || ((comp) == (4)));",
					"int has_alpha = (((comp) == (2)) || ((comp) == (4)))?1:0;");
				data = data.Replace("*arr?",
					"*arr != null?");
				data = data.Replace("sizeof(int)* * 2",
					"sizeof(int) * 2");
				data = data.Replace("(int)(sizeof((*(data))))",
					"sizeof(byte)");
				data = data.Replace("(int)(sizeof((*(_out_))))",
					"sizeof(byte)");
				data = data.Replace("(int)(sizeof((*(hash_table[h]))))",
					"sizeof(byte*)");
				data = data.Replace("sizeof((hash_table[h][0]))",
					"sizeof(byte*)");
				data = data.Replace("(byte***)(malloc((ulong)(16384 * sizeof(char**)))))",
					"(byte***)(malloc((ulong)(16384 * sizeof(byte**))))");
				data = data.Replace("(hlist)?",
					"(hlist != null)?");
				data = data.Replace("(hash_table[i])?",
					"(hash_table[i] != null)?");

				File.WriteAllText(@"..\..\..\..\StbSharp\Stb.ImageWrite.Generated.cs", data);
			}
		}


		static void ProcessImageResize()
		{
			using (var output = new StringWriter())
			{
				var parameters = new ConversionParameters
				{
					InputPath = @"D:\Projects\StbSharp\StbSharp.Generator\StbSource\stb_image_resize.h",
					Output = output,
					Defines = new[]
					{
						"STB_IMAGE_RESIZE_IMPLEMENTATION"
					},
					Namespace = "StbSharp",
					Class = "Stb",
					SkipStructs = new[]
					{
						"stbir__filter_info",
						"stbir__info",
						"stbir__FP32"
					},
					SkipGlobalVariables = new[]
					{
						"stbir__filter_info_table"
					},
					SkipFunctions = new[]
					{
						"stbir__linear_to_srgb_uchar",
						"stbiw__writefv",
						"stbiw__writef",
						"stbiw__outfile",
						"stbi_write_bmp_to_func",
						"stbi_write_tga_to_func",
						"stbi_write_hdr_to_func",
						"stbi_write_png_to_func",
						"stbi_write_hdr_core",
					},
					Structs = new[]
					{
						"stbir__contributors",
						"stbir__FP32"
					},
					GlobalArrays = new[]
					{
						"stbir__type_size",
						"stbir__srgb_uchar_to_linear_float",
						"fp32_to_srgb8_tab4"
					}
				};

				var cp = new ClangParser();

				cp.Process(parameters);
				var data = output.ToString();

				// Post processing
				Logger.Info("Post processing...");

/*                data = data.Replace("int has_alpha = (int)(((comp) == (2)) || ((comp) == (4)));",
                    "int has_alpha = (((comp) == (2)) || ((comp) == (4)))?1:0;");
                data = data.Replace("*arr?",
                    "*arr != null?");
                data = data.Replace("sizeof(int)* * 2",
                    "sizeof(int) * 2");
                data = data.Replace("(int)((*(data)).Size)",
                    "sizeof(byte)");
                data = data.Replace("(int)((*(_out_)).Size)",
                    "sizeof(byte)");
                data = data.Replace("(int)((*(hash_table[h])).Size)",
                    "sizeof(byte*)");
                data = data.Replace("(hash_table[h][0]).Size",
                    "sizeof(byte*)");
                data = data.Replace("(byte***)(malloc((ulong)(16384 * sizeof(char**)))))",
                    "(byte***)(malloc((ulong)(16384 * sizeof(byte**))))");
                data = data.Replace("(hlist)?",
                    "(hlist != null)?");
                data = data.Replace("(hash_table[i])?",
                    "(hash_table[i] != null)?");*/

				File.WriteAllText(@"..\..\..\..\StbSharp\Stb.ImageResize.Generated.cs", data);
			}
		}

		static void ProcessDXT()
		{
			using (var output = new StringWriter())
			{
				var parameters = new ConversionParameters
				{
					InputPath = @"D:\Projects\StbSharp\StbSharp.Generator\StbSource\stb_dxt.h",
					Output = output,
					Defines = new[]
					{
						"STB_DXT_IMPLEMENTATION"
					},
					Namespace = "StbSharp",
					Class = "Stb",
					SkipStructs = new string[]
					{
					},
					SkipGlobalVariables = new string[]
					{
					},
					SkipFunctions = new[]
					{
						"stb__DitherBlock"
					},
					Structs = new string[]
					{
					},
					GlobalArrays = new[]
					{
						"stb__Expand5",
						"stb__Expand6",
						"stb__OMatch5",
						"stb__OMatch6",
						"stb__QuantRBTab",
						"stb__QuantGTab"
					}
				};

				var cp = new ClangParser();

				cp.Process(parameters);
				var data = output.ToString();

				// Post processing
				Logger.Info("Post processing...");

				data = data.Replace("byte* minp;", "byte* minp = null;");
				data = data.Replace("byte* maxp;", "byte* maxp = null;");
				data = data.Replace("public static void stb__PrepareOptTable(byte* Table, byte* expand, int size)", "public static void stb__PrepareOptTable(byte[] Table, byte[] expand, int size)");

				File.WriteAllText(@"..\..\..\..\StbSharp\Stb.Dxt.Generated.cs", data);
			}
		}

		static void Main(string[] args)
		{
			try
			{
				ProcessImage();
				// ProcessImageWriter();
				// ProcessImageResize();
				// ProcessDXT();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}

			Console.WriteLine("Finished. Press any key to quit.");
			Console.ReadKey();
		}
	}
}