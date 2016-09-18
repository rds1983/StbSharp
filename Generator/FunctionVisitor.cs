using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ClangSharp;

namespace Generator
{
	public class FunctionVisitor : BaseVisitor
	{
		private static readonly string[] _toSkip = {
			"stbi__malloc",
			"stbi_image_free"
		};

		private CXCursor _functionStatement;
		private readonly Stack<Stack<string>> _expressionStacks = new Stack<Stack<string>>();
		private int _level;
		private string _functionName;

		private Stack<string> TopExpressionStack
		{
			get { return _expressionStacks.Peek(); }
		}

		public FunctionVisitor(CXTranslationUnit translationUnit, TextWriter writer)
			: base(translationUnit, writer)
		{
		}

		protected override CXChildVisitResult InternalVisit(CXCursor cursor, CXCursor parent, IntPtr data)
		{
			if (cursor.IsInSystemHeader())
			{
				return CXChildVisitResult.CXChildVisit_Continue;
			}

			var curKind = clang.getCursorKind(cursor);

			// look only at function decls
			if (curKind == CXCursorKind.CXCursor_FunctionDecl)
			{
				// Skip empty declarations
				var body = cursor.FindChild(CXCursorKind.CXCursor_CompoundStmt);
				if (!body.HasValue)
				{
					return CXChildVisitResult.CXChildVisit_Continue;
				}

				_functionStatement = body.Value;
				
				_functionName = clang.getCursorSpelling(cursor).ToString();

				if (_toSkip.Contains(_functionName))
				{
					return CXChildVisitResult.CXChildVisit_Continue;
				}

				Logger.Info("Processing function {0}", _functionName);

				ProcessFunction(cursor);
			}

			return CXChildVisitResult.CXChildVisit_Recurse;
		}

