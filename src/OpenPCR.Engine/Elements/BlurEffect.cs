using global::OpenPCR.Runtime.Api;
using System;
using Elements.Battle;

namespace Elements
{
	public class BlurEffect : SkillEffectCtrl
	{
		[Serializable]
		public class BlurEffectData
		{
			public float StartTime;
			public float Value;
			public float StartDuration;
			public float Duration;
			public float EndDuration;
			[global::OpenPCR.Migration("6088d52b-63de-4c79-b18a-f6a9e1d576c3")]
			public BlurEffectData()
			{
			}
		}

		[SerializeField]
		private BlurEffectData blueEffectData;
		private static IBattleCameraEffectForBlurEffect staticBattleCameraEffect;
		private static bool isStaticInit;
		[global::OpenPCR.ImplicitMigration("d5b7234f-faa3-4b6e-a700-5da8ebe5cf5f")]
		protected IBattleCameraEffectForBlurEffect battleCameraEffect => throw new global::OpenPCR.OutOfBoundaryException("Elements.BlurEffect::get_battleCameraEffect():Elements.Battle.IBattleCameraEffectForBlurEffect (event-quest enemy effect)");

		[global::OpenPCR.Migration("57e3fa5d-9e54-4ea0-97e1-ca074eeb3c2a")]
		public BlurEffect()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.BlurEffect::.ctor() (event-quest enemy effect)"); }

		[global::OpenPCR.Migration("93e8b984-1bfb-44a1-b351-2310cb6a248b")]
		private void Awake()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.BlurEffect::Awake():System.Void (event-quest enemy effect)"); }

		[global::OpenPCR.Migration("0cf6e06b-faa2-4e1a-b09d-0fec658eec7b")]
		public new static void StaticRelease()
		{
			staticBattleCameraEffect = null;
			isStaticInit = false;
		}

		[global::OpenPCR.Migration("62457388-60ad-46e6-bacc-9704d24cae37")]
		public override void ResetParameter(GameObject _prefab, int _skinId, bool _isShadow)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.BlurEffect::ResetParameter(OpenPCR.Runtime.Api.GameObject,System.Int32,System.Boolean):System.Void: runtime capability is unavailable."); }
	}
}
