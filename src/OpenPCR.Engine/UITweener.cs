using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;

namespace OpenPCR.UICompat
{

	public abstract class UITweener : MonoBehaviour
	{
		public enum Method
		{
			Linear,
			EaseIn,
			EaseOut,
			EaseInOut,
			BounceIn,
			BounceOut
		}

		public enum Style
		{
			Once,
			Loop,
			PingPong
		}

		private UITweener current;
		public Method method;
		public Style style;
		public AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
		public bool ignoreTimeScale = true;
		public float delay;
		public float duration = 1f;
		public bool steeperCurves;
		public List<EventDelegate> onFinished = new List<EventDelegate>();
		public GameObject eventReceiver;
		public string callWhenFinished;
		private bool mStarted;
		private float mStartTime;
		private float mDuration;
		private float mAmountPerDelta = 1000f;
		private float mFactor;
		private List<EventDelegate> mTemp;
		public float amountPerDelta
		{
			get
			{
				if (mDuration != duration)
				{
					mDuration = duration;
					mAmountPerDelta = Mathf.Abs((duration > 0f) ? (1f / duration) : 1000f) * Mathf.Sign(mAmountPerDelta);
				}

				return mAmountPerDelta;
			}
		}



		protected virtual void Update()
		{
			float delta = ignoreTimeScale ? RealTime.deltaTime : Time.deltaTime;
			float time = ignoreTimeScale ? RealTime.time : Time.time;
			if (!mStarted)
			{
				mStarted = true;
				mStartTime = time + delay;
			}

			if (!(time >= mStartTime))
				return;
			mFactor += delta * amountPerDelta;
			if (style == Style.Loop)
			{
				if (mFactor > 1f)
					mFactor -= Mathf.Floor(mFactor);
			}
			else if (style == Style.PingPong)
			{
				if (mFactor > 1f)
				{
					mFactor = (Mathf.Floor(mFactor) - mFactor) + 1f;
					mAmountPerDelta = -mAmountPerDelta;
				}
				else if (mFactor < 0f)
				{
					mFactor = -mFactor - Mathf.Floor(-mFactor);
					mAmountPerDelta = -mAmountPerDelta;
				}
			}
			else if (style == Style.Once && (duration == 0f || mFactor > 1f || mFactor < 0f))
			{
				mFactor = Mathf.Clamp01(mFactor);
				Sample(mFactor, true);
				enabled = false;
				if (current == null)
				{
					UITweener previous = current;
					current = this;
					if (onFinished != null)
					{
						mTemp = onFinished;
						onFinished = new List<EventDelegate>();
						EventDelegate.Execute(mTemp);
						for (int i = 0; i < mTemp.Count; i++)
						{
							EventDelegate ev = mTemp[i];
							if (ev != null && !ev.oneShot)
								EventDelegate.Add(onFinished, ev, false);
						}

						mTemp = null;
					}

					if (eventReceiver != null && !string.IsNullOrEmpty(callWhenFinished))
						eventReceiver.SendMessage(callWhenFinished, this, SendMessageOptions.DontRequireReceiver);
					current = previous;
				}

				return;
			}

			Sample(mFactor, false);
		}

		public void SetOnFinished(EventDelegate.Callback del)
		{
			EventDelegate.Set(onFinished, del);
		}

		[global::OpenPCR.Migration("e7694eaa-d0ae-484e-912e-00a587a41bb3")]
		public void Sample(float factor, bool isFinished)
		{
			float num = Mathf.Clamp01(factor);
			if (method == Method.EaseIn)
			{
				num = 1f - Mathf.Sin((float)Math.PI / 2f * (1f - num));
				if (steeperCurves)
				{
					num *= num;
				}
			}
			else if (method == Method.EaseOut)
			{
				num = Mathf.Sin((float)Math.PI / 2f * num);
				if (steeperCurves)
				{
					num = 1f - num;
					num = 1f - num * num;
				}
			}
			else if (method == Method.EaseInOut)
			{
				num -= Mathf.Sin(num * ((float)Math.PI * 2f)) / ((float)Math.PI * 2f);
				if (steeperCurves)
				{
					num = num * 2f - 1f;
					float num2 = Mathf.Sign(num);
					num = 1f - Mathf.Abs(num);
					num = 1f - num * num;
					num = num2 * num * 0.5f + 0.5f;
				}
			}
			else if (method == Method.BounceIn)
			{
				num = BounceLogic(num);
			}
			else if (method == Method.BounceOut)
			{
				num = 1f - BounceLogic(1f - num);
			}

			OnUpdate((animationCurve != null) ? animationCurve.Evaluate(num) : num, isFinished);
		}

		private float BounceLogic(float val)
		{
			val = ((val < 0.363636f) ? (7.5685f * val * val) : ((val < 0.727272f) ? (7.5625f * (val -= 0.545454f) * val + 0.75f) : ((!(val < 0.90909f)) ? (7.5625f * (val -= 0.9545454f) * val + 63f / 64f) : (7.5625f * (val -= 0.818181f) * val + 0.9375f))));
			return val;
		}

		[global::OpenPCR.Migration("46462f88-c6b0-4e7f-a2e9-a78226d853db")]
		public void PlayForward()
		{
			if (global::OpenPCR.PreparedEffectInputs.IsRendererOnlyTweener(this)) return;
			Play(forward: true);
		}

		[global::OpenPCR.Migration("ea920488-fa4d-42e7-8305-3c7f5da1bd1f")]
		public void Play(bool forward)
		{
			mAmountPerDelta = Mathf.Abs(amountPerDelta);
			if (!forward)
			{
				mAmountPerDelta = -mAmountPerDelta;
			}

			base.enabled = true;
			Update();
		}

		[global::OpenPCR.Migration("76a5383e-dcda-4a2c-a879-707618dcea4d")]
		public void ResetToBeginning()
		{
			if (global::OpenPCR.PreparedEffectInputs.IsRendererOnlyTweener(this)) return;
			mStarted = false;
			mFactor = 0f;
			Sample(mFactor, isFinished: false);
		}

		protected abstract void OnUpdate(float factor, bool isFinished);
	}

}
