using System;
using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterHpDrainAt : AbstractMasterData
	{
		private Elements.Data.MasterSkillDatabase _db;
		private System.Collections.Generic.Dictionary<System.Int32, Elements.Data.MasterHpDrainAt.HpDrainAt> _strictPrimaryKeyDictionary;
		private System.Collections.Generic.Dictionary<System.Int32, Elements.Data.MasterHpDrainAt.HpDrainAt> _lazyPrimaryKeyDictionary;
		public class HpDrainAt
		{
			protected global::System.Int32 _id;
			protected global::System.Int32 _limit_value;
			protected global::System.Double _correction_value;
			public global::System.Int32 limit_value { [global::OpenPCR.Migration("cc9adc26-8f11-4e34-92ef-e0fc69ac0bd7")] get => _limit_value; }
			public global::System.Double correction_value { [global::OpenPCR.Migration("47366f63-842e-49b0-9eef-bf010629b864")] get => _correction_value; }
			public HpDrainAt(System.Int32 id = 0, System.Int32 limit_value = 0, System.Double correction_value = 0.0)
			{
				_id = id;
				_limit_value = limit_value;
				_correction_value = correction_value;
			}
		}
		public List<HpDrainAt> Attenuations { get; private set; }
		public MasterHpDrainAt(Elements.Data.MasterSkillDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, HpDrainAt>();
			_db = db;
		}
		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_strictPrimaryKeyDictionary = null;
		}
	}
}
