namespace StbSharpSafe
{
	public class FakePtr<T> where T : new()
	{
		private readonly T[] _array;
		private int _offset;

		public T this[int index]
		{
			get
			{
				return _array[_offset + index];
			}

			set
			{
				_array[_offset + index] = value;
			}
		}

		public T this[long index]
		{
			get
			{
				return _array[_offset + index];
			}

			set
			{
				_array[_offset + index] = value;
			}
		}

		public T Value
		{
			get
			{
				return this[0];
			}

			set
			{
				this[0] = value;
			}
		}

		public FakePtr(T[] data)
		{
			_array = data;
		}

		public FakePtr(T value)
		{
			_array = new T[1];
			_array[0] = value;
		}

		public static FakePtr<T> operator +(FakePtr<T> p, int offset)
		{
			return new FakePtr<T>(p._array) { _offset = p._offset + offset };
		}

		public static FakePtr<T> operator -(FakePtr<T> p, int offset)
		{
			return p + -offset;
		}

		public static FakePtr<T> operator +(FakePtr<T> p, uint offset)
		{
			return p + (int)offset;
		}

		public static FakePtr<T> operator -(FakePtr<T> p, uint offset)
		{
			return p - (int)offset;
		}

		public static FakePtr<T> operator +(FakePtr<T> p, long offset)
		{
			return p + (int)offset;
		}

		public static FakePtr<T> operator -(FakePtr<T> p, long offset)
		{
			return p - (int)offset;
		}

		public static FakePtr<T> operator ++(FakePtr<T> p)
		{
			return p + 1;
		}

		public static bool operator ==(FakePtr<T> p1, FakePtr<T> p2)
		{
			if (p1 is null && p2 is null)
			{
				return true;
			}

			if (p1 is null)
			{
				return false;
			}

			if (p2 is null)
			{
				return false;
			}

			return p1._array == p2._array && p1._offset == p2._offset;
		}

		public static bool operator !=(FakePtr<T> p1, FakePtr<T> p2)
		{
			return !(p1 == p2);
		}

		public static FakePtr<T> CreateWithSize(int size)
		{
			var result = new FakePtr<T>(new T[size]);

			for (int i = 0; i < size; ++i)
			{
				result[i] = new T();
			}

			return result;
		}

		public static FakePtr<T> CreateWithSize(long size)
		{
			return CreateWithSize((int)size);
		}

		public static FakePtr<T> Create()
		{
			return CreateWithSize(1);
		}

	}
}