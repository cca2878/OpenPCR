using global::OpenPCR.AnimKit;
using System;
using System.Collections.Generic;

namespace OpenPCR.AnimKit
{
	public class Pool<T> where T : class, new()
	{
		public interface IPoolable
		{
			void Reset();
		}

		public readonly int max;

		private readonly Stack<T> freeObjects;

		public int Peak { get; private set; }

		public Pool(int initialCapacity = 16, int max = int.MaxValue)
		{
			freeObjects = new Stack<T>(initialCapacity);
			this.max = max;
		}

		[global::OpenPCR.Migration("c1327405-2299-4521-bd82-5ba32d868f6c")]
		public T Obtain()
		{
			if (freeObjects.Count != 0)
			{
				return freeObjects.Pop();
			}
			return new T();
		}

		[global::OpenPCR.Migration("3d80430b-17da-45cd-bd1a-2c2be919f824")]
		public void Free(T obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj", "obj cannot be null");
			}
			if (freeObjects.Count < max)
			{
				freeObjects.Push(obj);
				Peak = Math.Max(Peak, freeObjects.Count);
			}
			Reset(obj);
		}

		[global::OpenPCR.Migration("98d65a7d-3988-4d19-97bf-23b2404bd6e8")]
		protected void Reset(T obj)
		{
			if (obj is IPoolable poolable)
			{
				poolable.Reset();
			}
		}
	}
}
