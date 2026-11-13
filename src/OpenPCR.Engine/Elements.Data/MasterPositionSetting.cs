using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterPositionSetting : AbstractMasterData
	{
		private MasterUnitDatabase _db;
		private Dictionary<int, MasterPositionSetting.PositionSetting> _strictPrimaryKeyDictionary;
		private Dictionary<int, MasterPositionSetting.PositionSetting> _lazyPrimaryKeyDictionary;

		public class PositionSetting
		{
			protected global::System.Int32 _position_setting_id;
			protected global::System.Int32 _front;
			protected global::System.Int32 _middle;
			[global::OpenPCR.ImplicitMigration("8a42a422-d8b0-4e17-a6e6-4d8b7dc92bfb")]
			public global::System.Int32 front => _front;
			[global::OpenPCR.ImplicitMigration("1fa5344c-fa99-419c-90c9-78295bbe66e2")]
			public global::System.Int32 middle => _middle;

			public PositionSetting(int position_setting_id = 0, int front = 0, int middle = 0)
			{
				_position_setting_id = position_setting_id;
				_front = front;
				_middle = middle;
			}
		}

		public MasterPositionSetting(MasterUnitDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, MasterPositionSetting.PositionSetting>();
			_db = db;
		}

		[global::OpenPCR.Migration("f2ab8f7c-0e06-4423-a199-74608dd9ccb7")]
		public PositionSetting Get(int position_setting_id)
		{
			PositionSetting value = null;
			if (_strictPrimaryKeyDictionary != null)
				_strictPrimaryKeyDictionary.TryGetValue(position_setting_id, out value);
			else if (!_lazyPrimaryKeyDictionary.TryGetValue(position_setting_id, out value))
			{
				value = _db != null ? _SelectOne(position_setting_id) : null;
				_lazyPrimaryKeyDictionary[position_setting_id] = value;
			}
			return value;
		}

		[global::OpenPCR.Migration("7f422a65-1cda-46a7-bd41-77cbbeba6bf4")]
		private PositionSetting _SelectOne(int position_setting_id)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterPositionSetting._SelectOne: persistent MasterUnitDatabase row production");
		}

		public override void Unload()
		{
			_strictPrimaryKeyDictionary = null;
			_lazyPrimaryKeyDictionary = null;
		}
	}
}
