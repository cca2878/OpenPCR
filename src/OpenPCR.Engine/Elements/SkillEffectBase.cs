using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Elements.Battle.Core;

namespace Elements
{
	[Serializable]
	public class SkillEffectBase
	{
		[global::OpenPCR.Migration("dec96a6f-966d-4d2e-8e85-993238143777")] public GameObject Prefab;
		[global::OpenPCR.Migration("d35d04c7-ccc8-41f3-a01a-0efc4a817acc")] public GameObject PrefabLeft;
		public float[] ExecTime = new float[1];
		public bool IsReaction;
		public bool TargetActionIsReflexive;
		public int TargetActionIndex;
		public int TargetActionId;
		public ActionParameter TargetAction;
		public ActionParameter FireAction;
		public int FireActionId = -1;
		public int TargetMotionIndex;
		public eEffectBehavior EffectBehavior;
		public eEffectTarget EffectTarget;
		public eTargetBone TargetBone;
		public eEffectTarget FireArmEndTarget = eEffectTarget.ALL_TARGET;
		public eTargetBone FireArmEndTargetBone;
		public eTrackType TrackType;
		public eTrackDimension TrackDimension;
		public string TargetBoneName = string.Empty;
		public bool TrackRotation;
		public bool DestroyOnEnemyDead;
		public float CenterY;
		public float DeltaY;
		public bool TrackTarget;
		public float Height;
		public bool IsAbsoluteFireArm;
		public float AbsoluteFireDistance;
		public List<ShakeEffect> ShakeEffects = new List<ShakeEffect>();
		public int TargetBranchId;
		public bool PlayWithCutIn;
		[global::OpenPCR.Migration("74899f01-4bea-493f-85cc-986284d930f9")]
		public SkillEffectBase()
		{
		}

		[global::OpenPCR.Migration("61d12c54-9b38-4199-b7d2-e8172a749fa3")]
		public bool IsVisualEffect()
		{
			return EffectBehavior == eEffectBehavior.NORMAL || (uint)(EffectBehavior - 2) < 3u;
		}

		public Dictionary<UnitCtrl, bool> AlreadyFireArmExecedData { get; set; }
		public List<UnitCtrl> AlreadyFireArmExecedKeys { get; set; }

		[global::OpenPCR.Migration("3d955f75-af98-4b79-8961-4ecd86bfeaf5")]
		public bool AppendAndJudgeAlreadyExeced(UnitCtrl _target)
		{
			if (!AlreadyFireArmExecedData.ContainsKey(_target))
			{
				AlreadyFireArmExecedData.Add(_target, true);
				AlreadyFireArmExecedKeys.Add(_target);
				return false;
			}

			if (AlreadyFireArmExecedData[_target])
				return true;
			AlreadyFireArmExecedData[_target] = true;
			return false;
		}
	}
}