		private CXChildVisitResult VisitFunctionBody(CXCursor cursor, CXCursor parent, IntPtr data)
		{
			var cursorKind = clang.getCursorKind(cursor);

			// Process children
			_level++;

			// Create new expression stack
			var childStack = new Stack<string>();
			_expressionStacks.Push(childStack);

			clang.visitChildren(cursor, VisitFunctionBody, new CXClientData(IntPtr.Zero));
			_level--;

			_expressionStacks.Pop();

			var spelling = clang.getCursorSpelling(cursor).ToString();
			var expressionStack = TopExpressionStack;

			switch (cursorKind)
			{
				case CXCursorKind.CXCursor_UnexposedExpr:
				{
					var tokens = cursor.Tokenize(_translationUnit);

					var expr = string.Empty;
					if (childStack.Count == 0)
					{
						expr = string.Join(" ", tokens);
					}
					else
					{
						expr = childStack.Pop();
					}
/*					if (tokens.Contains("sizeof") && !expr.EndsWith(".Size"))
					{
						expr = expr + ".Size";
					}*/
					expressionStack.Push(expr);
				}
					break;
				case CXCursorKind.CXCursor_DeclRefExpr:
					if (!string.IsNullOrEmpty(spelling))
					{
						expressionStack.Push(spelling);
					}
					break;
				case CXCursorKind.CXCursor_CompoundAssignOperator:
				case CXCursorKind.CXCursor_BinaryOperator:
				{
					var a = childStack.Pop();
					var b = childStack.Pop();
					var type = cursor.GetBinaryOperatorType(_translationUnit);

					expressionStack.Push(b + " " + type + " " + a);
				}
					break;
				case CXCursorKind.CXCursor_UnaryOperator:
				{
					var a = childStack.Pop();
					var type = cursor.GetUnaryOperatorType(_translationUnit);

					if (type == "*" || type == "&")
					{
						// Skip all pointer staff
						type = string.Empty;
					}

					expressionStack.Push(type + a);
				}
					break;
				case CXCursorKind.CXCursor_CallExpr:
				{
					var sb = new StringBuilder();

					// Retrieve arguments
					var args = new List<string>();
					while (childStack.Count > 1)
					{
						args.Add(childStack.Pop());
					}
					args.Reverse();

					// Pop function name from the stack
					var functionName = childStack.Pop();

					sb.Clear();
					sb.Append(functionName + "(");
					sb.Append(string.Join(", ", args));
					sb.Append(")");

					expressionStack.Push(sb.ToString());
				}
					break;
				case CXCursorKind.CXCursor_ReturnStmt:
				{
					var ret = string.Empty;
					if (childStack.Count > 0)
					{
						ret = childStack.Pop();
					}

					var exp = string.IsNullOrEmpty(ret) ? "return" : "return " + ret;
					expressionStack.Push(exp);
				}
					break;
				case CXCursorKind.CXCursor_IfStmt:
				{
					var elseBlock = string.Empty;
					if (childStack.Count > 2)
					{
						elseBlock = childStack.Pop();
					}

					var executionBlock = childStack.Pop();

					if (childStack.Count == 0)
					{
						var k = 5;
					}

					var condition = childStack.Pop();

					var expr = "if (" + condition + ") " + executionBlock;

					if (!string.IsNullOrEmpty(elseBlock))
					{
						expr += " else " + elseBlock;
					}

					expressionStack.Push(expr);
				}
					break;
				case CXCursorKind.CXCursor_ForStmt:
				{
					var execution = childStack.Pop();

					var start = string.Empty;
					var condition = string.Empty;
					var it = string.Empty;

					switch (childStack.Count)
					{
						case 3:
							it = childStack.Pop();
							condition = childStack.Pop();
							start = childStack.Pop();
							break;
						case 2:
							it = childStack.Pop();
							start = childStack.Pop();
							break;
						case 1:
							start = childStack.Pop();
							break;
					}
				
					var exp = "for (" + start + "; " + condition + "; " + it + ")" + execution;
					expressionStack.Push(exp);
				}
					break;

				case CXCursorKind.CXCursor_LabelRef:
					expressionStack.Push(spelling);
					break;
				case CXCursorKind.CXCursor_GotoStmt:
				{
					var label = childStack.Pop();

					var exp = "goto " + label;
					expressionStack.Push(exp);
				}
					break;

				case CXCursorKind.CXCursor_LabelStmt:
				{
					var sb = new StringBuilder();

					sb.Append(spelling);
					sb.Append(":;\n");

					childStack = new Stack<string>(childStack);
					while (childStack.Count > 0)
					{
						sb.Append(childStack.Pop());
					}

					expressionStack.Push(sb.ToString());
				}
					break;

				case CXCursorKind.CXCursor_ConditionalOperator:
				{
					var a = childStack.Pop();
					var b = childStack.Pop();
					var condition = childStack.Pop();

					var expr = condition + "?" + a + ":" + b;

					expressionStack.Push(expr);
				}
					break;
				case CXCursorKind.CXCursor_MemberRefExpr:
				{
					var a = childStack.Pop();
					expressionStack.Push(a + "." + spelling);
				}
					break;
				case CXCursorKind.CXCursor_IntegerLiteral:
				case CXCursorKind.CXCursor_FloatingLiteral:
				{
					var tokens = cursor.Tokenize(_translationUnit);
					var t = string.Empty;
					if (tokens.Length > 0)
					{
						t = tokens[0];
					}

					if (string.IsNullOrEmpty(t))
					{
						t = "null";
					}
					expressionStack.Push(t);
					break;
				}
				case CXCursorKind.CXCursor_CharacterLiteral:
				case CXCursorKind.CXCursor_StringLiteral:
					expressionStack.Push(spelling);
					break;
				case CXCursorKind.CXCursor_VarDecl:
				{
					var rvalue = string.Empty;

					if (childStack.Count > 0)
					{
						rvalue = childStack.Pop();
					}

					string expr = !string.IsNullOrEmpty(rvalue) ? expr = "var " + spelling + " = " + rvalue : spelling;

					expressionStack.Push(expr);
				}
					break;
				case CXCursorKind.CXCursor_DeclStmt:
				{
					var tokens = cursor.Tokenize(_translationUnit);

					var sb = new StringBuilder();
					while (childStack.Count > 0)
					{
						var exp = childStack.Pop();

						if (!exp.StartsWith("var"))
						{
							// Determine type from tokens
							exp = tokens[0] + " " + exp;
						}

						exp = exp.EnsureStatementFinished() + "\n";
						sb.Append(exp);
					}

					expressionStack.Push(sb.ToString());
				}
					break;
				case CXCursorKind.CXCursor_CompoundStmt:
				{
					var sb = new StringBuilder();
					sb.Append("{\n");

					// Reverse stack
					childStack = new Stack<string>(childStack);

					// Proces it
					while (childStack.Count > 0)
					{
						var exp = childStack.Pop();
						sb.Append(exp.EnsureStatementFinished());
					}

					sb.Append("}\n");

					var fullExp = sb.ToString();
					expressionStack.Push(fullExp);
				}
					break;

				case CXCursorKind.CXCursor_ArraySubscriptExpr:
				{
					var expr = childStack.Pop();
					var var = childStack.Pop();

					var exp = var + "[" + expr + "]";
					expressionStack.Push(exp);
				}
					break;

				case CXCursorKind.CXCursor_InitListExpr:
				{
					var sb = new StringBuilder();

					sb.Append("{ ");
					while (childStack.Count > 0)
					{
						sb.Append(childStack.Pop());

						if (childStack.Count > 0)
						{
							sb.Append(", ");
						}
					}

					sb.Append(" }");
					expressionStack.Push(sb.ToString());
				}

					break;

				case CXCursorKind.CXCursor_ParenExpr:
				{
					var expr = string.Empty;
					if (childStack.Count > 1)
					{
						expr = childStack.Pop();
					}

					expr = "(" + expr + ")";
					expressionStack.Push(expr);
				}
					break;

				case CXCursorKind.CXCursor_BreakStmt:
					expressionStack.Push("break");
					break;

				default:
				{
					if (childStack.Count > 0)
					{
						var expr = childStack.Pop();
						expressionStack.Push(expr);
					}
				}

					break;
			}

			if (_level == 0 && expressionStack.Count > 0)
			{
				// Dump expressions on stack
				var reverted = expressionStack.Reverse();

				foreach (var r in reverted)
				{
					IndentedWriteLine(r + ";");
				}

				expressionStack.Clear();
			}

			return CXChildVisitResult.CXChildVisit_Continue;
		}

