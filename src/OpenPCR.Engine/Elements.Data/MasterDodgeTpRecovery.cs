using System;
using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterDodgeTpRecovery : AbstractMasterData
	{
		private Elements.Data.MasterQuestDatabase _db;
		private System.Collections.Generic.Dictionary<System.Int32, Elements.Data.MasterDodgeTpRecovery.DodgeTpRecovery> _lazyPrimaryKeyDictionary;
		public class DodgeTpRecovery
		{
			protected global::System.Int32 _system_id;
			protected global::System.Double _recovery_ratio;
			public global::System.Int32 system_id { get => _system_id; }
			public global::System.Double recovery_ratio { [global::OpenPCR.Migration("f4b3e5d6-99ad-4e41-b73d-241a404089bd")] get => _recovery_ratio; }
			public DodgeTpRecovery(System.Int32 system_id = 0, System.Double recovery_ratio = 0.0)
			{
				_system_id = system_id;
				_recovery_ratio = recovery_ratio;
			}
		}
		public MasterDodgeTpRecovery(Elements.Data.MasterQuestDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, DodgeTpRecovery>();
			_db = db;
		}
		[global::OpenPCR.Migration("5a44991d-9b21-49ff-aa9d-b7b54e20f38f")]
		public DodgeTpRecovery Get(int system_id)
		{
			DodgeTpRecovery value = null;
			if (!_lazyPrimaryKeyDictionary.TryGetValue(system_id, out value))
			{
				value = _db != null ? _SelectOne(system_id) : null;
				_lazyPrimaryKeyDictionary[system_id] = value;
			}
			return value;
		}
		[global::OpenPCR.Migration("86f41f8c-31b9-4afa-94ac-b315144c5039")]
		private DodgeTpRecovery _SelectOne(int system_id)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterDodgeTpRecovery._SelectOne SQLite query production and ORM mapping");
		}
		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
		}
	}
}
