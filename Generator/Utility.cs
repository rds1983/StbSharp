using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ClangSharp;

namespace Generator
{
	public static class Utility
	{
		private static readonly string[] _binaryOperators = {
			"<<=",
			">>=",
			"^=",
			"&=",
			"|=",
			"*=",
			"/=",
			"+=",
			"-=",
			"==",
			"!=",
			">=",
			"<=",
			">",
			"<",
			"=",
			"&&",
			"||",
			"-",
			"+",
			"*",
			"/",
			"^",
			">>",
			"<<",
			"&",
			"|",
			","
		};

		private static readonly string[] _unaryOperators =
		{
			"++",
			"--",
			"!",
			"-",
			"*",
			"&",
			"~"
		};

		public static bool IsInSystemHeader(this CXCursor cursor)
		{
			return clang.Location_isInSystemHeader(clang.getCursorLocation(cursor)) != 0;
		}

		public static bool IsPtrToConstChar(this CXType type)
		{
			var pointee = clang.getPointeeType(type);

			if (clang.isConstQualifiedType(pointee) != 0)
			{
				switch (pointee.kind)
				{
					case CXTypeKind.CXType_Char_S:
						return true;
				}
			}

			return false;
		}

		public static string ToPlainTypeString(this CXType type)
		{
			var canonical = clang.getCanonicalType(type);
			switch (type.kind)
			{
				case CXTypeKind.CXType_Bool:
					return "bool";
				case CXTypeKind.CXType_UChar:
				case CXTypeKind.CXType_Char_U:
					return "byte";
				case CXTypeKind.CXType_SChar:
				case CXTypeKind.CXType_Char_S:
					return "sbyte";
				case CXTypeKind.CXType_UShort:
					return "ushort";
				case CXTypeKind.CXType_Short:
					return "short";
				case CXTypeKind.CXType_Float:
					return "float";
				case CXTypeKind.CXType_Double:
					return "double";
				case CXTypeKind.CXType_Int:
					return "int";
				case CXTypeKind.CXType_UInt:
					return "uint";
				case CXTypeKind.CXType_Pointer:
				case CXTypeKind.CXType_NullPtr: // ugh, what else can I do?
					return "IntPtr";
				case CXTypeKind.CXType_Long:
					return "int";
				case CXTypeKind.CXType_ULong:
					return "int";
				case CXTypeKind.CXType_LongLong:
					return "long";
				case CXTypeKind.CXType_ULongLong:
					return "ulong";
				case CXTypeKind.CXType_Void:
					return "void";
				case CXTypeKind.CXType_Unexposed:
					if (canonical.kind == CXTypeKind.CXType_Unexposed)
					{
						return clang.getTypeSpelling(canonical).ToString();
					}
					return canonical.ToPlainTypeString();
				default:
					return type.ToString();
			}
		}

		public static CXType Desugar(this CXType type)
		{
			if (type.kind == CXTypeKind.CXType_Typedef)
			{
				return clang.getCanonicalType(type);
			}

			return type;
		}

		private static void ProcessPointerType(CXType type, TextWriter writer)
		{
			type = type.Desugar();

			if (type.kind == CXTypeKind.CXType_Void)
			{
				writer.Write("IntPtr");
				return;
			}

			if (type.kind != CXTypeKind.CXType_Record)
			{
				writer.Write("Pointer<");
			}

			CommonTypeHandling(type, writer);

			if (type.kind != CXTypeKind.CXType_Record)
			{
				writer.Write(">");
			}
		}

