using global::OpenPCR.Runtime.Api;

namespace Elements
{
	public class FixedTransformMonoBehavior : MonoBehaviour
	{
		public class FixedTransform
		{
			private int positionX;
			private int positionY;
			public Transform TargetTransform;
			public Vector3 localPosition
			{
				[global::OpenPCR.Migration("9c5575f6-21ea-4010-a41e-eefb6d412220")]
				get
				{
					return new Vector3((float)positionX / 100f, (float)positionY / 100f, 0f);
				}

				[global::OpenPCR.Migration("eb9a3ea8-3a62-47da-b7fa-f2b8bb24d037")]
				set
				{
					SetLocalPosX(value.x);
					SetLocalPosY(value.y);
				}
			}

			public Vector3 position
			{
				[global::OpenPCR.Migration("f711659c-34c6-43c6-a2de-57d1fe8f8763")]
				get
				{
					return TargetTransform.parent == null ? localPosition : TargetTransform.parent.TransformPoint(localPosition);
				}

				[global::OpenPCR.Migration("8f6aeb9f-cb4f-485a-bce4-cf333695a165")]
				set
				{ throw new global::OpenPCR.OutOfBoundaryException("Elements.FixedTransformMonoBehavior+FixedTransform::set_position(OpenPCR.Runtime.Api.Vector3):System.Void: runtime capability is unavailable."); }
			}

			public Transform parent
			{
				[global::OpenPCR.Migration("361730f9-65ca-4153-9164-7c7c90d4bb6e")]
				get
				{
					return TargetTransform.parent;
				}

				[global::OpenPCR.Migration("20ac3a97-9d42-437a-b7bf-168270c34eed")]
				set
				{
					TargetTransform.parent = value;
				}
			}

			public Vector3 lossyScale
			{
				[global::OpenPCR.Migration("5040db20-8b8d-49bb-84c8-8898d5512b23")]
				get => TargetTransform.lossyScale;
				private set { }
			}

			public Vector3 localScale
			{
				[global::OpenPCR.Migration("36aa84e5-f7ee-4b21-8a0f-099be5c8e5e3")]
				get
				{
					return TargetTransform.localScale;
				}

				[global::OpenPCR.Migration("1626c6ae-81b8-4645-91c0-8fc35ef8af7a")]
				set
				{
					TargetTransform.localScale = value;
				}
			}

			[global::OpenPCR.Migration("b50f40de-9a2c-4bff-8893-4af1b565ba43")]
			public FixedTransform(Transform _targetTransform)
			{
				TargetTransform = _targetTransform;
			}

			[global::OpenPCR.Migration("fa2fafa0-86c4-45c0-8d60-41442357f72f")]
			public void SetLocalPosX(float _x)
			{
				positionX = BattleUtil.FloatToInt(_x * 100f); TargetTransform.SetLocalPosX((float)positionX / 100f);
			}

			[global::OpenPCR.Migration("81a2e88b-58e2-4753-9ee6-ff030ac93c39")]
			public void SetLocalPosY(float _y)
			{
				positionY = global::OpenPCR.NativeNumeric.ToInt32(_y * 100f);
				TargetTransform.SetLocalPosY((float)positionY / 100f);
			}
		}

		private FixedTransform transformRaw;
		public new FixedTransform transform
		{
			[global::OpenPCR.Migration("24e6311f-e6ba-4146-b2a3-e1883b89ce40")]
			get
			{
				if (transformRaw == null)
				{
					transformRaw = new FixedTransform(base.transform);
				}

				if (transformRaw.TargetTransform == null)
				{
					transformRaw.TargetTransform = base.transform;
				}

				return transformRaw;
			}

			set
			{
			}
		}

		[global::OpenPCR.Migration("ee24ec47-2288-4f66-8f9a-ad0dc8fb8b93")]
		public FixedTransformMonoBehavior()
		{
		}

		[global::OpenPCR.Migration("d55119fd-2eaa-43c0-9f43-25e8235e5cc3")]
		public void OnAwake()
		{
			transformRaw = new FixedTransform(base.transform);
		}

		[global::OpenPCR.Migration("c7771b20-b09e-4fde-a798-b2c3e605aeb9")]
		protected virtual void DestructByOnDestroy()
		{
			transformRaw = null;
		}

		public void SetLocalPosX(float _x)
		{
			transform.SetLocalPosX(_x);
		}
	}
}
