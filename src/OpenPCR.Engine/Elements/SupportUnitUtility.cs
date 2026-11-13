using global::OpenPCR.Foundation;
using Elements.Data;

namespace Elements
{
	public static class SupportUnitUtility
	{
		[global::OpenPCR.Migration("fa6ab59d-5138-471b-93a9-8c550f194b8a")]
		public static void InitSupportUnitData()
		{
			TempData data = Singleton<TempData>.Instance;
			data.SupportFriendList = null;
			data.SupportUnitId = 0;
			data.SupportOwnerId = 0L;
			data.SupportGeneralList = null;
			data.SupportNpcList = null;
			data.SupportUnitSettingList = null;
		}

		[global::OpenPCR.Migration("dbaf6215-5967-4058-9c25-b06b3a6420a3")]
		public static void InitSupportUnitQuestStartParam()
		{
			SupportUnitDefine.QuestStartParam parameter = Singleton<TempData>.Instance.SupportQuestStartParam;
			if (parameter != null)
				parameter.Init();
		}

		[global::OpenPCR.Migration("b60c53bf-229a-4078-a71d-be9f153864ff")]
		public static SupportUnitStatus GetSupportUnit(long _ownerViewId, int _unitId)
		{
			TempData data = Singleton<TempData>.Instance;
			SupportUnitStatus result = data.SupportFriendList == null ? null : data.SupportFriendList.Find(unit => (long)unit.OwnerViewerId == _ownerViewId && (int)unit.UnitData.Id == _unitId);
			if (result != null)
				return result;
			result = data.SupportGeneralList == null ? null : data.SupportGeneralList.Find(unit => (long)unit.OwnerViewerId == _ownerViewId && (int)unit.UnitData.Id == _unitId);
			if (result != null)
				return result;
			return data.SupportNpcList == null ? null : data.SupportNpcList.Find(unit => (long)unit.OwnerViewerId == _ownerViewId && (int)unit.UnitData.Id == _unitId);
		}
	}
}
