using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements
{
	public class TransformEffectCtrlForBoss : MonoBehaviour
	{
		[SerializeField]
		private float duration = 0.53f;
		[SerializeField]
		private float fadeDuration = 0.16f;
		[SerializeField]
		private Vector3 effectScale = new Vector3(2.5f, 1f, 1f);
		private Action<Action> halfDurationCallback;


		[global::OpenPCR.Migration("bc3296a8-1dc1-4d1f-944c-ceb2784d0dac")]
		public void SetHalfDurationCallback(Action<Action> _callback)
		{
			halfDurationCallback = _callback;
		}
	}
}
