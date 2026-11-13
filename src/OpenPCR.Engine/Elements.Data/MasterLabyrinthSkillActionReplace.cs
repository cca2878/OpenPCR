using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterLabyrinthSkillActionReplace : AbstractMasterData
	{
		private MasterLabyrinthDatabase _db;
		private Dictionary<int, LabyrinthSkillActionReplace> _lazyPrimaryKeyDictionary;

		public class LabyrinthSkillActionReplace
		{
			protected global::System.Int32 _action_id;
			protected global::System.Int32 _class_id;
			protected global::System.Int32 _action_type;
			protected global::System.Int32 _action_detail_1;
			protected global::System.Int32 _action_detail_2;
			protected global::System.Int32 _action_detail_3;
			protected global::System.Double _action_value_1;
			protected global::System.Double _action_value_2;
			protected global::System.Double _action_value_3;
			protected global::System.Double _action_value_4;
			protected global::System.Double _action_value_5;
			protected global::System.Double _action_value_6;
			protected global::System.Double _action_value_7;
			protected global::System.Int32 _target_assignment;
			protected global::System.Int32 _target_area;
			protected global::System.Int32 _target_range;
			protected global::System.Int32 _target_type;
			protected global::System.Int32 _target_number;
			protected global::System.Int32 _target_count;
			protected global::System.String _description;

			public int DependActionId { get; set; }

			public global::System.Int32 action_id => _action_id;
			public global::System.Int32 class_id => _class_id;
			public global::System.Int32 action_type => _action_type;
			public global::System.Int32 action_detail_1 => _action_detail_1;
			public global::System.Int32 action_detail_2 => _action_detail_2;
			public global::System.Int32 action_detail_3 => _action_detail_3;
			public global::System.Double action_value_1 => _action_value_1;
			public global::System.Double action_value_2 => _action_value_2;
			public global::System.Double action_value_3 => _action_value_3;
			public global::System.Double action_value_4 => _action_value_4;
			public global::System.Double action_value_5 => _action_value_5;
			public global::System.Double action_value_6 => _action_value_6;
			public global::System.Double action_value_7 => _action_value_7;
			public global::System.Int32 target_assignment => _target_assignment;
			public global::System.Int32 target_area => _target_area;
			public global::System.Int32 target_range => _target_range;
			public global::System.Int32 target_type => _target_type;
			public global::System.Int32 target_number => _target_number;
			public global::System.Int32 target_count => _target_count;
			public global::System.String description => _description;

			public LabyrinthSkillActionReplace(int action_id = 0, int class_id = 0, int action_type = 0, int action_detail_1 = 0, int action_detail_2 = 0, int action_detail_3 = 0, double action_value_1 = 0.0, double action_value_2 = 0.0, double action_value_3 = 0.0, double action_value_4 = 0.0, double action_value_5 = 0.0, double action_value_6 = 0.0, double action_value_7 = 0.0, int target_assignment = 0, int target_area = 0, int target_range = 0, int target_type = 0, int target_number = 0, int target_count = 0, string description = "")
			{
				_action_id = action_id;
				_class_id = class_id;
				_action_type = action_type;
				_action_detail_1 = action_detail_1;
				_action_detail_2 = action_detail_2;
				_action_detail_3 = action_detail_3;
				_action_value_1 = action_value_1;
				_action_value_2 = action_value_2;
				_action_value_3 = action_value_3;
				_action_value_4 = action_value_4;
				_action_value_5 = action_value_5;
				_action_value_6 = action_value_6;
				_action_value_7 = action_value_7;
				_target_assignment = target_assignment;
				_target_area = target_area;
				_target_range = target_range;
				_target_type = target_type;
				_target_number = target_number;
				_target_count = target_count;
				_description = description;
			}

			[global::OpenPCR.Migration("2e11f97d-ffed-48e7-b4d5-fa296ed78cba")]
			public static implicit operator MasterSkillAction.SkillAction(LabyrinthSkillActionReplace _data)
			{
				if (_data == null) return null;
				var result = new MasterSkillAction.SkillAction(
					_data.action_id, _data.class_id, unchecked((ushort)(int)_data.action_type), _data.action_detail_1, _data.action_detail_2, _data.action_detail_3, _data.action_value_1, _data.action_value_2, _data.action_value_3, _data.action_value_4, _data.action_value_5, _data.action_value_6, _data.action_value_7, _data.target_assignment, _data.target_area, _data.target_range, _data.target_type, _data.target_number, _data.target_count, _data.description, "");
				result.DependActionId = _data.DependActionId;
				return result;
			}
		}

		public MasterLabyrinthSkillActionReplace(MasterLabyrinthDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, LabyrinthSkillActionReplace>();
			_db = db;
		}

		[global::OpenPCR.Migration("4f9bbd87-ad35-4877-9852-56e0f4d9d243")]
		public LabyrinthSkillActionReplace Get(int action_id)
		{
			if (!_lazyPrimaryKeyDictionary.TryGetValue(action_id, out var value))
			{
				value = _db != null ? _SelectOne(action_id) : null;
				_lazyPrimaryKeyDictionary[action_id] = value;
			}
			return value;
		}

		private LabyrinthSkillActionReplace _SelectOne(int action_id)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterLabyrinthSkillActionReplace._SelectOne: persistent Master database resource row production");
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
		}
	}
}
