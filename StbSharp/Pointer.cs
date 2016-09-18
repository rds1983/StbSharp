namespace StbSharp
{
	internal struct Pointer<T> where T: struct
	{
		public T[] Data;
		public int Position;

		public int Size
		{
			get { return Data != null?Data.Length:0; }
		}

		public bool IsNull
		{
			get { return Data == null; }
		}

		public T this[int index]
		{
			get { return Data[Position + index]; }

			set { Data[Position + index] = value; }
		}

		public Pointer(int size)
		{
			Data = new T[size];
			Position = 0;
		}

		public void Reset()
		{
			Data = null;
			Position = 0;
		}

		public T GetNext()
		{
			var result = Data[Position];
			Position++;
			return result;
		}

		public static Pointer<T> operator +(Pointer<T> a, int length)
		{
			return new Pointer<T>
			{
				Data = a.Data,
				Position = a.Position + length
			};
		}

		public static int operator -(Pointer<T> a, Pointer<T> b)
		{
			return a.Position - b.Position;
		}

		public static bool operator <(Pointer<T> a, Pointer<T> b)
		{
			return a.Position < b.Position;
		}

		public static bool operator >(Pointer<T> a, Pointer<T> b)
		{
			return a.Position > b.Position;
		}

		public static bool operator <=(Pointer<T> a, Pointer<T> b)
		{
			return a.Position <= b.Position;
		}

		public static bool operator >=(Pointer<T> a, Pointer<T> b)
		{
			return a.Position >= b.Position;
		}
	}
}
