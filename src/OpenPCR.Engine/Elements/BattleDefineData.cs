using global::OpenPCR.Runtime.Api;

namespace Elements
{
	public class BattleDefineData : ScriptableObject
	{

		[global::OpenPCR.Runtime.Api.SerializeField]
		private int suspendAbuseWarningThresholdCount;

		public int SuspendAbuseWarningThresholdCount => suspendAbuseWarningThresholdCount;


		public BattleDefineData()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.BattleDefineData::.ctor()", "body migration pending");
		}
	}
}
