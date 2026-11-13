using global::OpenPCR.DbStubs;
namespace Elements.Data
{
	public class MasterKnightEnhanceDatabase : global::Elements.Data.AbstractMasterDatabase
	{
		public MasterKnightEnhanceDatabase(global::OpenPCR.DbStubs.DBProxy dbProxy)
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.Data.MasterKnightEnhanceDatabase::.ctor(OpenPCR.DbStubs.DBProxy)", "Native consumer declaration; behavior evidence pending");
		}

		public override void Unload()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.Data.MasterKnightEnhanceDatabase::Unload():System.Void", "Native consumer declaration; behavior evidence pending");
		}

		public override global::OpenPCR.DbStubs.Query Query(string sql)
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.Data.MasterKnightEnhanceDatabase::Query(System.String):OpenPCR.DbStubs.Query", "Native consumer declaration; behavior evidence pending");
		}
	}
}