		public static void CommonTypeHandling(CXType type, TextWriter writer)
		{
			bool isConstQualifiedType = clang.isConstQualifiedType(type) != 0;
			var spelling = string.Empty;

			type = type.Desugar();
			switch (type.kind)
			{
				case CXTypeKind.CXType_Record:
				case CXTypeKind.CXType_Enum:
					spelling = clang.getTypeSpelling(type).ToString();
					break;
				case CXTypeKind.CXType_IncompleteArray:
					CommonTypeHandling(clang.getArrayElementType(type), writer);
					spelling = "[]";
					break;
				case CXTypeKind.CXType_Unexposed: // Often these are enums and canonical type gets you the enum spelling
					var canonical = clang.getCanonicalType(type);
					// unexposed decl which turns into a function proto seems to be an un-typedef'd fn pointer
					spelling = canonical.kind == CXTypeKind.CXType_FunctionProto ? "IntPtr" : clang.getTypeSpelling(canonical).ToString();
					break;
				case CXTypeKind.CXType_ConstantArray:
					ProcessPointerType(clang.getArrayElementType(type), writer);
					break;
				case CXTypeKind.CXType_Pointer:
					ProcessPointerType(clang.getPointeeType(type), writer);
					break;
				default:
					spelling = clang.getCanonicalType(type).ToPlainTypeString();
					break;
			}

			if (isConstQualifiedType)
			{
				spelling = spelling.Replace("const ", string.Empty); // ugh
			}

			writer.Write(spelling);
		}

		public static string FixSpecialWords(this string name)
		{
			return name.Replace("out", "output");
		}

		private static CXCursorKind _hasChildKind;
		private static CXCursor? _findChildResult;

		private static CXChildVisitResult HasChildrenChecker(CXCursor cursor, CXCursor parent, IntPtr data)
		{
			if (clang.getCursorKind(cursor) == _hasChildKind)
			{
				_findChildResult = cursor;
				return CXChildVisitResult.CXChildVisit_Break;
			}

			return CXChildVisitResult.CXChildVisit_Recurse;
		}

		public static CXCursor? FindChild(this CXCursor cursor, CXCursorKind kind)
		{
			_hasChildKind = kind;
			_findChildResult = null;

			clang.visitChildren(cursor, HasChildrenChecker, new CXClientData(IntPtr.Zero));

			return _findChildResult;
		}

		public static string[] Tokenize(this CXCursor cursor, CXTranslationUnit translationUnit)
		{
			var range = clang.getCursorExtent(cursor);
			IntPtr nativeTokens;
			uint numTokens;
			clang.tokenize(translationUnit, range, out nativeTokens, out numTokens);

			var result = new List<string>();
			var tokens = new CXToken[numTokens];
			for (uint i = 0; i < numTokens; ++i)
			{
				tokens[i] = (CXToken)Marshal.PtrToStructure(nativeTokens, typeof(CXToken));
				nativeTokens += Marshal.SizeOf(typeof(CXToken));

				var name = clang.getTokenSpelling(translationUnit, tokens[i]).ToString();
				result.Add(name);
			}

			return result.ToArray();
		}

		public static string GetBinaryOperatorType(this CXCursor cursor, CXTranslationUnit translationUnit)
		{
			var tokens = cursor.Tokenize(translationUnit);

			if (tokens.Length == 0)
			{
				return string.Empty;
			}

			foreach (var t in tokens)
			{
				var bo = _binaryOperators.FirstOrDefault(b => b == t);

				if (bo != null)
				{
					return bo;
				}
			}

			throw new Exception(string.Format("Could not determine operator type for cursor {0}", clang.getCursorSpelling(cursor)));
		}

		public static string GetUnaryOperatorType(this CXCursor cursor, CXTranslationUnit translationUnit)
		{
			var tokens = cursor.Tokenize(translationUnit);

			if (tokens.Length == 0)
			{
				return string.Empty;
			}

			foreach (var t in tokens)
			{
				var bo = _unaryOperators.FirstOrDefault(b => b == t);

				if (bo != null)
				{
					return bo;
				}
			}

			throw new Exception(string.Format("Could not determine operator type for cursor {0}", clang.getCursorSpelling(cursor)));
		}

		public static string EnsureStatementFinished(this string statement)
		{
			if (!statement.EndsWith(";"))
			{
				return statement + ";";
			}

			return statement;
		}

	}
}