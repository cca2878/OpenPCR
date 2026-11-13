using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using global::OpenPCR.AnimKit;

namespace Elements.Battle.Core
{
	public class UnitComponentBattleResult : UnitComponentBase, ISingletonField
	{
		private float scaleValue { get; set; }
		private float scaleForGuest { get; set; } = 1f;
		private float resultMoveDistance { get; set; }
		private FixedTransformMonoBehavior.FixedTransform transform { get; set; }
		private UnitComponentAnimation componentAnimation { get; set; }
		private UnitComponentSortOrder componentSortOrder { get; set; }

		[global::OpenPCR.Migration("0570a196-ffdb-497c-97fb-625ea79b3d03")]
		public UnitComponentBattleResult(UnitCtrl _unitController, FixedTransformMonoBehavior.FixedTransform _transform) : base(_unitController)
		{
			transform = _transform;
		}

		[global::OpenPCR.Migration("b6c2ab28-9610-4216-8bde-dbe043ff0a04")]
		public void ResetResultPos()
		{
			if (resultMoveDistance != 0f)
			{
				Vector3 localPosition = transform.localPosition;
				localPosition.x -= resultMoveDistance;
				resultMoveDistance = 0f;
				transform.localPosition = localPosition;
			}
		}

		[global::OpenPCR.Migration("2e9eee04-fdc0-4a27-882b-54ef1a308291")]
		public void RegisterComponentSet(IUnitComponentContainer _container)
		{
			componentAnimation = _container.ComponentAnimation;
			componentSortOrder = _container.ComponentSortOrder;
		}

	}
}
