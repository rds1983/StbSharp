namespace StbSharp
{
	internal struct Pointer<T> where T: struct
	{
		public T[] Data;
		public int Position;

		public int Size
		{
			get { return Data.Length; }
		}

		public Pointer(int size)
		{
			Data = new T[size];
			Position = 0;
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
	}
}
