using System.Collections.Generic;
using Elements.Data;
namespace Elements.Utility
{
	public class SkillUtility
	{


		[global::OpenPCR.Migration("cd02be78-7670-4eca-87af-169560fe7093")]
		public static SkillData GetSkillData(int _skillId, int _skillLevel)
		{
			var master = ManagerSingleton<MasterDataManager>.Instance.masterSkillData.Get(_skillId);
			if (master == null) return null;
			SkillData result = new SkillData();
			result.SetSkillId(_skillId);
			List<ActionData> list = new List<ActionData>();
			for (int i = 0; i < master.ActionDataList.Count; i++)
			{
				ActionData action = new ActionData();
				action.SetActionId(master.ActionDataList[i].action_id);
				action.SetActionValue(getSkillActionValue(action.ActionId, _skillLevel));
				list.Add(action);
			}
			result.SetActionInfoList(list);
			return result;
		}
		[global::OpenPCR.Migration("800a74ac-6c71-4d06-8a30-f6e5feed0d87")]
		private static SkillActionValue getSkillActionValue(int _actionId, int _skillLevel)
		{
			var master = ManagerSingleton<MasterDataManager>.Instance.masterSkillAction.Get(_actionId);
			if (master == null) return null;
			SkillActionValue result = new SkillActionValue();
			result.SetValue1((float)(double)master.action_value_1);
			result.SetValue2((float)(double)master.action_value_2);
			result.SetValue3((float)(double)master.action_value_3);
			result.SetValue4((float)(double)master.action_value_4);
			result.SetValue5((float)(double)master.action_value_5);
			result.SetValue6((float)(double)master.action_value_6);
			result.SetValue7((float)(double)master.action_value_7);
			switch ((ushort)master.action_type)
			{
				case 1:
				case 8:
				case 9:
				case 11:
				case 12:
				case 13:
				case 25:
				case 32:
				case 38:
				case 39:
				case 40:
				case 47:
				case 72:
				case 73:
				case 79:
				case 80:
				case 83:
				case 96:
				case 99:
				case 100:
				case 113:
				case 117:
					{
						float value1 = (float)(double)master.action_value_1;
						float growth1 = (float)(double)master.action_value_2;
						result.SetValue1((float)_skillLevel * growth1 + value1);
						float value3 = (float)(double)master.action_value_3;
						float growth3 = (float)(double)master.action_value_4;
						result.SetValue3((float)_skillLevel * growth3 + value3);
						break;
					}
				case 4:
				case 10:
				case 24:
				case 29:
				case 31:
				case 34:
				case 50:
				case 102:
					{
						float value2 = (float)(double)master.action_value_2;
						float growth2 = (float)(double)master.action_value_3;
						result.SetValue2((float)_skillLevel * growth2 + value2);
						float value4 = (float)(double)master.action_value_4;
						float growth4 = (float)(double)master.action_value_5;
						result.SetValue4((float)_skillLevel * growth4 + value4);
						break;
					}
				case 5:
				case 6:
				case 14:
				case 16:
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
				case 33:
				case 46:
				case 49:
				case 97:
				case 104:
					{
						float value1 = (float)(double)master.action_value_1;
						float growth1 = (float)(double)master.action_value_2;
						result.SetValue1((float)_skillLevel * growth1 + value1);
						break;
					}
				case 15:
				case 26:
				case 90:
				case 91:
				case 98:
				case 115:
				case 121:
					{
						float value2 = (float)(double)master.action_value_2;
						float growth2 = (float)(double)master.action_value_3;
						result.SetValue2((float)_skillLevel * growth2 + value2);
						break;
					}
				case 36:
				case 37:
				case 48:
					{
						float value1 = (float)(double)master.action_value_1;
						float growth1 = (float)(double)master.action_value_2;
						result.SetValue1((float)_skillLevel * growth1 + value1);
						float value3 = (float)(double)master.action_value_3;
						float growth3 = (float)(double)master.action_value_4;
						result.SetValue3((float)_skillLevel * growth3 + value3);
						float value5 = (float)(double)master.action_value_5;
						float growth5 = (float)(double)master.action_value_6;
						result.SetValue5((float)_skillLevel * growth5 + value5);
						break;
					}
				case 132:
					{
						float value1 = (float)(double)master.action_value_1;
						float growth1 = (float)(double)master.action_value_2;
						result.SetValue1((float)_skillLevel * growth1 + value1);
						float value4 = (float)(double)master.action_value_4;
						float growth4 = (float)(double)master.action_value_5;
						result.SetValue4((float)_skillLevel * growth4 + value4);
						break;
					}
				case 27:
				case 74:
					if ((double)master.action_value_4 == 1.0)
					{
						float value = (float)(double)master.action_value_2;
						float growth = (float)(double)master.action_value_3;
						result.SetValue2((float)_skillLevel * growth + value);
					}
					else if ((double)master.action_value_4 == 0.0)
					{
						float value = (float)(double)master.action_value_2;
						float growth = (float)((double)master.action_value_3 + 1.0);
						result.SetValue2((float)_skillLevel * growth + value);
					}
					break;
			}
			return result;
		}
	}
}
