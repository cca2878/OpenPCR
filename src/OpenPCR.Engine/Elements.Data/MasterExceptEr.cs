using System;
using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterExceptEr : AbstractMasterData
	{
		private MasterUnitDatabase _db;
		private Dictionary<int, ExceptEr> _lazyPrimaryKeyDictionary;

		public class ExceptEr
		{
			protected global::System.Int32 _category_id;

			public ExceptEr(int category_id = 0)
			{
				_category_id = category_id;
			}
		}

		public MasterExceptEr(MasterUnitDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, ExceptEr>();
			_db = db;
		}

		[global::OpenPCR.Migration("f87dabfb-461e-4f09-88d7-aec0d34cbc3d")]
		public bool HasKey(int category_id)
		{
			return Get(category_id) != null;
		}

		[global::OpenPCR.Migration("275d95f5-ac0a-4cca-a979-dd85e6170f2a")]
		public ExceptEr Get(int category_id)
		{
			ExceptEr value = null;
			if (!_lazyPrimaryKeyDictionary.TryGetValue(category_id, out value))
			{
				value = _db != null ? _SelectOne(category_id) : null;
				_lazyPrimaryKeyDictionary[category_id] = value;
			}
			return value;
		}

		[global::OpenPCR.Migration("cf45b12b-beff-471e-95f1-ab1b22d5f084")]
		private ExceptEr _SelectOne(int category_id)
		{
			if (_db == null)
				throw new NullReferenceException();
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterExceptEr._SelectOne SQLite prepared-query/bind/step/ORM/reset production");
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
		}
	}
}
