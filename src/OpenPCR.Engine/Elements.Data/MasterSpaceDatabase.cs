using global::OpenPCR.DbStubs;
using System.Runtime.CompilerServices;

namespace Elements.Data
{
	public class MasterSpaceDatabase : AbstractMasterDatabase
	{
		public MasterSpaceDatabase(DBProxy dbProxy)
		{ throw new global::OpenPCR.MigrationPendingException("Elements.Data.MasterSpaceDatabase::.ctor(OpenPCR.DbStubs.DBProxy)", "Behavior implementation is not available."); }
		public override void Unload()
		{ throw new global::OpenPCR.MigrationPendingException("Elements.Data.MasterSpaceDatabase::Unload():System.Void", "Behavior implementation is not available."); }
		public override Query Query(string sql)
		{ throw new global::OpenPCR.MigrationPendingException("Elements.Data.MasterSpaceDatabase::Query(System.String):OpenPCR.DbStubs.Query", "Behavior implementation is not available."); }
	}

}
