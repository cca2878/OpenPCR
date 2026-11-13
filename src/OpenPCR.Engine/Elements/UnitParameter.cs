using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Elements.Data;

namespace Elements
{
	public class UnitParameter
	{
		private UnitData limitedUnitData;
		private int _enemyColor;
		public UnitData UniqueData { get; set; }
		public UnitDataForView UniqueDataForView { get; set; }

		[global::OpenPCR.Migration("0479b50e-c5be-4047-9d5b-9c91595e8ba7")]
		public UnitData LimitedUnitData
		{
			[global::OpenPCR.Migration("7a08fdec-7120-4cff-9c20-b56917a8ebc8")]
			get
			{
				if (limitedUnitData == null)
				{
					UpdateLimitedUnitData();
				}

				return limitedUnitData;
			}

			[global::OpenPCR.Migration("fb9f2337-4d34-41a1-913d-9d001feb48ca")]
			set
			{
				limitedUnitData = value;
			}
		}

		public bool IsClanSupportOrReplay { get; set; }
		public MasterUnitData.UnitData MasterData { get; set; }
		public MasterUnitSkillData.UnitSkillData SkillData { get; set; }

		public int EnemyColor
		{
			[global::OpenPCR.Migration("314a60d6-12ed-4ca5-9912-d75ac310999f")]
			get
			{
				return _enemyColor;
			}

			set
			{
				_enemyColor = value;
			}
		}

		public UnitParameter()
		{
		}

		[global::OpenPCR.Migration("acb05123-7d07-430a-ad06-07c97da5e704")]
		public UnitParameter(UnitData _unitData)
		{
			UniqueData = _unitData;
			if (ManagerSingleton<MasterDataManager>.Instance.masterUnitData != null)
				SetMasterData();
		}

		[global::OpenPCR.Migration("cbcebd74-8ece-423e-9551-8b93eeceaf30")]
		public void SetMasterData()
		{
			int resourceId = UnitUtility.GetUnitResourceId(UniqueData.Id);
			MasterData = ManagerSingleton<MasterDataManager>.Instance.masterUnitData.Get(resourceId);
			SkillData = ManagerSingleton<MasterDataManager>.Instance.masterUnitSkillData[resourceId];
		}

		[global::OpenPCR.Migration("8e91e088-fd4e-4ca0-9154-e18461da9579")]
		public void UpdateLimitedUnitData(bool _calcLocalPower = true, bool _isSupportOrReplay = false)
		{
			IsClanSupportOrReplay = _isSupportOrReplay;
			// Level cap, skill-level sync, retained EX slots and CalcParamAndSkill are the external growth producer's.
			limitedUnitData = (UnitData)global::OpenPCR.PreparedParameters.Limited(ManagerSingleton<MasterDataManager>.Instance, UniqueData, _isSupportOrReplay);
		}
	}
}
