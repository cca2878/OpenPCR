using System.Text;
using Elements.Data;
using System.Collections.Generic;

namespace Elements.Battle
{
	public class BattleLog : IBattleLog
	{
		private IBattleManagerForBattleLog battleManager;
		private static readonly Dictionary<eBattleLogType, bool> BATTLE_LOG_SETTING = new Dictionary<eBattleLogType, bool>(new eBattleLogType_DictComparer())
		{
			{
				eBattleLogType.BUTTON_TAP,
				false
			},
			{
				eBattleLogType.DAMAGE_CHARGE,
				false
			},
			{
				eBattleLogType.GIVE_VALUE_ADDITIONAL,
				false
			},
			{
				eBattleLogType.GIVE_VALUE_MULTIPLY,
				false
			},
			{
				eBattleLogType.INVALID_VALUE,
				false
			},
			{
				eBattleLogType.MISS,
				false
			},
			{
				eBattleLogType.SET_ABNORMAL,
				false
			},
			{
				eBattleLogType.SET_BUFF_DEBUFF,
				false
			},
			{
				eBattleLogType.SET_DAMAGE,
				false
			},
			{
				eBattleLogType.SET_ENERGY,
				false
			},
			{
				eBattleLogType.SET_RECOVERY,
				false
			},
			{
				eBattleLogType.SET_STATE,
				false
			},
			{
				eBattleLogType.WAVE_END_DAMAGE_AMOUNT,
				false
			},
			{
				eBattleLogType.WAVE_END_HP,
				false
			},
			{
				eBattleLogType.BREAK,
				false
			},
			{
				eBattleLogType.SUSPEND_COUNT,
				false
			},
			{
				eBattleLogType.SUSPEND,
				false
			}
		};
		private List<BattleLogData> battleLogList = new List<BattleLogData>();
		[global::OpenPCR.Migration("6c27c037-e2cb-4b17-b53c-52021697dbc0")]
		public BattleLog(IBattleManagerForBattleLog _battleManager)
		{
			battleManager = _battleManager;
		}

		[global::OpenPCR.Migration("a2b3f3dd-6a50-45e5-ad37-9951255e2565")]
		public void AppendBattleLog(eBattleLogType _battleLogType, int _type, long _value, long _currentValue, int _duration, int _actionId, int _value2 = 1, int _value3 = 1, UnitCtrl _source = null, UnitCtrl _target = null)
		{
			if (BATTLE_LOG_SETTING[_battleLogType])
			{
				int waveCount = battleManager.GetCurrentWaveOffset() + battleManager.CurrentWave + 1;
				BattleLogData battleLogData = new BattleLogData();
				battleLogData.SetBattleLogType((int)_battleLogType);
				battleLogData.SetSourceUnitId((!(_source == null)) ? _source.UnitId : 0);
				battleLogData.SetTargetUnitId((!(_target == null)) ? _target.UnitId : 0);
				battleLogData.SetTargetIsOwnUnit((!(_target == null)) ? ((_target.IsOther ? 200 : 100) + _target.UnitInstanceId) : 0);
				battleLogData.SetType(_type);
				battleLogData.SetCurrentValue(_currentValue);
				battleLogData.SetValue1(_value);
				battleLogData.SetDuration(_duration);
				battleLogData.SetFrame(battleManager.FrameCount);
				battleLogData.SetSourceIsOwnUnit((!(_source == null)) ? ((_source.IsOther ? 200 : 100) + _source.UnitInstanceId) : 0);
				battleLogData.SetActionId(_actionId);
				battleLogData.SetWaveCount(waveCount);
				battleLogData.SetValue2(_value2);
				battleLogData.SetValue3(_value3);
				battleLogList.Add(battleLogData);
			}
		}

