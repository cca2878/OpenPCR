using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterLabyrinthRelicEffectGroup : AbstractMasterData
	{
		private MasterLabyrinthDatabase _db;
		private Dictionary<int, LabyrinthRelicEffectGroup> _lazyPrimaryKeyDictionary;

		public class LabyrinthRelicEffectGroup
		{
			protected int _effect_group_id;
			protected int _effect_id_1;
			protected int _effect_id_2;
			protected int _effect_id_3;
			protected int _effect_id_4;
			protected int _effect_id_5;
			public List<int> EffectIdList;

			public int effect_group_id => _effect_group_id;
			public int effect_id_1 => _effect_id_1;
			public int effect_id_2 => _effect_id_2;
			public int effect_id_3 => _effect_id_3;
			public int effect_id_4 => _effect_id_4;
			public int effect_id_5 => _effect_id_5;

			[global::OpenPCR.Migration("4df524a6-2292-45d7-9cbc-98187f9d4445")]
			public LabyrinthRelicEffectGroup(int effect_group_id = 0, int effect_id_1 = 0, int effect_id_2 = 0, int effect_id_3 = 0, int effect_id_4 = 0, int effect_id_5 = 0)
			{
				_effect_group_id = effect_group_id;
				_effect_id_1 = effect_id_1;
				_effect_id_2 = effect_id_2;
				_effect_id_3 = effect_id_3;
				_effect_id_4 = effect_id_4;
				_effect_id_5 = effect_id_5;
				SetUp();
			}

			[global::OpenPCR.Migration("113c7473-a61a-4d20-b0da-66fa6b38d752")]
			public void SetUp()
			{
				EffectIdList = new List<int> { effect_id_1, effect_id_2, effect_id_3, effect_id_4, effect_id_5 };
				EffectIdList.RemoveAll(value => value == 0);
			}
		}

		[global::OpenPCR.Migration("7407dfc9-c46f-423e-940d-9aecd4c84f61")]
		public MasterLabyrinthRelicEffectGroup(MasterLabyrinthDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, LabyrinthRelicEffectGroup>();
			_db = db;
		}

		[global::OpenPCR.Migration("33a18d4b-7c44-4eaf-a775-91456cb8dcdb")]
		public LabyrinthRelicEffectGroup Get(int effect_group_id)
		{
			if (!_lazyPrimaryKeyDictionary.TryGetValue(effect_group_id, out var value))
			{
				value = _db != null ? _SelectOne(effect_group_id) : null;
				_lazyPrimaryKeyDictionary[effect_group_id] = value;
			}
			return value;
		}

		[global::OpenPCR.Migration("612864d3-c610-498c-966b-e88e15d1d2c0")]
		private LabyrinthRelicEffectGroup _SelectOne(int effect_group_id)
		{
			throw new global::OpenPCR.OutOfBoundaryException("MasterLabyrinthRelicEffectGroup._SelectOne: persistent Master database resource row production");
		}

		[global::OpenPCR.Migration("f2ffc00a-a993-4460-8205-2e3af29c7a95")]
		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
		}
	}
}
