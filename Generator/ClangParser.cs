using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClangSharp;

namespace Generator
{
	public class ClangParser
	{
		private TextWriter _writer;

		public void Process()
		{
			var defines = new[]
			{
				"STBI_NO_SIMD",
				"STBI_NO_LINEAR",
				"STBI_NO_HDR",
				"STBI_NO_STDIO",
				"STB_IMAGE_IMPLEMENTATION",
			};

			var arr = new List<string>();

			foreach (var d in defines)
			{
				arr.Add("-D" + d);
			}

			var createIndex = clang.createIndex(0, 0);
			CXUnsavedFile unsavedFile;

			CXTranslationUnit tu;
			var res = clang.parseTranslationUnit2(createIndex,
				@"D:\Projects\StbSharp\Generator\StbSource\stb_image.h",
				arr.ToArray(),
				arr.Count,
				out unsavedFile,
				0,
				0,
				out tu);

			if (res != CXErrorCode.CXError_Success)
			{
				var sb = new StringBuilder();

				sb.AppendLine(res.ToString());

				var numDiagnostics = clang.getNumDiagnostics(tu);
				for (uint i = 0; i < numDiagnostics; ++i)
				{
					var diag = clang.getDiagnostic(tu, i);
					sb.AppendLine(clang.getDiagnosticSpelling(diag).ToString());
					clang.disposeDiagnostic(diag);
				}

				throw new Exception(sb.ToString());
			}

			var executableLocation = System.Reflection.Assembly.GetEntryAssembly().Location;
			var executableFolder = Path.GetDirectoryName(executableLocation);

			var outputPath = Path.Combine(executableFolder, @"..\..\..\StbSharp\Image.Generated.cs");

			using (_writer = new StreamWriter(outputPath))
			{
				_writer.WriteLine("using System;");
				_writer.WriteLine();
				_writer.Write("namespace StbSharp\n{\n\tpublic static partial class Image\n\t{\n");

				// Structs
				var structsVisitor = new StructVisitor(tu, _writer);
				structsVisitor.Run();

				var functionVisitor = new FunctionVisitor(tu, _writer);
				functionVisitor.Run();

				_writer.Write("\t}\n}\n");
			}

/*			using (_writer = new StreamWriter(outputPath + "2"))
			{
				var data = new CXClientData((IntPtr) 0);
				clang.visitChildren(clang.getTranslationUnitCursor(tu), Visit, data);
			}*/

			clang.disposeTranslationUnit(tu);
			clang.disposeIndex(createIndex);
		}

		string getCursorKindName(CXCursorKind cursorKind)
		{
			var kindName = clang.getCursorKindSpelling(cursorKind);
			var result = kindName.ToString();

			clang.disposeString(kindName);
			return result;
		}

		string getCursorSpelling(CXCursor cursor)
		{
			var cursorSpelling = clang.getCursorSpelling(cursor);
			var result = cursorSpelling.ToString();

			clang.disposeString(cursorSpelling);
			return result;
		}

		private CXChildVisitResult Visit(CXCursor cursor, CXCursor parent, IntPtr data)
		{
			var location = clang.getCursorLocation(cursor);
			if (clang.Location_isFromMainFile(location) == 0)
				return CXChildVisitResult.CXChildVisit_Continue;

			var cursorKind = clang.getCursorKind(cursor);

			var curLevel = (uint) data;
			var nextLevel = curLevel + 1;

			_writer.WriteLine("{0}- {1} {2})\n", curLevel, getCursorKindName(cursorKind), getCursorSpelling(cursor));

			clang.visitChildren(cursor,
				Visit,
				new CXClientData((IntPtr) nextLevel));

			return CXChildVisitResult.CXChildVisit_Continue;
		}
	}
}