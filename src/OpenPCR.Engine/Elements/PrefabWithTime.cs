using global::OpenPCR.Runtime.Api;
using System;

namespace Elements
{
	[Serializable]
	public class PrefabWithTime
	{
		public enum eEffectDifficulty
		{
			ALL,
			NORMAL,
			HARD,
			VERY_HARD,
			ALL_FOR_CHARA
		}

		public GameObject Prefab;
		public GameObject PrefabLeft;
		public float Time;
		public int SummonSkillIndex;
		public string TargetBoneName = "";
		public eEffectDifficulty EffectDifficlty;
		public bool IsTrackRotation;
		[global::OpenPCR.Migration("5a8bb790-a814-4dff-9125-3607ff0221cb")]
		public PrefabWithTime()
		{
		}
	}
}
