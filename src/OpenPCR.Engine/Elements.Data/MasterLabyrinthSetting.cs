using global::OpenPCR.DbStubs;
using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterLabyrinthSetting : AbstractMasterData
	{
		private MasterLabyrinthDatabase _db;
		private Dictionary<int, LabyrinthSetting> _lazyPrimaryKeyDictionary;
		public class LabyrinthSetting
		{
			protected global::System.Int32 _setting_id;
			protected global::System.Int32 _value;
			public global::System.Int32 value => _value;

			public LabyrinthSetting(int setting_id = 0, int value = 0)
			{
				_setting_id = setting_id;
				_value = value;
			}
		}

		public MasterLabyrinthSetting(MasterLabyrinthDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, LabyrinthSetting>();
			_db = db;
		}

		[global::OpenPCR.Migration("fe1be38c-b45f-42a6-81c6-6d382cd22c43")]
		public LabyrinthSetting Get(int setting_id)
		{
			if (!_lazyPrimaryKeyDictionary.TryGetValue(setting_id, out LabyrinthSetting value))
			{
				value = _db != null ? _SelectOne(setting_id) : null;
				_lazyPrimaryKeyDictionary[setting_id] = value;
			}

			return value;
		}

		[global::OpenPCR.Migration("87ce3953-c9e8-484b-ba6f-a3601c069f33")]
		public bool HasKey(int setting_id)
		{
			return Get(setting_id) != null;
		}

		[global::OpenPCR.Migration("0ce512f5-b60f-4ba7-9421-2421bf6e1f40")]
		private LabyrinthSetting _SelectOne(int setting_id)
		{
			PreparedQuery query = _db.GetSelectQuery_LabyrinthSetting();
			if (query == null)
				return null;
			LabyrinthSetting value = null;
			if (query.BindInt(1, setting_id))
			{
				if (query.Step())
					value = _CreateCachedOrmByQueryResult(query);
				query.Reset();
			}

			return value;
		}

		private LabyrinthSetting _CreateCachedOrmByQueryResult(Query query)
		{
			int id = query.GetInt(0);
			if (!_lazyPrimaryKeyDictionary.TryGetValue(id, out LabyrinthSetting value))
			{
				int settingValue = query.GetInt(1);
				value = new LabyrinthSetting(id, settingValue);
				_lazyPrimaryKeyDictionary.Add(id, value);
			}

			return value;
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
		}
	}
}
