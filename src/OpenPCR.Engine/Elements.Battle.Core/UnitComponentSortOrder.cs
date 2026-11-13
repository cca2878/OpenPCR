using global::OpenPCR.AnimKit;
using global::OpenPCR.AnimKit.Scene.Modules;
using global::OpenPCR.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class UnitComponentSortOrder : UnitComponentBase, IUnitComponent
	{
		private int changeSortOrderId { get; set; }
		private UnitComponentMultiPartsBoss componentMultiPartsBoss { get; set; }
		public bool IsFront { get; set; }

		[global::OpenPCR.Migration("ec80073e-53d9-4925-8fa8-ce7683f7c706")]
		public int SortOrder
		{
			[global::OpenPCR.Migration("2efc8c6f-d0c4-4247-bb4a-1178cb585948")]
			get { return sortOrder; }
			[global::OpenPCR.Migration("282feb14-9065-492a-bcfe-fd53aef66738")]
			set { sortOrder = value; unitController.SetSpineSortOrder(value); }
		}

		[global::OpenPCR.Migration("8c621fe8-d8d2-4ff0-8edf-1e6cadf4bb18")] public UnitComponentSortOrder(UnitCtrl _unitController) : base(_unitController) { }
		[global::OpenPCR.Migration("8a80bb7b-c383-408b-b69b-5525c6036098")]
		public void RegisterComponentSet(IUnitComponentContainer _container)
		{
			componentMultiPartsBoss = _container.ComponentMultiPartsBoss;
		}

		[global::OpenPCR.Migration("88de8b41-aa88-4a5d-9eb4-9b153ef37411")]
		public int GetSortOrderConsiderBlackout(bool _isSkillExecuteUnit)
		{ return global::OpenPCR.Presentation.Noop<int>(); }

		[global::OpenPCR.Migration("f3ca56ab-d66e-420a-8353-b37b69c1aeaf")]
		public void SetSortOrderBack()
		{
			int order = BattleDefine.GetUnitSortOrder(unitController);
			setSortOrderBack(order);
		}

		[global::OpenPCR.Migration("c396b2b9-9de2-4ab5-9a3e-39c56b1ccfdc")]
		public void SetSortOrderResultUIBack(int _depth)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.UnitComponentSortOrder::SetSortOrderResultUIBack(System.Int32):System.Void (outside first-phase mode-result-sort boundary)"); }

		[global::OpenPCR.Migration("c930ee95-0321-4a6d-a94c-3bfda803e948")]
		public void SetPartsSortForResult()
		{
			if (!(unitController.SkeletonRenderSeparator != null))
			{
				return;
			}

			foreach (SkeletonPartsRenderer partsRenderer in unitController.SkeletonRenderSeparator.partsRenderers)
			{
				partsRenderer.MeshRenderer.sortingOrder = SortOrder;
			}
		}

		[global::OpenPCR.Migration("ac969885-61e8-49ca-b23e-29a8d49beb2e")]
		public void SetSortOrderFront()
		{
			var attachmentChanges = unitController.SortFrontDiappearAttachmentChangeDataList;
			for (int i = 0; i < attachmentChanges.Count; i++)
			{
				AttachmentChangeData attachmentChangeData = attachmentChanges[i];
				var skeleton = unitController.UnitSpineCtrl.skeleton;
				((skeleton.skin == null) ? skeleton.data.defaultSkin : skeleton.skin).AddAttachment(attachmentChangeData.TargetIndex, attachmentChangeData.TargetAttachmentName, attachmentChangeData.AppliedAttachment);
				skeleton.slots.Items[attachmentChangeData.TargetIndex].attachment = attachmentChangeData.AppliedAttachment;
			}

			IsFront = true;
			SortOrder = BattleDefine.GetUnitSortOrder(unitController) + 11500;
			unitController.LifeGauge.SetSortOrderFront();
			var circleEffects = unitController.CircleEffectList;
			int j = 0;
			for (int count = circleEffects.Count; j < count; j++)
			{
				circleEffects[j].SetSortOrderFront();
			}
		}

		[global::OpenPCR.Migration("6b355111-b985-4309-bdf6-0269bd71577a")]
		public void SetSortOrderSuperFront()
		{
			(this).setSortOrderSuper(unchecked((int)(22350u)));
		}

		[global::OpenPCR.Migration("3223a55a-96c9-45fe-a17e-dd347ee1024c")]
		public void SetSortOrderSuperBack()
		{
			(this).setSortOrderSuper(unchecked((int)(11850u)));
		}

		[global::OpenPCR.Migration("79371cff-4c8a-401b-89f0-27049efa7147")]
		private void setSortOrderSuper(int _order)
		{
			if (!IsFront)
				return;
			SortOrder = _order;
			unitController.LifeGauge.SetSortOrderFront();
			var list = unitController.CircleEffectList;
			for (int i = 0, count = list.Count; i < count; i++)
				list[i].SetSortOrderFront();
		}

		[global::OpenPCR.Migration("1295b392-f90a-4205-80f3-d70de8111c55")]
		public void StartChangeSortOrder(List<ChangeSortOrderData> _changeSortOrderDatas, float _startTime)
		{
			if (_changeSortOrderDatas.Count != 0)
				unitController.AppendCoroutine(updateChangeSortOrder(_changeSortOrderDatas, _startTime, ++changeSortOrderId), ePauseType.NO_DIALOG);
		}

		[global::OpenPCR.Migration("50ff6384-1245-4b3f-b7b3-40658b10fc33")]
		private IEnumerator updateChangeSortOrder(List<ChangeSortOrderData> _changeSortOrderDatas, float _startTime, int _thisId)
		{
			float timer = _startTime;
			int currentIndex = 0;
			ChangeSortOrderData currentData = _changeSortOrderDatas[currentIndex];
			while (_thisId == changeSortOrderId)
			{
				timer += unitController.GetBattleDeltaTime();
				if (timer < currentData.Time)
				{
					yield return null;
					continue;
				}

				switch (currentData.SortType)
				{
					case ChangeSortOrderData.eSortType.BACK:
						SetSortOrderSuperBack();
						break;
					case ChangeSortOrderData.eSortType.FRONT:
						SetSortOrderSuperFront();
						break;
					case ChangeSortOrderData.eSortType.DEFAULT:
						if (IsFront)
						{
							SetSortOrderFront();
						}
						else
						{
							SetSortOrderBack();
						}

						break;
				}

				currentIndex++;
				if (_changeSortOrderDatas.Count == currentIndex)
				{
					break;
				}

				currentData = _changeSortOrderDatas[currentIndex];
			}
		}

		private int sortOrder { get; set; }

		[global::OpenPCR.Migration("c14c60f5-3427-42a4-9f17-10bc99d6955c")]
		private void setSortOrderBack(int @_order)
		{
			List<AttachmentChangeData> attachments = unitController.SortFrontDiappearAttachmentChangeDataList;
			for (int i = 0; i < attachments.Count; i++)
			{
				AttachmentChangeData attachmentChangeData = attachments[i];
				var skeleton = unitController.UnitSpineCtrl.skeleton;
				(skeleton.skin ?? skeleton.data.defaultSkin).AddAttachment(attachmentChangeData.TargetIndex, attachmentChangeData.TargetAttachmentName, attachmentChangeData.TargetAttachment);
				skeleton.slots.Items[attachmentChangeData.TargetIndex].attachment = attachmentChangeData.TargetAttachment;
			}

			IsFront = false;
			SortOrder = _order;
			unitController.LifeGauge.SetSortOrderBack();
			var circles = unitController.CircleEffectList;
			for (int j = 0, count = circles.Count; j < count; j++)
			{
				circles[j].SetSortOrderBack();
			}

			var bossParts = componentMultiPartsBoss.BossPartsListForBattle;
			for (int k = 0; k < bossParts.Count; k++)
			{
				bossParts[k].IsBlackoutTarget = false;
			}

			unitController.OnIsFrontFalse.Call();
		}
	}
}
