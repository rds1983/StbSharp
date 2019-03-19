using System;
using System.IO;
using Sichem;

namespace StbSharp.StbDxt.Generator
{
	class Program
	{
		static void Process()
		{
			using (var output = new StringWriter())
			{
				var parameters = new ConversionParameters
				{
					InputPath = @"stb_dxt.h",
					Output = output,
					Defines = new[]
					{
						"STB_DXT_IMPLEMENTATION"
					},
					Namespace = "StbSharp",
					Class = "StbDxt",
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
					Classes = new string[]
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

				data = Utility.ReplaceNativeCalls(data);

				data = data.Replace("byte* minp;", "byte* minp = null;");
				data = data.Replace("byte* maxp;", "byte* maxp = null;");
				data = data.Replace("public static void stb__PrepareOptTable(byte* Table, byte* expand, int size)",
					"public static void stb__PrepareOptTable(byte[] Table, byte[] expand, int size)");

				File.WriteAllText(@"..\..\..\..\..\StbSharp\StbDxt.Generated.cs", data);
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