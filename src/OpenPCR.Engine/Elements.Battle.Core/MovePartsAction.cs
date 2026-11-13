using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class MovePartsAction : ActionParameter
	{
		private BasePartsData parts;
		[global::OpenPCR.Migration("65728cf2-720e-4617-b1f8-07714fc0d929")]
		public MovePartsAction() : base()
		{
		}

		[global::OpenPCR.Migration("72d7fee7-a144-4537-a54f-1dccccbf6ce2")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			parts = _source.ComponentMultiPartsBoss.BossPartsListForBattle.Find((PartsData e) => e.Index == BattleUtil.DoubleToInt(base.Value[eValueNumber.VALUE_4]));
		}

		[global::OpenPCR.Migration("863d8b04-28c1-4164-a98f-9b488ee7cc9a")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			if ((_num & 1) == 0)
			{
				parts.PositionX += (float)_valueDictionary[eValueNumber.VALUE_1];
				parts.Owner.PartsMotionPrefix = BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_5]);
				OnActionEnd = () =>
				{
					parts.Owner.PartsMotionPrefix = 0;
					float position = parts.InitialPositionX;
					OnActionEnd = null;
					parts.PositionX = position;
				};
			}
			else
			{
				parts.Owner.PartsMotionPrefix = 0;
				float position = parts.InitialPositionX;
				OnActionEnd = null;
				parts.PositionX = position;
			}
		}
	}
}
