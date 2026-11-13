using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;
using Common;
using global::OpenPCR.AudioStubs;
using global::OpenPCR.Foundation;
using global::OpenPCR.Foundation.Audio;

namespace Elements
{
	public class AnimatorFunction : MonoBehaviour
	{
		[SerializeField] private Animator animator;
		[SerializeField] private UIPanel panel;
		protected List<Action> callback = new List<Action>();
		protected Queue<Action<float>> callbackArgEvent = new Queue<Action<float>>();
		protected bool isFirstFrame;
		protected Action callbackEndEvent;

		public Animator Animator { get { return animator; } }



		public void Init()
		{
			callback.Clear();
			callbackArgEvent.Clear();
			callbackEndEvent = null;
		}

		public void Init(Action _callback, int _sortingOrder = 0, Action _endAction = null)
		{
			Init();
			Add(_callback);
			callbackEndEvent = _endAction;
			if ((global::OpenPCR.Runtime.Api.Object)panel != null && _sortingOrder != 0)
			{
				panel.sortingOrder = _sortingOrder;
			}
		}

		public void Add(Action _callback)
		{
			if (_callback != null)
			{
				callback.Add(_callback);
			}
		}

		private void Update()
		{
			if (callbackEndEvent != null && !isFirstFrame)
			{
				AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
				if (state.normalizedTime >= 1f)
				{
					callbackEndEvent.Call();
					callbackEndEvent = null;
				}
			}
			isFirstFrame = false;
		}
	}
}
