namespace StbSharpSafe
{
	public class FakePtr<T>
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

		public FakePtr(T[] data)
		{
			_array = data;
		}

		public FakePtr(int size)
		{
			_array = new T[size];
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

		public static FakePtr<T> operator ++(FakePtr<T> p)
		{
			return p + 1;
		}

		public static bool operator ==(FakePtr<T> p1, FakePtr<T> p2)
		{
			return p1._array == p2._array && p1._offset == p2._offset;
		}

		public static bool operator !=(FakePtr<T> p1, FakePtr<T> p2)
		{
			return !(p1 == p2);
		}
	}
}