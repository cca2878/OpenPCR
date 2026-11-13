using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using global::OpenPCR.DbStubs;

namespace Elements.Data
{

	public class MasterSkillAction : AbstractMasterData
	{

		[global::OpenPCR.ImplicitMigration("241913c5-0962-465e-8864-c9c0fddca052")]
		public SkillAction this[int action_id] => Get(action_id);

		[global::OpenPCR.Migration("d5e08070-ccd3-4f2f-a005-cbefdf12b904")]
		public SkillAction Get(int action_id)
		{
			SkillAction value = null;
			if (_strictPrimaryKeyDictionary != null)
				_strictPrimaryKeyDictionary.TryGetValue(action_id, out value);
			else if (!_lazyPrimaryKeyDictionary.TryGetValue(action_id, out value))
			{
				value = _db != null ? _SelectOne(action_id) : null;
				_lazyPrimaryKeyDictionary[action_id] = value;
			}
			return value;
		}

		[global::OpenPCR.Migration("bb05212d-3cb6-42a3-a8ca-ae67072418bf")]
		private SkillAction _SelectOne(int action_id)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterSkillAction._SelectOne: persistent Master database resource row production");
		}

		public class SkillAction
		{
			public global::System.Int32 DependActionId { get; set; }

			public global::System.Int32 action_id
			{
				[global::OpenPCR.Migration("a644a854-9c47-4493-9754-05869d772df0")]
				get => _action_id;
			}

			public global::System.UInt16 action_type
			{
				[global::OpenPCR.Migration("f8fc3bfa-d563-4539-a144-4dda9e986d05")]
				get => _action_type;
			}

			public global::System.Int32 action_detail_1
			{
				[global::OpenPCR.Migration("dc5b7ea5-eaa3-4b51-a85e-4ab106f121f9")]
				get => _action_detail_1;
			}

			public global::System.Int32 action_detail_2
			{
				[global::OpenPCR.Migration("5d242e36-546f-451e-923b-f99dc47e438c")]
				get => _action_detail_2;
			}

			public global::System.Int32 action_detail_3
			{
				[global::OpenPCR.Migration("39403896-6571-45e5-bcaa-c4107278c7d7")]
				get => _action_detail_3;
			}

			public global::System.Int32 target_assignment
			{
				[global::OpenPCR.Migration("91fabe6c-b75d-457a-b50e-1981520a51e2")]
				get => _target_assignment;
			}

			public global::System.Int32 target_area
			{
				[global::OpenPCR.Migration("981c475e-7257-4878-aeb4-5beeb9ddd86f")]
				get => _target_area;
			}

			public global::System.Int32 target_range
			{
				[global::OpenPCR.Migration("f0e0212d-155a-4ecf-b72c-42185db3ca1d")]
				get => _target_range;
			}

			public global::System.Int32 target_type
			{
				[global::OpenPCR.Migration("4a901190-a179-461e-ab59-e9cabf243ffe")]
				get => _target_type;
			}

			public global::System.Int32 target_number
			{
				[global::OpenPCR.Migration("1bc925de-7b8f-44ba-abe7-7bcc92254068")]
				get => _target_number;
			}

			public global::System.Int32 target_count
			{
				[global::OpenPCR.Migration("2c9c1f68-bdd4-4d05-8593-eb160066fd99")]
				get => _target_count;
			}

			protected global::System.Int32 _action_id;
			protected global::System.Int32 _class_id;
			protected global::System.UInt16 _action_type;
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
			protected global::System.String _level_up_disp;

			public global::System.Double action_value_1
			{
				[global::OpenPCR.Migration("b12d4fd8-ebec-4f78-934a-125b1c631c9e")]
				get
				{
					return _action_value_1;
				}
			}

			public global::System.Double action_value_2
			{
				[global::OpenPCR.Migration("24428769-24bf-40b1-b4a4-252e6e6b7405")]
				get
				{
					return _action_value_2;
				}
			}

			public global::System.Double action_value_3
			{
				[global::OpenPCR.Migration("171333b7-de03-47d7-b1cc-c198809e140a")]
				get
				{
					return _action_value_3;
				}
			}

			public global::System.Double action_value_4
			{
				[global::OpenPCR.Migration("c65497a0-14b4-42a6-b7f4-97bb7f03e3ec")]
				get
				{
					return _action_value_4;
				}
			}

			public global::System.Double action_value_5
			{
				[global::OpenPCR.Migration("081b3a50-9b90-4190-b70a-4d0fa158547a")]
				get
				{
					return _action_value_5;
				}
			}

			public global::System.Double action_value_6
			{
				[global::OpenPCR.Migration("7d6904fb-a962-4fe8-b419-4990cbe14704")]
				get
				{
					return _action_value_6;
				}
			}

			public global::System.Double action_value_7
			{
				[global::OpenPCR.Migration("0b6dda0c-e570-4882-bddb-5096d8eb96ab")]
				get
				{
					return _action_value_7;
				}
			}

			public SkillAction(int action_id = 0, int class_id = 0, ushort action_type = 0, int action_detail_1 = 0, int action_detail_2 = 0, int action_detail_3 = 0, double action_value_1 = 0.0, double action_value_2 = 0.0, double action_value_3 = 0.0, double action_value_4 = 0.0, double action_value_5 = 0.0, double action_value_6 = 0.0, double action_value_7 = 0.0, int target_assignment = 0, int target_area = 0, int target_range = 0, int target_type = 0, int target_number = 0, int target_count = 0, string description = "", string level_up_disp = "")
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
				_level_up_disp = level_up_disp;
			}
		}
		private MasterSkillDatabase _db;
		private Dictionary<int, SkillAction> _lazyPrimaryKeyDictionary;
		private Dictionary<int, SkillAction> _strictPrimaryKeyDictionary;

		public MasterSkillAction(MasterSkillDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, SkillAction>();
			_db = db;
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_strictPrimaryKeyDictionary = null;
		}
	}
}
