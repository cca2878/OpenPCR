using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Elements.Data;

namespace Elements.Battle.Core
{
	public class PassiveSkillInitializer
	{
		private List<Dictionary<eParamType, PassiveActionValue>> unitPassiveDictionaryByUnit { get; set; }
		private List<Dictionary<eParamType, PassiveActionValue>> unitPassiveDictionaryByEnemy { get; set; }
		private List<Dictionary<eParamType, PassiveActionValue>> enemyPassiveDictionaryByUnit { get; set; }
		private List<Dictionary<eParamType, PassiveActionValue>> enemyPassiveDictionaryByEnemy { get; set; }


		[global::OpenPCR.Migration("60f096c6-c7de-4dca-8052-00ef0df422be")]
		public PassiveSkillInitializer(List<Dictionary<eParamType, PassiveActionValue>> _unitPassiveDictionaryByUnit, List<Dictionary<eParamType, PassiveActionValue>> _unitPassiveDictionaryByEnemy, List<Dictionary<eParamType, PassiveActionValue>> _enemyPassiveDictionaryByUnit, List<Dictionary<eParamType, PassiveActionValue>> _enemyPassiveDictionaryByEnemy)
		{
			unitPassiveDictionaryByUnit = _unitPassiveDictionaryByUnit;
			unitPassiveDictionaryByEnemy = _unitPassiveDictionaryByEnemy;
			enemyPassiveDictionaryByUnit = _enemyPassiveDictionaryByUnit;
			enemyPassiveDictionaryByEnemy = _enemyPassiveDictionaryByEnemy;
		}

		[global::OpenPCR.Migration("99c963aa-a00a-4d1a-ba88-4e41bd371ad9")]
		public void InitializeExAndFreeSkill(UnitData _unitData, ePassiveInitType _passiveInitType, int _waveNumber, UnitCtrl _unit)
		{
			if (global::System.Object.ReferenceEquals(@_unitData, null))
				throw new global::System.NullReferenceException();
			global::System.Collections.Generic.List<global::Elements.SkillLevelInfo> nativeValue1 = (@_unitData).ExSkill;
			(this).initializePassiveSkillList((global::System.Collections.Generic.List<global::Elements.SkillLevelInfo>)(nativeValue1), unchecked((global::Elements.Battle.Core.ePassiveInitType)(unchecked((uint)(@_passiveInitType)))), unchecked((int)(unchecked((uint)(@_waveNumber)))), (global::Elements.UnitCtrl)(@_unit));
			global::System.Collections.Generic.List<global::Elements.SkillLevelInfo> nativeValue2 = (@_unitData).FreeSkill;
			(this).initializePassiveSkillList((global::System.Collections.Generic.List<global::Elements.SkillLevelInfo>)(nativeValue2), unchecked((global::Elements.Battle.Core.ePassiveInitType)(unchecked((uint)(@_passiveInitType)))), unchecked((int)(unchecked((uint)(@_waveNumber)))), (global::Elements.UnitCtrl)(@_unit));
			global::System.Collections.Generic.List<global::Elements.SkillLevelInfo> nativeValue3 = (@_unitData).MainSkill;
			(this).initializePassiveSkillList((global::System.Collections.Generic.List<global::Elements.SkillLevelInfo>)(nativeValue3), unchecked((global::Elements.Battle.Core.ePassiveInitType)(unchecked((uint)(@_passiveInitType)))), unchecked((int)(unchecked((uint)(@_waveNumber)))), (global::Elements.UnitCtrl)(@_unit));
		}

		[global::OpenPCR.Migration("0cac0b9c-39e2-4aea-8444-de5f92416849")]
		private void initializePassiveSkillList(List<SkillLevelInfo> _skillLevelInfoList, ePassiveInitType _passiveInitType, int _waveNumber, UnitCtrl _unit)
		{
			int i = 0;
			for (int count = _skillLevelInfoList.Count; i < count; i++)
			{
				initializePassiveSkill(_skillLevelInfoList[i], _passiveInitType, _waveNumber, _unit);
			}
		}

		[global::OpenPCR.Migration("7a79bb30-0790-4fd0-9a32-a91a5e4731cd")]
		private void initializePassiveSkill(SkillLevelInfo _skillLevelInfo, ePassiveInitType _passiveInitType, int _waveNumber, UnitCtrl _unit)
		{
			if ((int)_skillLevelInfo.SkillId != 0)
			{
				MasterSkillData.SkillData skillData = ManagerSingleton<MasterDataManager>.Instance.masterSkillData[_skillLevelInfo.SkillId];
				int i = 0;
				for (int count = skillData.ActionIds.Count; i < count; i++)
				{
					initializePassiveAction(skillData.ActionIds[i], _skillLevelInfo.SkillLevel, _passiveInitType, _waveNumber, _unit);
				}
			}
		}

