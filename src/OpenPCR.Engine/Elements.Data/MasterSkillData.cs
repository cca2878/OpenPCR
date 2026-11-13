using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using global::OpenPCR.DbStubs;

namespace Elements.Data
{
	public class MasterSkillData : AbstractMasterData
	{
		[global::OpenPCR.ImplicitMigration("7c2e5a55-505a-4b16-b966-abe3adae28e7")]
		public SkillData this[int skill_id] => Get(skill_id);

		[global::OpenPCR.Migration("d6dbf467-2f2c-4e97-99b4-9fee0397d2af")]
		public SkillData Get(int skill_id)
		{
			SkillData value = null;
			if (!_lazyPrimaryKeyDictionary.TryGetValue(skill_id, out value))
			{
				value = _db != null ? _SelectOne(skill_id) : null;
				_lazyPrimaryKeyDictionary[skill_id] = value;
			}
			return value;
		}

		[global::OpenPCR.Migration("943495e5-f5eb-4849-8d7b-f8e152a78b55")]
		private SkillData _SelectOne(int skill_id)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterSkillData._SelectOne: persistent Master database resource row production");
		}

		public class SkillData
		{

			public global::System.Collections.Generic.List<System.Int32> ActionIds { get; private set; }
			public List<int> DependedIds { get; private set; }
			public bool nameIsNull { get; private set; }
			private MasterDataManager masterDataMng = Elements.ManagerSingleton<MasterDataManager>.Instance;

			public global::System.Int32 skill_id { [global::OpenPCR.Migration("a6b87bda-7f34-4a40-a0c0-8ab1c319e33b")] get => _skill_id; }
			public global::System.String name { [global::OpenPCR.Migration("427512e0-f26d-4a7a-a9b5-829ad4b81b16")] get => _name; }

			public global::System.Int32 SkillId
			{
				[global::OpenPCR.Migration("25ee1da6-ac67-49d7-ab18-773985869272")]
				get => (int)skill_id;
			}

			public global::System.String Name
			{
				[global::OpenPCR.Migration("efe47c18-545e-4add-9e0b-cde908992479")]
				get => (string)name;
			}

			public global::System.Collections.Generic.List<Elements.Data.MasterSkillAction.SkillAction> ActionDataList { get; set; }

			public global::System.Int32 skill_area_width
			{
				[global::OpenPCR.Migration("32e2e866-ccd5-4266-b479-81608b5d541a")]
				get => _skill_area_width;
			}

			public global::System.Double skill_cast_time
			{
				[global::OpenPCR.Migration("e7c36c12-a91e-4a96-82df-ed03d0c7409f")]
				get => _skill_cast_time;
			}

			public global::System.Double boss_ub_cool_time
			{
				[global::OpenPCR.Migration("1b64539d-062a-4902-a933-8760d0e3b311")]
				get => _boss_ub_cool_time;
			}

			public global::System.Int32 icon_type
			{
				[global::OpenPCR.Migration("87d51b92-5ca0-45b7-b3e3-1b0afc80a32e")]
				get => _icon_type;
			}

			protected global::System.Int32 _skill_id;
			protected global::System.String _name;
			protected global::System.Int32 _skill_type;
			protected global::System.Int32 _skill_area_width;
			protected global::System.Double _skill_cast_time;
			protected global::System.Double _boss_ub_cool_time;
			protected global::System.Int32 _action_1;
			protected global::System.Int32 _action_2;
			protected global::System.Int32 _action_3;
			protected global::System.Int32 _action_4;
			protected global::System.Int32 _action_5;
			protected global::System.Int32 _action_6;
			protected global::System.Int32 _action_7;
			protected global::System.Int32 _action_8;
			protected global::System.Int32 _action_9;
			protected global::System.Int32 _action_10;
			protected global::System.Int32 _depend_action_1;
			protected global::System.Int32 _depend_action_2;
			protected global::System.Int32 _depend_action_3;
			protected global::System.Int32 _depend_action_4;
			protected global::System.Int32 _depend_action_5;
			protected global::System.Int32 _depend_action_6;
			protected global::System.Int32 _depend_action_7;
			protected global::System.Int32 _depend_action_8;
			protected global::System.Int32 _depend_action_9;
			protected global::System.Int32 _depend_action_10;
			protected global::System.String _description;
			protected global::System.Int32 _icon_type;

