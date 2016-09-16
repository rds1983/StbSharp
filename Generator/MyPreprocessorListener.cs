using System;
using CppNet;

namespace Generator
{
	public class MyPreprocessorListener: PreprocessorListener
	{
		public void handleWarning(Source source, int line, int column, string msg)
		{
			Console.WriteLine("Warning: " + msg);
		}

		public void handleError(Source source, int line, int column, string msg)
		{
			Console.WriteLine("Error: " + msg);
		}

		public void handleSourceChange(Source source, string ev)
		{
			Console.WriteLine("Source Change: " + ev);
		}
	}
}
