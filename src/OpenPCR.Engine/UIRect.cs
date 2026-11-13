using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System;

namespace OpenPCR.UICompat
{

	public abstract class UIRect : MonoBehaviour
	{
		public abstract float alpha { get; set; }
		[Serializable]
		public class AnchorPoint
		{
			public Transform target;
			public float relative;
			public int absolute;

		}

		public enum AnchorUpdate
		{
			OnEnable,
			OnUpdate,
			OnStart
		}

		public AnchorPoint leftAnchor;
		public AnchorPoint rightAnchor;
		public AnchorPoint bottomAnchor;
		public AnchorPoint topAnchor;
		public AnchorUpdate updateAnchors;
		[NonSerialized]
		protected Transform mTrans;
		[NonSerialized]
		protected BetterList<UIRect> mChildren;
		[NonSerialized]
		protected bool mChanged;
		[NonSerialized]
		public float finalAlpha;

		protected UIRect()
		{
			leftAnchor = new AnchorPoint();
			rightAnchor = new AnchorPoint { relative = 1f };
			bottomAnchor = new AnchorPoint();
			topAnchor = new AnchorPoint { relative = 1f };
			updateAnchors = AnchorUpdate.OnUpdate;
			mChildren = new BetterList<UIRect>();
			mChanged = true;
			mUpdateAnchors = true;
			mUpdateFrame = -1;
			finalAlpha = 1f;
		}

		[global::System.NonSerialized]
		private bool mUpdateAnchors;
		[global::System.NonSerialized]
		private int mUpdateFrame;
		public Transform cachedTransform
		{
			get { if (mTrans == null) mTrans = transform; return mTrans; }
		}
		public virtual bool canBeAnchored => true;
		public bool isAnchored => (leftAnchor.target || rightAnchor.target || topAnchor.target || bottomAnchor.target) && canBeAnchored;
		public void UpdateAnchors()
		{
			if (!isAnchored) return;
			mUpdateFrame = -1;
			mUpdateAnchors = true;
			UpdateAnchorsInternal(Time.frameCount);
		}
		protected void UpdateAnchorsInternal(int frame)
		{
			throw new global::OpenPCR.OutOfBoundaryException("UIRect::UpdateAnchorsInternal(System.Int32):System.Void");
		}
	}

}
