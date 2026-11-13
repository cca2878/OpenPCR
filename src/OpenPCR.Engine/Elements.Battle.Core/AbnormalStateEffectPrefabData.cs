using global::OpenPCR.Runtime.Api;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class AbnormalStateEffectPrefabData
	{
		public GameObject RightEffectPrefab { get; set; }
		public GameObject LeftEffectPrefab { get; set; }
		public bool IsHead { get; set; }
		public eTargetBone TargetBone { get; set; }
		public eTrackType TrackType { get; set; }

		[global::OpenPCR.Migration("c06261ea-9b99-46b9-8510-c3ba582aafba")]
		public AbnormalStateEffectPrefabData()
		{
		}
	}
}
