using System;
using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterResistVariationData : AbstractMasterData
	{
		private Elements.Data.MasterEnemyDatabase _db;
		private System.Collections.Generic.Dictionary<System.Int32, Elements.Data.MasterResistVariationData.ResistVariationData> _lazyPrimaryKeyDictionary;
		public class ResistVariationData
		{
			protected global::System.Int32 _resist_variation_id;
			protected global::System.Int32 _value_1;
			protected global::System.Int32 _value_2;
			protected global::System.Int32 _value_3;
			protected global::System.Int32 _value_4;
			public int[] DebuffDecrement { get; set; }
			public ResistVariationData(System.Int32 resist_variation_id = 0, System.Int32 value_1 = 0, System.Int32 value_2 = 0, System.Int32 value_3 = 0, System.Int32 value_4 = 0)
			{
				_resist_variation_id = resist_variation_id;
				_value_1 = value_1;
				_value_2 = value_2;
				_value_3 = value_3;
				_value_4 = value_4;
				SetUp();
			}
			public void SetUp()
			{
				DebuffDecrement = new int[4];
				DebuffDecrement[0] = _value_1;
				DebuffDecrement[1] = _value_2;
				DebuffDecrement[2] = _value_3;
				DebuffDecrement[3] = _value_4;
			}
		}
		public MasterResistVariationData(Elements.Data.MasterEnemyDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, ResistVariationData>();
			_db = db;
		}
		[global::OpenPCR.Migration("3b8786e1-d0bc-4cae-81d0-0c5e43a0b031")]
		public ResistVariationData Get(int resist_variation_id)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterResistVariationData::Get(System.Int32):Elements.Data.MasterResistVariationData+ResistVariationData (resist_variation_data is empty)"); }
		[global::OpenPCR.Migration("6f298874-3d07-4922-9fab-b9b94ff72a77")]
		private ResistVariationData _SelectOne(int resist_variation_id)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterResistVariationData._SelectOne SQLite query production and ORM mapping");
		}
		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
		}
	}
}
