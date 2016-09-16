using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Generator
{
	public class CodeGenerator
	{
		private readonly StringBuilder _output = new StringBuilder();

		private static readonly HashSet<string> _lineBreaks = new HashSet<string>(new[]
		{
			";",
			"{",
			"}",
			"else"
		});

		private static readonly HashSet<string> _spaces = new HashSet<string>(new[]
		{
			"typedef",
			"extern",
			"static",
			",",
			"return"
		}); 

		private static readonly HashSet<string> _skipFunctions = new HashSet<string>(new []
		{
			"stbi__malloc",
			"stbi_image_free"
		});

		private static readonly Dictionary<string, string> _replaces = new Dictionary<string, string>();



		static CodeGenerator()
		{
			_replaces["stbi__uint32"] = "uint";
			_replaces["stbi_uc"] = "byte";
			_replaces["const"] = string.Empty;
			_replaces["NULL"] = "null";
			_replaces["extern"] = "static";
			_replaces["->"] = ".";
		}

		private void ProcessTerminal(IParseTree tree, StringBuilder sb)
		{
			if (tree.ChildCount != 0) return;

			var text = tree.GetText();

			string replaced;
			sb.Append(_replaces.TryGetValue(text, out replaced) ? replaced : text);

			if (_lineBreaks.Contains(text))
			{
				sb.AppendLine();
			}

			if (_spaces.Contains(text))
			{
				sb.Append(" ");
			}
		}

		private void Process(IParseTree tree)
		{
			if (tree is CParser.DeclarationContext)
			{
				// Skip enums
				if (tree.HasRule("enum"))
				{
					return;
				}

				// Skip typedefs
				if (tree.HasRule("typedef"))
				{
					return;
				}

				if (tree.HasRule("extern") || tree.HasRule("static"))
				{
					// Skip functions declarations
					return;
				}
			}

			if (tree is CParser.FunctionDefinitionContext)
			{
				foreach (var skip in _skipFunctions)
				{
					if (tree.HasRule(skip))
					{
						return;
					}
				}

				// Prepend unsafe private
				_output.Append("private unsafe ");
			}

			for (var i = 0; i < tree.ChildCount; ++i)
			{
				var child = tree.GetChild(i);
				Process(child);
			}

			ProcessTerminal(tree, _output);


			if (tree is CParser.EnumSpecifierContext ||
				tree is CParser.FunctionDefinitionContext ||
				tree is CParser.EnumeratorListContext)
			{
				_output.AppendLine();
			}

			if (tree is CParser.TypeSpecifierContext)
			{
				_output.Append(" ");
			}
		}

		public string Generate(ParserRuleContext root)
		{
			_output.Clear();

			_output.Append("namespace StbSharp\n{\n\tpublic static partial class Image\n{\n");

			for (var i = 0; i < root.ChildCount; ++i)
			{
				var child = root.GetChild(i);

				Process(child);
			}

			_output.Append("\t}\n}\n");

			return _output.ToString();
		}
	}
}
