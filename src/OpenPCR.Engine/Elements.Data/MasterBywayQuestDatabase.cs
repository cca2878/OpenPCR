using global::OpenPCR.DbStubs;

namespace Elements.Data
{
	public class MasterBywayQuestDatabase : AbstractMasterDatabase
	{
		private PreparedQuery _selectQuery_masterBywayBattleQuestData;
		public PreparedQuery GetSelectQuery_BywayBattleQuestData()
		{
			if (_selectQuery_masterBywayBattleQuestData != null)
				return _selectQuery_masterBywayBattleQuestData;
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterBywayQuestDatabase::GetSelectQuery_BywayBattleQuestData():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}

		public MasterBywayQuestDatabase(DBProxy dbProxy)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterBywayQuestDatabase::.ctor(OpenPCR.DbStubs.DBProxy): runtime capability is unavailable.");
		}

		public override Query Query(string sql)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterBywayQuestDatabase::Query(System.String):OpenPCR.DbStubs.Query: runtime capability is unavailable.");
		}

		public override void Unload()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterBywayQuestDatabase::Unload(): SQLite provider/query lifecycle");
		}
	}
}
