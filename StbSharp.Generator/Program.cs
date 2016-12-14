using System;
using Sichem;

namespace Generator
{
	class Program
	{
		static void Process()
		{
			var parameters = new ConversionParameters
			{
				InputPath = @"D:\Projects\StbSharp\StbSharp.Generator\StbSource\stb_image.h",
				OutputPath = @"..\..\..\StbSharp\Image.Generated.cs",
				Defines = new[]
				{
					"STBI_NO_SIMD",
					"STBI_NO_LINEAR",
					"STBI_NO_HDR",
					"STBI_NO_STDIO",
					"STB_IMAGE_IMPLEMENTATION",
				},
				Namespace = "StbSharp",
				Class = "Image",
				SkipStructs = new[]
				{
					"stbi_io_callbacks",
					"img_comp",
					"stbi__jpeg",
					"stbi__resample"
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
					"stbi_is_hdr_from_callbacks"
				}
			};

			var cp = new ClangParser();

			cp.Process(parameters);
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