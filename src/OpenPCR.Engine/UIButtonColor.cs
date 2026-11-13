using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using System;

namespace OpenPCR.UICompat
{

	public class UIButtonColor : UIWidgetContainer
	{
		public enum State
		{
			Normal,
			Hover,
			Pressed,
			Disabled
		}
		public float duration;
		public virtual bool isEnabled { get => throw new global::OpenPCR.MigrationPendingException("UIButtonColor::property isEnabled():System.Boolean", "Native base property declaration; behavior pending"); set => throw new global::OpenPCR.MigrationPendingException("UIButtonColor::property isEnabled():System.Boolean", "Native base property declaration; behavior pending"); }

		public virtual void SetState(State state, bool instant)
		{
			// Native initializes the scene color consumer, stores mState on change, then calls
			// UpdateColor. TweenColor.Begin can retain a reused tweener's onFinished list.
			throw new global::OpenPCR.OutOfBoundaryException("UIButtonColor::SetState(UIButtonColor.State,System.Boolean): scene color tween producer with retained completion callbacks is not implemented");
		}

		public UIButtonColor()
		{
			// Native constructor only initializes hover/pressed/disabled colors and their 0.2s renderer tween.
			// These rendering-only stores are intentionally excluded.
		}
	}

}