		private void ProcessFunction(CXCursor cursor)
		{
			WriteFunctionStart(cursor);

			_indentLevel++;

			var expressionStack = new Stack<string>();
			_expressionStacks.Push(expressionStack);
			clang.visitChildren(_functionStatement, VisitFunctionBody, new CXClientData(IntPtr.Zero));
			_expressionStacks.Pop();

			_indentLevel--;

			IndentedWriteLine("}");
			_writer.WriteLine();
		}

		private void WriteFunctionStart(CXCursor cursor)
		{
			var functionType = clang.getCursorType(cursor);
			var functionName = clang.getCursorSpelling(cursor).ToString();
			var resultType = clang.getCursorResultType(cursor);

			IndentedWrite("private static ");

			Utility.CommonTypeHandling(resultType, _writer);

			_writer.Write(" " + functionName + "(");

			var numArgTypes = clang.getNumArgTypes(functionType);
			for (uint i = 0; i < numArgTypes; ++i)
			{
				ArgumentHelper(functionType, clang.Cursor_getArgument(cursor, i), i);
			}

			_writer.WriteLine(")");
			IndentedWriteLine("{");
		}

		private void ArgumentHelper(CXType functionType, CXCursor paramCursor, uint index)
		{
			var numArgTypes = clang.getNumArgTypes(functionType);
			var type = clang.getArgType(functionType, index);

			var spelling = clang.getCursorSpelling(paramCursor).ToString();

			Utility.CommonTypeHandling(type, _writer);

			_writer.Write(" ");

			spelling = spelling.FixSpecialWords();
			_writer.Write(spelling);

			if (index != numArgTypes - 1)
			{
				_writer.Write(", ");
			}
		}
	}
}