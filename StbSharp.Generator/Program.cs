using System;
using System.IO;
using Sichem;

namespace Generator
{
	class Program
	{
		static void Process()
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
						"STBI_NO_STDIO",
						"STB_IMAGE_IMPLEMENTATION",
						"STBI_NO_PNM",
						"STBI_NO_PIC",
						"STBI_NO_GIF",
						"STBI_NO_PSD",
						"STBI_NO_TGA",
						"STBI_NO_BMP",
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
				var data = output.ToString();

				// Post processing
				Logger.Info("Post processing...");

				data = data.Replace("s.io.read = Pointer<IntPtr>.Null;",
					"s.io.read = null;");
				data = data.Replace("memset(data, (int)(0), (ulong)(64 * (short)(data[0]).Size));", "memset(data, 0, 64);");
				data = data.Replace("Pointer<stbi__jpeg> j = stbi__malloc((ulong)(.Size));", "Pointer<stbi__jpeg> j = new NormalPointer<stbi__jpeg>(new stbi__jpeg());");
				data = data.Replace("Pointer<stbi__jpeg> j = (stbi__malloc((ulong)(.Size)));", "Pointer<stbi__jpeg> j = new NormalPointer<stbi__jpeg>(new stbi__jpeg());");

				File.WriteAllText(@"..\..\..\StbSharp\Image.Generated.cs", data);
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