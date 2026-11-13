using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterClanBattle2MapData : AbstractMasterData
	{
		public class ClanBattle2MapData : InterfaceClanBattleMapData
		{
			protected global::System.Int32 _clan_battle_id;
			[global::OpenPCR.ImplicitMigration("7b7d199b-2832-426b-aee0-b9b82216b306")]
			public global::System.Int32 clan_battle_id => _clan_battle_id;
			protected global::System.Int32 _lap_num_from;
			[global::OpenPCR.ImplicitMigration("9c06e859-4d00-48bb-83f5-9fe122b05deb")]
			public global::System.Int32 lap_num_from => _lap_num_from;
			protected global::System.Int32 _lap_num_to;
			public global::System.Int32 lap_num_to => _lap_num_to;
			protected global::System.Int32 _aura_effect;
			public global::System.Int32 aura_effect => _aura_effect;
			protected global::System.Int32 _phase;
			[global::OpenPCR.ImplicitMigration("fd68ee01-aeb4-41e1-a438-ed7090a1b87d")]
			public global::System.Int32 phase => _phase;
			[global::OpenPCR.ImplicitMigration("29e3480b-905e-4774-a13c-bd46eb88e02c")]
			public int LapNumFrom => lap_num_from;
			public int LapNumTo => lap_num_to;
			public int AuraEffect => aura_effect;
			[global::OpenPCR.ImplicitMigration("a8e0dbb7-6bc0-449d-9a4d-8970e5f1c537")]
			public int Phase => phase;
			public int[] BossIds { get; private set; }

			public ClanBattle2MapData(int id = 0, int clan_battle_id = 0, int map_bg = 0, int difficulty = 0, int lap_num_from = 0, int lap_num_to = 0, int boss_id_1 = 0, int boss_id_2 = 0, int boss_id_3 = 0, int boss_id_4 = 0, int boss_id_5 = 0, int aura_effect = 0, int phase = 0, int wave_group_id_1 = 0, int wave_group_id_2 = 0, int wave_group_id_3 = 0, int wave_group_id_4 = 0, int wave_group_id_5 = 0, int fix_reward_id_1 = 0, int fix_reward_id_2 = 0, int fix_reward_id_3 = 0, int fix_reward_id_4 = 0, int fix_reward_id_5 = 0, int damage_rank_id_1 = 0, int damage_rank_id_2 = 0, int damage_rank_id_3 = 0, int damage_rank_id_4 = 0, int damage_rank_id_5 = 0, double reward_gold_coefficient = 0.0, int limited_mana = 0, int last_attack_reward_id = 0, double score_coefficient_1 = 0.0, double score_coefficient_2 = 0.0, double score_coefficient_3 = 0.0, double score_coefficient_4 = 0.0, double score_coefficient_5 = 0.0, int param_adjust_id = 0, int param_adjust_interval = 0)
			{
				// Complete master-row materialization is a resource production boundary; combat lap/phase/aura readers consume the prepared integer fields.
				throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterClanBattle2MapData+ClanBattle2MapData::.ctor");
			}
		}

		private MasterClanBattleDatabase _db;
		private Dictionary<int, ClanBattle2MapData> _strictPrimaryKeyDictionary;
		private Dictionary<int, ClanBattle2MapData> _lazyPrimaryKeyDictionary;
		private Dictionary<int, List<ClanBattle2MapData>> _dictionaryOfListWithClanBattleId;

		public Dictionary<int, ClanBattle2MapData> dictionary
		{
			[global::OpenPCR.Migration("2fdf017f-8aae-4aca-9137-8323063e0cc1")]
			get
			{
				if (_strictPrimaryKeyDictionary == null)
					_StrictLoadAllEntries();
				return _strictPrimaryKeyDictionary;
			}
		}

		public MasterClanBattle2MapData(MasterClanBattleDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, ClanBattle2MapData>();
			_dictionaryOfListWithClanBattleId = new Dictionary<int, List<ClanBattle2MapData>>();
			_db = db;
		}

		public ClanBattle2MapData Get(int id)
		{
			ClanBattle2MapData value = null;
			if (_strictPrimaryKeyDictionary != null)
				_strictPrimaryKeyDictionary.TryGetValue(id, out value);
			else if (!_lazyPrimaryKeyDictionary.TryGetValue(id, out value))
			{
				value = _db != null ? _SelectOne(id) : null;
				_lazyPrimaryKeyDictionary[id] = value;
			}
			return value;
		}

		[global::OpenPCR.Migration("5f5ded12-2e9c-4c61-ad21-562d59b1affb")]
		public List<ClanBattle2MapData> GetListWithClanBattleId(int clan_battle_id)
		{
			List<ClanBattle2MapData> value = null;
			if (!_dictionaryOfListWithClanBattleId.TryGetValue(clan_battle_id, out value))
			{
				value = _ListSelectWithClanBattleId(clan_battle_id);
				_dictionaryOfListWithClanBattleId.Add(clan_battle_id, value);
			}
			return value;
		}

		private ClanBattle2MapData _SelectOne(int id)
		{
			// SQLite master-row production is outside battle core; no empty row or fallback map is invented.
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterClanBattle2MapData::_SelectOne");
		}

		[global::OpenPCR.Migration("b35f0a24-1c89-4667-9df8-ba6c9d5fef63")]
		private List<ClanBattle2MapData> _ListSelectWithClanBattleId(int clan_battle_id)
		{
			if (_db == null)
				throw new System.NullReferenceException();
			// SQLite map-list production is outside battle core; native query order and independent list cache are not replaced by dictionary filtering.
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterClanBattle2MapData::_ListSelectWithClanBattleId");
		}

		[global::OpenPCR.Migration("5eefe2d4-dc04-4df3-a640-9e7c1806947f")]
		private void _StrictLoadAllEntries()
		{
			if (_db == null)
				throw new System.NullReferenceException();
			// SQLite complete map-table production is outside battle core; missing rows fail explicitly.
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterClanBattle2MapData::_StrictLoadAllEntries");
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_strictPrimaryKeyDictionary = null;
			_dictionaryOfListWithClanBattleId = null;
		}
	}
}
