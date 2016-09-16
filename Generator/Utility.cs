using Antlr4.Runtime.Tree;

namespace Generator
{
	public static class Utility
	{
		public static bool HasRule(this IParseTree tree, string name)
		{
			if (tree.ChildCount == 0)
			{
				return tree.GetText() == name;
			}

			for (var i = 0; i < tree.ChildCount; ++i)
			{
				var child = tree.GetChild(i);

				if (child.HasRule(name))
				{
					return true;
				}
			}

			return false;
		}
	}
}
