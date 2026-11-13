using global::OpenPCR.Foundation;
using Elements.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class ExSkillData
	{
		private global::Elements.UnitCtrl owner;
		private global::Elements.ExSkillScriptable exSkillScriptable;
		private float coolTime;
		private List<ActionParameter> actionParameterList = new List<ActionParameter>();
		public global::System.Collections.Generic.Dictionary<Elements.Battle.Core.eExSkillCondition, Elements.Battle.Core.ExConditionPassiveData> ExSkillConditionData { get; private set; } = new Dictionary<eExSkillCondition, ExConditionPassiveData>();

		public enum eTryExResult
		{
			FAILED,
			SUCCESS,
			CANCEL,
			SHORTEN
		}

		public static readonly Dictionary<eAbnormalState, List<eExSkillCondition>> ABNORMAL_CONDITION_PAIR = new Dictionary<global::Elements.Battle.Core.eAbnormalState, List<eExSkillCondition>>
		{
			{
				global::Elements.Battle.Core.eAbnormalState.PARALYSIS,
				new List<eExSkillCondition>
				{
					eExSkillCondition.PARALYSIS,
					eExSkillCondition.UNABLE_ACTION
				}
			},
			{
				global::Elements.Battle.Core.eAbnormalState.FREEZE,
				new List<eExSkillCondition>
				{
					eExSkillCondition.FREEZE,
					eExSkillCondition.UNABLE_ACTION
				}
			},
			{
				global::Elements.Battle.Core.eAbnormalState.CHAINED,
				new List<eExSkillCondition>
				{
					eExSkillCondition.CHAINED,
					eExSkillCondition.UNABLE_ACTION
				}
			},
			{
				global::Elements.Battle.Core.eAbnormalState.SLEEP,
				new List<eExSkillCondition>
				{
					eExSkillCondition.SLEEP,
					eExSkillCondition.UNABLE_ACTION
				}
			},
			{
				global::Elements.Battle.Core.eAbnormalState.STUN,
				new List<eExSkillCondition>
				{
					eExSkillCondition.STUN,
					eExSkillCondition.UNABLE_ACTION
				}
			},
			{
				global::Elements.Battle.Core.eAbnormalState.STONE,
				new List<eExSkillCondition>
				{
					eExSkillCondition.STONE,
					eExSkillCondition.UNABLE_ACTION
				}
			},
			{
				global::Elements.Battle.Core.eAbnormalState.PAUSE_ACTION,
				new List<eExSkillCondition>
				{
					eExSkillCondition.PAUSE_ACTION,
					eExSkillCondition.UNABLE_ACTION
				}
			},
			{
				global::Elements.Battle.Core.eAbnormalState.POISON,
				new List<eExSkillCondition>
				{
					eExSkillCondition.POISON,
					eExSkillCondition.SLIP_DAMAGE,
					eExSkillCondition.POISON_OR_VENOM
				}
			},
			{
				global::Elements.Battle.Core.eAbnormalState.BURN,
				new List<eExSkillCondition>
				{
					eExSkillCondition.BURN,
					eExSkillCondition.SLIP_DAMAGE
				}
			},
			{
				global::Elements.Battle.Core.eAbnormalState.CURSE,
				new List<eExSkillCondition>
				{
					eExSkillCondition.CURSE,
					eExSkillCondition.SLIP_DAMAGE,
					eExSkillCondition.CURSE_OR_HEX
				}
			},
			{
				global::Elements.Battle.Core.eAbnormalState.VENOM,
				new List<eExSkillCondition>
				{
					eExSkillCondition.VENOM,
					eExSkillCondition.SLIP_DAMAGE,
					eExSkillCondition.POISON_OR_VENOM
				}
			},
			{
				global::Elements.Battle.Core.eAbnormalState.HEX,
				new List<eExSkillCondition>
				{
					eExSkillCondition.HEX,
					eExSkillCondition.SLIP_DAMAGE,
					eExSkillCondition.CURSE_OR_HEX
				}
			},
			{
				global::Elements.Battle.Core.eAbnormalState.CONVERT,
				new List<eExSkillCondition>
				{
					eExSkillCondition.CONVERT
				}
			},
			{
				global::Elements.Battle.Core.eAbnormalState.CONFUSION,
				new List<eExSkillCondition>
				{
					eExSkillCondition.CONFUSION
				}
			}
		};
		private Skill skill;
		public int LimitNumMax;
		public int LimitNum;
		public bool UseCoolTime;
		[global::OpenPCR.Migration("8eb66659-926f-4243-9bb8-6308eaf169a6")]
		public ExSkillData()
		{ }

		[global::OpenPCR.Migration("ec727b86-d810-47b4-ad9f-5ded951b7984")]
		public void Initialize(int _id, UnitCtrl _owner, UnitActionController _actionController, BattleManager _battleManager)
		{
			owner = _owner;
			exSkillScriptable = ManagerSingleton<ResourceManager>.Instance.LoadResourceImmediately(eResourceId.EQUIPMANT_SKILL, _id, true) as ExSkillScriptable;
			skill = _actionController.CreateExSkill(_id, exSkillScriptable, this);
			if (UseCoolTime) _battleManager.AppendCoroutine(updateCoolTime(_battleManager), ePauseType.SYSTEM, owner);
		}

		[global::OpenPCR.Migration("e70f723d-427d-4e7c-9857-fd6c963198d8")]
		private IEnumerator updateCoolTime(BattleManager _battleManager)
		{
			while (true)
			{
				if (_battleManager.GetBlackOutUnitLength() == 0)
				{
					if (coolTime > 0f) coolTime -= _battleManager.DeltaTime;
					if (coolTime <= 0f)
						foreach (var entry in ExSkillConditionData) owner.ComponentAbnormal.OnChangeStatePassiveEnable.Call(entry.Value.IconType, true);
				}
				yield return null;
			}
		}

		[global::OpenPCR.Migration("96b55af4-659f-41ab-a793-a619a2e98b63")]
		public void Exec(UnitActionController _actionController)
		{ _actionController.ExecExSkill(skill); }

		[global::OpenPCR.Migration("f2ab7a25-96f6-40d4-ab8a-586a2c6006c8")]
		public eTryExResult TryExSkill(eExSkillCondition _skillCondition, UnitActionController _actionController, bool _all, ExConditionPassiveData.eEffectType _targetEffectType = ExConditionPassiveData.eEffectType.NORMAL)
		{
			var condition = ExSkillConditionData[_skillCondition];
			if (!_all && condition.EffectType != _targetEffectType) return eTryExResult.FAILED;
			if (coolTime > 0f || LimitNum == 0) return eTryExResult.FAILED;
			if (BattleManager.Random(0f, 1f) > condition.HitRate) return eTryExResult.FAILED;
			LimitNum = unchecked(LimitNum - 1);
			if (LimitNum == 0) owner.ComponentAbnormal.OnChangeState.Call(condition.IconType, false);
			coolTime = condition.CoolTime;
			if (coolTime > 0f) owner.ComponentAbnormal.OnChangeStatePassiveEnable.Call(condition.IconType, false);
			_actionController.ExecExSkill(skill);
			if (condition.EffectType == ExConditionPassiveData.eEffectType.CANCEL) return eTryExResult.CANCEL;
			if (condition.EffectType == ExConditionPassiveData.eEffectType.SHORTEN) return eTryExResult.SHORTEN;
			return eTryExResult.SUCCESS;
		}

		[global::OpenPCR.Migration("f6942029-3e06-4112-bda0-179a7eab3669")]
		public void ResetLimitNum(UnitCtrl _owner)
		{
			coolTime = 0f; LimitNum = LimitNumMax;
			foreach (var entry in ExSkillConditionData) _owner.ComponentAbnormal.OnChangeState.Call(entry.Value.IconType, true);
		}

		[global::OpenPCR.Migration("eb384f07-fbd9-494f-984d-2ceae276a092")]
		public void DeleteIcon(UnitCtrl _owner)
		{
			foreach (var entry in ExSkillConditionData) _owner.ComponentAbnormal.OnChangeState.Call(entry.Value.IconType, false);
		}
	}
}
