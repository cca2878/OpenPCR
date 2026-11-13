using System.Collections.Generic;
using global::OpenPCR.DbStubs;

namespace Elements.Data
{
	public class MasterExtraEffectData : AbstractMasterData
	{
		public class ExtraEffectData
		{
			public List<int> EnemyIds = new List<int>();
			public List<BattleDefine.eExtraEffectExecTiming> ExecTimings = new List<BattleDefine.eExtraEffectExecTiming>();
			protected global::System.Int32 _extra_effect_id;
			protected global::System.Int32 _content_type;
			protected global::System.Int32 _target_value_1;
			protected global::System.Int32 _target_value_2;
			protected global::System.Int32 _set_id;
			protected global::System.Byte _exec_timing_1;
			protected global::System.Byte _exec_timing_2;
			protected global::System.Byte _exec_timing_3;
			protected global::System.Byte _exec_timing_4;
			protected global::System.Byte _exec_timing_5;
			protected global::System.Int32 _enemy_id_1;
			protected global::System.Int32 _enemy_id_2;
			protected global::System.Int32 _enemy_id_3;
			protected global::System.Int32 _enemy_id_4;
			protected global::System.Int32 _enemy_id_5;
			public global::System.Int32 target_value_2 { [global::OpenPCR.Migration("d4870356-1e04-4653-aef1-d18081189c1e")] get { return _target_value_2; } }
			public global::System.Byte exec_timing_1 { get { return _exec_timing_1; } }
			public global::System.Int32 enemy_id_1 { get { return _enemy_id_1; } }
			public global::System.Byte exec_timing_2 { get { return _exec_timing_2; } }
			public global::System.Int32 enemy_id_2 { get { return _enemy_id_2; } }
			public global::System.Byte exec_timing_3 { get { return _exec_timing_3; } }
			public global::System.Int32 enemy_id_3 { get { return _enemy_id_3; } }
			public global::System.Byte exec_timing_4 { get { return _exec_timing_4; } }
			public global::System.Int32 enemy_id_4 { get { return _enemy_id_4; } }
			public global::System.Byte exec_timing_5 { get { return _exec_timing_5; } }
			public global::System.Int32 enemy_id_5 { get { return _enemy_id_5; } }

			public void SetUp()
			{
				if ((int)enemy_id_1 != 0)
				{
					EnemyIds.Add(enemy_id_1);
					ExecTimings.Add((BattleDefine.eExtraEffectExecTiming)(byte)exec_timing_1);
				}
				if ((int)enemy_id_2 != 0)
				{
					EnemyIds.Add(enemy_id_2);
					ExecTimings.Add((BattleDefine.eExtraEffectExecTiming)(byte)exec_timing_2);
				}
				if ((int)enemy_id_3 != 0)
				{
					EnemyIds.Add(enemy_id_3);
					ExecTimings.Add((BattleDefine.eExtraEffectExecTiming)(byte)exec_timing_3);
				}
				if ((int)enemy_id_4 != 0)
				{
					EnemyIds.Add(enemy_id_4);
					ExecTimings.Add((BattleDefine.eExtraEffectExecTiming)(byte)exec_timing_4);
				}
				if ((int)enemy_id_5 != 0)
				{
					EnemyIds.Add(enemy_id_5);
					ExecTimings.Add((BattleDefine.eExtraEffectExecTiming)(byte)exec_timing_5);
				}
			}

			public ExtraEffectData(int extra_effect_id = 0, int content_type = 0, int target_value_1 = 0, int target_value_2 = 0, int set_id = 0, byte exec_timing_1 = 0, byte exec_timing_2 = 0, byte exec_timing_3 = 0, byte exec_timing_4 = 0, byte exec_timing_5 = 0, int enemy_id_1 = 0, int enemy_id_2 = 0, int enemy_id_3 = 0, int enemy_id_4 = 0, int enemy_id_5 = 0)
			{
				_extra_effect_id = extra_effect_id;
				_content_type = content_type;
				_target_value_1 = target_value_1;
				_target_value_2 = target_value_2;
				_set_id = set_id;
				_exec_timing_1 = exec_timing_1;
				_exec_timing_2 = exec_timing_2;
				_exec_timing_3 = exec_timing_3;
				_exec_timing_4 = exec_timing_4;
				_exec_timing_5 = exec_timing_5;
				_enemy_id_1 = enemy_id_1;
				_enemy_id_2 = enemy_id_2;
				_enemy_id_3 = enemy_id_3;
				_enemy_id_4 = enemy_id_4;
				_enemy_id_5 = enemy_id_5;
				SetUp();
			}
		}

		private MasterQuestDatabase _db;
		private Dictionary<int, ExtraEffectData> _lazyPrimaryKeyDictionary;
		private Dictionary<ulong, List<ExtraEffectData>> _dictionaryOfListWithContentTypeAndTargetValue1;

		[global::OpenPCR.Migration("be4e47b6-de3f-4607-a278-f67506263a5d")]
		public List<ExtraEffectData> GetListWithContentTypeAndTargetValue1(int content_type, int target_value_1)
		{
			ulong key = unchecked((uint)content_type | ((ulong)(uint)target_value_1 << 32));
			if (!_dictionaryOfListWithContentTypeAndTargetValue1.TryGetValue(key, out List<ExtraEffectData> value))
			{
				value = _ListSelectWithContentTypeAndTargetValue1(content_type, target_value_1);
				_dictionaryOfListWithContentTypeAndTargetValue1.Add(key, value);
			}
			return value;
		}

		[global::OpenPCR.Migration("afa35416-c8fc-4b26-a7a4-a5d855309123")]
		private List<ExtraEffectData> _ListSelectWithContentTypeAndTargetValue1(int content_type, int target_value_1)
		{
			_ = new List<ExtraEffectData>();
			if (_db == null)
				throw new System.NullReferenceException();
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterExtraEffectData::_ListSelectWithContentTypeAndTargetValue1(Int32,Int32): Native MasterQuestDatabase.GetSelectQueryWithIndex_ExtraEffectData_ContentType_TargetValue1 requires the SQLite master provider; subsequent BindInt(1,content_type)/BindInt(2,target_value_1), Step/ORM cache insertion and Reset remain at this producer boundary.");
		}

		public MasterExtraEffectData(MasterQuestDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, ExtraEffectData>();
			_dictionaryOfListWithContentTypeAndTargetValue1 = new Dictionary<ulong, List<ExtraEffectData>>();
			_db = db;
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_dictionaryOfListWithContentTypeAndTargetValue1 = null;
		}

		[global::OpenPCR.Migration("143b5c34-489f-4df8-beda-0d1da8adaf33")]
		public ExtraEffectData SearchExtraEffectList(eBattleCategory _battleCategory, int _targetValue1, int _targetValue2)
		{
			List<ExtraEffectData> list = GetListWithContentTypeAndTargetValue1((int)_battleCategory, _targetValue1);
			return list != null ? list.Find(data => (int)data.target_value_2 == _targetValue2) : null;
		}
	}
}
