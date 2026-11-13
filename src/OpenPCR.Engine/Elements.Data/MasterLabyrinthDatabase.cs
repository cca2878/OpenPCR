using global::OpenPCR.DbStubs;
namespace Elements.Data
{
	public class MasterLabyrinthDatabase : global::Elements.Data.AbstractMasterDatabase
	{
		public MasterLabyrinthDatabase(global::OpenPCR.DbStubs.DBProxy dbProxy)
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.Data.MasterLabyrinthDatabase::.ctor(OpenPCR.DbStubs.DBProxy)", "Native consumer declaration; behavior evidence pending");
		}

		public override void Unload()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.Data.MasterLabyrinthDatabase::Unload():System.Void", "Native consumer declaration; behavior evidence pending");
		}

		public override global::OpenPCR.DbStubs.Query Query(string sql)
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.Data.MasterLabyrinthDatabase::Query(System.String):OpenPCR.DbStubs.Query", "Native consumer declaration; behavior evidence pending");
		}
		private global::OpenPCR.DbStubs.PreparedQuery _selectQuery_masterLabyrinthSetting;

		public global::OpenPCR.DbStubs.PreparedQuery GetSelectQuery_LabyrinthSetting()
		{
			if (_selectQuery_masterLabyrinthSetting != null)
				return _selectQuery_masterLabyrinthSetting;
			// Native uncached branch creates a prepared SQL query through the live SQLite provider.
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterLabyrinthDatabase::GetSelectQuery_LabyrinthSetting():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}
	}
}
