using global::OpenPCR.Runtime.Api;
using System;

namespace Elements
{
	[Serializable]
	public class CountEndEffectData
	{
		public GameObject Prefab;
		public GameObject PrefabLeft;
		public float Time;
		[global::OpenPCR.Migration("b2d548ad-823f-47c9-8125-500062d5b3f9")]
		public CountEndEffectData()
		{
		}
	}
}
