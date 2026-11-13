using global::OpenPCR.Foundation;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class AttackSealDataForAllEnemy
	{
		private global::Elements.eStateIconType iconType;
		private int maxCount;
		private bool isDisplayCount;
		public global::Elements.Battle.Core.AttackSealData.eExecConditionType ExecConditionType { get; } = (AttackSealData.eExecConditionType)1;

		public float LimitTime;
		public float SealEffectTime;
		[global::OpenPCR.Migration("eb8a1a78-0043-41df-a833-48a70e3a1766")]
		public AttackSealDataForAllEnemy(eStateIconType _iconType, int _maxCount, bool _isDisplayCount, AttackSealData.eExecConditionType _execConditionType, float _sealEffectTime, float _limitTime)
		{
			iconType = _iconType;
			maxCount = _maxCount;
			isDisplayCount = _isDisplayCount;
			ExecConditionType = _execConditionType;
			LimitTime = _limitTime;
			SealEffectTime = _sealEffectTime;
		}

		[global::OpenPCR.Migration("ec419d55-f9c0-4f95-9ccc-f251217ff978")]
		public void AddSeal(UnitCtrl _target)
		{
			UnitComponentAbnormal abnormal = _target.ComponentAbnormal;
			if (!abnormal.SealDictionary.ContainsKey(iconType))
				abnormal.SealDictionary.Add(iconType, new SealData { Max = maxCount, DisplayCount = isDisplayCount });
			SealData data = abnormal.SealDictionary[iconType];
			if (data.GetCurrentCount() == 0)
				abnormal.OnChangeState.Call(iconType, true);
			data.AddSeal(SealEffectTime, _target, iconType, 1);
		}
	}
}
