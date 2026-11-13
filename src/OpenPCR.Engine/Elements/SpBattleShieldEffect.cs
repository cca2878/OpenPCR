using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements
{
	public class SpBattleShieldEffect : SkillEffectCtrl
	{
		[Serializable]
		public class ShieldEffectData
		{
			public ParticleSystem TargetEndParticle;

		}
		[SerializeField]
		private List<ShieldEffectData> shieldEffectList;

		private List<Action> waitCallback { get; set; } = new List<Action>();
		[global::OpenPCR.Migration("193b8bf4-9a4d-4d67-ae26-03ba3f763924")]
		public SpBattleShieldEffect()
		{

		}

		[global::OpenPCR.Migration("b6a3ce16-f5dc-4aad-a24a-1c6bea0c82ff")]
		public void DisableEndParticle()
		{
			for (int i = 0; i < shieldEffectList.Count; i++)
			{
				shieldEffectList[i].TargetEndParticle.gameObject.SetActive(value: false);
			}
		}

		[global::OpenPCR.Migration("66a781e7-a536-4aff-b90b-8f6fcdd0a7d2")]
		public void SetEnemyPoint(long _num, UnitCtrl _source, bool _init, float _purposeCount)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.SpBattleShieldEffect::SetEnemyPoint(System.Int64,Elements.UnitCtrl,System.Boolean,System.Single):System.Void");
		}
	}
}