		[global::OpenPCR.Migration("4e5f815f-ea3d-469f-bea0-71c45036f78b")]
		private void initializePassiveAction(int _actionId, int _level, ePassiveInitType _passiveInitType, int _waveNumber, UnitCtrl _unit)
		{
			if (_actionId == 0)
			{
				return;
			}
			MasterSkillAction.SkillAction skillAction = ManagerSingleton<MasterDataManager>.Instance.masterSkillAction[_actionId];
			if ((ushort)skillAction.action_type == 90)
			{
				switch (_passiveInitType)
				{
					case ePassiveInitType.ALL_BY_ENEMY:
						initializePassiveActionAllByEnemy(skillAction, _level, _waveNumber);
						break;
					case ePassiveInitType.ALL_BY_UNIT:
						initializePassiveActionAllByUnit(skillAction, _level, _waveNumber);
						break;
					case ePassiveInitType.OWNER:
						initializePassiveActionOwner(skillAction, _level, _unit);
						break;
				}
			}
		}

		[global::OpenPCR.Migration("c9aae079-d717-4d32-a18d-dd7a2f90b0db")]
		private void initializePassiveActionAllByEnemy(MasterSkillAction.SkillAction _actionData, int _level, int _waveNumber)
		{
			switch ((ePassiveSideType)(int)_actionData.target_assignment)
			{
				case ePassiveSideType.ALL:
					appendPassiveSkill(enemyPassiveDictionaryByEnemy[_waveNumber], _actionData, _level);
					break;
				case ePassiveSideType.ALL_OTHER:
					if ((int)_actionData.action_detail_1 != 1)
					{
						appendPassiveSkill(unitPassiveDictionaryByEnemy[_waveNumber], _actionData, _level);
					}
					break;
			}
		}

		[global::OpenPCR.Migration("56729e3d-ca9d-4c1a-a700-a919af7d7110")]
		private void initializePassiveActionAllByUnit(MasterSkillAction.SkillAction _actionData, int _level, int _waveNumber)
		{
			switch ((ePassiveSideType)(int)_actionData.target_assignment)
			{
				case ePassiveSideType.ALL:
					appendPassiveSkill(unitPassiveDictionaryByUnit[_waveNumber], _actionData, _level);
					break;
				case ePassiveSideType.ALL_OTHER:
					if ((int)_actionData.action_detail_1 != 1)
					{
						appendPassiveSkill(enemyPassiveDictionaryByUnit[_waveNumber], _actionData, _level);
					}
					break;
			}
		}

		[global::OpenPCR.Migration("59477ff4-f1a2-484d-b687-1920ad53ae3f")]
		private void initializePassiveActionOwner(MasterSkillAction.SkillAction _actionData, int _level, UnitCtrl _unit)
		{
			ePassiveSideType ePassiveSideType2 = (ePassiveSideType)(int)_actionData.target_assignment;
			if (ePassiveSideType2 != ePassiveSideType.OWNER)
			{
				_ = ePassiveSideType2 - 1;
				_ = 1;
			}
			else if (_unit != null)
			{
				appendPassiveSkill(_unit.ComponentPassiveSkill.OwnerPassiveAction, _actionData, _level);
			}
		}

		[global::OpenPCR.Migration("1ac225ed-f4d6-495a-83d7-e0e969c3e44f")]
		private void appendPassiveSkill(Dictionary<eParamType, PassiveActionValue> _target, MasterSkillAction.SkillAction _actionData, int _level)
		{
			eParamType key = (eParamType)(int)_actionData.action_detail_1;
			bool flag = _target.TryGetValue(key, out var value);
			if (!flag)
			{
				value = default(PassiveActionValue);
			}
			switch ((ePassiveSkillValueType)OpenPCR.NativeNumeric.ToInt32((double)_actionData.action_value_1))
			{
				case ePassiveSkillValueType.VALUE:
					value.Value += calculateValue(_actionData, _level);
					break;
				case ePassiveSkillValueType.PERCENTAGE:
					value.Percentage += calculateValue(_actionData, _level);
					break;
			}
			if (!flag)
			{
				_target.Add(key, value);
			}
			else
			{
				_target[key] = value;
			}
		}

		[global::OpenPCR.Migration("025f6f09-b78c-4425-b4d4-2a2ede5663cf")]
		private float calculateValue(MasterSkillAction.SkillAction _actionData, int _level)
		{
			float result = 0f;
			if (_level == 0)
			{
				return 0f;
			}
			switch ((ePassiveBuffDebuff)OpenPCR.NativeNumeric.ToInt32((double)_actionData.action_value_4))
			{
				case ePassiveBuffDebuff.BUFF:
					result = (float)((double)_actionData.action_value_2 + (double)_actionData.action_value_3 * (double)_level);
					break;
				case ePassiveBuffDebuff.DEBUFF:
					result = -(float)((double)_actionData.action_value_2 + (double)_actionData.action_value_3 * (double)_level);
					break;
			}
			return result;
		}
	}
}
