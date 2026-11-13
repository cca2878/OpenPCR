using System;

namespace Elements.Battle.Core
{
	public class UnitComponentCompare : UnitComponentBase, IUnitComponent
	{
		private UnitComponentMultiPartsBoss componentMultiPartsBoss { get; set; }

		[global::OpenPCR.Migration("4abc9370-814b-4265-b780-590bfb136e6c")]
		public UnitComponentCompare(UnitCtrl _unitController) : base(_unitController)
		{
		}

		[global::OpenPCR.Migration("d5a15479-b843-4509-be1e-4e0ba7ed9981")]
		public void RegisterComponentSet(IUnitComponentContainer _container)
		{
			componentMultiPartsBoss = _container.ComponentMultiPartsBoss;
		}

		[global::OpenPCR.Migration("25eb654d-9e80-44de-a7b6-21cd257e5e6a")]
		public int CompareLifeAscNear(BasePartsData _a, BasePartsData _b)
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
				return CompareDistanceAsc(_a, _b);
			return BattleUnitComparator.CompareLifeAsc(_a, _b);
		}

		[global::OpenPCR.Migration("f63de837-f3e5-44d1-8169-fb7ac4b4f8f2")]
		public int CompareLifeDecNear(BasePartsData _a, BasePartsData _b)
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
				return CompareDistanceAsc(_a, _b);
			return BattleUnitComparator.CompareLifeDec(_a, _b);
		}

		[global::OpenPCR.Migration("559a9088-7699-44eb-a810-bf506a5f7791")]
		public int CompareAtkAscNear(BasePartsData _a, BasePartsData _b)
		{
			if (_a.GetAtkZero() == _b.GetAtkZero())
				return CompareDistanceAsc(_a, _b);
			return BattleUnitComparator.CompareAtkAsc(_a, _b);
		}

		[global::OpenPCR.Migration("b689c9fe-b406-4060-bb7f-2d609ec27a1c")]
		public int CompareAtkDecNear(BasePartsData _a, BasePartsData _b)
		{
			if (_a.GetAtkZero() == _b.GetAtkZero())
				return CompareDistanceAsc(_a, _b);
			return BattleUnitComparator.CompareAtkDec(_a, _b);
		}

		[global::OpenPCR.Migration("74c26008-139e-4324-9cd2-bdc869c28357")]
		public int CompareMagicStrAscNear(BasePartsData _a, BasePartsData _b)
		{
			if (_a.GetMagicStrZero() == _b.GetMagicStrZero())
				return CompareDistanceAsc(_a, _b);
			return BattleUnitComparator.CompareMagicStrAsc(_a, _b);
		}

		[global::OpenPCR.Migration("4ffb1e84-4149-4c42-97e0-146547e8f41f")]
		public int CompareMagicStrDecNear(BasePartsData _a, BasePartsData _b)
		{
			if (_a.GetMagicStrZero() == _b.GetMagicStrZero())
				return CompareDistanceAsc(_a, _b);
			return BattleUnitComparator.CompareMagicStrDec(_a, _b);
		}

		[global::OpenPCR.Migration("4ac01005-6d1a-4dab-9399-7f949507771d")]
		public int CompareHigherAtkOrMagicStrAscSameNear(BasePartsData _a, BasePartsData _b)
		{
			if (_a == null)
				return _b != null ? -1 : 0;
			if (_b == null)
				return 1;
			if (Math.Max(_a.GetAtkZero(), _a.GetMagicStrZero()) == Math.Max(_b.GetAtkZero(), _b.GetMagicStrZero()))
				return CompareDistanceAsc(_a, _b);
			return BattleUnitComparator.CompareHigherAtkOrMagicaStrAsc(_a, _b);
		}

		[global::OpenPCR.Migration("840bb5ad-c7d6-4a66-9799-536a4001661e")]
		public int CompareHigherAtkOrMagicStrDecSameNear(BasePartsData _a, BasePartsData _b)
		{
			if (_a == null)
				return _b != null ? -1 : 0;
			if (_b == null)
				return -1;
			if (Math.Max(_a.GetAtkZero(), _a.GetMagicStrZero()) == Math.Max(_b.GetAtkZero(), _b.GetMagicStrZero()))
				return CompareDistanceAsc(_a, _b);
			return BattleUnitComparator.CompareHigherAtkOrMagicStrDec(_a, _b);
		}

		[global::OpenPCR.Migration("dcae0516-2a64-48a3-9370-efc8f60643d2")]
		public int CompareDistanceAsc(BasePartsData a, BasePartsData b)
		{
			float aX = a.GetBottomTransformPosition().x;
			float aBaseX = unitController.BaseX;
			float bX = b.GetBottomTransformPosition().x;
			float bBaseX = unitController.BaseX;
			float aDistance = aX - aBaseX;
			float bDistance = bX - bBaseX;
			if (BattleUtil.Approximately(aDistance, bDistance))
				return a.Owner.UnitInstanceId.CompareTo(b.Owner.UnitInstanceId);
			return Math.Abs(aDistance).CompareTo(Math.Abs(bDistance));
		}

		[global::OpenPCR.Migration("6d16416d-76d4-49df-a5aa-c80efa38572e")]
		public int CompareDistanceDec(BasePartsData a, BasePartsData b)
		{
			float aX = a.GetBottomTransformPosition().x;
			float aBaseX = unitController.BaseX;
			float bX = b.GetBottomTransformPosition().x;
			float bBaseX = unitController.BaseX;
			float aDistance = aX - aBaseX;
			float bDistance = bX - bBaseX;
			if (BattleUtil.Approximately(aDistance, bDistance))
				return a.Owner.UnitInstanceId.CompareTo(b.Owner.UnitInstanceId);
			return Math.Abs(bDistance).CompareTo(Math.Abs(aDistance));
		}

		[global::OpenPCR.Migration("e07d9705-0c38-42dc-ad75-7964a270a089")]
		public int CompareEnergyDecNearForward(BasePartsData _a, BasePartsData _b)
		{
			if (!BattleUtil.Approximately(_a.Owner.ComponentParameter.Energy, _b.Owner.ComponentParameter.Energy))
				return BattleUnitComparator.CompareEnergyDec(_a, _b);
			float baseX = componentMultiPartsBoss.GetFirstParts(false, 0f).GetPosition().x;
			float aX = _a.GetPosition().x;
			bool aLeft = unitController.IsLeftDir;
			float bX = _b.GetPosition().x;
			bool bLeft = unitController.IsLeftDir;
			bool aForward = !(aX <= baseX) ^ aLeft;
			bool bForward = !(bX <= baseX) ^ bLeft;
			if (aForward == bForward)
				return CompareDistanceAsc(_a, _b);
			return bForward.CompareTo(aForward);
		}

		[global::OpenPCR.Migration("b5fa355a-ab55-49c4-8357-fc4f746e8a77")]
		public int CompareEnergyAscNear(BasePartsData _a, BasePartsData _b)
		{
			if (global::System.Object.ReferenceEquals(@_a, null))
				throw new global::System.NullReferenceException();
			global::Elements.UnitCtrl nativeGetter1 = (@_a).Owner;
			if (global::System.Object.ReferenceEquals(nativeGetter1, null))
				throw new global::System.NullReferenceException();
			global::Elements.Battle.Core.UnitComponentParameter nativeValue2 = (nativeGetter1).ComponentParameter;
			if (global::System.Object.ReferenceEquals(nativeValue2, null))
				throw new global::System.NullReferenceException();
			if (global::System.Object.ReferenceEquals(@_b, null))
				throw new global::System.NullReferenceException();
			double nativeValue3 = (nativeValue2).Energy;
			global::Elements.UnitCtrl nativeGetter4 = (@_b).Owner;
			if (global::System.Object.ReferenceEquals(nativeGetter4, null))
				throw new global::System.NullReferenceException();
			global::Elements.Battle.Core.UnitComponentParameter nativeValue5 = (nativeGetter4).ComponentParameter;
			if (global::System.Object.ReferenceEquals(nativeValue5, null))
				throw new global::System.NullReferenceException();
			double nativeValue6 = (nativeValue5).Energy;
			bool nativeResult1 = global::Elements.BattleUtil.Approximately(nativeValue3, nativeValue6);
			if (((nativeResult1 ? 1u : 0u) & 1u) == 0)
			{
				int nativeResult2 = global::Elements.BattleUnitComparator.CompareEnergyAsc((global::Elements.BasePartsData)(@_a), (global::Elements.BasePartsData)(@_b));
				return unchecked((int)(unchecked((uint)(nativeResult2))));
			}
			else
			{
				int nativeResult3 = (this).CompareDistanceAsc((global::Elements.BasePartsData)(@_a), (global::Elements.BasePartsData)(@_b));
				return unchecked((int)(unchecked((uint)(nativeResult3))));
			}
		}

		[global::OpenPCR.Migration("33e603a1-4dbc-4fc2-8ca9-527e50446c30")]
		public int CompareEnergyDecNear(BasePartsData _a, BasePartsData _b)
		{
			if (global::System.Object.ReferenceEquals(@_a, null))
				throw new global::System.NullReferenceException();
			global::Elements.UnitCtrl nativeGetter1 = (@_a).Owner;
			if (global::System.Object.ReferenceEquals(nativeGetter1, null))
				throw new global::System.NullReferenceException();
			global::Elements.Battle.Core.UnitComponentParameter nativeValue2 = (nativeGetter1).ComponentParameter;
			if (global::System.Object.ReferenceEquals(nativeValue2, null))
				throw new global::System.NullReferenceException();
			if (global::System.Object.ReferenceEquals(@_b, null))
				throw new global::System.NullReferenceException();
			double nativeValue3 = (nativeValue2).Energy;
			global::Elements.UnitCtrl nativeGetter4 = (@_b).Owner;
			if (global::System.Object.ReferenceEquals(nativeGetter4, null))
				throw new global::System.NullReferenceException();
			global::Elements.Battle.Core.UnitComponentParameter nativeValue5 = (nativeGetter4).ComponentParameter;
			if (global::System.Object.ReferenceEquals(nativeValue5, null))
				throw new global::System.NullReferenceException();
			double nativeValue6 = (nativeValue5).Energy;
			bool nativeResult1 = global::Elements.BattleUtil.Approximately(nativeValue3, nativeValue6);
			if (((nativeResult1 ? 1u : 0u) & 1u) == 0)
			{
				int nativeResult2 = global::Elements.BattleUnitComparator.CompareEnergyDec((global::Elements.BasePartsData)(@_a), (global::Elements.BasePartsData)(@_b));
				return unchecked((int)(unchecked((uint)(nativeResult2))));
			}
			else
			{
				int nativeResult3 = (this).CompareDistanceAsc((global::Elements.BasePartsData)(@_a), (global::Elements.BasePartsData)(@_b));
				return unchecked((int)(unchecked((uint)(nativeResult3))));
			}
		}

		[global::OpenPCR.Migration("554edf25-1895-49ee-b3dc-ecf180758a0c")]
		public int CompareBoss(BasePartsData a, BasePartsData b)
		{
			bool flag = a.Owner == unitController;
			bool flag2 = b.Owner == unitController;
			if (flag | flag2)
			{
				return flag.CompareTo(flag2);
			}

			if (a.Owner.IsBoss == b.Owner.IsBoss)
			{
				return CompareDistanceAsc(a, b);
			}

			return b.Owner.IsBoss.CompareTo(a.Owner.IsBoss);
		}

		[global::OpenPCR.Migration("6602f5de-082f-485b-983a-b02fb9585610")]
		public int CompareShadow(BasePartsData _a, BasePartsData _b)
		{
			bool flag = _a.Owner == unitController;
			bool flag2 = _b.Owner == unitController;
			if (flag | flag2)
			{
				return flag.CompareTo(flag2);
			}

			if (_a.Owner.IsShadow == _b.Owner.IsShadow)
			{
				return CompareDistanceAsc(_a, _b);
			}

			return _b.Owner.IsShadow.CompareTo(_a.Owner.IsShadow);
		}
	}
}
