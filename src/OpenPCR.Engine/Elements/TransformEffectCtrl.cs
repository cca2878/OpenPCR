using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements
{
	public class TransformEffectCtrl : MonoBehaviour
	{
		public enum eDirection
		{
			LEFT,
			RIGHT
		}
		[SerializeField] private int offset;
		[SerializeField] private float duration;
		[SerializeField] private Vector3 effectScale = new Vector3(1.5f, 1.5f, 1f);
		private Action<Action> halfDurationCallback;
		private float timer;
		private float x;
		private float halfDuration;
		private float speed;
		public bool IsPlay { get; private set; }
		public bool Pause { get; set; }

		public TransformEffectCtrl()
		{

		}



		private void Awake()
		{
			gameObject.SetActive(false);
			transform.localScale = effectScale;
			IsPlay = false;
			Pause = false;
			halfDuration = duration * 0.5f;
		}

		[global::OpenPCR.Migration("c838817a-acd7-43cd-8906-db3c3e98184d")]
		public void Play(eDirection _direction = eDirection.LEFT, Action _endCallback = null, eSE _eSe = eSE.SYS_SWIPE)
		{
			int value = _direction == eDirection.RIGHT ? -offset : offset;
			IsPlay = true;
			x = value;
			speed = (float)value * -2f / duration;
			transform.localPosition = new Vector3(x, 0f, 0f);
			gameObject.SetActive(true);
			if (_eSe != eSE.NONE) ManagerSingleton<SoundManager>.Instance.PlaySe(_eSe, false);
			StartCoroutine(playCoroutine(_endCallback));
		}

		private IEnumerator playCoroutine(Action _endCallback)
		{
			timer = 0f;
			while (timer < halfDuration)
			{
				moveObject(true);
				yield return null;
			}
			bool isCallbackCalled = false;
			halfDurationCallback.Call(() => isCallbackCalled = true);
			while (!isCallbackCalled) yield return null;
			timer = 0f;
			while (timer < halfDuration)
			{
				moveObject(false);
				yield return null;
			}
			IsPlay = false;
			_endCallback.Call();
		}

		private void moveObject(bool _isHalfBefor)
		{
			if (Pause) return;
			float deltaTime = Time.deltaTime;
			timer += deltaTime;
			x += deltaTime * speed;
			if (_isHalfBefor && ((speed > 0f && x > 0f) || (speed < 0f && x < 0f))) x = 0f;
			transform.localPosition = new Vector3(x, 0f, 0f);
		}

		[global::OpenPCR.Migration("9cfd5b5a-385b-47ad-ab12-014ed50606dc")]
		public void SetHalfDurationCallback(Action<Action> _callback)
		{
			halfDurationCallback = _callback;
		}
	}
}
