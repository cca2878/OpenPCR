using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterLabyrinthRelicEffect : AbstractMasterData
	{
		private MasterLabyrinthDatabase _db;
		private Dictionary<int, LabyrinthRelicEffect> _lazyPrimaryKeyDictionary;

		public class LabyrinthRelicEffect
		{
			protected int _effect_id;
			protected int _effect_type;
			protected int _effect_value_1;
			protected int _effect_value_2;
			protected int _effect_value_3;
			protected int _effect_value_4;

			public int effect_id => _effect_id;
			public int effect_type => _effect_type;
			public int effect_value_1 => _effect_value_1;
			public int effect_value_2 => _effect_value_2;
			public int effect_value_3 => _effect_value_3;
			public int effect_value_4 => _effect_value_4;

			[global::OpenPCR.Migration("f686830f-c3c6-4073-b215-acd6fcf5b31b")]
			public LabyrinthRelicEffect(int effect_id = 0, int effect_type = 0, int effect_value_1 = 0, int effect_value_2 = 0, int effect_value_3 = 0, int effect_value_4 = 0)
			{
				_effect_id = effect_id;
				_effect_type = effect_type;
				_effect_value_1 = effect_value_1;
				_effect_value_2 = effect_value_2;
				_effect_value_3 = effect_value_3;
				_effect_value_4 = effect_value_4;
			}
		}

		[global::OpenPCR.Migration("336c5794-128c-4a01-8267-51e715d01b2c")]
		public MasterLabyrinthRelicEffect(MasterLabyrinthDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, LabyrinthRelicEffect>();
			_db = db;
		}

		[global::OpenPCR.Migration("9971bd41-cf00-48a6-b383-67deb7dc1430")]
		public LabyrinthRelicEffect Get(int effect_id)
		{
			if (!_lazyPrimaryKeyDictionary.TryGetValue(effect_id, out var value))
			{
				value = _db != null ? _SelectOne(effect_id) : null;
				_lazyPrimaryKeyDictionary[effect_id] = value;
			}
			return value;
		}

		[global::OpenPCR.Migration("2be4401b-9b83-4a6e-9f0c-13d6f043372e")]
		private LabyrinthRelicEffect _SelectOne(int effect_id)
		{
			throw new global::OpenPCR.OutOfBoundaryException("MasterLabyrinthRelicEffect._SelectOne: persistent Master database resource row production");
		}

		[global::OpenPCR.Migration("3a6a0731-d625-4dbe-9cb9-55a9c8c297f6")]
		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
		}
	}
}
