using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using global::OpenPCR.Foundation;
namespace Elements
{
	public class CircleEffectChild : global::OpenPCR.Runtime.Api.MonoBehaviour
	{
		public float StartPitch;
		public float CurrentPitch { get; set; }
		public bool IsFront { get; set; }
		public global::Elements.CircleEffectController CircleEffectController { get; set; }

		[global::OpenPCR.Migration("a60b4caa-6441-4915-8c2c-0cb46901bdda")]
		public CircleEffectChild()
		{
		}

		[global::OpenPCR.Migration("cb5cba3d-1b1a-47e5-96e0-f26757659022")]
		public virtual void Initialize(global::Elements.CircleEffectController _circleEffectController, global::Elements.UnitCtrl _target)
		{ CircleEffectController = _circleEffectController; CurrentPitch = StartPitch; }

		[global::OpenPCR.Migration("7ad056e4-7d04-415c-a9e0-81f5c8b42f6c")]
		public void UpdatePosition(float _deltaAngle, int _sortOrder)
		{
			CurrentPitch += _deltaAngle;
			if (CurrentPitch < 0f)
			{
				CurrentPitch += 360f;
			}

			CurrentPitch %= 360f;
			if (CurrentPitch < 180f && !IsFront)
			{
				IsFront = true;
				SetSortOder(_sortOrder - 1);
			}

			if (CurrentPitch > 180f && IsFront)
			{
				IsFront = false;
				SetSortOder(_sortOrder + 1);
			}

			base.transform.localPosition = Quaternion.AngleAxis(CircleEffectController.Roll, Vector3.left) * Quaternion.AngleAxis(CircleEffectController.Yaw, Vector3.forward) * new Vector3(CircleEffectController.Radius * Mathf.Cos((float)Math.PI / 180f * CurrentPitch), 0f, CircleEffectController.Radius * Mathf.Sin((float)Math.PI / 180f * CurrentPitch));
		}

		[global::OpenPCR.Migration("fec92e88-a2e9-47db-876b-1bab3b26a60f")]
		protected virtual void SetSortOder(int _sortOrder)
		{ global::OpenPCR.Presentation.Noop(); }

		[global::OpenPCR.Migration("4d55b027-54eb-472a-9e8e-238975fb592e")]
		public void UpdateSortOrder(int _sortOrder)
		{
			if (CurrentPitch <= 180f)
			{
				IsFront = true;
				SetSortOder(_sortOrder - 1);
			}

			if (CurrentPitch > 180f)
			{
				IsFront = false;
				SetSortOder(_sortOrder + 1);
			}
		}

		[global::OpenPCR.Migration("0dd86136-7c3e-4aa8-8f00-109ea9591be3")]
		public virtual void StartEffect()
		{ gameObject.SetActive(true); }

		[global::OpenPCR.Migration("193524cf-f777-4fb1-832d-61b15d185459")]
		public virtual void EndEffect(global::System.Action _callback)
		{
			base.gameObject.SetActive(value: false);
			_callback.Call();
		}

		[global::OpenPCR.Migration("de7a4484-3cc2-4640-8dde-7af6158ff153")]
		public virtual void Pause()
		{
		}

		[global::OpenPCR.Migration("7f63cb46-eed8-4c18-b241-41be8dbcb557")]
		public virtual void Resume()
		{
		}

		[global::OpenPCR.Migration("d02310bf-4a5d-4936-9a28-a541f6bd59ed")]
		public void SetExecEffectDisable()
		{ global::OpenPCR.Presentation.Noop(); }
	}
}
