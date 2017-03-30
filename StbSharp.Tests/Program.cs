using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Sichem;
using StbNative;

namespace StbSharp.Tests
{
	internal static class Program
	{
		private const int LoadTries = 100;

		private static readonly Stopwatch _sw = new Stopwatch();

		public static void Log(string message)
		{
			Console.WriteLine(message);
		}

		public static void Log(string format, params object[] args)
		{
			Log(string.Format(format, args));
		}

		private static void BeginWatch()
		{
			_sw.Restart();
		}

		private static int EndWatch()
		{
			_sw.Stop();
			return (int) _sw.ElapsedMilliseconds;
		}

		private delegate byte[] LoadDelegate(out int x, out int y, out int comp);

		private static void ParseTest(LoadDelegate load1, LoadDelegate load2,
			out int load1Passed, out int load2Passed)
		{
			Log("With StbSharp");
			int x = 0, y = 0, comp = 0;
			byte[] parsed = new byte[0];
			BeginWatch();

			for (var i = 0; i < LoadTries; ++i)
			{
				parsed = load1(out x, out y, out comp);
			}

			Log("x: {0}, y: {1}, comp: {2}, size: {3}", x, y, comp, parsed.Length);
			var passed = EndWatch()/LoadTries;
			Log("Span: {0} ms", passed);
			load1Passed = passed;

			Log("With Stb.Native");
			int x2 = 0, y2 = 0, comp2 = 0;
			byte[] parsed2 = new byte[0];

			BeginWatch();
			for (var i = 0; i < LoadTries; ++i)
			{
				parsed2 = load2(out x2, out y2, out comp2);
			}
			Log("x: {0}, y: {1}, comp: {2}, size: {3}", x2, y2, comp2, parsed2.Length);
			passed = EndWatch()/LoadTries;
			Log("Span: {0} ms", passed);
			load2Passed = passed;

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
		}

		public static bool RunTests()
		{
			try
			{
				var stbSharpLoadingFromStream = 0;
				var stbNativeLoadingFromStream = 0;
				var stbSharpLoadingFromMemory = 0;
				var stbNativeLoadingFromMemory = 0;
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

					Log("Loading From Stream");
					int x = 0, y = 0, comp = 0;
					int stbSharpPassed, stbNativePassed;
					byte[] parsed = new byte[0];
					ParseTest(
						(out int xx, out int yy, out int ccomp) =>
						{
							using (var ms = new MemoryStream(data))
							{
								var loader = new ImageReader();
								var img = loader.Read(ms);

								parsed = img.Data;
								xx = img.Width;
								yy = img.Height;
								ccomp = img.SourceComp;

								x = xx;
								y = yy;
								comp = ccomp;
								return parsed;
							}
						},
						(out int xx, out int yy, out int ccomp) =>
						{
							using (var ms = new MemoryStream(data))
							{
								return Native.load_from_stream(ms, out xx, out yy, out ccomp, Stb.STBI_default);
							}
						},
						out stbSharpPassed, out stbNativePassed
						);
					stbSharpLoadingFromStream += stbSharpPassed;
					stbNativeLoadingFromStream += stbNativePassed;

					Log("Loading from memory");
					ParseTest(
						(out int xx, out int yy, out int ccomp) =>
						{
							var img = Stb.LoadFromMemory(data);

							var res = img.Data;
							xx = img.Width;
							yy = img.Height;
							ccomp = img.SourceComp;

							x = xx;
							y = yy;
							comp = ccomp;
							return res;
						},
						(out int xx, out int yy, out int ccomp) =>
							Native.load_from_memory(data, out xx, out yy, out ccomp, Stb.STBI_default),
						out stbSharpPassed, out stbNativePassed
						);
					stbSharpLoadingFromMemory += stbSharpPassed;
					stbNativeLoadingFromMemory += stbNativePassed;

					for (var k = 0; k <= 3; ++k)
					{
						// Skip HDR for now
						if (k == 2)
						{
							continue;
						}

						Log("Saving as {0} with StbSharp", ((ImageWriterFormat) k).ToString());
						byte[] save;
						BeginWatch();
						using (var stream = new MemoryStream())
						{
							var writer = new ImageWriter();
							var image = new Image
							{
								Comp = comp,
								Data = parsed,
								Width = x,
								Height = y
							};
							writer.Write(image, (ImageWriterFormat) k, stream);
							save = stream.ToArray();
						}
						var passed = EndWatch();
						Log("Span: {0} ms", passed);
						Log("StbSharp Size: {0}", save.Length);

						Log("Saving as {0} with Stb.Native", ((ImageWriterFormat) k).ToString());
						BeginWatch();
						byte[] save2;
						using (var stream = new MemoryStream())
						{
							Native.save_to_memory(parsed, x, y, comp, k, stream);
							save2 = stream.ToArray();
						}

						passed = EndWatch();
						Log("Span: {0} ms", passed);
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

					Log("Total StbSharp Loading From Stream Time: {0} ms", stbSharpLoadingFromStream);
					Log("Total Stb.Native Loading From Stream Time: {0} ms", stbNativeLoadingFromStream);
					Log("Total StbSharp Loading From memory Time: {0} ms", stbSharpLoadingFromMemory);
					Log("Total Stb.Native Loading From memory Time: {0} ms", stbNativeLoadingFromMemory);

					Log("GC Memory: {0}", GC.GetTotalMemory(true));
					Log("Sichem Allocated: {0}", Operations.AllocatedTotal);
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