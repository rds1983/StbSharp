using System;
using System.IO;
using Sichem;

namespace StbSharp.StbImageResize.Generator
{
	class Program
	{
		static void Process()
		{
			using (var output = new StringWriter())
			{
				var parameters = new ConversionParameters
				{
					InputPath = @"stb_image_resize.h",
					Output = output,
					Defines = new[]
					{
						"STB_IMAGE_RESIZE_IMPLEMENTATION"
					},
					Namespace = "StbSharp",
					Class = "StbImageResize",
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
					Classes = new[]
					{
						"stbir__filter_info",
						"stbir__info"
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

				data = Utility.ReplaceNativeCalls(data);

				File.WriteAllText(@"..\..\..\..\..\StbSharp\StbImageResize.Generated.cs", data);
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