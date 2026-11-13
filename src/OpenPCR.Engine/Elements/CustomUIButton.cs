using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Elements
{
	public sealed partial class CustomUIButton : UIButton
	{
		private BitArray flag;
		private List<EventDelegate> onDisabledClick;
		private List<EventDelegate> onPressNow;
		private List<EventDelegate> onLongPress;
		private List<EventDelegate> onPressDown;
		[global::OpenPCR.Runtime.Api.SerializeField]
		private BoxCollider curBoxCollider;
		private ParticleSystem[] childParticleArray;
		private bool initEnable;
		private float pressStartActionInterval;
		private float pressEndActionInterval;
		private float changingDuration;
		private float changeCoefficient;
		private float actionInterval;

		public enum eButtonSE
		{
			NONE,
			OK,
			CANCEL,
			PRODUCT_UP,
			SELECT,
			TITLE,
			CLOSE_MENU,
			OPEN_MENU,
			START_BUTTON_01,
			START_BUTTON_02,
			ROOM_ITEM_SET,
			GO_TO_NEXT_WAVE,
			SWIPE,
			SMALL,
			CANCEL_SMALL,
			WINDOW_CLOSE,
			LARGE,
			INVALID
		}

		public override bool isEnabled
		{
			[global::OpenPCR.Migration("b3fc5d99-b987-4f7f-ac37-0e2dcce2587b")]
			get => flag[0];
			[global::OpenPCR.Migration("0578f9ff-cb4b-4966-aba3-6d01f2e033a3")]
			set
			{
				initEnable = value;
				if (isEnabled != value)
				{
					flag[0] = initEnable;
					// Pure color/palette output is excluded; SetState remains a real scene consumer
					// because its reused TweenColor may retain completion callbacks.
					// Active particle consumers are retained because their lifecycle may consume RNG.
					if (childParticleArray != null)
						for (int i = 0; i < childParticleArray.Length; i++)
							childParticleArray[i].gameObject.SetActive(isEnabled);
					SetState(isEnabled ? State.Normal : State.Disabled, false);
				}
			}
		}

		public bool isCollider { get => curBoxCollider.enabled; set => curBoxCollider.enabled = value; }

		public CustomUIButton()
		{
			flag = new BitArray(1);
			onDisabledClick = new List<EventDelegate>();
			onPressNow = new List<EventDelegate>();
			onLongPress = new List<EventDelegate>();
			onPressDown = new List<EventDelegate>();
			initEnable = true;
			actionInterval = 0.5f;
			pressStartActionInterval = 0.5f;
			pressEndActionInterval = 0.0625f;
			changingDuration = 1f;
			changeCoefficient = -0.4375f;
			// Native child color/label output lists are excluded with their rendering consumers.
		}

		[global::OpenPCR.Migration("d7d0d65d-2f93-4163-8a9b-60603a04fa0f")]
		public void ChangeEnabledSE(eButtonSE seType = eButtonSE.NONE)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("0a9c0385-8e52-4dcf-b9c9-2b6afe1141b2")]
		public void ChangeDisabledSE(eButtonSE seType = eButtonSE.NONE)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("db8001af-59c1-438d-afc6-c5684c4ee316")]
		public override void SetOnClickDelegate(MonoBehaviour _trgIns, Action _func)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("d6f3e345-334f-4379-ba36-f0e2fed20865")]
		public void SetOnClickDelegate(MonoBehaviour _trgIns, Action<int, bool, bool> _func, int _arg0, bool _arg1, bool _arg2)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("96ebbac1-7db7-4232-8ea9-ed781e3648bc")]
		public void SetOnDisabledClickDelegate(MonoBehaviour _trgIns, Action _func)
		{
			EventDelegate.Set(onDisabledClick, createEventDelegate(_trgIns, _func.Method.Name, new object[0]));
		}
	}
}
