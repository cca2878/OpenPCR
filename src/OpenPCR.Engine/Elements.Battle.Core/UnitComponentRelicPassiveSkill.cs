using Elements.Data;

namespace Elements.Battle.Core
{
	public class UnitComponentRelicPassiveSkill : UnitComponentBase
	{
		private LabyrinthEnhanceParameter enhanceParameter;
		private UnitComponentParameter componentParameter;

		[global::OpenPCR.Migration("2dd7d24a-8919-4385-9845-c009c0e754e7")]
		public UnitComponentRelicPassiveSkill(UnitCtrl _unitController) : base(_unitController)
		{
		}

		[global::OpenPCR.Migration("32ebb5bc-1855-48ff-b33a-4b87ea8f5a87")]
		public void RegisterComponentSet(IUnitComponentContainer _container)
		{
			componentParameter = _container.ComponentParameter;
		}

		[global::OpenPCR.Migration("d39a4df7-80f1-4194-9434-dcda0d89082e")]
		public void SetTeamEnhanceData(LabyrinthEnhanceParameter _enhanceParameter)
		{
			enhanceParameter = _enhanceParameter;
		}

		[global::OpenPCR.Migration("de0996d9-9cf3-43bf-9694-1a9650a17c50")]
		public void ApplyRelicPassiveSkill(bool _isFirst, bool _considerEquipmentAndBonus)
		{
			if (enhanceParameter != null)
			{
				bool immutableMaxHp = unitController.IsClanBattleOrSekaiEnemy;
				foreach (var parameter in enhanceParameter.LabyrinthRelicPassiveParameter)
				{
					componentParameter.ApplyEnhanceValue(parameter.Key, parameter.Value, _isFirst,
						immutableMaxHp, UnitDefine.eParameterEnhanceType.RELIC);
				}
			}
		}
	}
}
