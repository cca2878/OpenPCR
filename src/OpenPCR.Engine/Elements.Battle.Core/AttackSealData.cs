using global::OpenPCR.Foundation;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class AttackSealData
	{
		public float LimitTime { get; set; }
		public float SealEffectTime { get; set; }
		public global::Elements.eStateIconType IconType { get; set; }
		public int ActionId { get; set; }
		public global::Elements.Battle.Core.AttackSealData.eExecConditionType ExecConditionType { get; set; }

		[global::OpenPCR.Migration("a4f71bae-df9e-43c9-a33f-0bc813d2d945")]
		public AttackSealData()
		{
		}

		[global::OpenPCR.Migration("090ac174-8c7b-46a4-85cd-6f73ebf9b7ea")]
		public void AddSeal(UnitCtrl _target)
		{
			SealData data = _target.ComponentAbnormal.SealDictionary[IconType];
			if (data.GetCurrentCount() == 0)
				_target.ComponentAbnormal.OnChangeState.Call(IconType, true);
			data.AddSeal(SealEffectTime, _target, IconType, 1);
		}

		public enum eExecConditionType
		{
			DAMAGE_ONCE = 1,
			TARGET = 2,
			DAMAGE = 3,
			CRITICAL = 4,
			HIT = 5
		}
	}
}
