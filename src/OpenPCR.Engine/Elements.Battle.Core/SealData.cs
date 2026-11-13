using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class SealData
	{
		public global::System.Collections.Generic.Dictionary<System.Int32, Elements.Battle.Core.SealBuffDebuffData> SealBuffDebuffDataDictionary { get; set; }
		public bool IsEtherealization { get; set; }
		public global::Elements.SkillEffectCtrl Effect { get; set; }
		public bool DisplayCount { get; set; }
		private int currentSealId { get; set; }
		public global::Elements.Battle.Core.SealCancelData SealCancelData { get; set; }
		public global::System.Action<System.Int32, System.Boolean> OnAddAction { get; set; }
		public global::System.Action<System.Int32, System.Boolean> OnRemoveAction { get; set; }
		public global::Elements.UnitCtrl SourceForMonitorAlive { get; set; }
		public int Count { get; set; }
		public int Max { get; set; }
		private List<int> enableSealIdList { get; set; }
		private Dictionary<int, IEnumerator> updateDataDic { get; set; } = new Dictionary<int, IEnumerator>();

		[global::OpenPCR.Migration("bbc8024d-7b76-4eda-ba47-6f8a8a8b2d4c")]
		public SealData()
		{
		}

		[global::OpenPCR.Migration("7b83ed53-d79a-4805-94c7-a4b4c7150a37")]
		public int GetCurrentCount()
		{
			return Mathf.Min(Count, Max);
		}

		[global::OpenPCR.Migration("a99139e1-ec1d-41f9-b05f-56dd1a30ef04")]
		public void RemoveSeal(int _count, bool _isPassiveSeal)
		{
			if (enableSealIdList == null)
			{
				return;
			}

			_count = Mathf.Min(_count, enableSealIdList.Count);
			for (int i = 0; i < _count; i++)
			{
				int key = enableSealIdList[0];
				enableSealIdList.RemoveAt(0);
				if (_isPassiveSeal && updateDataDic.ContainsKey(key))
				{
					updateDataDic[key].MoveNext();
					updateDataDic.Remove(key);
				}
			}
		}

		[global::OpenPCR.Migration("ee09305c-f128-420c-9127-378fb45362d4")]
		public void AddSeal(float _limitTime, UnitCtrl _target, eStateIconType _iconType, int _count)
		{
			RemoveSeal(Mathf.Max(0, GetCurrentCount() + _count - Max), _isPassiveSeal: false);
			for (int i = 0; i < _count; i++)
			{
				addSealImpl(_limitTime, _target, _iconType);
			}
		}

		[global::OpenPCR.Migration("6b55f63a-2211-4a42-9563-5270183a081c")]
		private void addSealImpl(float _limitTime, UnitCtrl _target, eStateIconType _iconType)
		{
			if (currentSealId == 0)
				enableSealIdList = new List<int>();
			enableSealIdList.Add(currentSealId);
			OnAddAction.Call(currentSealId, Count == 0);
			Count = unchecked(Count + 1);
			if (SealBuffDebuffDataDictionary != null)
				foreach (var pair in SealBuffDebuffDataDictionary)
					pair.Value.UpdateValue(GetCurrentCount());
			if (DisplayCount)
				_target.ComponentAbnormal.OnChangeStateNum.Call(_iconType, GetCurrentCount());
			IEnumerator enumerator = updateData(_limitTime, _target, _iconType, currentSealId);
			updateDataDic.Add(currentSealId, enumerator);
			_target.AppendCoroutine(enumerator, ePauseType.SYSTEM, null);
			currentSealId = unchecked(currentSealId + 1);
		}

		[global::OpenPCR.Migration("05e42b40-23dd-4525-9e25-e88a740004c7")]
		private IEnumerator updateData(float _limitTime, UnitCtrl _target, eStateIconType _iconType, int _sealId)
		{
			UnitCtrl owner = SourceForMonitorAlive;
			while (!(_limitTime <= 0f) && !_target.ComponentState.IdleOnly && !_target.ComponentState.IsDead && !(owner != null && owner.ComponentState.IsDead) && enableSealIdList.Contains(_sealId))
			{
				_limitTime -= _target.DeltaTimeForPause;
				yield return null;
			}

			enableSealIdList.Remove(_sealId);
			Count = unchecked(Count - 1);
			OnRemoveAction.Call(_sealId, Count == 0);
			if (SealBuffDebuffDataDictionary != null)
				foreach (var pair in SealBuffDebuffDataDictionary)
					pair.Value.UpdateValue(GetCurrentCount());
			UnitComponentAbnormal abnormal = _target.ComponentAbnormal;
			if (DisplayCount)
				abnormal.OnChangeStateNum.Call(_iconType, GetCurrentCount());
			if (Count == 0 && GetCurrentCount() == 0)
			{
				if (Effect != null)
					Effect.SetTimeToDie(true);
				abnormal.OnChangeState.Call(_iconType, false);
			}
		}

		[global::OpenPCR.Migration("551be14c-f77a-4cd1-9e23-efaafc9ddc56")]
		public void RemoveSealBuffDebuff(SealBuffDebuffData _data)
		{
			int key = SealBuffDebuffDataDictionary.First(pair => pair.Value == _data).Key;
			SealBuffDebuffDataDictionary.Remove(key);
		}
	}
}
