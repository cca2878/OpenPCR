using global::OpenPCR.DbStubs;

namespace Elements.Data
{
	public class MasterEtcDatabase : AbstractMasterDatabase
	{
		private PreparedQuery _selectQuery_masterContentReleaseData;
		public MasterContentReleaseData masterContentReleaseData { get; private set; }

		public MasterEtcDatabase(DBProxy dbProxy)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterEtcDatabase::.ctor(OpenPCR.DbStubs.DBProxy): runtime capability is unavailable.");
		}

		public PreparedQuery GetSelectQuery_ContentReleaseData()
		{
			if (_selectQuery_masterContentReleaseData != null)
				return _selectQuery_masterContentReleaseData;
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterEtcDatabase::GetSelectQuery_ContentReleaseData():OpenPCR.DbStubs.PreparedQuery: runtime capability is unavailable.");
		}

		public override Query Query(string sql)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterEtcDatabase::Query(System.String):OpenPCR.DbStubs.Query: runtime capability is unavailable.");
		}

		public override void Unload()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterEtcDatabase::Unload():System.Void");
		}
	}
}
