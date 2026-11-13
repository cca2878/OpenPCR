using global::OpenPCR.AnimKit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenPCR.AnimKit
{
	[Serializable]
	[DebuggerDisplay("Count={Count}")]
	public class ExposedList<T> : IEnumerable<T>, IEnumerable
	{
		[Serializable]
		[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
		public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
		{
			private ExposedList<T> l;

			private int next;

			private int ver;

			private T current;

			public T Current => current;

			object IEnumerator.Current
			{
				get
				{
					VerifyState();
					if (next <= 0)
					{
						throw new InvalidOperationException();
					}
					return current;
				}
			}

			internal Enumerator(ExposedList<T> l)
			{
				this = default(Enumerator);
				this.l = l;
				ver = l.version;
			}

			public void Dispose()
			{
				l = null;
			}

			private void VerifyState()
			{
				if (l == null)
				{
					throw new ObjectDisposedException(GetType().FullName);
				}
				if (ver != l.version)
				{
					throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
				}
			}

			public bool MoveNext()
			{
				VerifyState();
				if (next < 0)
				{
					return false;
				}
				if (next < l.Count)
				{
					current = l.Items[next++];
					return true;
				}
				next = -1;
				return false;
			}

			void IEnumerator.Reset()
			{
				VerifyState();
				next = 0;
			}
		}

		public T[] Items;

		public int Count;

		private const int DefaultCapacity = 4;

		private static readonly T[] EmptyArray = new T[0];

		private int version;

		[global::OpenPCR.Migration("aa4bf772-189a-45af-89cd-e268a4b8bc27")]
		public int Capacity
		{
			[global::OpenPCR.Migration("1118a1fb-b6f4-4d94-be52-49dddf268b1c")]
			get
			{
				return Items.Length;
			}
			[global::OpenPCR.Migration("bf638c90-da0d-4f44-9988-a18b47f7f3a6")]
			set
			{
				if ((uint)value < (uint)Count)
				{
					throw new ArgumentOutOfRangeException();
				}
				Array.Resize(ref Items, value);
			}
		}

		[global::OpenPCR.Migration("232631a9-83b6-4395-8cdb-7076b2229726")]
		public ExposedList()
		{
			Items = EmptyArray;
		}

		public ExposedList(IEnumerable<T> collection)
		{
			CheckCollection(collection);
			if (!(collection is ICollection<T> collection2))
			{
				Items = EmptyArray;
				AddEnumerable(collection);
			}
			else
			{
				Items = new T[collection2.Count];
				AddCollection(collection2);
			}
		}

		public ExposedList(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			Items = new T[capacity];
		}

		internal ExposedList(T[] data, int size)
		{
			Items = data;
			Count = size;
		}

		[global::OpenPCR.Migration("a700bb6f-409b-4152-ac05-9e30d03f28e7")]
		public void Add(T item)
		{
			if (Count == Items.Length)
			{
				GrowIfNeeded(1);
			}
			Items[Count++] = item;
			version++;
		}

		[global::OpenPCR.Migration("4c65133e-c3f7-437e-8240-6c66a1625aa5")]
		public void GrowIfNeeded(int newCount)
		{
			int num = Count + newCount;
			if (num > Items.Length)
			{
				Capacity = Math.Max(Math.Max(Capacity * 2, 4), num);
			}
		}

		[global::OpenPCR.Migration("009143fc-3bc5-4dd3-9f1f-af82d3039183")]
		public ExposedList<T> Resize(int newSize)
		{
			if (newSize > Items.Length)
			{
				Array.Resize(ref Items, newSize);
			}
			Count = newSize;
			return this;
		}

		[global::OpenPCR.Migration("4042b3ae-6ca1-4fde-b39f-a1e27ef7c6fe")]
		public void EnsureCapacity(int min)
		{
			if (Items.Length < min)
			{
				int num = ((Items.Length == 0) ? 4 : (Items.Length * 2));
				if (num < min)
				{
					num = min;
				}
				Capacity = num;
			}
		}

		private void AddCollection(ICollection<T> collection)
		{
			int count = collection.Count;
			if (count != 0)
			{
				GrowIfNeeded(count);
				collection.CopyTo(Items, Count);
				Count += count;
			}
		}

		private void AddEnumerable(IEnumerable<T> enumerable)
		{
			foreach (T item in enumerable)
			{
				Add(item);
			}
		}

		[global::OpenPCR.Migration("0ad98e87-fb3b-4037-85dc-0b20cbf4f4a9")]
		public void Clear(bool clearArray = true)
		{
			if (clearArray)
			{
				Array.Clear(Items, 0, Items.Length);
			}
			Count = 0;
			version++;
		}

		[global::OpenPCR.Migration("df25df03-de41-4eba-bbd5-51c0895fcfc6")]
		public bool Contains(T item)
		{
			return Array.IndexOf(Items, item, 0, Count) != -1;
		}

		[global::OpenPCR.Migration("d1e7e7d7-cdce-4583-a797-73fbb90d0ba0")]
		public T Find(Predicate<T> match)
		{
			CheckMatch(match);
			int index = GetIndex(0, Count, match);
			if (index == -1)
			{
				return default(T);
			}
			return Items[index];
		}

		[global::OpenPCR.Migration("6ef30b1f-b21d-4572-a787-8b849dd8159c")]
		private static void CheckMatch(Predicate<T> match)
		{
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
		}

		[global::OpenPCR.Migration("6cbbda42-263d-46fe-9f49-d3e290368b67")]
		public int FindIndex(Predicate<T> match)
		{
			CheckMatch(match);
			return GetIndex(0, Count, match);
		}

		[global::OpenPCR.Migration("918eb134-05d5-4a0b-a625-50b7d32a5334")]
		private int GetIndex(int startIndex, int count, Predicate<T> match)
		{
			int num = startIndex + count;
			for (int i = startIndex; i < num; i++)
			{
				if (match(Items[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		private void CheckCollection(IEnumerable<T> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
		}

		[global::OpenPCR.Migration("9f4a2e74-18b0-4682-ac6c-bce618eca789")]
		public T Pop()
		{
			if (Count == 0)
			{
				throw new InvalidOperationException("List is empty. Nothing to pop.");
			}
			int num = Count - 1;
			T result = Items[num];
			Items[num] = default(T);
			Count--;
			version++;
			return result;
		}

		[global::OpenPCR.Migration("5dac7034-c7a3-40ba-a3ee-518d46de45c0")]
		public T[] ToArray()
		{
			T[] array = new T[Count];
			Array.Copy(Items, array, Count);
			return array;
		}

		[global::OpenPCR.Migration("12f252d4-1639-4d87-9ce7-99ccefb8d21e")]
		public void TrimExcess()
		{
			Capacity = Count;
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
