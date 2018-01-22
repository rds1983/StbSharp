using System;
using System.IO;
using Sichem;

namespace StbSharp.StbVorbis.Generator
{
	class Program
	{
		static void Process()
		{
			using (var output = new StringWriter())
			{
				var parameters = new ConversionParameters
				{

					InputPath = @"stb_vorbis.c",
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
				data =
					data.Replace("memcpy(really_zero_channel, zero_channel, (ulong)(sizeof((really_zero_channel[0])) * f.channels));",
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

				File.WriteAllText(@"..\..\..\..\..\StbSharp\Stb.Vorbis.Generated.cs", data);
			}
		}

		static void Main(string[] args)
		{
			try
			{
				Process();
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