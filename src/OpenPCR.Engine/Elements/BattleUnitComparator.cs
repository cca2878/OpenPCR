using System;
using System.Collections.Generic;
using Elements.Battle.Core;

namespace Elements
{
	public static class BattleUnitComparator
	{
		[global::OpenPCR.Migration("0bbe31cb-91a3-4c3b-8094-93a3f457197a")]
		public static void QuickSort<T>(List<T> _array, Func<T, T, int> _compare)
		{
			if (_array.Count == 0)
				return;
			FunctionalComparer<T> comparer = FunctionalComparer<T>.Instance;
			comparer.SetComparer(_compare);
			quickSortImpl(_array, 0, _array.Count - 1, comparer);
		}

		[global::OpenPCR.Migration("9093b3f7-85d4-4182-8c1e-bdf76eae301c")]
		private static void quickSortImpl<T>(List<T> _array, int _left, int _right, FunctionalComparer<T> _compare)
		{
			if (_right <= _left)
				return;
			T pivot = _array[_left + (_right - _left) / 2];
			int left = _left;
			int right = _right;
			while (true)
			{
				while (left < _right && _compare.Compare(_array[left], pivot) < 0)
					++left;
				while (right > _left && _compare.Compare(pivot, _array[right]) < 0)
					--right;
				if (left > right)
					break;
				swap(_array, left++, right--);
			}

			quickSortImpl(_array, _left, right, _compare);
			if (left < _right)
				quickSortImpl(_array, left, _right, _compare);
		}

		[global::OpenPCR.Migration("8c1e888a-c439-48f3-8526-aa5953f69e97")]
		private static void swap<T>(List<T> _list, int i, int j)
		{
			T item = _list[i];
			T other = _list[j];
			_list[i] = other;
			_list[j] = item;
		}

		[global::OpenPCR.Migration("e16a50c6-1003-42bc-8fca-6832b727ecc3")]
		public static int CompareLeft(BasePartsData _a, BasePartsData _b)
		{
			return _a.GetPosition().x.CompareTo(_b.GetPosition().x);
		}

		[global::OpenPCR.Migration("6b6464a0-fc43-487e-92e3-e104f161306f")]
		public static int CompareRight(BasePartsData _a, BasePartsData _b)
		{
			return _b.GetPosition().x.CompareTo(_a.GetPosition().x);
		}

		[global::OpenPCR.Migration("681da269-4bee-466e-b1d9-b06b8159d666")]
		public static int CompareEnergyAsc(BasePartsData a, BasePartsData b)
		{
			if (a == null)
				return b != null ? -1 : 0;
			if (b == null)
				return 1;
			return a.Owner.ComponentParameter.Energy.CompareTo(b.Owner.ComponentParameter.Energy);
		}

		[global::OpenPCR.Migration("165b554c-b439-4486-be83-9ec0450544e7")]
		public static int CompareEnergyDec(BasePartsData a, BasePartsData b)
		{
			if (a == null)
				return b != null ? -1 : 0;
			if (b == null)
				return -1;
			return b.Owner.ComponentParameter.Energy.CompareTo(a.Owner.ComponentParameter.Energy);
		}

		[global::OpenPCR.Migration("267648e3-c76e-41e3-97cb-77b16f1174a5")]
		public static int CompareAtkAsc(BasePartsData a, BasePartsData b)
		{
			if (a == null)
				return b != null ? -1 : 0;
			if (b == null)
				return 1;
			return a.GetAtkZero().CompareTo(b.GetAtkZero());
		}

		[global::OpenPCR.Migration("df0bbf13-e09b-4f67-8474-98a5d4a79bb4")]
		public static int CompareAtkDec(BasePartsData a, BasePartsData b)
		{
			if (a == null)
				return b != null ? -1 : 0;
			if (b == null)
				return -1;
			return b.GetAtkZero().CompareTo(a.GetAtkZero());
		}

		[global::OpenPCR.Migration("3ec14039-9cfd-43a8-b3c2-54ab4beae77f")]
		public static int CompareMagicStrAsc(BasePartsData a, BasePartsData b)
		{
			if (a == null)
				return b != null ? -1 : 0;
			if (b == null)
				return 1;
			return a.GetMagicStrZero().CompareTo(b.GetMagicStrZero());
		}

