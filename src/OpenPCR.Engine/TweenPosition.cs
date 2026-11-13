using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System;

namespace OpenPCR.UICompat
{

	public class TweenPosition : UITweener
	{
		public Vector3 from;
		public Vector3 to;
		public bool worldSpace;

		private Transform mTrans;

		private UIRect mRect;

		public Transform cachedTransform
		{
			get
			{
				if (mTrans == null)
				{
					mTrans = base.transform;
				}
				return mTrans;
			}
		}

		[Obsolete("Use 'value' instead")]
		public Vector3 position
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;
			}
		}

		public Vector3 value
		{
			get
			{
				if (!worldSpace)
				{
					return cachedTransform.localPosition;
				}
				return cachedTransform.position;
			}
			set
			{
				if (mRect == null || !mRect.isAnchored || worldSpace)
				{
					if (worldSpace)
					{
						cachedTransform.position = value;
					}
					else
					{
						cachedTransform.localPosition = value;
					}
				}
				else
				{
					value -= cachedTransform.localPosition;
					UIMath.MoveRect(mRect, value.x, value.y);
				}
			}
		}

		private void Awake()
		{
			mRect = GetComponent<UIRect>();
		}

		protected override void OnUpdate(float factor, bool isFinished)
		{
			value = from * (1f - factor) + to * factor;
		}
		[global::OpenPCR.Migration("5bd9faff-7788-4a08-919f-0be549c69f59")]
		public TweenPosition()
		{
		}
	}

}
