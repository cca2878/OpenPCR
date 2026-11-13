using global::OpenPCR.DbStubs;

namespace Elements.Data
{
	public abstract class AbstractMasterDatabase
	{


		public abstract void Unload();
		public abstract Query Query(string sql);
	}
}
