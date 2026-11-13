using global::OpenPCR.Foundation;
using System.Collections.Generic;
using global::OpenPCR.DbStubs;

namespace Elements.Data
{
	public class MasterSpaceSchedule : AbstractMasterData
	{
		public class SpaceSchedule
		{

			public SpaceSchedule(int space_id = 0, string teaser_time = "", string start_time = "", string count_start_time = "", string count_end_time = "", string end_time = "", int sid = 0, int pre_story_id = 0)
			{
				throw new global::OpenPCR.MigrationPendingException("Elements.Data.MasterSpaceSchedule+SpaceSchedule::.ctor(System.Int32,System.String,System.String,System.String,System.String,System.String,System.Int32,System.Int32)", "constructor body has no exact 7.0.1 match");
			}
		}

		public MasterSpaceSchedule(MasterSpaceDatabase db) : base(db)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.MasterSpaceSchedule::MasterSpaceSchedule(Elements.MasterSpaceDatabase)");
		}

		public override void Unload()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.MasterSpaceSchedule::Unload()");
		}
	}
}
