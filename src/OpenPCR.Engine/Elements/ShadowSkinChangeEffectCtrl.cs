using global::OpenPCR.AudioStubs;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;

namespace Elements
{
	public class ShadowSkinChangeEffectCtrl : SkillEffectCtrl
	{
		[Serializable]
		private class ShadowSkinTextureChange
		{
			public int SkinId;
		}

		[SerializeField]
		private List<ShadowSkinTextureChange> shadowSkinTextureSetting;

		[global::OpenPCR.Migration("663ffc98-fe3d-4591-bec0-a2c7769ff0b3")]
		public override void ResetParameter(GameObject _prefab, int _skinID = 0, bool _isShadow = false)
		{
			foreach (ShadowSkinTextureChange setting in shadowSkinTextureSetting)
			{
				// Child texture visibility is presentation; the per-setting lifecycle reset is retained.
				base.ResetParameter(_prefab, _skinID, _isShadow);
			}
		}
	}
}
