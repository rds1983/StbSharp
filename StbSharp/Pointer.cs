namespace StbSharp
{
	internal struct Pointer<T> where T: struct
	{
		private T[] _data;
		private int _position;

		public int Size
		{
			get { return _data.Length; }
		}

		public bool IsNull
		{
			get { return _data.Length == 0; }
		}

		public T this[int index]
		{
			get { return _data[_position + index]; }
			set { _data[_position + index] = value; }
		}

		public T CurrentValue
		{
			get { return _data[_position]; }
			set { _data[_position] = value; }
		}

		public Pointer(int size)
		{
			_data = new T[size];
			_position = 0;
		}

		public Pointer(T[] data)
		{
			_data = data;
			_position = 0;
		}

		public T GetAndMove()
		{
			var result = CurrentValue;
			_position++;

			return result;
		}

		public void Reset()
		{
			_data = null;
			_position = 0;
		}

		public static bool operator ==(Pointer<T> a, object b)
		{
			if (b == null)
			{
				return a.IsNull;
			}

			if (!b.GetType().IsValueType)
			{
				return false;
			}

			var asp = (Pointer<T>) b;

			return a._data == asp._data &&
			       a._position == asp._position;
		}

		public static bool operator !=(Pointer<T> a, object b)
		{
			return !(a == b);
		}

		public static Pointer<T> operator +(Pointer<T> a, int length)
		{
			return new Pointer<T>
			{
				_data = a._data,
				_position = a._position + length
			};
		}

		public static Pointer<T> operator +(Pointer<T> a, long length)
		{
			return a + (int) length;
		}

		public static int operator -(Pointer<T> a, Pointer<T> b)
		{
			return a._position - b._position;
		}

		public static bool operator <(Pointer<T> a, Pointer<T> b)
		{
			return a._position < b._position;
		}

		public static bool operator >(Pointer<T> a, Pointer<T> b)
		{
			return a._position > b._position;
		}

		public static bool operator <=(Pointer<T> a, Pointer<T> b)
		{
			return a._position <= b._position;
		}

		public static bool operator >=(Pointer<T> a, Pointer<T> b)
		{
			return a._position >= b._position;
		}
	}
}
