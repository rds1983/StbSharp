using System;
using System.IO;
using Sichem;

namespace StbSharp.StbTrueType.Generator
{
	class Program
	{
		private const string SourceFile = @"..\..\..\..\..\StbSharpSafe\StbTrueType.Generated.cs";

		static void Process()
		{
			var data = string.Empty;
			using (var output = new StringWriter())
			{
				var parameters = new ConversionParameters
				{
					InputPath = @"stb_truetype.h",
					Output = output,
					Defines = new[]
					{
						"STB_TRUETYPE_IMPLEMENTATION",
					},
					Namespace = "StbSharpSafe",
					Class = "StbTrueType",
					SkipStructs = new string[]
					{
						"stbtt__hheap_chunk",
						"stbtt__hheap",
					},
					SkipGlobalVariables = new string[]
					{
					},
					SkipFunctions = new string[]
					{
						"stbtt__find_table",
						"stbtt_FreeShape",
						"stbtt__hheap_alloc",
						"stbtt__hheap_free",
						"stbtt__hheap_cleanup",
					},
					Classes = new string[]
					{
						"stbtt__buf",
						"stbtt_pack_range",
						"stbtt_pack_context",
						"stbtt_fontinfo",
						"stbtt__bitmap",
						"stbtt__csctx",
						"stbtt__hheap_chunk",
						"stbtt__hheap",
						"stbtt__active_edge",
						"stbtt_vertex",
						"stbtt_fontinfo",
					},
					GlobalArrays = new string[]
					{
					},
					GenerateSafeCode = true,
					TreatStructFieldClassPointerAsArray = (structName, fieldName) =>
					{
						if (fieldName == "pvertices")
						{
							return true;
						}

						return false;
					},
					TreatFunctionArgClassPointerAsArray = (functionName, argName) =>
					{
						if (argName == "num_points" || argName == "num_contours" || argName == "contour_lengths")
						{
							return FunctionArgumentType.Ref;
						}

						if ((functionName == "stbtt_GetCodepointShape" || functionName == "stbtt_GetGlyphShape") &&
							(argName == "vertices" || argName == "pvertices"))
						{
							return FunctionArgumentType.Ref;
						}

						if (argName == "vertices" || argName == "pvertices" || argName == "verts" || argName == "ranges")
						{
							return FunctionArgumentType.Pointer;
						}

						return FunctionArgumentType.Default;
					},
					TreatLocalVariableClassPointerAsArray = (functionName, localVarName) =>
					{
						if (localVarName == "vertices" || localVarName == "verts" || localVarName == "comp_verts")
						{
							return true;
						}

						if (functionName == "stbtt__GetGlyphShapeTT" && 
							localVarName == "tmp")
						{
							return true;
						}

						if (functionName == "stbtt__run_charstring" && localVarName == "subr_stack")
						{
							return true;
						}

						return false;
					}
				};

				var cp = new ClangParser();

				cp.Process(parameters);

				data = output.ToString();
			}


			// Post processing
			Logger.Info("Post processing...");

			data = Utility.ReplaceNativeCalls(data);

			data = data.Replace("(void *)(0)", "null");
			data = data.Replace("stbtt_vertex* vertices = 0;", "stbtt_vertex* vertices = null;");
			data = data.Replace("(flags & 16)?dx:-dx", "(flags & 16) != 0?dx:-dx");
			data = data.Replace("(flags & 32)?dy:-dy", "(flags & 32) != 0?dy:-dy");
			data = data.Replace("(vertices) == (0)", "vertices == null");
			data = data.Replace("sizeof((vertices[0]))", "sizeof(stbtt_vertex)");
			data = data.Replace("(int)(!(flags & 1))", "((flags & 1) != 0?0:1)");
			data = data.Replace("vertices = 0;", "vertices = null;");
			data = data.Replace("stbtt_vertex* comp_verts = 0;", "stbtt_vertex* comp_verts = null;");
			data = data.Replace("stbtt_vertex* tmp = 0;", "stbtt_vertex* tmp = null;");
			data = data.Replace(",)", ")");
			data = data.Replace("+ +", "+");
			data = data.Replace("(sizeof(stbtt__hheap_chunk) + size * count)",
				"((ulong)sizeof(stbtt__hheap_chunk)+ size * (ulong)(count))");
			data = data.Replace("size * hh->num_remaining_in_head_chunk",
				"size * (ulong)hh->num_remaining_in_head_chunk");
			data = data.Replace("sizeof((*z))", "sizeof(stbtt__active_edge)");
			data = data.Replace("_next_ = 0;", "_next_ = null;");
			data = data.Replace("sizeof((scanline[0]))", "sizeof(float)");
			data = data.Replace("int c = (int)(((a)->y0) < ((b)->y0));", "int c = (int)(a->y0 < b->y0?1:0);");
			data = data.Replace("sizeof((*e))", "sizeof(stbtt__edge)");
			data = data.Replace("sizeof((**contour_lengths))", "sizeof(int)");
			data = data.Replace("sizeof((points[0]))", "sizeof(stbtt__point)");
			data = data.Replace("sizeof((*context))", "sizeof(stbrp_context)");
			data = data.Replace("sizeof((*nodes))", "sizeof(stbrp_node)");
			data = data.Replace("sizeof((*rects))", "sizeof(stbrp_rect)");
			data = data.Replace("(int)(((a[0]) == (b[0])) && ((a[1]) == (b[1])));",
				"(int)(((a[0] == b[0]) && (a[1] == b[1]))?1:0);");
			data = data.Replace("(int)(((a).Value.y0) < ((b).Value.y0));", "a.Value.y0 < b.Value.y0?1:0;");

			data = data.Replace("CRuntime.malloc((ulong)(m * sizeof(stbtt_vertex)))", "FakePtr<stbtt_vertex>.CreateWithSize(m)");
			data = data.Replace("CRuntime.malloc((ulong)((num_vertices + comp_num_verts) * sizeof(stbtt_vertex)))", "FakePtr<stbtt_vertex>.CreateWithSize(num_vertices + comp_num_verts)");
			data = data.Replace("CRuntime.malloc((ulong)(count_ctx.num_vertices * sizeof(stbtt_vertex)))", "FakePtr<stbtt_vertex>.CreateWithSize(count_ctx.num_vertices)");
			data = data.Replace("CRuntime.malloc((ulong)((result.w * 2 + 1) * sizeof(float)))", "FakePtr<float>.CreateWithSize(result.w * 2 + 1)");
			data = data.Replace("CRuntime.malloc((ulong)(sizeof((e.Value)) * (n + 1)))", "FakePtr<stbtt__edge>.CreateWithSize(n + 1)");
			data = data.Replace("CRuntime.malloc((ulong)(sizeof((contour_lengths)) * n))", "FakePtr<int>.CreateWithSize(n + 1)");
			data = data.Replace("CRuntime.malloc((ulong)(num_points * sizeof(stbtt__point)))", "FakePtr<stbtt__point>.CreateWithSize(num_points)");

			data = data.Replace("(ulong)(num_vertices * sizeof(stbtt_vertex))", "num_vertices");
			data = data.Replace("(ulong)(comp_num_verts * sizeof(stbtt_vertex))", "comp_num_verts");
			data = data.Replace("(ulong)(result.w * sizeof(float))", "result.w");
			data = data.Replace("(ulong)((result.w + 1) * sizeof(float))", "result.w + 1");

			data = data.Replace("stbtt__hheap hh, ", "");
			data = data.Replace("stbtt__active_edge z = stbtt__hheap_alloc(hh, (ulong)(sizeof((z))), userdata);", "stbtt__active_edge z = new stbtt__active_edge();");
			data = data.Replace("stbtt__hheap hh = (stbtt__hheap)({ null, null, 0 });", "");
			data = data.Replace("stbtt__hheap_free(hh, z);", "");
			data = data.Replace("stbtt__hheap_cleanup(hh, userdata);", "");
			data = data.Replace("FakePtr<stbtt__edge> a = ref t;", "FakePtr<stbtt__edge> a = new FakePtr<stbtt__edge>(t);");
			data = data.Replace("hh, ", "");

			File.WriteAllText(SourceFile, data);
		}

		static void Main(string[] args)
		{
			try
			{
				Process();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}

			Console.WriteLine("Finished. Press any key to quit.");
			Console.ReadKey();
		}
	}
}