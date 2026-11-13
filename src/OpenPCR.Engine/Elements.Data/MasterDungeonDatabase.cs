using global::OpenPCR.DbStubs;
namespace Elements.Data
{
	public class MasterDungeonDatabase : AbstractMasterDatabase
	{
		public MasterDungeonDatabase(global::OpenPCR.DbStubs.DBProxy dbProxy) : base()
		{
			throw new global::OpenPCR.OutOfBoundaryException("MasterDungeonDatabase.ctor: outside guild/training data production");
		}

		public override void Unload()
		{
			throw new global::OpenPCR.OutOfBoundaryException("MasterDungeonDatabase.Unload: outside guild/training data production");
		}

		public override global::OpenPCR.DbStubs.Query Query(string sql)
		{
			throw new global::OpenPCR.OutOfBoundaryException("MasterDungeonDatabase.Query: outside guild/training data production");
		}
	}
}
