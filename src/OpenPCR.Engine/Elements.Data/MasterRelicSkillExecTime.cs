using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterRelicSkillExecTime : AbstractMasterData
	{
		private MasterLabyrinthDatabase _db;
		private Dictionary<int, RelicSkillExecTime> _lazyPrimaryKeyDictionary;
		private Dictionary<int, List<RelicSkillExecTime>> _dictionaryOfListWithSkillId;

		public class RelicSkillExecTime
		{
			protected int _id;
			protected int _skill_id;
			protected double _exec_time;

			public int id => _id;
			public int skill_id => _skill_id;
			public double exec_time => _exec_time;

			[global::OpenPCR.Migration("de06d8c0-7730-4f6f-befc-207b24d92cb9")]
			public RelicSkillExecTime(int id = 0, int skill_id = 0, double exec_time = 0.0)
			{
				_id = id;
				_skill_id = skill_id;
				_exec_time = exec_time;
			}
		}

		[global::OpenPCR.Migration("8079f245-37a5-4b75-bbc7-70c5d42be280")]
		public MasterRelicSkillExecTime(MasterLabyrinthDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, RelicSkillExecTime>();
			_dictionaryOfListWithSkillId = new Dictionary<int, List<RelicSkillExecTime>>();
			_db = db;
		}

		[global::OpenPCR.Migration("c0e68add-aad3-4d61-b0ef-17061c29606a")]
		public RelicSkillExecTime Get(int id)
		{
			if (!_lazyPrimaryKeyDictionary.TryGetValue(id, out var value))
			{
				value = _db != null ? _SelectOne(id) : null;
				_lazyPrimaryKeyDictionary[id] = value;
			}
			return value;
		}

		[global::OpenPCR.Migration("10bc565b-cbd4-4f9e-b7f2-26572f879bfc")]
		private RelicSkillExecTime _SelectOne(int id)
		{
			throw new global::OpenPCR.OutOfBoundaryException("MasterRelicSkillExecTime._SelectOne: persistent Master database resource row production");
		}

		[global::OpenPCR.Migration("5ddc127f-5c9a-4081-acac-710da628358e")]
		public List<RelicSkillExecTime> GetListWithSkillIdOrderByIdAsc(int skill_id)
		{
			if (!_dictionaryOfListWithSkillId.TryGetValue(skill_id, out var value))
			{
				value = _ListSelectWithSkillIdOrderByIdAsc(skill_id);
				_dictionaryOfListWithSkillId.Add(skill_id, value);
			}
			return value;
		}

		[global::OpenPCR.Migration("44759e56-58dc-48a8-8f1c-d8b88da78280")]
		private List<RelicSkillExecTime> _ListSelectWithSkillIdOrderByIdAsc(int skill_id)
		{
			throw new global::OpenPCR.OutOfBoundaryException("MasterRelicSkillExecTime: prepared skill_id timing index is missing");
		}

		[global::OpenPCR.Migration("082b3087-5be7-4b7d-984d-e481bd9ddfd3")]
		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_dictionaryOfListWithSkillId = null;
		}
	}
}
