using System;
using System.IO;
using Sichem;

namespace StbSharp.StbImageWrite.Generator
{
	class Program
	{
		static void Process()
		{
			using (var output = new StringWriter())
			{
				var parameters = new ConversionParameters
				{
					InputPath = @"stb_image_write.h",
					Output = output,
					Defines = new[]
					{
						"STBI_WRITE_NO_STDIO",
						"STB_IMAGE_WRITE_IMPLEMENTATION"
					},
					Namespace = "StbSharp",
					Class = "StbImageWrite",
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
					Classes = new []
					{
						"stbi__write_context"
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

				data = Utility.ReplaceNativeCalls(data);

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

				File.WriteAllText(@"..\..\..\..\..\StbSharp\StbImageWrite.Generated.cs", data);
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