namespace Elements.Battle.Core
{
	public interface IUnitComponentContainer
	{
		UnitComponentAbnormal ComponentAbnormal { get; }

		UnitComponentActionPattern ComponentActionPattern { get; }

		UnitComponentAnimation ComponentAnimation { get; }

		UnitComponentAnnihilation ComponentAnnihilation { get; }

		UnitComponentBattleResult ComponentBattleResult { get; }

		UnitComponentColor ComponentColor { get; }

		UnitComponentCompare ComponentCompare { get; }

		UnitComponentDamageControl ComponentDamageControl { get; }

		UnitComponentLog ComponentLog { get; }

		UnitComponentMultiPartsBoss ComponentMultiPartsBoss { get; }

		UnitComponentParameter ComponentParameter { get; }

		UnitComponentParameterClamped ComponentParameterClamped { get; }

		UnitComponentPassiveSkill ComponentPassiveSkill { get; }

		UnitComponentRelicSkill ComponentRelicSkill { get; }

		UnitComponentRelicPassiveSkill ComponentRelicPassiveSkill { get; }

		UnitComponentChangeScale ComponentScaleChange { get; }

		UnitComponentSearchArea ComponentSearchArea { get; }

		UnitComponentSortOrder ComponentSortOrder { get; }

		UnitComponentSound ComponentSound { get; }

		UnitComponentState ComponentState { get; }

		UnitComponentUnionBurst ComponentUnionBurst { get; }
	}
}