		[global::OpenPCR.Migration("36b814f5-0de5-4c02-bcd6-96d957f9ab4e")]
		public static int CompareMagicStrDec(BasePartsData a, BasePartsData b)
		{
			if (a == null)
				return b != null ? -1 : 0;
			if (b == null)
				return -1;
			return b.GetMagicStrZero().CompareTo(a.GetMagicStrZero());
		}

		[global::OpenPCR.Migration("cb117efd-b339-4693-8c0c-3c08b163227c")]
		public static int CompareAtkDefAsc(BasePartsData _a, BasePartsData _b)
		{
			if (_a == null)
				return _b != null ? -1 : 0;
			if (_b == null)
				return 1;
			return _a.GetDefZero().CompareTo(_b.GetDefZero());
		}

		[global::OpenPCR.Migration("8d2fed87-dc47-441f-899a-1b14a042a2bc")]
		public static int CompareMagicDefAsc(BasePartsData _a, BasePartsData _b)
		{
			if (_a == null)
				return _b != null ? -1 : 0;
			if (_b == null)
				return 1;
			return _a.GetMagicDefZero().CompareTo(_b.GetMagicDefZero());
		}

		[global::OpenPCR.Migration("9765ff96-865e-4b0c-a973-a7874d26effa")]
		public static int CompareLifeValueAsc(BasePartsData _a, BasePartsData _b)
		{
			if (_a == null)
				return _b != null ? -1 : 0;
			if (_b == null)
				return 1;
			return (_a.Owner.ComponentParameter.Hp).CompareTo((_b.Owner.ComponentParameter.Hp));
		}

		[global::OpenPCR.Migration("a1f150be-cc7b-4816-96de-1d77a1f4f9f7")]
		public static int CompareLifeValueDec(BasePartsData _a, BasePartsData _b)
		{
			if (_a == null)
				return _b != null ? -1 : 0;
			if (_b == null)
				return -1;
			return (_b.Owner.ComponentParameter.Hp).CompareTo((_a.Owner.ComponentParameter.Hp));
		}

		[global::OpenPCR.Migration("63bc950f-7e59-4408-bd02-bc8be53b812b")]
		public static int CompareHigherAtkOrMagicaStrAsc(BasePartsData _a, BasePartsData _b)
		{
			if (_a == null)
				return _b != null ? -1 : 0;
			if (_b == null)
				return 1;
			return Math.Max(_a.GetAtkZero(), _a.GetMagicStrZero()).CompareTo(Math.Max(_b.GetAtkZero(), _b.GetMagicStrZero()));
		}

		[global::OpenPCR.Migration("3b758c37-bac2-4370-97df-3c2d47d8f303")]
		public static int CompareHigherAtkOrMagicStrDec(BasePartsData _a, BasePartsData _b)
		{
			if (_a == null)
				return _b != null ? -1 : 0;
			if (_b == null)
				return -1;
			return Math.Max(_b.GetAtkZero(), _b.GetMagicStrZero()).CompareTo(Math.Max(_a.GetAtkZero(), _a.GetMagicStrZero()));
		}

		[global::OpenPCR.Migration("5ee2875e-dfcc-4ec5-b1b8-7ba9729e20a2")]
		public static int CompareLifeAsc(BasePartsData _a, BasePartsData _b)
		{
			if (_a == null)
				return _b != null ? -1 : 0;
			UnitCtrl aOwner = _a.Owner;
			if (ReferenceEquals(aOwner, null))
				throw new NullReferenceException();
			if (_b == null)
				throw new NullReferenceException();
			UnitComponentParameter aParameter = aOwner.ComponentParameter;
			UnitCtrl bOwner = _b.Owner;
			if (ReferenceEquals(bOwner, null))
				throw new NullReferenceException();
			UnitComponentParameter bParameter = bOwner.ComponentParameter;
			double first = (double)(long)aParameter.Hp / (double)(long)aParameter.MaxHp;
			double second = (double)(long)bParameter.Hp / (double)(long)bParameter.MaxHp;
			return first.CompareTo(second);
		}

