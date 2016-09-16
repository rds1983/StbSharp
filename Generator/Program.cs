using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using CppNet;

namespace Generator
{
	class Program
	{
		private static string[] Structs = {
			"stbi_io_callbacks",
			"stbi__context"
		};

		static void Process()
		{
			var executableLocation = System.Reflection.Assembly.GetEntryAssembly().Location;
			var executableFolder = Path.GetDirectoryName(executableLocation);

			var outputPath = Path.Combine(executableFolder, @"..\..\..\StbSharp\Image.Generated.cs");

			var data = Resources.stb_image;
			
			// Remove includes
			data = Regex.Replace(data, @"^(.*)#include(.*)$", string.Empty, RegexOptions.Multiline);

			var sb = new StringBuilder();
			
			// Preprocess
			Console.WriteLine("Preprocessing...");
			using (var pp = new Preprocessor())
			{
				pp.addMacro("_WIN32");
				pp.addMacro("_MSC_VER");
				pp.addMacro("STBI_NO_SIMD");
				pp.addMacro("STBI_NO_LINEAR");
				pp.addMacro("STBI_NO_HDR");
				pp.addMacro("STBI_NO_STDIO");
				pp.addMacro("STB_IMAGE_IMPLEMENTATION");
				pp.getSystemIncludePath().Add(@"C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\include");
				pp.setListener(new MyPreprocessorListener());

				pp.addInput(new StringLexerSource(data, true));

				var index = 0;
				while (true)
				{
					var token = pp.token();

				 	if (token == null || token.getType() == Token.EOF)
					{
						Console.WriteLine("{0} tokens processed.", index);
						break;
					}


					sb.Append(token.getText());
					++index;
				}
			}

			data = sb.ToString();

			Console.WriteLine("Parsing...");
			var input = new AntlrInputStream(data);
			var lexer = new CLexer(input);
			var tokens = new CommonTokenStream(lexer);
			var parser = new CParser(tokens);

			parser.AddErrorListener(new MyErrorListener());
			var tu = parser.translationUnit();

			Console.WriteLine("Generating code...");
			var codeGen = new CodeGenerator();
			data = codeGen.Generate(tu);

			Console.WriteLine("Postprocessing...");
			
			data = data.Replace("(byte *)", string.Empty);
			data = data.Replace("byte *", "Pointer<byte> ");
			data = data.Replace("*c;", "c;");
			data = data.Replace("unsigned char", "byte");
			data = data.Replace("(void)", "()");
			data = data.Replace("(void )", "()");
			data = data.Replace("size_t", "int");
			data = data.Replace("char *", "string ");
			data = data.Replace("__forceinline ", string.Empty);
			data = data.Replace("sizeof(s.buffer_start)", "s.buffer_start.Size");

			foreach (var s in Structs)
			{
				data = data.Replace(s + " *", s + " ");
			}

			Console.WriteLine("Writing output data...");
			using (var writer = new StreamWriter(outputPath))
			{
				writer.Write(data);
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
