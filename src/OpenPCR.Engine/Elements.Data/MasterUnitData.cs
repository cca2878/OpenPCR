using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using global::OpenPCR.DbStubs;

namespace Elements.Data
{
	public class MasterUnitData : AbstractMasterData
	{

		[global::OpenPCR.Migration("5842120f-0202-4779-922c-f060dc186994")]
		public global::Elements.Data.MasterUnitData.UnitData Get(int UnitId)
		{
			UnitData value = null;
			if (_strictPrimaryKeyDictionary != null)
			{
				_strictPrimaryKeyDictionary.TryGetValue(UnitId, out value);
				return value;
			}
			if (!_lazyPrimaryKeyDictionary.TryGetValue(UnitId, out value))
			{
				value = _db != null ? _SelectOne(UnitId) : null;
				_lazyPrimaryKeyDictionary[UnitId] = value;
			}
			return value;
		}

		public UnitData this[int id] { [global::OpenPCR.Migration("569eefe1-83a0-4ece-a33d-1582e68a0db8")] get => Get(id); }

		public class UnitData
		{

			public global::System.Int32 MotionType { [global::OpenPCR.Migration("848ce384-1716-4bc5-9557-5947a6976701")] get => _MotionType; }

			public global::System.Int32 SeType { [global::OpenPCR.Migration("4eb60468-7f1c-41be-b15b-5ff5c3334420")] get => _SeType; }

			protected global::System.Int32 _UnitId;
			protected global::System.String _UnitName;
			protected global::System.Int32 _PrefabId;
			protected global::System.Int32 _MotionType;
			protected global::System.Int32 _SeType;
			protected global::System.Int32 _MoveSpeed;
			protected global::System.Int32 _SearchAreaWidth;
			protected global::System.Int32 _AtkType;
			protected global::System.Double _AtkCastTime;
			protected global::System.Int32 _CutIn1;
			protected global::System.Int32 _cutin1_star6;
			protected global::System.String _Comment;

			protected global::System.Int32 _prefab_id_battle;
			protected global::System.Int32 _CutIn2;
			protected global::System.Int32 _cutin2_star6;
			[global::OpenPCR.ImplicitMigration("8447fdb9-e517-4e19-a32d-58f1b1b59d16")]
			public global::System.Int32 prefab_id_battle => _prefab_id_battle;
			[global::OpenPCR.ImplicitMigration("985ae8d8-b645-4ef1-a4bb-846a0733ad96")]
			public global::System.Int32 CutIn2 => _CutIn2;
			[global::OpenPCR.ImplicitMigration("e21d0d7f-a2b7-479b-aa52-432a1e9f4dfb")]
			public global::System.Int32 cutin2_star6 => _cutin2_star6;
			protected global::System.Int32 _Rarity;
			protected global::System.Int32 _OriginalUnitId;
			[global::OpenPCR.ImplicitMigration("ecc3d639-cbd4-48ac-af66-6eada3975dea")]
			public global::System.Int32 Rarity => _Rarity;
			[global::OpenPCR.ImplicitMigration("64c785c8-3e0a-4dc4-9dc4-3fb24623a315")]
			public global::System.Int32 OriginalUnitId => _OriginalUnitId;
			[global::OpenPCR.ImplicitMigration("9649d90c-7081-4317-9821-7d602cdecd70")]
			public global::System.Int32 MoveSpeed => _MoveSpeed;
			[global::OpenPCR.ImplicitMigration("2b0b43f9-09ab-4b52-9992-61a2c3a384aa")]
			public global::System.Int32 AtkType => _AtkType;
			[global::OpenPCR.ImplicitMigration("e4a74e94-8494-47cd-ae30-3fd5bab19658")]
			public global::System.Double AtkCastTime => _AtkCastTime;

			[global::OpenPCR.ImplicitMigration("29eb260c-2f29-4b0d-a91a-10d5142cd6c8")]
			public global::System.Int32 PrefabId => _PrefabId;
			[global::OpenPCR.ImplicitMigration("f08c0d1c-9389-4a36-95b0-7289c0fbe170")]
			public global::System.Int32 CutIn1 => _CutIn1;
			[global::OpenPCR.ImplicitMigration("ba96a487-fe9a-4544-a8b2-7a505b7a20c8")]
			public global::System.Int32 cutin1_star6 => _cutin1_star6;

			public int VisualChangeFlag { get; private set; }

			public global::System.Int32 UnitId
			{
				[global::OpenPCR.Migration("d7bb4a64-32d3-4c9c-8a76-a0aed717f1ab")]
				get
				{
					return _UnitId;
				}
			}

			public global::System.Int32 SearchAreaWidth
			{
				[global::OpenPCR.Migration("e6bf600a-a5fc-4b76-a855-29900eb240a9")]
				get
				{
					return _SearchAreaWidth;
				}
			}

			[global::OpenPCR.Migration("38cfc248-a112-43b0-8c0b-08ccfc8387ce")]
			public UnitData(MasterUnitEnemyData.UnitEnemyData unitEnemyData)
			{
				_UnitId = unitEnemyData.unit_id;
				_UnitName = unitEnemyData.unit_name;
				_PrefabId = unitEnemyData.prefab_id;
				_MotionType = unitEnemyData.motion_type;
				_SeType = unitEnemyData.se_type;
				_MoveSpeed = unitEnemyData.move_speed;
				_SearchAreaWidth = unitEnemyData.search_area_width;
				_AtkType = unitEnemyData.atk_type;
				_AtkCastTime = unitEnemyData.normal_atk_cast_time;
				_CutIn1 = unitEnemyData.cutin;
				_cutin1_star6 = unitEnemyData.cutin_star6;
				VisualChangeFlag = unitEnemyData.visual_change_flag;
				_Comment = unitEnemyData.comment;
			}
		}
		private Dictionary<int, UnitData> _strictPrimaryKeyDictionary;

		[global::OpenPCR.Migration("5de51276-bc5e-433c-95fd-da6f56f425a3")]
		private UnitData _SelectOne(int UnitId)
		{
			if (_db == null)
				throw new global::System.NullReferenceException();
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterUnitData._SelectOne SQLite prepared-query/bind/step/ORM/reset production");
		}

		private MasterUnitDatabase _db;
		private Dictionary<int, UnitData> _lazyPrimaryKeyDictionary;
		private Dictionary<int, List<UnitData>> _dictionaryOfListWithOriginalUnitId;

		public MasterUnitData(MasterUnitDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, UnitData>();
			_dictionaryOfListWithOriginalUnitId = new Dictionary<int, List<UnitData>>();
			_db = db;
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_strictPrimaryKeyDictionary = null;
			_dictionaryOfListWithOriginalUnitId = null;
		}
	}
}
