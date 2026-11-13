using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterEReduction : AbstractMasterData
	{
		public class EReduction
		{
			public double[] DamageTpReduceRateArray;
			protected global::System.Int32 _Id;
			protected global::System.Int32 _Border;
			protected global::System.Int32 _Threshold1;
			protected global::System.Double _Value1;
			protected global::System.Int32 _Threshold2;
			protected global::System.Double _Value2;
			protected global::System.Int32 _Threshold3;
			protected global::System.Double _Value3;
			protected global::System.Int32 _Threshold4;
			protected global::System.Double _Value4;
			protected global::System.Int32 _Threshold5;
			protected global::System.Double _Value5;
			public global::System.Int32 Id { get => _Id; }
			public global::System.Int32 Border { [global::OpenPCR.Migration("c3cf9b91-ec7b-4842-95bc-06a5964f54cd")] get => _Border; }
			public global::System.Int32 Threshold1 { [global::OpenPCR.Migration("13f83198-3b18-4504-ae0e-79588e5f3c30")] get => _Threshold1; }
			public global::System.Double Value1 { [global::OpenPCR.Migration("6986c3c6-79c4-49af-ac7e-438d42f72706")] get => _Value1; }
			public global::System.Int32 Threshold2 { [global::OpenPCR.Migration("f247b67e-44dc-444a-98be-a8ef6126fd11")] get => _Threshold2; }
			public global::System.Double Value2 { [global::OpenPCR.Migration("7394b2df-fabb-4646-a4d4-b115b0a10bb8")] get => _Value2; }
			public global::System.Int32 Threshold3 { [global::OpenPCR.Migration("a48377c0-cb07-4461-b2ac-db9766941d9c")] get => _Threshold3; }
			public global::System.Double Value3 { [global::OpenPCR.Migration("90f8e8fb-7968-4cee-bc9b-2253f1a9a647")] get => _Value3; }
			public global::System.Int32 Threshold4 { [global::OpenPCR.Migration("3b7327a7-0ab0-435e-b3fc-a54ff48b67f1")] get => _Threshold4; }
			public global::System.Double Value4 { [global::OpenPCR.Migration("70e82957-9b25-4089-890c-5bb7708f795d")] get => _Value4; }
			public global::System.Int32 Threshold5 { [global::OpenPCR.Migration("94f27f70-6a76-4e3b-bf4e-4da6439842b4")] get => _Threshold5; }
			public global::System.Double Value5 { [global::OpenPCR.Migration("6d8e1311-d6d9-480f-b657-b7e22e50740a")] get => _Value5; }
			[global::OpenPCR.Migration("aacba246-49d3-4f88-a212-565dab5deb12")]
			public EReduction(int Id = 0, int Border = 0, int Threshold1 = 0, double Value1 = 0.0, int Threshold2 = 0, double Value2 = 0.0, int Threshold3 = 0, double Value3 = 0.0, int Threshold4 = 0, double Value4 = 0.0, int Threshold5 = 0, double Value5 = 0.0)
			{
				_Id = Id;
				_Border = Border;
				_Threshold1 = Threshold1;
				_Value1 = Value1;
				_Threshold2 = Threshold2;
				_Value2 = Value2;
				_Threshold3 = Threshold3;
				_Value3 = Value3;
				_Threshold4 = Threshold4;
				_Value4 = Value4;
				_Threshold5 = Threshold5;
				_Value5 = Value5;
			}
		}
		private MasterUnitDatabase _db;
		private Dictionary<int, EReduction> _strictPrimaryKeyDictionary;
		private Dictionary<int, EReduction> _lazyPrimaryKeyDictionary;
		public Dictionary<int, EReduction> dictionary
		{
			[global::OpenPCR.Migration("f0ace67c-651b-42f7-b670-899e8981a1ac")]
			get
			{
				if (_strictPrimaryKeyDictionary == null)
					_StrictLoadAllEntries();
				return _strictPrimaryKeyDictionary;
			}
		}
		public MasterEReduction(MasterUnitDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, EReduction>();
			_db = db;
		}
		[global::OpenPCR.Migration("b95feb33-baf7-45fd-80db-7df34c90de34")]
		private void _StrictLoadAllEntries()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterEReduction::_StrictLoadAllEntries(): Native SQLite producer must supply the complete strict row dictionary with target Border, Threshold1..5 and Value1..5; missing input is not an empty dictionary or a TP multiplier of one.");
		}
		[global::OpenPCR.Migration("f8797f12-a16d-41bc-b577-e0d9e649cfa6")]
		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_strictPrimaryKeyDictionary = null;
		}
	}
}
