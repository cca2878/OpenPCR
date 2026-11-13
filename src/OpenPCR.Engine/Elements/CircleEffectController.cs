using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using global::OpenPCR.Foundation;
namespace Elements
{
	public class CircleEffectController : global::Elements.SkillEffectCtrl
	{
		[global::OpenPCR.Runtime.Api.SerializeField]
		private float duration = 10f;
		public bool FollowXY = true;
		public global::System.Collections.Generic.List<Elements.CircleEffectChild> Children;
		public float Radius = 0.5f;
		public float Roll = 10f;
		public float Yaw;
		[global::OpenPCR.Runtime.Api.SerializeField]
		private float angularVelocity = 180f;
		[global::OpenPCR.Migration("77b1b087-6a72-443a-beff-5d8bc9337799")]
		public CircleEffectController()
		{

		}

		[global::OpenPCR.Migration("8cb6f5e0-3589-4212-a512-1ee20d9ed4c7")]
		protected override void onDestroy()
		{
			Children = null;
			base.onDestroy();
		}

		[global::OpenPCR.Migration("14a9f4b6-1898-4c11-bd0b-2b5a804aae27")]
		public override void ResetParameter(global::OpenPCR.Runtime.Api.GameObject _prefab, int _skinId, bool _isShadow)
		{
			base.ResetParameter(_prefab);
			int i = 0;
			for (int count = Children.Count; i < count; i++)
			{
				Children[i].SetActive(_isActive: true);
			}
		}

		[global::OpenPCR.Migration("bae08c86-bb12-496d-a77f-361ba5288dbb")]
		public void SetChildrenInactive()
		{
			int i = 0;
			for (int count = Children.Count; i < count; i++)
			{
				Children[i].SetActive(_isActive: false);
			}
		}

		[global::OpenPCR.Migration("9c1e5e49-37ba-457d-8d06-89d4e3eb4dae")]
		public void AddEffect(int number)
		{
			int num = 0;
			for (int i = 0; i < Children.Count; i++)
			{
				if (!Children[i].gameObject.activeSelf)
				{
					Children[i].StartEffect();
					num++;
					if (num == number)
					{
						break;
					}
				}
			}
		}

		[global::OpenPCR.Migration("a58b13c9-e23b-43c1-bed4-fa208021a14e")]
		public override void ExecAppendCoroutine(global::Elements.UnitCtrl _unit = null, bool _isAbnormal = false)
		{
			AppendCoroutine(UpdateTimer(), ePauseType.VISUAL, _unit);
			int i = 0;
			for (int count = Children.Count; i < count; i++)
			{
				Children[i].Initialize(this, base.SortTarget);
			}

			base.ExecAppendCoroutine(_unit);
			base.SortTarget.CircleEffectList.Add(this);
		}

		[global::OpenPCR.Migration("d1965341-baa1-4585-b96b-bb8f2c15a1bc")]
		public global::System.Collections.IEnumerator UpdateTimer()
		{
			float time = 0f;
			while (true)
			{
				if (!base.IsPlaying)
				{
					yield break;
				}

				time += base.battleManager.DeltaTime;
				int i = 0;
				for (int count = Children.Count; i < count; i++)
				{
					Children[i].UpdatePosition(base.battleManager.DeltaTime * angularVelocity, base.SortTarget.ComponentSortOrder.SortOrder);
				}

				if (time > duration)
				{
					break;
				}

				yield return null;
			}

			base.SortTarget.CircleEffectList.Remove(this);
			timeToDie = true;
		}

		[global::OpenPCR.Migration("ad38b275-53ed-4b2b-b581-ea2768192be1")]
		public override void SetSortOrderBack()
		{
			setSortOrder();
		}

		[global::OpenPCR.Migration("51e94e6c-7510-47fa-9a19-8e8cffef0f73")]
		private void setSortOrder()
		{
			int i = 0;
			for (int count = Children.Count; i < count; i++)
			{
				Children[i].UpdateSortOrder(base.SortTarget.ComponentSortOrder.SortOrder);
			}
		}

		[global::OpenPCR.Migration("7c4a6b0d-0d3d-461c-a4f5-5f5da29a96ce")]
		public override void SetSortOrderFront()
		{
			setSortOrder();
			int i = 0;
			for (int count = Children.Count; i < count; i++)
			{
				Children[i].SetExecEffectDisable();
			}
		}

		[global::OpenPCR.Migration("f1b5b057-1164-46a9-946d-be30a1c454f2")]
		public void DisableTop(bool isLeftDir, global::System.Action _callback)
		{
			CircleEffectChild circleEffectChild = null;
			bool flag = true;
			int i = 0;
			for (int count = Children.Count; i < count; i++)
			{
				if (Children[i].gameObject.activeSelf)
				{
					if (flag)
					{
						circleEffectChild = Children[i];
						flag = false;
					}
					else if (isLeftDir == circleEffectChild.transform.localPosition.x > Children[i].transform.localPosition.x)
					{
						circleEffectChild = Children[i];
					}
				}
			}

			if (circleEffectChild != null)
			{
				circleEffectChild.EndEffect(_callback);
			}
		}

		[global::OpenPCR.Migration("5b6dc0d1-ecdb-42ca-9a96-6c8bb2ae5af6")]
		public override void Pause()
		{
			base.Pause();
			for (int i = 0; i < Children.Count; i++)
			{
				Children[i].Pause();
			}
		}

		[global::OpenPCR.Migration("4d5aff37-9c1d-4ae4-b162-25c925176dea")]
		public override void Resume()
		{
			base.Resume();
			for (int i = 0; i < Children.Count; i++)
			{
				Children[i].Resume();
			}
		}

		[global::OpenPCR.Migration("c25c7808-eeb0-400e-a258-891abc1fece0")]
		protected override void resetStartDelay()
		{ global::OpenPCR.Presentation.Noop(); }
	}
}
