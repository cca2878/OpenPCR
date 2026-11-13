using System;
using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterTpRecoveryAt : AbstractMasterData
	{
		private Elements.Data.MasterSkillDatabase _db;
		private System.Collections.Generic.Dictionary<System.Int32, Elements.Data.MasterTpRecoveryAt.TpRecoveryAt> _strictPrimaryKeyDictionary;
		private System.Collections.Generic.Dictionary<System.Int32, Elements.Data.MasterTpRecoveryAt.TpRecoveryAt> _lazyPrimaryKeyDictionary;
		public class TpRecoveryAt
		{
			protected global::System.Int32 _id;
			protected global::System.Int32 _limit_value;
			protected global::System.Double _correction_value;
			public global::System.Int32 limit_value { [global::OpenPCR.Migration("70c9f4d8-b869-4f8e-998b-e8992cb2f6cc")] get => _limit_value; }
			public global::System.Double correction_value { [global::OpenPCR.Migration("632d0e76-614f-407d-a07a-4a83b09f426e")] get => _correction_value; }
			public TpRecoveryAt(System.Int32 id = 0, System.Int32 limit_value = 0, System.Double correction_value = 0.0)
			{
				_id = id;
				_limit_value = limit_value;
				_correction_value = correction_value;
			}
		}
		public List<TpRecoveryAt> Attenuations { get; private set; }
		public MasterTpRecoveryAt(Elements.Data.MasterSkillDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, TpRecoveryAt>();
			_db = db;
		}
		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_strictPrimaryKeyDictionary = null;
		}
	}
}
