using global::OpenPCR.DbStubs;
using System.Collections.Generic;
namespace Elements.Data
{
	public class MasterItemData : AbstractMasterData
	{
		public class ItemData
		{
			protected global::System.Int32 _Id;

			protected global::System.String _Name;

			protected global::System.String _Description;

			protected global::System.Int32 _PromotionLevel;

			protected global::System.Int32 _Type;

			protected global::System.Int32 _Value;

			protected global::System.Int32 _Price;

			protected global::System.Int32 _LimitNum;

			protected global::System.Int32 _GojuonOrder;

			protected global::System.Int32 _SellCheckDisp;

			protected global::System.String _StartTime;

			protected global::System.String _EndTime;

			public global::System.Int32 Id => _Id;

			[global::OpenPCR.ImplicitMigration("3941835b-fbea-44fc-a782-ae9918034c5a")]
			public global::System.Int32 Type => throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterItemData\u002BItemData::get_Type():System.Int32 (outside first-phase quest-drops boundary)");

			public global::System.Int32 Value => _Value;

			public global::System.String StartTime => _StartTime;

			public ItemData(int Id = 0, string Name = "", string Description = "", int PromotionLevel = 0, int Type = 0, int Value = 0, int Price = 0, int LimitNum = 0, int GojuonOrder = 0, int SellCheckDisp = 0, string StartTime = "", string EndTime = "")
			{
				_Id = Id;
				_Name = Name;
				_Description = Description;
				_PromotionLevel = PromotionLevel;
				_Type = Type;
				_Value = Value;
				_Price = Price;
				_LimitNum = LimitNum;
				_GojuonOrder = GojuonOrder;
				_SellCheckDisp = SellCheckDisp;
				_StartTime = StartTime;
				_EndTime = EndTime;
			}
		}

		private global::Elements.Data.MasterInventoryDatabase _db;

		private global::System.Collections.Generic.Dictionary<System.Int32, Elements.Data.MasterItemData.ItemData> _strictPrimaryKeyDictionary;

		private global::System.Collections.Generic.Dictionary<System.Int32, Elements.Data.MasterItemData.ItemData> _lazyPrimaryKeyDictionary;

		public MasterItemData(global::Elements.Data.MasterInventoryDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, ItemData>();
			_db = db;
		}

		public global::Elements.Data.MasterItemData.ItemData Get(int Id)
		{
			if (_strictPrimaryKeyDictionary != null) { _strictPrimaryKeyDictionary.TryGetValue(Id, out ItemData value); return value; }
			if (!_lazyPrimaryKeyDictionary.TryGetValue(Id, out ItemData row))
			{
				row = _db != null ? _SelectOne(Id) : null;
				_lazyPrimaryKeyDictionary[Id] = row;
			}
			return row;
		}

		private global::Elements.Data.MasterItemData.ItemData _SelectOne(int Id)
		{
			PreparedQuery query = _db.GetSelectQuery_ItemData();
			if (query == null) return null;
			ItemData row = null;
			if (query.BindInt(1, Id)) { if (query.Step()) row = _CreateCachedOrmByQueryResult(query); query.Reset(); }
			return row;
		}

		private global::Elements.Data.MasterItemData.ItemData _CreateCachedOrmByQueryResult(global::OpenPCR.DbStubs.Query query)
		{
			int keyValue = query.GetInt(0);
			if (_strictPrimaryKeyDictionary != null) { _strictPrimaryKeyDictionary.TryGetValue(keyValue, out ItemData row); return row; }
			if (!_lazyPrimaryKeyDictionary.TryGetValue(keyValue, out ItemData value))
			{
				string argument1 = query.GetText(1);
				string argument2 = query.GetText(2);
				int argument3 = query.GetInt(3);
				int argument4 = query.GetInt(4);
				int argument5 = query.GetInt(5);
				int argument6 = query.GetInt(6);
				int argument7 = query.GetInt(7);
				int argument8 = query.GetInt(8);
				int argument9 = query.GetInt(9);
				string argument10 = query.GetText(10);
				string argument11 = query.GetText(11);
				value = new ItemData(keyValue, argument1, argument2, argument3, argument4, argument5, argument6, argument7, argument8, argument9, argument10, argument11);
				_lazyPrimaryKeyDictionary.Add(keyValue, value);
			}
			return value;
		}

		public override void Unload()
		{
			_strictPrimaryKeyDictionary = null;
			_lazyPrimaryKeyDictionary = null;
		}

		[global::OpenPCR.Migration("94352739-4364-4297-8d34-52d9d82d35fd")]
		private void _StrictLoadAllEntries()
		{
			var values = new Dictionary<int, ItemData>();
			using (Query query = _db.GetSelectAllQuery_ItemData())
			{
				while (query.Step()) { int id = query.GetInt(0); ItemData row = _CreateCachedOrmByQueryResult(query); values.Add(id, row); }
			}
			_strictPrimaryKeyDictionary = values;
			_lazyPrimaryKeyDictionary = null;
		}

		public Dictionary<int, ItemData> dictionary
		{
			[global::OpenPCR.Migration("54a798e6-b23b-4c8d-836c-24f1867fa1b0")]
			get
			{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterItemData::get_dictionary():System.Collections.Generic.Dictionary\u00602\u003CSystem.Int32,Elements.Data.MasterItemData\u002BItemData\u003E (outside first-phase quest-drops boundary)"); }
		}
	}
}
