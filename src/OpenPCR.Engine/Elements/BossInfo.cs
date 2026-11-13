
namespace Elements
{
	public class BossInfo
	{
		public global::System.Int64 MaxHp { get; private set; }
		public global::System.Int64 CurrentHp { get; private set; }

		public BossInfo()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.BossInfo::.ctor()", "parameter container constructor body migration pending");
		}
	}
}
