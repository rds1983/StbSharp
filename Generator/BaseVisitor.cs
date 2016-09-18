using System;
using System.IO;
using ClangSharp;

namespace Generator
{
	public abstract class BaseVisitor
	{
		protected readonly CXTranslationUnit _translationUnit;
		protected readonly TextWriter _writer;
		protected int _indentLevel = 2;

		protected BaseVisitor(CXTranslationUnit translationUnit, TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}

			_translationUnit = translationUnit;
			_writer = writer;
		}

		public void Run()
		{
			clang.visitChildren(clang.getTranslationUnitCursor(_translationUnit), InternalVisit, new CXClientData(IntPtr.Zero));
		}

		protected abstract CXChildVisitResult InternalVisit(CXCursor cursor, CXCursor parent, IntPtr data);

		protected void WriteIndent()
		{
			for (var i = 0; i < _indentLevel; ++i)
			{
				_writer.Write("\t");
			}
		}

		protected void IndentedWriteLine(string line)
		{
			WriteIndent();
			_writer.WriteLine(line);
		}

		protected void IndentedWrite(string data)
		{
			WriteIndent();
			_writer.Write(data);
		}

		private CXChildVisitResult DumpCursor(CXCursor cursor, CXCursor parent, IntPtr data)
		{
			var cursorKind = clang.getCursorKind(cursor);

			IndentedWriteLine(string.Format("// {0}- {1}", clang.getCursorKindSpelling(cursorKind),
				clang.getCursorSpelling(cursor)));

			_indentLevel++;
			clang.visitChildren(cursor, DumpCursor, new CXClientData(IntPtr.Zero));
			_indentLevel--;

			return CXChildVisitResult.CXChildVisit_Continue;
		}

		protected void DumpCursor(CXCursor cursor)
		{
			clang.visitChildren(cursor, DumpCursor, new CXClientData(IntPtr.Zero));
		}
	}
}
