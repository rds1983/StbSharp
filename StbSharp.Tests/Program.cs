using System;
using System.IO;
using System.Linq;
using Sichem;
using StbNative;

namespace StbSharp.Tests
{
	internal static class Program
	{
		public static void Log(string message)
		{
			Console.WriteLine(message);
		}

		public static void Log(string format, params object[] args)
		{
			Log(string.Format(format, args));
		}

		public static bool RunTests()
		{
			try
			{
				var stbSharpLoading = 0;
				var stbNativeLoading = 0;
				var imagesPath = "..\\..\\..\\TestImages";

				var files = Directory.EnumerateFiles(imagesPath, "*.*", SearchOption.AllDirectories).ToArray();
				Log("Files count: {0}", files.Length);
				int filesProcessed = 0;
				foreach (var f in files)
				{
					if (!f.EndsWith(".bmp") && !f.EndsWith(".jpg") && !f.EndsWith(".png") &&
						!f.EndsWith(".jpg") && !f.EndsWith(".psd") && !f.EndsWith(".pic") && 
						!f.EndsWith(".tga"))
					{
						continue;
					}

					Log(string.Empty);
					Log("#{0}: Loading {1} into memory", filesProcessed, f);
					var data = File.ReadAllBytes(f);
					Log("----------------------------");

					Log("Parsing with StbSharp");
					var stamp = DateTime.Now;
					int x, y, comp;
					var parsed = Stb.stbi_load_from_memory(data, out x, out y, out comp, Stb.STBI_default);
					Log("x: {0}, y: {1}, comp: {2}, size: {3}", x, y, comp, parsed.Length);
					var passed = DateTime.Now - stamp;
					Log("Span: {0} ms", passed.TotalMilliseconds);
					stbSharpLoading += (int)passed.TotalMilliseconds;

					Log("Parsing with Stb.Native");
					stamp = DateTime.Now;
					int x2, y2, comp2;
					var parsed2 = Native.load_from_memory(data, out x2, out y2, out comp2, Stb.STBI_default);
					Log("x: {0}, y: {1}, comp: {2}, size: {3}", x2, y2, comp2, parsed2.Length);
					passed = DateTime.Now - stamp;
					Log("Span: {0} ms", passed.TotalMilliseconds);
					stbNativeLoading += (int)passed.TotalMilliseconds;

					if (x != x2)
					{
						throw new Exception(string.Format("Inconsistent x: StbSharp={0}, Stb.Native={1}", x, x2));
					}

					if (y != y2)
					{
						throw new Exception(string.Format("Inconsistent y: StbSharp={0}, Stb.Native={1}", y, y2));
					}

					if (comp != comp2)
					{
						throw new Exception(string.Format("Inconsistent comp: StbSharp={0}, Stb.Native={1}", comp, comp2));
					}

					if (parsed.Length != parsed2.Length)
					{
						throw new Exception(string.Format("Inconsistent parsed length: StbSharp={0}, Stb.Native={1}", parsed.Length,
							parsed2.Length));
					}

					for (var i = 0; i < parsed.Length; ++i)
					{
						if (parsed[i] != parsed2[i])
						{
							throw new Exception(string.Format("Inconsistent data: index={0}, StbSharp={1}, Stb.Native={2}",
								i,
								(int) parsed[i],
								(int) parsed2[i]));
						}
					}

					for (var k = 0; k <= 3; ++k)
					{
						Log("Saving as {0} with StbSharp", ((Stb.ImageWriterType) k).ToString());
						byte[] save;
						stamp = DateTime.Now;
						using (var stream = new MemoryStream())
						{
							Stb.stbi_write_to(parsed, x, y, comp, (Stb.ImageWriterType) k, stream);
							save = stream.ToArray();
						}
						passed = DateTime.Now - stamp;
						Log("Span: {0} ms", passed.TotalMilliseconds);
						Log("StbSharp Size: {0}", save.Length);

						Log("Saving as {0} with Stb.Native", ((Stb.ImageWriterType)k).ToString());
						stamp = DateTime.Now;
						byte[] save2;
						using (var stream = new MemoryStream())
						{
							Native.save_to_memory(parsed, x, y, comp, k, stream);
							save2 = stream.ToArray();
						}

						passed = DateTime.Now - stamp;
						Log("Span: {0} ms", passed.TotalMilliseconds);
						Log("Stb.Native Size: {0}", save2.Length);

						if (save.Length != save2.Length)
						{
							throw new Exception(string.Format("Inconsistent output size: StbSharp={0}, Stb.Native={1}",
								save.Length, save2.Length));
						}

						for (var i = 0; i < save.Length; ++i)
						{
							if (save[i] != save2[i])
							{
								throw new Exception(string.Format("Inconsistent data: index={0}, StbSharp={1}, Stb.Native={2}",
									i,
									(int) save[i],
									(int) save2[i]));
							}
						}
					}

					++filesProcessed;

					Log("Total StbSharp Loading Time: {0} ms", stbSharpLoading);
					Log("Total Stb.Native Loading Time: {0} ms", stbNativeLoading);

					GC.Collect();
					Log(string.Format("Sichem Allocated: {0}", Operations.AllocatedTotal));
				}

				Log("Files processed: {0}", filesProcessed);
			}
			catch (Exception ex)
			{
				Log(ex.Message);
				return false;
			}

			return true;
		}

		public static int Main(string[] args)
		{
			var start = DateTime.Now;
			var res = RunTests();
			var passed = DateTime.Now - start;
			Log("Span: {0} ms", passed.TotalMilliseconds);
			Log(res ? "Success" : "Failure");

			return res ? 1 : 0;
		}
	}
}