using global::OpenPCR.DbStubs;

namespace Elements.Data
{
	public class MasterUnitRoleDatabase : AbstractMasterDatabase
	{
		private PreparedQuery _selectQuery_masterUnitRoleData;
		public PreparedQuery GetSelectQuery_UnitRoleData()
		{
			if (_selectQuery_masterUnitRoleData != null)
				return _selectQuery_masterUnitRoleData;
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterUnitRoleDatabase::GetSelectQuery_UnitRoleData():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}

		public MasterUnitRoleDatabase(DBProxy dbProxy)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterUnitRoleDatabase::.ctor(OpenPCR.DbStubs.DBProxy): runtime capability is unavailable.");
		}

		public override void Unload()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterUnitRoleDatabase::Unload(): SQLite provider lifecycle");
		}

		public override Query Query(string sql)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterUnitRoleDatabase::Query(System.String):OpenPCR.DbStubs.Query: runtime capability is unavailable.");
		}
	}
}
