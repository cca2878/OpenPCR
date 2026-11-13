using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class ChangeCriticalReferenceAction : ActionParameter
	{
		[global::OpenPCR.Migration("af2f122d-6d50-4575-8f01-eead88b9b3bb")]
		public ChangeCriticalReferenceAction() : base()
		{
		}

		[global::OpenPCR.Migration("154ac403-d285-4262-b725-b20e326cfc88")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			var action = _skill.ActionParameters.Find(e => e.ActionId == ActionDetail1) as AttackActionBase;
			action.CriticalReference = (AttackActionBase.eCriticalReference)ActionDetail2;
			action.AdditionalCritical = BattleUtil.DoubleToLong(Value[eValueNumber.VALUE_1]);
		}

		[global::OpenPCR.Migration("a75f84e5-08ed-4a44-a843-484595e7c88f")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction((global::Elements.UnitCtrl)(@_source), (global::Elements.BasePartsData)(@_target), unchecked((int)(unchecked((uint)(@_num)))), (global::Elements.UnitActionController)(@_sourceActionController), (global::Elements.Skill)(@_skill), @_starttime, (global::System.Collections.Generic.Dictionary<int, bool>)(@_enabledChildAction), (global::System.Collections.Generic.Dictionary<global::Elements.Battle.Core.eValueNumber, double>)(@_valueDictionary));
		}

		[global::OpenPCR.Migration("ce627ddd-1856-48ab-838d-5f2013a8b182")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
		}
	}
}
