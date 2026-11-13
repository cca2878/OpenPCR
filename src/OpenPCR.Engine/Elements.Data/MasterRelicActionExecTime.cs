using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterRelicActionExecTime : AbstractMasterData
	{
		private MasterLabyrinthDatabase _db;
		private Dictionary<int, RelicActionExecTime> _lazyPrimaryKeyDictionary;
		private Dictionary<int, List<RelicActionExecTime>> _dictionaryOfListWithActionId;

		public class RelicActionExecTime
		{
			protected int _id;
			protected int _action_id;
			protected double _exec_time;

			public int id => _id;
			public int action_id => _action_id;
			public double exec_time => _exec_time;

			[global::OpenPCR.Migration("93e7ef5e-176c-444b-b550-61bc90adbf11")]
			public RelicActionExecTime(int id = 0, int action_id = 0, double exec_time = 0.0)
			{
				_id = id;
				_action_id = action_id;
				_exec_time = exec_time;
			}
		}

		[global::OpenPCR.Migration("6a0f4cae-d173-4653-a6d6-da12ffcf8496")]
		public MasterRelicActionExecTime(MasterLabyrinthDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, RelicActionExecTime>();
			_dictionaryOfListWithActionId = new Dictionary<int, List<RelicActionExecTime>>();
			_db = db;
		}

		[global::OpenPCR.Migration("92eaef91-cb7f-49a6-a2df-83ecd9323d59")]
		public RelicActionExecTime Get(int id)
		{
			if (!_lazyPrimaryKeyDictionary.TryGetValue(id, out var value))
			{
				value = _db != null ? _SelectOne(id) : null;
				_lazyPrimaryKeyDictionary[id] = value;
			}
			return value;
		}

		[global::OpenPCR.Migration("a3569639-9a5d-46c3-bd65-efcc0693de3a")]
		private RelicActionExecTime _SelectOne(int id)
		{
			throw new global::OpenPCR.OutOfBoundaryException("MasterRelicActionExecTime._SelectOne: persistent Master database resource row production");
		}

		[global::OpenPCR.Migration("7e2fe9f5-27a5-483f-ade2-61e380ae4d42")]
		public List<RelicActionExecTime> GetListWithActionIdOrderByIdAsc(int action_id)
		{
			if (!_dictionaryOfListWithActionId.TryGetValue(action_id, out var value))
			{
				value = _ListSelectWithActionIdOrderByIdAsc(action_id);
				_dictionaryOfListWithActionId.Add(action_id, value);
			}
			return value;
		}

		[global::OpenPCR.Migration("a9e84e12-80e8-46fe-8b25-2206399f3ef7")]
		private List<RelicActionExecTime> _ListSelectWithActionIdOrderByIdAsc(int action_id)
		{
			throw new global::OpenPCR.OutOfBoundaryException("MasterRelicActionExecTime: prepared action_id timing index is missing");
		}

		[global::OpenPCR.Migration("7a2fe339-cca7-4271-abf4-75e9a43e9ec1")]
		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_dictionaryOfListWithActionId = null;
		}
	}
}
