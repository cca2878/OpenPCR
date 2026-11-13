using System;
using System.Collections.Generic;

namespace Elements
{
	public class FunctionalComparer<T> : IComparer<T>
	{
		private Func<T, T, int> comparer;
		public static FunctionalComparer<T> Instance;

		[global::OpenPCR.Migration("9b833bda-bca1-4514-a1c8-c5c9a9649537")]
		public FunctionalComparer(Func<T, T, int> _comparer)
		{
			comparer = _comparer;
		}

		[global::OpenPCR.Migration("c908b247-a056-447e-b9a6-bf170a251e6b")]
		public int Compare(T x, T y)
		{
			return comparer(x, y);
		}

		[global::OpenPCR.Migration("d6fddcae-93f0-4b2a-aa77-2dcbefd4b0ab")]
		public void SetComparer(Func<T, T, int> _comparer)
		{
			comparer = _comparer;
		}

		[global::OpenPCR.Migration("0b9ce72e-7465-4ab8-a14e-203a585d8118")]
		public static void CreateInstance()
		{
			Instance = new FunctionalComparer<T>(null);
		}
	}
}
