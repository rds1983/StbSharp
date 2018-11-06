using System;

namespace StbSharpSafe
{
	internal static class CRuntime
	{
		public const long DBL_EXP_MASK = 0x7ff0000000000000L;
		public const int DBL_MANT_BITS = 52;
		public const long DBL_SGN_MASK = -1 - 0x7fffffffffffffffL;
		public const long DBL_MANT_MASK = 0x000fffffffffffffL;
		public const long DBL_EXP_CLR_MASK = DBL_SGN_MASK | DBL_MANT_MASK;

		public static void memcpy<T>(FakePtr<T> a, FakePtr<T> b, long size) where T : new()
		{
			for (long i = 0; i < size; ++i)
			{
				a[i] = b[i];
			}
		}

		public static void memcpy<T>(FakePtr<T> a, FakePtr<T> b, ulong size) where T : new()
		{
			memcpy(a, b, (long)size);
		}

		public static void memmove<T>(FakePtr<T> a, FakePtr<T> b, long size) where T : new()
		{
			FakePtr<T> temp = FakePtr<T>.CreateWithSize(size);
			memcpy(temp, b, size);
			memcpy(a, temp, size);
		}

		public static void memmove<T>(FakePtr<T> a, FakePtr<T> b, ulong size) where T : new()
		{
			memmove(a, b, (long)size);
		}

		public static int memcmp<T>(FakePtr<T> a, FakePtr<T> b, long size) where T : new()
		{
			var result = 0;
			for (long i = 0; i < size; ++i)
			{
				if (!a[i].Equals(b[i]))
				{
					result += 1;
				}
			}

			return result;
		}

		public static int memcmp<T>(FakePtr<T> a, FakePtr<T> b, ulong size) where T : new()
		{
			return memcmp(a, b, (long)size);
		}

		public static void free<T>(FakePtr<T> ptr) where T : new()
		{
			// Do nothing
		}

		public static void memset<T>(FakePtr<T> a, T value, long size) where T : new()
		{
			for (long i = 0; i < size; ++i)
			{
				a[i] = value;
			}
		}

		public static void memset<T>(FakePtr<T> a, T value, ulong size) where T : new()
		{
			memset(a, value, (long)size);
		}

		public static uint _lrotl(uint x, int y)
		{
			return (x << y) | (x >> (32 - y));
		}

		public static int abs(int v)
		{
			return Math.Abs(v);
		}

		public static double pow(double a, double b)
		{
			return Math.Pow(a, b);
		}

		public static float fabs(double a)
		{
			return (float)Math.Abs(a);
		}

		public static double ceil(double a)
		{
			return Math.Ceiling(a);
		}


		public static double floor(double a)
		{
			return Math.Floor(a);
		}

		public static double log(double value)
		{
			return Math.Log(value);
		}

		public static double exp(double value)
		{
			return Math.Exp(value);
		}

		public static double cos(double value)
		{
			return Math.Cos(value);
		}

		public static double acos(double value)
		{
			return Math.Acos(value);
		}

		public static double sin(double value)
		{
			return Math.Sin(value);
		}

		public static double ldexp(double number, int exponent)
		{
			return number * Math.Pow(2, exponent);
		}

		public static double sqrt(double val)
		{
			return Math.Sqrt(val);
		}

		public static double fmod(double x, double y)
		{
			return x % y;
		}

		public static ulong strlen(FakePtr<sbyte> str)
		{
			ulong result = 0;
			for (int i = 0; ;)
			{
				if (str[i] == '\0')
				{
					result = (ulong)i;
					break;
				}
			}

			return result;
		}
	}
}