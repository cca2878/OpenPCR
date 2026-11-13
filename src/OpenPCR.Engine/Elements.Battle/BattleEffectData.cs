using System;
using System.Collections.Generic;

namespace Elements.Battle
{
	[Serializable]
	public class BattleEffectData : BattleEffectDataBase
	{
		public Dictionary<UnitCtrl, BattleEffectDataBase> BattleEffectDataDictionary = new Dictionary<UnitCtrl, BattleEffectDataBase>();
		private readonly List<UnitCtrl> blackOutUnitList;
		private readonly List<UnitCtrl> unitList;
		private readonly List<UnitCtrl> otherList;
		[global::OpenPCR.Migration("3246a136-60e5-4549-b859-c373c89c054a")]
		public BattleEffectData(List<UnitCtrl> _blackOutUnitList, List<UnitCtrl> _unitList, List<UnitCtrl> _otherList)
		{
			blackOutUnitList = _blackOutUnitList;
			unitList = _unitList;
			otherList = _otherList;
		}

		[global::OpenPCR.Migration("440fca23-ff07-4728-8017-3c394a4d5d56")]
		public override void Pause()
		{
			base.Pause();
			foreach (KeyValuePair<UnitCtrl, BattleEffectDataBase> item in BattleEffectDataDictionary)
			{
				item.Value.Pause();
			}
		}

		[global::OpenPCR.Migration("9b8043fc-2023-4935-9241-182489a9665c")]
		public override void SetSkillEffectSortBack()
		{
			base.SetSkillEffectSortBack();
			foreach (KeyValuePair<UnitCtrl, BattleEffectDataBase> item in BattleEffectDataDictionary)
			{
				item.Value.SetSkillEffectSortBack();
			}
		}

		[global::OpenPCR.Migration("b8fb2f2d-0b82-4d65-8e6c-081a8d339806")]
		public override void Resume()
		{
			if (blackOutUnitList.Count == 0)
			{
				base.Resume();
				foreach (KeyValuePair<UnitCtrl, BattleEffectDataBase> item in BattleEffectDataDictionary)
				{
					item.Value.Resume();
				}
			}
			else
			{
				for (int num = blackOutUnitList.Count - 1; num >= 0; num--)
				{
					if (BattleEffectDataDictionary.ContainsKey(blackOutUnitList[num]))
					{
						BattleEffectDataDictionary[blackOutUnitList[num]].Resume();
					}
				}
			}

			for (int num2 = UiTweenerListIgnoreBlackout.Count - 1; num2 >= 0; num2--)
			{
				if (UiTweenerListIgnoreBlackout[num2] == null)
				{
					UiTweenerListIgnoreBlackout.RemoveAt(num2);
				}
				else
				{
					UiTweenerListIgnoreBlackout[num2].enabled = true;
				}
			}
		}

		[global::OpenPCR.Migration("c6fd7b28-4dcc-46af-a3e9-cd36a3f82c77")]
		public override void SetSortOrderBack()
		{
			int i = 0;
			for (int count = unitList.Count; i < count; i++)
			{
				if (!unitList[i].ComponentSortOrder.IsFront && BattleEffectDataDictionary.ContainsKey(unitList[i]))
				{
					BattleEffectDataDictionary[unitList[i]].SetSortOrderBack();
				}
			}

			int j = 0;
			for (int count2 = otherList.Count; j < count2; j++)
			{
				if (!otherList[j].ComponentSortOrder.IsFront && BattleEffectDataDictionary.ContainsKey(otherList[j]))
				{
					BattleEffectDataDictionary[otherList[j]].SetSortOrderBack();
				}
			}
		}

	}
}
