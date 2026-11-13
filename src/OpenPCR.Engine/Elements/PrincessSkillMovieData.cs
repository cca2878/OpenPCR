using System;

namespace Elements
{
	[Serializable]
	public class PrincessSkillMovieData
	{
		public bool UseFade;
		public float FadeStartTime;
		public float FadeDuration;
		[global::OpenPCR.Migration("fb044569-dee4-4296-bb24-15e6dd5dd63f")]
		public PrincessSkillMovieData()
		{
		}
	}
}
