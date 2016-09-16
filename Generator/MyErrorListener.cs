using System;
using Antlr4.Runtime;

namespace Generator
{
	public class MyErrorListener: IAntlrErrorListener<IToken>
	{
		public void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg,
			RecognitionException e)
		{
			Console.WriteLine("Error({0}, {1}): {2}", line, charPositionInLine, msg);
			throw e;
		}
	}
}
