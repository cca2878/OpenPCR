using global::OpenPCR.Runtime.Impl;
namespace OpenPCR.Runtime.Api
{
	public class Time
	{
		public static float time
		{
			[global::OpenPCR.Migration("dd89be73-79fc-40b6-9928-ff295b08edaf")]
			get
			{
				return global::OpenPCR.Runtime.Impl.ScriptingTime.Time;
			}
		}
		public static float deltaTime
		{
			[global::OpenPCR.Migration("6f151cb8-ce06-4079-9191-2cc89620d92f")]
			get
			{
				return global::OpenPCR.Runtime.Impl.ScriptingTime.DeltaTime;
			}
		}
		public static float unscaledTime
		{
			[global::OpenPCR.Migration("abd2c7ea-e3c2-41b2-81fa-a39da4c48655")]
			get
			{
				return global::OpenPCR.Runtime.Impl.ScriptingTime.UnscaledTime;
			}
		}
		public static float unscaledDeltaTime
		{
			[global::OpenPCR.Migration("1ee3a4ad-ff40-46e4-bf84-341e5c690520")]
			get
			{
				return global::OpenPCR.Runtime.Impl.ScriptingTime.UnscaledDeltaTime;
			}
		}
		public static float timeScale
		{
			[global::OpenPCR.Migration("4a78047d-20c6-4a31-a21f-ea445a17eff7")]
			get
			{
				return global::OpenPCR.Runtime.Impl.ScriptingTime.TimeScale;
			}
			[global::OpenPCR.Migration("4e80b1eb-d78e-4ff3-8ce1-2f9f841262c5")]
			set
			{
				global::OpenPCR.Runtime.Impl.ScriptingTime.TimeScale = value;
			}
		}
		public static int frameCount
		{
			[global::OpenPCR.Migration("44f5ff85-7611-4b1d-bbc3-c64b73a531eb")]
			get
			{
				return global::OpenPCR.Runtime.Impl.ScriptingTime.FrameCount;
			}
		}
	}
}
