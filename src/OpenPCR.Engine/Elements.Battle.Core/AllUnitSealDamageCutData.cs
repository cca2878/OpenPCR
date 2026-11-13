using global::Elements;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using global::OpenPCR.Foundation;
using Elements.Battle;

namespace Elements.Battle.Core
{
	public class AllUnitSealDamageCutData : ISingletonField
	{
		private static Yggdrasil<AllUnitSealDamageCutData> staticSingletonTree;
		private static IBattleEffectPool staticBattleEffectPool;
		private static IBattleManagerForUnitActionController staticBattleManager;
		public GameObject EffectPrefab { get; set; }
		public GameObject EffectPrefabLeft { get; set; }
		public UnitCtrl Source { get; set; }
		public eStateIconType IconType { get; set; }
		public int Priority { get; set; }
		public double DamageCutRate { get; set; }
		public int StartCount { get; set; }
		public float Duration { get; set; }
		public List<ActionParameter> TargetActions { get; set; }
		public Skill Skill { get; set; }
		[global::OpenPCR.ImplicitMigration("72d33fd8-a61b-4d47-ad92-8ff45c768530")][global::OpenPCR.Migration("b81034f3-b05f-4f4a-97e4-75a62b5fb218")] protected IBattleEffectPool battleEffectPool => staticBattleEffectPool;
		[global::OpenPCR.ImplicitMigration("98e0e726-6ac1-4b7b-ae17-aff3ee1e412a")][global::OpenPCR.Migration("63a791c5-43b1-4087-b66f-91ddd8a51e41")] private IBattleManagerForUnitActionController battleManager => staticBattleManager;

		[global::OpenPCR.Migration("d4a5b43e-c696-4c4b-91f8-5ffd5da46a35")]
		public AllUnitSealDamageCutData()
		{
			if (staticSingletonTree == null)
			{
				staticSingletonTree = this.CreateSingletonTree();
				staticBattleEffectPool = staticSingletonTree.Get<BattleEffectPool>();
				staticBattleManager = staticSingletonTree.Get<BattleManager>();
			}
		}

		[global::OpenPCR.Migration("3de09425-79a3-41e4-9a8e-1f5f0f27bc5d")]
		public static void StaticRelease()
		{
			staticSingletonTree = null;
			staticBattleEffectPool = null;
			staticBattleManager = null;
		}

		[global::OpenPCR.Migration("467c257c-d9e2-4c83-b3f0-c375aedc9af3")]
		public void Start()
		{
			UnitComponentAbnormal abnormal = Source.ComponentAbnormal;
			if (!abnormal.SealDictionary.ContainsKey(IconType))
				abnormal.SealDictionary.Add(IconType, new SealData { DisplayCount = true, Max = StartCount });
			SealData seal = abnormal.SealDictionary[IconType];
			if (seal.GetCurrentCount() == 0)
				abnormal.OnChangeState.Call(IconType, true);
			seal.AddSeal(Duration, Source, IconType, StartCount);
		}

		[global::OpenPCR.Migration("03a12782-82c0-4761-b930-1ed32f134d4e")]
		public void Exec(UnitCtrl _target, UnitCtrl _effectTarget)
		{
			Action callback = () =>
			{
				GameObject prefab = _effectTarget.IsLeftDir ? EffectPrefabLeft : EffectPrefab;
				if (prefab != null)
				{
					SkillEffectCtrl effect = battleEffectPool.GetEffect(prefab, null);
					effect.ResetParameter(prefab, 0, false);
					effect.InitializeSort();
					effect.PlaySe(Source.ComponentSound.SoundUnitId, _effectTarget.IsLeftDir);
					effect.transform.position = _effectTarget.transform.position;
					effect.transform.parent = ExceptUIRoot.Transform;
					effect.SortTarget = _effectTarget;
					effect.ExecAppendCoroutine(_effectTarget, true);
					battleManager.StartCoroutine(effect.TrackTargetSort(_effectTarget, true));
				}

				Source.UnitActionController.ExecSkillWithTargetPartsAndActionList(Skill, _target.ComponentMultiPartsBoss.GetIndexSmallestParts(), TargetActions, true);
			};
			_target.AppendCoroutine(_target.ComponentState.CurrentState == ActionState.UNION_BURST ? waitUnionBurstEnd(_target, callback) : waitFrame(callback), ePauseType.SYSTEM, Source);
		}

		[global::OpenPCR.Migration("96d1eb6a-c14b-4b98-a375-49b7389bcf3c")]
		private IEnumerator waitFrame(Action _callback)
		{
			yield return null;
			_callback.Call();
		}

		[global::OpenPCR.Migration("22d21876-283e-47d6-b364-dfaa97ff1921")]
		private IEnumerator waitUnionBurstEnd(UnitCtrl _target, Action _callback)
		{
			UnitComponentState state = _target.ComponentState;
			while (state.CurrentState == ActionState.UNION_BURST)
				yield return null;
			_callback.Call();
		}

		[global::OpenPCR.Migration("66910a32-aa3c-41f6-baca-f3b90af8227c")]
		public void Clear()
		{
			SealData seal = Source.ComponentAbnormal.SealDictionary[IconType];
			seal.RemoveSeal(seal.Count, true);
		}
	}
}
