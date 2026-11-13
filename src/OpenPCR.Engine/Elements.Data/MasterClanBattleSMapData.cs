using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterClanBattleSMapData : AbstractMasterData
	{
		public class ClanBattleSMapData : InterfaceClanBattleMapData
		{
			protected global::System.Int32 _clan_battle_id;
			public global::System.Int32 clan_battle_id => _clan_battle_id;
			protected global::System.Int32 _lap_num_from;
			public global::System.Int32 lap_num_from => _lap_num_from;
			protected global::System.Int32 _lap_num_to;
			public global::System.Int32 lap_num_to => _lap_num_to;
			protected global::System.Int32 _aura_effect;
			public global::System.Int32 aura_effect => _aura_effect;
			protected global::System.Int32 _phase;
			public global::System.Int32 phase => _phase;
			public int LapNumFrom => lap_num_from;
			public int LapNumTo => lap_num_to;
			public int AuraEffect => aura_effect;
			public int Phase => phase;
			public int[] BossIds { get; private set; }

			public ClanBattleSMapData(int id = 0, int clan_battle_id = 0, int map_bg = 0, int difficulty = 0, int lap_num_from = 0, int lap_num_to = 0, int boss_id_1 = 0, int boss_id_2 = 0, int boss_id_3 = 0, int boss_id_4 = 0, int boss_id_5 = 0, int extra_battle_flag1 = 0, int extra_battle_flag2 = 0, int extra_battle_flag3 = 0, int extra_battle_flag4 = 0, int extra_battle_flag5 = 0, int aura_effect = 0, int phase = 0, int wave_group_id_1 = 0, int wave_group_id_2 = 0, int wave_group_id_3 = 0, int wave_group_id_4 = 0, int wave_group_id_5 = 0, int fix_reward_id_1 = 0, int fix_reward_id_2 = 0, int fix_reward_id_3 = 0, int fix_reward_id_4 = 0, int fix_reward_id_5 = 0, int damage_rank_id_1 = 0, int damage_rank_id_2 = 0, int damage_rank_id_3 = 0, int damage_rank_id_4 = 0, int damage_rank_id_5 = 0, double reward_gold_coefficient = 0.0, int limited_mana = 0, int last_attack_reward_id = 0, double score_coefficient_1 = 0.0, double score_coefficient_2 = 0.0, double score_coefficient_3 = 0.0, double score_coefficient_4 = 0.0, double score_coefficient_5 = 0.0, int param_adjust_id = 0, int param_adjust_interval = 0)
			{
				// Complete master-row materialization is a resource production boundary; combat lap/phase/aura readers consume the prepared integer fields.
				throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterClanBattleSMapData+ClanBattleSMapData::.ctor");
			}
		}

		private MasterClanBattleDatabase _db;
		private Dictionary<int, ClanBattleSMapData> _strictPrimaryKeyDictionary;
		private Dictionary<int, ClanBattleSMapData> _lazyPrimaryKeyDictionary;
		private Dictionary<int, List<ClanBattleSMapData>> _dictionaryOfListWithClanBattleId;

		public MasterClanBattleSMapData(MasterClanBattleDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, ClanBattleSMapData>();
			_dictionaryOfListWithClanBattleId = new Dictionary<int, List<ClanBattleSMapData>>();
			_db = db;
		}

		public ClanBattleSMapData Get(int id)
		{
			ClanBattleSMapData value = null;
			if (_strictPrimaryKeyDictionary != null)
				_strictPrimaryKeyDictionary.TryGetValue(id, out value);
			else if (!_lazyPrimaryKeyDictionary.TryGetValue(id, out value))
			{
				value = _db != null ? _SelectOne(id) : null;
				_lazyPrimaryKeyDictionary[id] = value;
			}
			return value;
		}

		[global::OpenPCR.Migration("1c08a602-ee2b-4eea-8a9e-d4305ee59bf2")]
		public List<ClanBattleSMapData> GetListWithClanBattleId(int clan_battle_id)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterClanBattleSMapData::GetListWithClanBattleId(System.Int32) (single-mode clan battle preset-effect difficulty)"); }

		private ClanBattleSMapData _SelectOne(int id)
		{
			// SQLite master-row production is outside battle core; no empty row or fallback map is invented.
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterClanBattleSMapData::_SelectOne");
		}

		[global::OpenPCR.Migration("6a7d9f3c-1c23-43c5-84a1-772f89d86760")]
		private List<ClanBattleSMapData> _ListSelectWithClanBattleId(int clan_battle_id)
		{
			if (_db == null)
				throw new System.NullReferenceException();
			// SQLite map-list production is outside battle core; native query order and independent list cache are not replaced by dictionary filtering.
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterClanBattleSMapData::_ListSelectWithClanBattleId");
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_strictPrimaryKeyDictionary = null;
			_dictionaryOfListWithClanBattleId = null;
		}
	}
}
