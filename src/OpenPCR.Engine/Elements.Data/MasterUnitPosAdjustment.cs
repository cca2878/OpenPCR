using System;
using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterUnitPosAdjustment : AbstractMasterData
	{
		private MasterUnitDatabase _db;
		private Dictionary<int, UnitPosAdjustment> _lazyPrimaryKeyDictionary;

		public class UnitPosAdjustment
		{
			protected global::System.Int32 _unit_id;
			protected global::System.Int32 _id_1;
			protected global::System.Int32 _id_2;
			protected global::System.Int32 _id_3;
			protected global::System.Int32 _home_1_pos_x;
			protected global::System.Int32 _home_1_pos_y;
			protected global::System.Int32 _home_1_depth;
			protected global::System.Int32 _home_1_clip;
			protected global::System.Int32 _home_2_pos_x;
			protected global::System.Int32 _home_2_pos_y;
			protected global::System.Int32 _home_2_depth;
			protected global::System.Int32 _home_2_clip;
			protected global::System.Int32 _home_3_pos_x;
			protected global::System.Int32 _home_3_pos_y;
			protected global::System.Int32 _home_3_depth;
			protected global::System.Int32 _home_3_clip;
			protected global::System.Int32 _profile_1_pos_x;
			protected global::System.Int32 _profile_1_pos_y;
			protected global::System.Int32 _profile_1_depth;
			protected global::System.Double _profile_1_scale;
			protected global::System.Int32 _profile_1_clip;
			protected global::System.Int32 _profile_2_pos_x;
			protected global::System.Int32 _profile_2_pos_y;
			protected global::System.Int32 _profile_2_depth;
			protected global::System.Double _profile_2_scale;
			protected global::System.Int32 _profile_2_clip;
			protected global::System.Int32 _profile_3_pos_x;
			protected global::System.Int32 _profile_3_pos_y;
			protected global::System.Int32 _profile_3_depth;
			protected global::System.Double _profile_3_scale;
			protected global::System.Int32 _profile_3_clip;
			protected global::System.Int32 _actual_id1;
			protected global::System.Int32 _actual_1_pos_x;
			protected global::System.Int32 _actual_1_pos_y;
			protected global::System.Int32 _actual_1_depth;
			protected global::System.Int32 _actual_1_clip;
			protected global::System.Int32 _actual_id2;
			protected global::System.Int32 _actual_2_pos_x;
			protected global::System.Int32 _actual_2_pos_y;
			protected global::System.Int32 _actual_2_depth;
			protected global::System.Int32 _actual_2_clip;
			protected global::System.Int32 _actual_id3;
			protected global::System.Int32 _actual_3_pos_x;
			protected global::System.Int32 _actual_3_pos_y;
			protected global::System.Int32 _actual_3_depth;
			protected global::System.Int32 _actual_3_clip;
			protected global::System.Int32 _skip_position_x;
			protected global::System.Int32 _friend_pos_x;
			protected global::System.Boolean _is_myprofile_image;

			public List<int> UnitIdList { get; private set; }

			public UnitPosAdjustment(int unit_id = 0, int id_1 = 0, int id_2 = 0, int id_3 = 0, int home_1_pos_x = 0, int home_1_pos_y = 0, int home_1_depth = 0, int home_1_clip = 0, int home_2_pos_x = 0, int home_2_pos_y = 0, int home_2_depth = 0, int home_2_clip = 0, int home_3_pos_x = 0, int home_3_pos_y = 0, int home_3_depth = 0, int home_3_clip = 0, int profile_1_pos_x = 0, int profile_1_pos_y = 0, int profile_1_depth = 0, double profile_1_scale = 0.0, int profile_1_clip = 0, int profile_2_pos_x = 0, int profile_2_pos_y = 0, int profile_2_depth = 0, double profile_2_scale = 0.0, int profile_2_clip = 0, int profile_3_pos_x = 0, int profile_3_pos_y = 0, int profile_3_depth = 0, double profile_3_scale = 0.0, int profile_3_clip = 0, int actual_id1 = 0, int actual_1_pos_x = 0, int actual_1_pos_y = 0, int actual_1_depth = 0, int actual_1_clip = 0, int actual_id2 = 0, int actual_2_pos_x = 0, int actual_2_pos_y = 0, int actual_2_depth = 0, int actual_2_clip = 0, int actual_id3 = 0, int actual_3_pos_x = 0, int actual_3_pos_y = 0, int actual_3_depth = 0, int actual_3_clip = 0, int skip_position_x = 0, int friend_pos_x = 0, bool is_myprofile_image = false)
			{
				_unit_id = unit_id;
				_id_1 = id_1;
				_id_2 = id_2;
				_id_3 = id_3;
				_home_1_pos_x = home_1_pos_x;
				_home_1_pos_y = home_1_pos_y;
				_home_1_depth = home_1_depth;
				_home_1_clip = home_1_clip;
				_home_2_pos_x = home_2_pos_x;
				_home_2_pos_y = home_2_pos_y;
				_home_2_depth = home_2_depth;
				_home_2_clip = home_2_clip;
				_home_3_pos_x = home_3_pos_x;
				_home_3_pos_y = home_3_pos_y;
				_home_3_depth = home_3_depth;
				_home_3_clip = home_3_clip;
				_profile_1_pos_x = profile_1_pos_x;
				_profile_1_pos_y = profile_1_pos_y;
				_profile_1_depth = profile_1_depth;
				_profile_1_scale = profile_1_scale;
				_profile_1_clip = profile_1_clip;
				_profile_2_pos_x = profile_2_pos_x;
				_profile_2_pos_y = profile_2_pos_y;
				_profile_2_depth = profile_2_depth;
				_profile_2_scale = profile_2_scale;
				_profile_2_clip = profile_2_clip;
				_profile_3_pos_x = profile_3_pos_x;
				_profile_3_pos_y = profile_3_pos_y;
				_profile_3_depth = profile_3_depth;
				_profile_3_scale = profile_3_scale;
				_profile_3_clip = profile_3_clip;
				_actual_id1 = actual_id1;
				_actual_1_pos_x = actual_1_pos_x;
				_actual_1_pos_y = actual_1_pos_y;
				_actual_1_depth = actual_1_depth;
				_actual_1_clip = actual_1_clip;
				_actual_id2 = actual_id2;
				_actual_2_pos_x = actual_2_pos_x;
				_actual_2_pos_y = actual_2_pos_y;
				_actual_2_depth = actual_2_depth;
				_actual_2_clip = actual_2_clip;
				_actual_id3 = actual_id3;
				_actual_3_pos_x = actual_3_pos_x;
				_actual_3_pos_y = actual_3_pos_y;
				_actual_3_depth = actual_3_depth;
				_actual_3_clip = actual_3_clip;
				_skip_position_x = skip_position_x;
				_friend_pos_x = friend_pos_x;
				_is_myprofile_image = is_myprofile_image;
				SetUp();
			}

			public void SetUp()
			{
				if (UnitIdList != null)
					return;
				UnitIdList = new List<int>();
				if ((int)_id_1 != 0)
					UnitIdList.Add(_id_1);
				if ((int)_id_2 != 0)
					UnitIdList.Add(_id_2);
				if ((int)_id_3 != 0)
					UnitIdList.Add(_id_3);
				// Home/profile/actual display projections are outside the core battle scope.
			}
		}

		public MasterUnitPosAdjustment(MasterUnitDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, UnitPosAdjustment>();
			_db = db;
		}

		[global::OpenPCR.Migration("07d99bcf-62ca-4d51-a571-53477214b46f")]
		public UnitPosAdjustment Get(int unit_id)
		{
			UnitPosAdjustment value = null;
			if (!_lazyPrimaryKeyDictionary.TryGetValue(unit_id, out value))
			{
				value = _db != null ? _SelectOne(unit_id) : null;
				_lazyPrimaryKeyDictionary[unit_id] = value;
			}
			return value;
		}

		[global::OpenPCR.Migration("77b45e25-7a1f-4e07-aa2d-5c24813fea1d")]
		private UnitPosAdjustment _SelectOne(int unit_id)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterUnitPosAdjustment._SelectOne SQLite prepared-query production and ORM row mapping");
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
		}
	}
}
