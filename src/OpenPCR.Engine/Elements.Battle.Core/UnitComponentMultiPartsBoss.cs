using Elements.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class UnitComponentMultiPartsBoss : UnitComponentBase, IUnitComponent
	{
		public List<PartsData> BossPartsListForBattle { get; private set; }
		public BasePartsData DummyPartsData { get; private set; }
		public PartsData MultiBossPartsData { get; private set; }
		public Dictionary<int, float> PartsBreakTimeDictionary { get; private set; } = new Dictionary<int, float>();

		[global::OpenPCR.Migration("a5408f33-549f-4664-8e76-8446a825c198")]
		public UnitComponentMultiPartsBoss(UnitCtrl _unitController) : base(_unitController)
		{
			BossPartsListForBattle = new List<PartsData>();
		}

		[global::OpenPCR.Migration("5523cf0a-4983-4477-9358-a652ae404af0")]
		public void RegisterComponentSet(IUnitComponentContainer _container)
		{
		}

		[global::OpenPCR.Migration("1493cfa7-d17e-4265-802f-ad860a513243")]
		public BasePartsData GetFirstParts(bool _owner = false, float _basePos = 0f)
		{
			if (!unitController.IsPartsBoss || _owner) return DummyPartsData;
			var targetable = BossPartsListForBattle.FindAll(e => e.GetTargetable());
			BasePartsData closest = targetable[0];
			for (int i = 0; i < targetable.Count; i++)
			{
				PartsData candidate = targetable[i];
				float distance = System.Math.Abs(candidate.GetPosition().x - _basePos);
				float closestDistance = System.Math.Abs(closest.GetPosition().x - _basePos);
				if (distance < closestDistance || BattleUtil.Approximately(distance, closestDistance)) closest = candidate;
			}
			return closest;
		}

		[global::OpenPCR.Migration("e24a4b93-8901-47d5-82eb-cdc3f1a7e519")]
		public IEnumerator UpdateBreak(float _duration, PartsData _data)
		{
			_data.BreakTime = _duration;
			while (_data.BreakTime > 0f)
			{
				if ((long)_data.Owner.ComponentParameter.Hp < 1L) yield break;
				_data.BreakTime -= unitController.GetBattleDeltaTime();
				yield return null;
			}
			_data.BreakTime = 0f;
			_data.SetBreak(false, null, true);
		}

		public bool IsAlreadyCounted { get; private set; }

		[global::OpenPCR.Migration("e0addd7f-8e0a-43f4-aec4-a5f0e9f9d7b5")]
		public void SetDummyPartsData(BasePartsData _dummyPartsData)
		{
			DummyPartsData = _dummyPartsData;
		}

		[global::OpenPCR.Migration("86e26038-5cec-41b7-91cc-7b163d721649")]
		public void SetMultiBossPartsData(PartsData _partsData)
		{
			MultiBossPartsData = _partsData;
		}

		[global::OpenPCR.Migration("e8e4e74a-0afd-4699-a78f-d9ac4f1c1f55")]
		public BasePartsData GetIndexSmallestParts()
		{
			if (!unitController.IsPartsBoss)
				return DummyPartsData;
			return System.Linq.Enumerable.FirstOrDefault(System.Linq.Enumerable.OrderBy(BossPartsListForBattle.FindAll(e => e.GetTargetable()), s => s.Index));
		}

		[global::OpenPCR.Migration("c7671b07-e8b6-4ff2-bd12-a056d8d9ee6c")]
		public void AddPartsBreakTime(int _index, float _value)
		{
			PartsBreakTimeDictionary.Add(_index, _value);
		}

		[global::OpenPCR.Migration("1436a7dd-7e7c-4b1d-9ce0-d92f1b2f21e9")]
		public void SetIsAlreadyCountedParts(bool _isCounted)
		{
			IsAlreadyCounted = _isCounted;
		}
	}
}
