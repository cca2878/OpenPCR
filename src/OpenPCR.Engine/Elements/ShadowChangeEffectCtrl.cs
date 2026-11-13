using global::OpenPCR.AudioStubs;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;

namespace Elements
{
	public class ShadowChangeEffectCtrl : SkillEffectCtrl
	{

		[global::OpenPCR.Migration("5b8314c4-7d23-4034-af71-cb71d58ce454")]
		public override void ResetParameter(GameObject _prefab, int _skinID = 0, bool _isShadow = false)
		{
			// Shadow/normal material textures and trail textures are rendered output.
			// Native performs exactly one base lifecycle reset after these writes.
			base.ResetParameter(_prefab, _skinID, _isShadow);
		}
	}
}
