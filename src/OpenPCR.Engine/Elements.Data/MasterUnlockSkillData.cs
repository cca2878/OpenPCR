using System.Collections.Generic;
namespace Elements.Data
{
	public class MasterUnlockSkillData : AbstractMasterData
	{
		private MasterUnitDatabase _db;
		private Dictionary<int, UnlockSkillData> _strictPrimaryKeyDictionary;
		private Dictionary<int, UnlockSkillData> _lazyPrimaryKeyDictionary;
		public class UnlockSkillData
		{
			protected global::System.Int32 _promotion_level;
			protected global::System.Int32 _unlock_skill;
			[global::OpenPCR.ImplicitMigration("1cd9f0dc-b48d-477c-a77f-13ecb37ba77c")]
			public global::System.Int32 promotion_level => _promotion_level;
			public UnlockSkillData(int promotion_level = 0, int unlock_skill = 0)
			{
				_promotion_level = promotion_level;
				_unlock_skill = unlock_skill;
			}
		}
		public MasterUnlockSkillData(MasterUnitDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, UnlockSkillData>();
			_db = db;
		}
		public Dictionary<int, UnlockSkillData> dictionary
		{
			[global::OpenPCR.Migration("d85dae9f-9ffa-4c3b-82ac-202aa266dc27")]
			get
			{
				if (_strictPrimaryKeyDictionary == null) _StrictLoadAllEntries();
				return _strictPrimaryKeyDictionary;
			}
		}
		[global::OpenPCR.Migration("80982e5e-c3a1-4112-9ef6-47aa3a780ec7")]
		private void _StrictLoadAllEntries()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterUnlockSkillData::_StrictLoadAllEntries(): Native persistent MasterUnitDatabase row production; strict parameter read remains implemented");
		}
		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_strictPrimaryKeyDictionary = null;
		}
	}
}