		[global::OpenPCR.Migration("f754dd42-00e9-45aa-950c-38cffefe6b7d")]
		public static int CompareLifeDec(BasePartsData _a, BasePartsData _b)
		{
			if (_a == null)
				return _b != null ? -1 : 0;
			UnitCtrl aOwner = _a.Owner;
			if (ReferenceEquals(aOwner, null))
				throw new NullReferenceException();
			if (_b == null)
				throw new NullReferenceException();
			UnitComponentParameter aParameter = aOwner.ComponentParameter;
			UnitCtrl bOwner = _b.Owner;
			if (ReferenceEquals(bOwner, null))
				throw new NullReferenceException();
			UnitComponentParameter bParameter = bOwner.ComponentParameter;
			double first = (double)(long)bParameter.Hp / (double)(long)bParameter.MaxHp;
			double second = (double)(long)aParameter.Hp / (double)(long)aParameter.MaxHp;
			return first.CompareTo(second);
		}

		[global::OpenPCR.Migration("3db2c26f-705a-4ce6-a840-eb1a0185681a")]
		public static int CompareAtkDefAscSameLeft(BasePartsData _a, BasePartsData _b)
		{
			if (_a.GetDefZero() == _b.GetDefZero())
				return CompareLeft(_a, _b);
			return CompareAtkDefAsc(_a, _b);
		}

		[global::OpenPCR.Migration("a3a8fbae-81ee-4ac9-884f-fa947e9f6944")]
		public static int CompareAtkDefAscSameRight(BasePartsData _a, BasePartsData _b)
		{
			if (_a.GetDefZero() == _b.GetDefZero())
				return CompareRight(_a, _b);
			return CompareAtkDefAsc(_a, _b);
		}

		[global::OpenPCR.Migration("643e9cfe-60ba-441c-aa6f-7cdc2a1fe696")]
		public static int CompareMagicDefAscSameLeft(BasePartsData _a, BasePartsData _b)
		{
			if (_a.GetMagicDefZero() == _b.GetMagicDefZero())
				return CompareLeft(_a, _b);
			return CompareMagicDefAsc(_a, _b);
		}

		[global::OpenPCR.Migration("5592d309-d4b1-48bf-bb53-7b74abf97795")]
		public static int CompareMagicDefAscSameRight(BasePartsData _a, BasePartsData _b)
		{
			if (_a.GetMagicDefZero() == _b.GetMagicDefZero())
				return CompareRight(_a, _b);
			return CompareMagicDefAsc(_a, _b);
		}

		[global::OpenPCR.Migration("5bc03b48-1217-4dc5-baf0-4cc749365d09")]
		public static int CompareLifeValueAscSameLeft(BasePartsData _a, BasePartsData _b)
		{
			if ((long)_a.Owner.ComponentParameter.Hp == (long)_b.Owner.ComponentParameter.Hp)
				return CompareLeft(_a, _b);
			return CompareLifeValueAsc(_a, _b);
		}

		[global::OpenPCR.Migration("cd53679e-a4af-4f03-82ad-5abc9609bc6e")]
		public static int CompareLifeValueAscSameRight(BasePartsData _a, BasePartsData _b)
		{
			if ((long)_a.Owner.ComponentParameter.Hp == (long)_b.Owner.ComponentParameter.Hp)
				return CompareRight(_a, _b);
			return CompareLifeValueAsc(_a, _b);
		}

		[global::OpenPCR.Migration("2de85a69-a751-4b22-97c8-22f5af3fd0b6")]
		public static int CompareLifeValueDecSameLeft(BasePartsData _a, BasePartsData _b)
		{
			if ((long)_a.Owner.ComponentParameter.Hp == (long)_b.Owner.ComponentParameter.Hp)
				return CompareLeft(_a, _b);
			return CompareLifeValueDec(_a, _b);
		}

		[global::OpenPCR.Migration("8b08cc27-fa1b-4df0-bc80-e03c98ac9dee")]
		public static int CompareLifeValueDecSameRight(BasePartsData _a, BasePartsData _b)
		{
			if ((long)_a.Owner.ComponentParameter.Hp == (long)_b.Owner.ComponentParameter.Hp)
				return CompareRight(_a, _b);
			return CompareLifeValueDec(_a, _b);
		}

