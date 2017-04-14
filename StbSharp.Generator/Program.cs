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
				data = data.Replace("s.img_buffer_end = s.buffer_start + 1;",
					"s.img_buffer_end = s.buffer_start; s.img_buffer_end++;");
				data = data.Replace("s.img_buffer_end = s.buffer_start + n;",
					"s.img_buffer_end = s.buffer_start; s.img_buffer_end += n;");
				data = data.Replace(" != 0?(null):(null)", " != 0?((byte *)null):(null)");
				data = data.Replace("(int)(j.code_buffer)", "j.code_buffer");
				data = data.Replace("z.huff_dc + ", "(stbi__huffman *)z.huff_dc + ");
				data = data.Replace("z.huff_ac + ", "(stbi__huffman *)z.huff_ac + ");
				data = data.Replace("z.dequant[z.img_comp[n].tq]", "(ushort *)z.dequant[z.img_comp[n].tq]");
				data = data.Replace("int sixteen = (int)(p != 0);", "int sixteen = (p != 0)?1:0;");
				data = data.Replace("(byte)('')", "(byte)('\\0')");
				data = data.Replace("coeff = 0", "coeff = null");
				data =
					data.Replace("if (stbi__zbuild_huffman(&a->z_length, stbi__zdefault_length, (int)(288))== 0) return (int)(0);",
						"fixed (byte* b = stbi__zdefault_length) {if (stbi__zbuild_huffman(&a->z_length, b, (int) (288)) == 0) return (int) (0);}");
				data =
					data.Replace("if (stbi__zbuild_huffman(&a->z_distance, stbi__zdefault_distance, (int)(32))== 0) return (int)(0);",
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
				data = data.Replace("public static void stb__PrepareOptTable(byte* Table, byte* expand, int size)",
					"public static void stb__PrepareOptTable(byte[] Table, byte[] expand, int size)");

				data = data.Replace("(enum STBVorbisError)", string.Empty);

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

				File.WriteAllText(@"..\..\..\..\StbSharp\Stb.Dxt.Generated.cs", data);
			}
		}

		static void ProcessVorbis()
		{
			using (var output = new StringWriter())
			{
				var parameters = new ConversionParameters
				{

					InputPath = @"D:\Projects\StbSharp\StbSharp.Generator\StbSource\stb_vorbis.c",
					Output = output,
					Defines = new[]
					{
						"STB_VORBIS_NO_STDIO",
						"STB_VORBIS_NO_INLINE_DECODE",
						"STB_VORBIS_NO_FAST_SCALED_FLOAT"
					},
					Namespace = "StbSharp",
					Class = "Stb",
					SkipStructs = new[]
					{
						"Residue",
						"stb_vorbis",
					},
					SkipGlobalVariables = new[]
					{
						"channel_position"
					},
					SkipFunctions = new[]
					{
						"get_bits",
					},
					Structs = new[]
					{
						"stb_vorbis_alloc",
						"stb_vorbis_info",
						"Codebook",
						"Floor0",
						"Floor1",
						"Floor",
						"MappingChannel",
						"Mapping",
						"Mode",
						"CRCscan",
						"ProbedPage",
						"stbv__floor_ordering",
					},
					GlobalArrays = new[]
					{
						"crc_table",
						"ogg_page_header",
						"inverse_db_table",
						"log2_4",
						"channel_selector"
					}
				};

				var cp = new ClangParser();

				cp.Process(parameters);
				var data = output.ToString();

				// Post processing
				Logger.Info("Post processing...");

				data = data.Replace("byte* minp;", "byte* minp = null;");
				data = data.Replace("byte* maxp;", "byte* maxp = null;");
				data = data.Replace("public static void stb__PrepareOptTable(byte* Table, byte* expand, int size)",
					"public static void stb__PrepareOptTable(byte[] Table, byte[] expand, int size)");

				data = data.Replace("(enum STBVorbisError)", string.Empty);
				data = data.Replace("crc_table", "_crc_table");
				data = data.Replace("memset(available, (int)(0), (ulong)(sizeof((available))))", "memset(available, 0, 32)");
				data = data.Replace("return (int)((x) < (y)?-1:(x) > (y));", "return (int)((x) < (y)?-1:((x) > (y)?1:0));");
				data = data.Replace("sizeof(float)* * n", "sizeof(float) * n");
				data = data.Replace("uint16", "ushort");
				data = data.Replace("sizeof(ushort)* * n", "sizeof(ushort) * n");
				data = data.Replace("return (int)((a->x) < (b->x)?-1:(a->x) > (b->x));",
					"return (int)((a->x) < (b->x)?-1:((a->x) > (b->x)?1:0));");
				data = data.Replace("!c->codewords) != 0", "c->codewords == null)");
				data = data.Replace("(void *)(0)", "null");
				data = data.Replace("sizeof((**part_classdata))", "sizeof(byte *)");
				data = data.Replace("sizeof((**part_classdata))", "sizeof(byte *)");
				data = data.Replace("alloc.alloc_buffer?", "alloc.alloc_buffer != null?");
				data = data.Replace("float*[]", "float**");
				data = data.Replace("sizeof((*buf2))", "sizeof(float)");
				data = data.Replace("f.mode_config +", "(Mode *)f.mode_config +");
				data = data.Replace("memcpy(really_zero_channel, zero_channel, (ulong)(sizeof((really_zero_channel[0])) * f.channels));", 
					"memcpy(really_zero_channel, zero_channel, (ulong)(sizeof(int) * f.channels));");
				data = data.Replace("memset(f.channel_buffers[i], (int)(0), (ulong)(sizeof((*f.channel_buffers[i])) * n2));",
					"memset(f.channel_buffers[i], (int)(0), (ulong)(sizeof(float) * n2));");
				data = data.Replace("if ((f.page_flag & 4))",
					"if ((f.page_flag & 4) != 0)");
				data = data.Replace("for ((s) == (-1); {",
					"for (;(s) == (-1);) {");
				data = data.Replace("float** residue_buffers = stackalloc float[16];",
					"float** residue_buffers = stackalloc float*[16];");
				data = data.Replace("if ((p[5] & 1))",
					"if ((p[5] & 1) != 0)");
				data = data.Replace("sizeof((*f.codebooks))",
					"sizeof(Codebook)");
				data = data.Replace("sizeof((c->codewords[0]))",
					"sizeof(uint)");
				data = data.Replace("sizeof((*c->codewords))",
					"sizeof(uint)");
				data = data.Replace("sizeof((*values))",
					"sizeof(uint)");
				data = data.Replace("sizeof((*c->sorted_codewords))",
					"sizeof(uint)");
				data = data.Replace("sizeof((*c->sorted_values))",
					"sizeof(int)");
				data = data.Replace("sizeof((mults[0]))",
					"sizeof(ushort)");
				data = data.Replace("sizeof((c->multiplicands[0]))",
					"sizeof(float)");
				data = data.Replace("sizeof((*f.floor_config))",
					"sizeof(Floor)");
				data = data.Replace("(Residue)(setup_malloc(f, (int)(f.residue_count * sizeof((f.residue_config[0])))))",
					"new Residue[f.residue_count]");
				data = data.Replace("sizeof((*r.classdata))",
					"sizeof(byte *)");
				data = data.Replace("sizeof((*f.mapping))",
					"sizeof(Mapping)");
				data = data.Replace("sizeof((r.classdata[j][0]))",
					"sizeof(byte)");
				data = data.Replace("sizeof((r.residue_books[0]))",
					"sizeof(short)");
				data = data.Replace("sizeof((*m->chan))",
					"sizeof(MappingChannel)");
				data = data.Replace("(short [8]*)(setup_malloc(f, (int)(sizeof(short) * r.classifications)))",
					"new short*[r.classifications]");
				data = data.Replace("sizeof(float)* *",
					"sizeof(float) *");
				data = data.Replace("sizeof(short)* *",
					"sizeof(short) *");
				data = data.Replace("sizeof(float)>> >>",
					"sizeof(float) >>");
				data = data.Replace("sizeof(float)+ +",
					"sizeof(float) +");
				data = data.Replace("sizeof(void*)+ +",
					"sizeof(void*) +");
				data = data.Replace("classify_mem = (uint)((f.channels * (sizeof(void*) + max_part_read * sizeof(char*)))));",
					"classify_mem = (uint)((f.channels * (sizeof(void*) + max_part_read * sizeof(char*))));");
				data = data.Replace("(stb_vorbis)(setup_malloc(f, (int)(sizeof((p)))))",
					"new stb_vorbis()");
				data = data.Replace("memset(buffer, (int)(0), (ulong)(sizeof((buffer))))",
					"memset(buffer, (int)(0), (ulong)(sizeof(float) * 32))");
				data = data.Replace("channel_position[num_c][j]",
					"channel_position[num_c,j]");
				data = data.Replace("channel_selector[buf_c][i]",
					"channel_selector[buf_c,i]");
				data = data.Replace("int[] channel_selector",
					"int[,] channel_selector");
				data = data.Replace("sizeof((*data))",
					"sizeof(short)");
				data = data.Replace("float** output;",
					"float*[] output = null;");
				data = data.Replace("float** outputs;",
					"float*[] outputs = null;");
				data = data.Replace("&output",
					"ref output");
				data = data.Replace("float*** output",
					"ref float*[] output");
				data = data.Replace("float** data",
					"float*[] data");
				data = data.Replace("if ((output) != null) *output = f.outputs;",
					"if ((output) != null) output = f.outputs;");
				data = data.Replace("stb_vorbis_get_frame_float(f, &n, (null));",
					"float*[] output = null; stb_vorbis_get_frame_float(f, &n, ref output);");
				data = data.Replace("*output = f.outputs;",
					"output = f.outputs;");
				data = data.Replace("*output = f.outputs;",
					"output = f.outputs;");
				data = data.Replace("f.current_loc_valid = (int)(f.current_loc != ~0U);",
					"f.current_loc_valid = f.current_loc != ~0U?1:0;");
				data = data.Replace("setup_free(p, c->sorted_values?c->sorted_values - 1:(null));",
					"setup_free(p, c->sorted_values != null?c->sorted_values - 1:(null));");
				data = data.Replace("f.scan[n].sample_loc = (uint)(~0);",
					"f.scan[n].sample_loc = uint.MaxValue;");
				data = data.Replace("; ) {}",
					"");
				data = data.Replace("if (((f.current_loc_valid) != 0) && (f.page_flag & 4))",
					"if (((f.current_loc_valid) != 0) && (f.page_flag & 4) != 0)");

				File.WriteAllText(@"..\..\..\..\StbSharp\Stb.Vorbis.Generated.cs", data);
			}
		}

		static void Main(string[] args)
		{
			try
			{
				// ProcessImage();
				// ProcessImageWriter();
				// ProcessImageResize();
				// ProcessDXT();
				ProcessVorbis();
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