		[global::OpenPCR.Migration("cb7ccd9a-a3f4-43f4-b646-bd85f5e31fd7")]
		public bool GetLogEnable(BattleLogType _logType)
		{
			BattleLogType battleLogType = Singleton<UserData>.Instance.InitSettingParameter.BattleLogType;
			BATTLE_LOG_SETTING[eBattleLogType.BUTTON_TAP] = (int)battleLogType.ButtonTap == 1;
			BATTLE_LOG_SETTING[eBattleLogType.DAMAGE_CHARGE] = (long)battleLogType.DamageCharge == 1;
			BATTLE_LOG_SETTING[eBattleLogType.GIVE_VALUE_ADDITIONAL] = (int)battleLogType.GiveValueAdditional == 1;
			BATTLE_LOG_SETTING[eBattleLogType.GIVE_VALUE_MULTIPLY] = (int)battleLogType.GiveValueMultiply == 1;
			BATTLE_LOG_SETTING[eBattleLogType.MISS] = (int)battleLogType.Miss == 1;
			BATTLE_LOG_SETTING[eBattleLogType.SET_ABNORMAL] = (int)battleLogType.SetAbnormal == 1;
			BATTLE_LOG_SETTING[eBattleLogType.SET_BUFF_DEBUFF] = (int)battleLogType.SetBuffDebuff == 1;
			BATTLE_LOG_SETTING[eBattleLogType.SET_DAMAGE] = (long)battleLogType.SetDamage == 1;
			BATTLE_LOG_SETTING[eBattleLogType.SET_ENERGY] = (int)battleLogType.SetEnergy == 1;
			BATTLE_LOG_SETTING[eBattleLogType.SET_RECOVERY] = (int)battleLogType.SetRecovery == 1;
			BATTLE_LOG_SETTING[eBattleLogType.SET_STATE] = (int)battleLogType.SetState == 1;
			BATTLE_LOG_SETTING[eBattleLogType.WAVE_END_DAMAGE_AMOUNT] = (long)battleLogType.WaveEndDamageAmount == 1;
			BATTLE_LOG_SETTING[eBattleLogType.WAVE_END_HP] = (int)battleLogType.WaveEndHp == 1;
			BATTLE_LOG_SETTING[eBattleLogType.BREAK] = (int)battleLogType.Break == 1;
			BATTLE_LOG_SETTING[eBattleLogType.SUSPEND_COUNT] = (int)battleLogType.SuspendCount == 1;
			BATTLE_LOG_SETTING[eBattleLogType.SUSPEND] = (int)battleLogType.Suspend == 1;
			Dictionary<eBattleLogType, bool>.Enumerator enumerator = BATTLE_LOG_SETTING.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Value)
				{
					return true;
				}
			}

			return false;
		}

		[global::OpenPCR.Migration("56cd156e-8242-466d-b62d-00fa48fdcedf")]
		private string battleLogListToCsv(List<BattleLogData> _dataList)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < _dataList.Count; i++)
			{
				BattleLogData battleLogData = _dataList[i];
				stringBuilder.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}\n", battleLogData.battle_log_type, battleLogData.type, battleLogData.target_unit_id, battleLogData.target_is_own_unit, battleLogData.source_unit_id, battleLogData.source_is_own_unit, battleLogData.action_id, battleLogData.frame, battleLogData.value1, battleLogData.value2, battleLogData.value3, battleLogData.duration, battleLogData.current_value, battleLogData.wave_count);
			}

			return stringBuilder.ToString();
		}

		[global::OpenPCR.Migration("67f104b9-adb2-4509-91e9-e1a228b25787")]
		public void AddSendBattleLogCsvPostParam()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleLog.AddSendBattleLogCsvPostParam: API request construction/upload outside offline battle; Native only serializes log and invokes ApiManager with null callback");
		}

		[global::OpenPCR.Migration("cacbd86f-5105-42c0-87af-74e844eca488")]
		public string GetBattleLogListCsv()
		{
			return battleLogListToCsv(battleLogList);
		}

		[global::OpenPCR.Migration("bc747967-575c-4036-aa33-94853b5ec409")]
		public void Release()
		{
			battleManager = null;
		}
	}
}
