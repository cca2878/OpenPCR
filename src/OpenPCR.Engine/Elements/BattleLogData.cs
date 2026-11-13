using System.Runtime.CompilerServices;

namespace Elements
{
	public class BattleLogData
	{
		public int battle_log_type { get; private set; }
		public int type { get; private set; }
		public int target_unit_id { get; private set; }
		public int target_is_own_unit { get; private set; }
		public int source_unit_id { get; private set; }
		public int source_is_own_unit { get; private set; }
		public int action_id { get; private set; }
		public int frame { get; private set; }
		public long value1 { get; private set; }
		public int value2 { get; private set; }
		public int value3 { get; private set; }
		public int duration { get; private set; }
		public long current_value { get; private set; }
		public int wave_count { get; private set; }

		[global::OpenPCR.Migration("197511a0-4466-4cf4-bb75-c16cfb0798ca")]
		private void initializeBattleLogData()
		{
			current_value = 0;
			wave_count = 0;
			duration = 0;
			battle_log_type = 0;
			type = 0;
			target_unit_id = 0;
			target_is_own_unit = 0;
			source_unit_id = 0;
			source_is_own_unit = 0;
			action_id = 0;
			frame = 0;
			value1 = 0;
			value2 = 0;
			value3 = 0;
		}

		[global::OpenPCR.Migration("e56093ed-e1db-4464-8b74-8ed3adbaf394")]
		public BattleLogData()
		{
			initializeBattleLogData();
		}

		[global::OpenPCR.Migration("721c3cae-f5e3-4d42-90e3-5b4906cdc314")]
		public void SetBattleLogType(int _battleLogType)
		{
			battle_log_type = _battleLogType;
		}

		[global::OpenPCR.Migration("c08545f1-cb37-41b0-8138-3af9a0116ef7")]
		public void SetType(int _type)
		{
			type = _type;
		}

		[global::OpenPCR.Migration("7090efe3-b4c2-4f7d-b47a-a8fcc948b209")]
		public void SetTargetUnitId(int _targetUnitId)
		{
			target_unit_id = _targetUnitId;
		}

		[global::OpenPCR.Migration("dee00524-6b25-49b7-aa17-a0369ed2b5d7")]
		public void SetTargetIsOwnUnit(int _targetIsOwnUnit)
		{
			target_is_own_unit = _targetIsOwnUnit;
		}

		[global::OpenPCR.Migration("c9d58f1c-dfe0-4798-abfa-c875e86c4abc")]
		public void SetSourceUnitId(int _sourceUnitId)
		{
			source_unit_id = _sourceUnitId;
		}

		[global::OpenPCR.Migration("b6c69d4f-f117-45a1-925e-789f97331888")]
		public void SetSourceIsOwnUnit(int _sourceIsOwnUnit)
		{
			source_is_own_unit = _sourceIsOwnUnit;
		}

		[global::OpenPCR.Migration("d82a37e1-d4e5-4da0-bd50-baf0c78dac38")]
		public void SetActionId(int _actionId)
		{
			action_id = _actionId;
		}

		[global::OpenPCR.Migration("e28131d0-9ca8-4602-917d-5fef078eb8db")]
		public void SetFrame(int _frame)
		{
			frame = _frame;
		}

		[global::OpenPCR.Migration("043bfc16-208d-4c0e-b718-e4de27fab06e")]
		public void SetValue1(long _value1)
		{
			value1 = _value1;
		}

		[global::OpenPCR.Migration("0f4c4867-aeeb-4392-8dfb-ce23f19b3a76")]
		public void SetValue2(int _value2)
		{
			value2 = _value2;
		}

		[global::OpenPCR.Migration("4bed8cd5-f85b-4180-af43-ee3f6e6f2a1b")]
		public void SetValue3(int _value3)
		{
			value3 = _value3;
		}

		[global::OpenPCR.Migration("65a9d3f0-393a-4bce-9099-6f5669e48f2b")]
		public void SetDuration(int _duration)
		{
			duration = _duration;
		}

		[global::OpenPCR.Migration("f0722370-9421-406d-ae3e-e352c8f3f99f")]
		public void SetCurrentValue(long _currentValue)
		{
			current_value = _currentValue;
		}

		[global::OpenPCR.Migration("697bcf5a-868d-4ec2-b07d-869cd87613b4")]
		public void SetWaveCount(int _waveCount)
		{
			wave_count = _waveCount;
		}
	}
}