			public void SetUp()
			{
				ActionIds = new List<int>();
				ActionIds.Add(_action_1);
				ActionIds.Add(_action_2);
				ActionIds.Add(_action_3);
				ActionIds.Add(_action_4);
				ActionIds.Add(_action_5);
				ActionIds.Add(_action_6);
				ActionIds.Add(_action_7);
				ActionIds.Add(_action_8);
				ActionIds.Add(_action_9);
				ActionIds.Add(_action_10);
				DependedIds = new List<int>();
				DependedIds.Add(_depend_action_1);
				DependedIds.Add(_depend_action_2);
				DependedIds.Add(_depend_action_3);
				DependedIds.Add(_depend_action_4);
				DependedIds.Add(_depend_action_5);
				DependedIds.Add(_depend_action_6);
				DependedIds.Add(_depend_action_7);
				DependedIds.Add(_depend_action_8);
				DependedIds.Add(_depend_action_9);
				DependedIds.Add(_depend_action_10);
				ActionDataList = new List<MasterSkillAction.SkillAction>();
				for (int i = 0, count = ActionIds.Count; i < count; i++)
				{
					int actionId = ActionIds[i];
					if (actionId != 0)
					{
						var action = masterDataMng.masterSkillAction[actionId];
						if (action != null)
						{
							action.DependActionId = DependedIds[i];
							ActionDataList.Add(action);
							var labyrinth = masterDataMng.masterLabyrinthSkillActionReplace.Get(actionId);
							if (labyrinth != null)
								labyrinth.DependActionId = DependedIds[i];
						}
					}
				}
			}

			public SkillData(int skill_id = 0, bool nameIsNull = true, string name = "", int skill_type = 0, int skill_area_width = 0, double skill_cast_time = 0.0, double boss_ub_cool_time = 0.0, int action_1 = 0, int action_2 = 0, int action_3 = 0, int action_4 = 0, int action_5 = 0, int action_6 = 0, int action_7 = 0, int action_8 = 0, int action_9 = 0, int action_10 = 0, int depend_action_1 = 0, int depend_action_2 = 0, int depend_action_3 = 0, int depend_action_4 = 0, int depend_action_5 = 0, int depend_action_6 = 0, int depend_action_7 = 0, int depend_action_8 = 0, int depend_action_9 = 0, int depend_action_10 = 0, string description = "", int icon_type = 0)
			{
				_skill_id = skill_id;
				this.nameIsNull = nameIsNull;
				_name = name;
				_skill_type = skill_type;
				_skill_area_width = skill_area_width;
				_skill_cast_time = skill_cast_time;
				_boss_ub_cool_time = boss_ub_cool_time;
				_action_1 = action_1;
				_action_2 = action_2;
				_action_3 = action_3;
				_action_4 = action_4;
				_action_5 = action_5;
				_action_6 = action_6;
				_action_7 = action_7;
				_action_8 = action_8;
				_action_9 = action_9;
				_action_10 = action_10;
				_depend_action_1 = depend_action_1;
				_depend_action_2 = depend_action_2;
				_depend_action_3 = depend_action_3;
				_depend_action_4 = depend_action_4;
				_depend_action_5 = depend_action_5;
				_depend_action_6 = depend_action_6;
				_depend_action_7 = depend_action_7;
				_depend_action_8 = depend_action_8;
				_depend_action_9 = depend_action_9;
				_depend_action_10 = depend_action_10;
				_description = description;
				_icon_type = icon_type;
				SetUp();
			}
		}

		private MasterSkillDatabase _db;
		private Dictionary<int, SkillData> _lazyPrimaryKeyDictionary;

		public MasterSkillData(MasterSkillDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, SkillData>();
			_db = db;
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
		}
	}
}
