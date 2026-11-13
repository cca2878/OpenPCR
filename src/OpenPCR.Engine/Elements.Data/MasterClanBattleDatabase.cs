using global::OpenPCR.DbStubs;

namespace Elements.Data
{
	public class MasterClanBattleDatabase : AbstractMasterDatabase
	{
		public MasterClanBattleDatabase(DBProxy dbProxy)
		{
			// SQLite resource connection and prepared-query production are outside battle core.
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterClanBattleDatabase::.ctor");
		}
		public override Query Query(string sql)
		{
			// SQLite resource query production is outside battle core.
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterClanBattleDatabase::Query");
		}
		public override void Unload()
		{
			// SQLite connection and query disposal belong to the resource producer.
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterClanBattleDatabase::Unload");
		}
	}
}