		[global::OpenPCR.Migration("4ffd393a-2cf5-4b9d-951a-9bb891eab2af")]
		public static int CompareEnergyAscSameLeft(BasePartsData _a, BasePartsData _b)
		{
			if (BattleUtil.Approximately(_a.Owner.ComponentParameter.Energy, _b.Owner.ComponentParameter.Energy))
				return CompareLeft(_a, _b);
			return CompareEnergyAsc(_a, _b);
		}

		[global::OpenPCR.Migration("99f3ae14-2305-4772-aec3-f272e5a4642a")]
		public static int CompareEnergyAscSameRight(BasePartsData _a, BasePartsData _b)
		{
			if (BattleUtil.Approximately(_a.Owner.ComponentParameter.Energy, _b.Owner.ComponentParameter.Energy))
				return CompareRight(_a, _b);
			return CompareEnergyAsc(_a, _b);
		}

		[global::OpenPCR.Migration("4fc6aa91-cf54-43c8-9ac6-c149f3568cf6")]
		public static int CompareHpAscSameLeft(BasePartsData _a, BasePartsData _b)
		{
			UnitCtrl aOwner = _a.Owner;
			if (ReferenceEquals(aOwner, null))
				throw new NullReferenceException();
			if (_b == null)
				throw new NullReferenceException();
			UnitComponentParameter aParameter = aOwner.ComponentParameter;
			UnitCtrl bOwner = _b.Owner;
			if (ReferenceEquals(bOwner, null))
				throw new NullReferenceException();
			UnitComponentParameter bParameter = bOwner.ComponentParameter;
			long aHp = aParameter.Hp;
			long aMaxHp = aParameter.MaxHp;
			long bHp = bParameter.Hp;
			long bMaxHp = bParameter.MaxHp;
			if (BattleUtil.Approximately((double)aHp / (double)aMaxHp, (double)bHp / (double)bMaxHp))
				return CompareLeft(_a, _b);
			return CompareLifeAsc(_a, _b);
		}

		[global::OpenPCR.Migration("9d8b3689-cffd-43aa-ac20-a397ba8c3d4f")]
		public static int CompareHpAscSameRight(BasePartsData _a, BasePartsData _b)
		{
			UnitCtrl aOwner = _a.Owner;
			if (ReferenceEquals(aOwner, null))
				throw new NullReferenceException();
			if (_b == null)
				throw new NullReferenceException();
			UnitComponentParameter aParameter = aOwner.ComponentParameter;
			UnitCtrl bOwner = _b.Owner;
			if (ReferenceEquals(bOwner, null))
				throw new NullReferenceException();
			UnitComponentParameter bParameter = bOwner.ComponentParameter;
			long aHp = aParameter.Hp;
			long aMaxHp = aParameter.MaxHp;
			long bHp = bParameter.Hp;
			long bMaxHp = bParameter.MaxHp;
			if (BattleUtil.Approximately((double)aHp / (double)aMaxHp, (double)bHp / (double)bMaxHp))
				return CompareRight(_a, _b);
			return CompareLifeAsc(_a, _b);
		}

		[global::OpenPCR.Migration("42058df1-46a0-41f7-b2d3-e5e38e685d7c")]
		public static int CompareAtkDecSameLeft(BasePartsData _a, BasePartsData _b)
		{
			int result = CompareAtkDec(_a, _b);
			if (result == 0)
				return CompareLeft(_a, _b);
			return result;
		}

		[global::OpenPCR.Migration("80205b46-7258-44ba-b52f-0952a4984cc1")]
		public static int CompareAtkDecSameRight(BasePartsData _a, BasePartsData _b)
		{
			int result = CompareAtkDec(_a, _b);
			if (result == 0)
				return CompareRight(_a, _b);
			return result;
		}

		[global::OpenPCR.Migration("699fe114-cee0-4e4b-b2a9-795628fd5a21")]
		public static int CompareMagicStrDecSameLeft(BasePartsData _a, BasePartsData _b)
		{
			int result = CompareMagicStrDec(_a, _b);
			if (result == 0)
				return CompareLeft(_a, _b);
			return result;
		}

		[global::OpenPCR.Migration("491e4d81-89e4-4328-b81a-04d00a9485df")]
		public static int CompareMagicStrDecSameRight(BasePartsData _a, BasePartsData _b)
		{
			int result = CompareMagicStrDec(_a, _b);
			if (result == 0)
				return CompareRight(_a, _b);
			return result;
		}
	}
}
