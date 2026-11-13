using System;
using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterAilmentData : AbstractMasterData
	{
		private Elements.Data.MasterEnemyDatabase _db;
		private System.Collections.Generic.Dictionary<System.Int32, Elements.Data.MasterAilmentData.AilmentData> _strictPrimaryKeyDictionary;
		private System.Collections.Generic.Dictionary<System.Int32, Elements.Data.MasterAilmentData.AilmentData> _lazyPrimaryKeyDictionary;
		public class AilmentData
		{
			protected global::System.Int32 _ailment_id;
			protected global::System.Int32 _ailment_action;
			protected global::System.Int32 _ailment_detail_1;
			public global::System.Int32 ailment_action { [global::OpenPCR.Migration("b2350052-9ecc-437f-a9da-136e264dc131")] get => _ailment_action; }
			public global::System.Int32 ailment_detail_1 { [global::OpenPCR.Migration("9b79a0ab-8ad8-42df-ae00-33423f087787")] get => _ailment_detail_1; }
			public AilmentData(System.Int32 ailment_id = 0, System.Int32 ailment_action = 0, System.Int32 ailment_detail_1 = 0)
			{
				_ailment_id = ailment_id;
				_ailment_action = ailment_action;
				_ailment_detail_1 = ailment_detail_1;
			}
		}
		public MasterAilmentData(Elements.Data.MasterEnemyDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, AilmentData>();
			_db = db;
		}
		[global::OpenPCR.Migration("2cbfd3a0-da9a-49d7-9b57-ffb618875bd0")]
		public AilmentData Get(int ailment_id)
		{
			AilmentData value = null;
			if (_strictPrimaryKeyDictionary != null)
			{
				_strictPrimaryKeyDictionary.TryGetValue(ailment_id, out value);
				return value;
			}
			if (!_lazyPrimaryKeyDictionary.TryGetValue(ailment_id, out value))
			{
				value = _db != null ? _SelectOne(ailment_id) : null;
				_lazyPrimaryKeyDictionary[ailment_id] = value;
			}
			return value;
		}
		[global::OpenPCR.Migration("f9a46a82-1ce8-4931-9db2-b9b451a15b01")]
		private AilmentData _SelectOne(int ailment_id)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterAilmentData._SelectOne SQLite query production and ORM mapping");
		}
		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_strictPrimaryKeyDictionary = null;
		}
	}
}
