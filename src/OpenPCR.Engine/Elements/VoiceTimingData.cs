using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;

namespace Elements
{
	public class VoiceTimingData : ScriptableObject
	{
		public List<VoiceTimingGroup> VoiceTimingGroupList;
		public VoiceTimingData()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.VoiceTimingData::.ctor()", "body migration pending");
		}
	}
}
