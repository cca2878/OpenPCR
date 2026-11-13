using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class CopyAtkParamAction : ActionParameter
	{
		private AttackActionBase targetAction;
		[global::OpenPCR.Migration("09e38108-2c79-4ced-ae4a-7d692b97a7b0")]
		public CopyAtkParamAction() : base()
		{
		}

		[global::OpenPCR.Migration("b8a757e1-8184-42ad-8198-6b598c6730e4")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			targetAction = (AttackActionBase)_skill.ActionParameters.Find(e => e.ActionId == ActionDetail2);
			targetAction.UseCopyAtkParam = true;
		}

		[global::OpenPCR.Migration("b55de762-d622-4825-a86f-192870ca2ae6")]
		public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			targetAction.CopyAtk = 0L;
		}

		[global::OpenPCR.Migration("99204e23-e22d-4e68-b88b-6e4b640cdaff")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			switch (ActionDetail1)
			{
				case 1: targetAction.CopyAtk = _target.GetAtkZero(); break;
				case 2: targetAction.CopyAtk = _target.GetMagicStrZero(); break;
			}
		}

		[global::OpenPCR.Migration("3121199e-b129-4955-85d0-7eed8dd36eb4")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
		}
	}
}
