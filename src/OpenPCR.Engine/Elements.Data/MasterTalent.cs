using System.Collections.Generic;
using System.Linq;

namespace Elements.Data
{
	public class MasterTalent : AbstractMasterData
	{
		public class Talent
		{
			protected global::System.Int32 _talent_id;
			protected global::System.String _talent_name;
			protected global::System.String _talent_color;

			public Talent(int talent_id = 0, string talent_name = "", string talent_color = "")
			{
				_talent_id = talent_id;
				_talent_name = talent_name;
				_talent_color = talent_color;
			}
		}

		private MasterKnightEnhanceDatabase _db;
		private Dictionary<int, Talent> _strictPrimaryKeyDictionary;
		private Dictionary<int, Talent> _lazyPrimaryKeyDictionary;

		public Dictionary<int, Talent> dictionary
		{
			[global::OpenPCR.Migration("e67c4678-cbaf-41c0-9415-54f38844a19e")]
			get
			{
				if (_strictPrimaryKeyDictionary == null)
					_StrictLoadAllEntries();
				return _strictPrimaryKeyDictionary;
			}
		}

		public MasterTalent(MasterKnightEnhanceDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, Talent>();
			_db = db;
		}

		[global::OpenPCR.Migration("18bf3461-de95-4918-8fd5-f820e070bc6a")]
		private void _StrictLoadAllEntries()
		{
			_ = new Dictionary<int, Talent>();
			if (_db == null)
				throw new System.NullReferenceException();
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterTalent::_StrictLoadAllEntries(): Native MasterKnightEnhanceDatabase.GetSelectAllQuery_Talent and the complete table Step/ORM cache/disposal producer are required before strict-cache publication and lazy-cache release.");
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_strictPrimaryKeyDictionary = null;
		}

		[global::OpenPCR.Migration("b1db93b5-4eab-408b-b2b9-e8b02ff00c5e")]
		public List<int> GetTalentIdListOrderByAsc()
		{
			return dictionary.Keys.OrderBy(id => id).ToList();
		}
	}
}
