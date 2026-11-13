using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;

namespace OpenPCR.UICompat
{

	public class UIButton : UIButtonColor
	{
		public override bool isEnabled { get => global::OpenPCR.MigrationPending.Throw<bool>("UIButton.get_isEnabled()", "behavior migration pending"); set => global::OpenPCR.MigrationPending.Throw("UIButton.set_isEnabled(System.Boolean)", "behavior migration pending"); }

		public virtual void SetOnClickDelegate(MonoBehaviour _trgIns, Action _func) => global::OpenPCR.MigrationPending.Throw("OpenPCR.UICompat.UIButton::SetOnClickDelegate(OpenPCR.Runtime.Api.MonoBehaviour,System.Action):System.Void", "behavior migration pending");
		protected List<EventDelegate> onClick;

		public UIButton()
		{
			onClick = new List<EventDelegate>();
			// Pure inherited hover/pressed/disabled color output is intentionally excluded.
		}

		protected EventDelegate createEventDelegate(MonoBehaviour trgIns, string trgMethodsName, params object[] trgArguments)
		{
			EventDelegate result = new EventDelegate(trgIns, trgMethodsName);
			for (int i = 0; i < trgArguments.Length; i++)
				result.parameters[i] = new EventDelegate.Parameter(trgArguments[i]);
			return result;
		}

		public override void SetState(State state, bool immediate)
		{
			// Native first calls UIButtonColor.SetState and then updates its sprite.
			// The inherited TweenColor scene consumer may reuse onFinished callbacks.
			throw new global::OpenPCR.OutOfBoundaryException("UIButton::SetState(UIButtonColor.State,System.Boolean): scene tween producer with retained completion callbacks is not implemented");
		}
	}

}
