using global::OpenPCR.DbStubs;
using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterContentReleaseData : AbstractMasterData
	{
		public class ContentReleaseData
		{
			protected global::System.Int32 _system_id;
			protected global::System.Int32 _team_level;
			protected global::System.Int32 _story_id;
			protected global::System.Int32 _quest_id;
			protected global::System.String _dialog;
			public global::System.Int32 system_id => _system_id;
			[global::OpenPCR.ImplicitMigration("4fb5c2b4-78e7-4f72-9ea2-04e34ffbd1a9")]
			public global::System.Int32 team_level => _team_level;
			[global::OpenPCR.ImplicitMigration("2faf86d8-e323-4570-9314-1e4b584dd7a3")]
			public global::System.Int32 quest_id => _quest_id;

			public ContentReleaseData(int system_id = 0, int team_level = 0, int story_id = 0, int quest_id = 0, string dialog = "")
			{
				_system_id = system_id;
				_team_level = team_level;
				_story_id = story_id;
				_quest_id = quest_id;
				_dialog = dialog;
			}
		}

		public const string TABLE_NAME = "v1_e3db2d180b3df3fbd694dcfd430422d5f3e6879c61a1d42a3d2a8f9391722862";
		public const int HX = 999000;
		public const int MUTE_VO = 999002;
		public const int DUN_SKIP = 999003;
		public const int CUBR = 999004;
		public const int ALTERNATIVE = 999005;
		public const int ARENA_UI_NEW = 999006;
		public const int UNIT_PINYIN_SORT = 999007;
		public const int GACHA_PULL_10SINGLE = 999008;
		public const int UNIT_ROLE = 999009;
		public static List<int> LockContentList = new List<int>();
		[global::OpenPCR.Migration("58e73843-a916-4c0b-b4fd-47190660af71")]
		public static bool HasLock(int _lockId) => LockContentList.Contains(_lockId);
		private MasterEtcDatabase _db;
		private Dictionary<int, ContentReleaseData> _strictPrimaryKeyDictionary;
		private Dictionary<int, ContentReleaseData> _lazyPrimaryKeyDictionary;

		public MasterContentReleaseData(MasterEtcDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, ContentReleaseData>();
			_db = db;
		}

		[global::OpenPCR.Migration("3cefbb1c-775b-4381-bc34-cf14df4745ff")]
		public ContentReleaseData Get(int system_id)
		{
			ContentReleaseData value = null;
			if (_strictPrimaryKeyDictionary != null)
			{
				_strictPrimaryKeyDictionary.TryGetValue(system_id, out value);
				return value;
			}

			if (!_lazyPrimaryKeyDictionary.TryGetValue(system_id, out value))
			{
				value = ((_db != null) ? _SelectOne(system_id) : null);
				_lazyPrimaryKeyDictionary[system_id] = value;
			}

			return value;
		}

		[global::OpenPCR.Migration("88899305-d9e9-45d7-a73d-48a9c59f94f7")]
		private ContentReleaseData _SelectOne(int system_id)
		{
			PreparedQuery selectQuery_ContentReleaseData = _db.GetSelectQuery_ContentReleaseData();
			if (selectQuery_ContentReleaseData == null)
			{
				return null;
			}

			if (!selectQuery_ContentReleaseData.BindInt(1, system_id))
			{
				return null;
			}

			ContentReleaseData result = null;
			if (selectQuery_ContentReleaseData.Step())
			{
				result = _CreateCachedOrmByQueryResult(selectQuery_ContentReleaseData);
			}

			selectQuery_ContentReleaseData.Reset();
			return result;
		}

		private ContentReleaseData _CreateCachedOrmByQueryResult(Query query)
		{
			ContentReleaseData value = null;
			int num = query.GetInt(0);
			int key = num;
			if (_strictPrimaryKeyDictionary != null)
			{
				_strictPrimaryKeyDictionary.TryGetValue(key, out value);
				return value;
			}

			if (!_lazyPrimaryKeyDictionary.TryGetValue(key, out value))
			{
				int team_level = query.GetInt(1);
				int story_id = query.GetInt(2);
				int quest_id = query.GetInt(3);
				string text = query.GetText(4);
				value = new ContentReleaseData(num, team_level, story_id, quest_id, text);
				_lazyPrimaryKeyDictionary.Add(key, value);
			}

			return value;
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_strictPrimaryKeyDictionary = null;
		}

		[global::OpenPCR.Migration("de1a6459-53ca-4690-ae6b-9cd3409caa99")]
		public bool CheckReleaseContents(eSystemId _systemId)
		{
			return UserUtil.IsContentReleased(Get((int)_systemId));
		}
	}
}
