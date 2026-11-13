using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class AutoRevivalAction : ActionParameter
	{
		private BasePartsData parts;
		[global::OpenPCR.Migration("6eddeaea-08ca-4020-939b-ea632b95ad39")]
		public AutoRevivalAction() : base()
		{
		}

		[global::OpenPCR.Migration("73fabed1-ec88-4a1d-8a71-35b631a8598f")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			parts = _source.ComponentMultiPartsBoss.BossPartsListForBattle.Find(e => e.Index == _skill.ParameterTarget);
		}

		[global::OpenPCR.Migration("6930c0a7-4abd-43eb-893d-ae0ebf86fcc0")]
		public override void ExecActionOnWaveStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnWaveStart(_skill, _source, _sourceActionController);
			_source.ComponentAbnormal.AutoRevivalData = new AutoRevivalData(parts, ActionDetail1, ActionDetail3, Value, _source, (eStateIconType)ActionDetail2, (eEffectType)EffectType, _skill, battleManager);
		}

		[global::OpenPCR.Migration("ee3af10e-b9fd-41f7-a8b9-7597facc2917")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction((global::Elements.UnitCtrl)(@_source), (global::Elements.BasePartsData)(@_target), unchecked((int)(unchecked((uint)(@_num)))), (global::Elements.UnitActionController)(@_sourceActionController), (global::Elements.Skill)(@_skill), @_starttime, (global::System.Collections.Generic.Dictionary<int, bool>)(@_enabledChildAction), (global::System.Collections.Generic.Dictionary<global::Elements.Battle.Core.eValueNumber, double>)(@_valueDictionary));
		}

		[global::OpenPCR.Migration("9ba4bce5-f5db-4cc7-9d68-c59c486112a7")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_2] = MasterData.action_value_2 + MasterData.action_value_3 * _level;
		}
	}
}
