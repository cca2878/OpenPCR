using System.Collections.Generic;
using global::OpenPCR.DbStubs;

namespace Elements.Data
{
	public class MasterEnemyMParts : AbstractMasterData
	{

		[global::OpenPCR.Migration("59175c11-062e-499f-9e8f-1c2d97dd5d6b")]
		public global::Elements.Data.MasterEnemyMParts.EnemyMParts Get(int enemy_id)
		{
			if (_strictPrimaryKeyDictionary != null)
			{
				_strictPrimaryKeyDictionary.TryGetValue(enemy_id, out EnemyMParts value);
				return value;
			}
			if (!_lazyPrimaryKeyDictionary.TryGetValue(enemy_id, out EnemyMParts result))
			{
				result = _db != null ? _SelectOne(enemy_id) : null;
				_lazyPrimaryKeyDictionary[enemy_id] = result;
			}
			return result;
		}

		public class EnemyMParts
		{

			[global::OpenPCR.Migration("cdc9a965-eabc-4493-95dc-9bc4c240a5f5")]
			public int GetChildUnitId(int _index)
			{
				return childEnemyParameter[unchecked(_index - 1)];
			}

			protected global::System.Int32 _enemy_id;
			protected global::System.Int32 _child_enemy_parameter_1;
			protected global::System.Int32 _child_enemy_parameter_2;
			protected global::System.Int32 _child_enemy_parameter_3;
			protected global::System.Int32 _child_enemy_parameter_4;
			protected global::System.Int32 _child_enemy_parameter_5;
			private const int MAX_PARAMETRE_NUM = 5;
			public const int INVALID_INDEX_ID = -1;
			private int[] childEnemyParameter = new int[MAX_PARAMETRE_NUM];
			public global::System.Int32 child_enemy_parameter_1 { get { return _child_enemy_parameter_1; } }
			public global::System.Int32 child_enemy_parameter_2 { get { return _child_enemy_parameter_2; } }
			public global::System.Int32 child_enemy_parameter_3 { get { return _child_enemy_parameter_3; } }
			public global::System.Int32 child_enemy_parameter_4 { get { return _child_enemy_parameter_4; } }
			public global::System.Int32 child_enemy_parameter_5 { get { return _child_enemy_parameter_5; } }

			public EnemyMParts(int enemy_id = 0, int child_enemy_parameter_1 = 0, int child_enemy_parameter_2 = 0, int child_enemy_parameter_3 = 0, int child_enemy_parameter_4 = 0, int child_enemy_parameter_5 = 0)
			{
				_enemy_id = enemy_id;
				_child_enemy_parameter_1 = child_enemy_parameter_1;
				_child_enemy_parameter_2 = child_enemy_parameter_2;
				_child_enemy_parameter_3 = child_enemy_parameter_3;
				_child_enemy_parameter_4 = child_enemy_parameter_4;
				_child_enemy_parameter_5 = child_enemy_parameter_5;
				SetUp();
			}

			public void SetUp()
			{
				childEnemyParameter[0] = child_enemy_parameter_1;
				childEnemyParameter[1] = child_enemy_parameter_2;
				childEnemyParameter[2] = child_enemy_parameter_3;
				childEnemyParameter[3] = child_enemy_parameter_4;
				childEnemyParameter[4] = child_enemy_parameter_5;
			}
		}
		[global::OpenPCR.Migration("0a640cf7-0b6e-467b-9780-f85f148ec002")]
		private EnemyMParts _SelectOne(int enemy_id)
		{
			if (_db == null)
				throw new System.NullReferenceException();
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterEnemyMParts::_SelectOne(Int32): Native MasterEnemyDatabase.GetSelectQuery_EnemyMParts followed by BindInt(1)/Step/ORM cache production requires the SQLite master provider.");
		}

		private Dictionary<int, EnemyMParts> _strictPrimaryKeyDictionary;
		private MasterEnemyDatabase _db;
		private Dictionary<int, EnemyMParts> _lazyPrimaryKeyDictionary;

		public MasterEnemyMParts(MasterEnemyDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, EnemyMParts>();
			_db = db;
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_strictPrimaryKeyDictionary = null;
		}
	}
}
