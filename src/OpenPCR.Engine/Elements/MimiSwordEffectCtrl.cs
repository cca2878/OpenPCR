using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements
{
	public class MimiSwordEffectCtrl : SkillEffectCtrl
	{
		[SerializeField]
		private string stopAnimationName = "";
		[global::OpenPCR.Migration("c926a30e-69e7-4921-8066-a11b28ebcab3")]
		public MimiSwordEffectCtrl()
		{
		}

		[global::OpenPCR.Migration("f984aad8-1286-44ad-86f9-ab331da55f26")]
		public override void ExecAppendCoroutine(UnitCtrl _unit = null, bool _isAbnormal = false)
		{
			base.ExecAppendCoroutine(_unit);
			StartCoroutine(updateMimiEffect());
		}

		[global::OpenPCR.Migration("adf3ef0c-4861-4c23-97a0-8272bc4a1708")]
		private IEnumerator updateMimiEffect()
		{
			yield return new WaitWhile(() => source == null);
			yield return new WaitUntil(() => source.GetCurrentSpineCtrl().AnimationName == stopAnimationName);
			// Muted CRI output has no battle consumer; retain both waits and pooled lifetime.
			base.IsPlaying = false;
			base.gameObject.SetActive(value: false);
		}
	}
}
