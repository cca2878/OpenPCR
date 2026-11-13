using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterEnemyTalentWeakness : AbstractMasterData
	{
		public class EnemyTalentWeakness
		{
			protected global::System.Int32 _enemy_id;
			protected global::System.Int32 _resist_id;
			public global::System.Int32 resist_id { [global::OpenPCR.Migration("344830e0-2400-46d8-8518-67b82d116146")] get { return _resist_id; } }

			public EnemyTalentWeakness(int enemy_id = 0, int resist_id = 0)
			{
				_enemy_id = enemy_id;
				_resist_id = resist_id;
			}
		}

		private MasterTalentDatabase _db;
		private Dictionary<int, EnemyTalentWeakness> _lazyPrimaryKeyDictionary;

		public MasterEnemyTalentWeakness(MasterTalentDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, EnemyTalentWeakness>();
			_db = db;
		}

		[global::OpenPCR.Migration("218010f9-4913-4b1e-8e77-894a9a0b2b3d")]
		public Dictionary<int, int> GetWeaknessDictionary(int _unitId)
		{
			var result = new Dictionary<int, int>();
			EnemyTalentWeakness enemy = Get(_unitId);
			if (enemy == null)
				return null;
			MasterTalentWeakness master = ManagerSingleton<MasterDataManager>.Instance.masterTalentWeakness;
			MasterTalentWeakness.TalentWeakness weakness = master.Get(enemy.resist_id);
			List<int> values = weakness.TalentWeaknessPercentList;
			for (int i = 0; i < values.Count; i++)
				result.Add(i + 1, values[i]);
			return result;
		}

		[global::OpenPCR.Migration("7360129d-eaf8-4f5d-aecf-33a33e8428f6")]
		public EnemyTalentWeakness Get(int enemy_id)
		{
			if (!_lazyPrimaryKeyDictionary.TryGetValue(enemy_id, out EnemyTalentWeakness value))
			{
				value = _db != null ? _SelectOne(enemy_id) : null;
				_lazyPrimaryKeyDictionary[enemy_id] = value;
			}
			return value;
		}

		[global::OpenPCR.Migration("b8b0747b-a698-45c4-ad76-04eebaca64a3")]
		private EnemyTalentWeakness _SelectOne(int enemy_id)
		{
			if (_db == null)
				throw new System.NullReferenceException();
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterEnemyTalentWeakness::_SelectOne(Int32): Native MasterTalentDatabase.GetSelectQuery_EnemyTalentWeakness and BindInt(1)/Step/ORM caching require the SQLite input producer.");
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
		}
	}
}
