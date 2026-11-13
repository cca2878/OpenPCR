using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class ExtendSlipDamageAction : ActionParameter
	{

		public int EnvironmentId { get; set; } = -1;
		[global::OpenPCR.Migration("45b9dcac-241e-4bf9-80eb-abee1a659459")]
		public ExtendSlipDamageAction() : base()
		{
		}

		[global::OpenPCR.Migration("e21560ca-7d96-4ba0-9c63-8e83d0820f96")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("abe7ea4c-7670-47d9-9dd0-35b1a0c847fe")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			var abnormal = _target.Owner.ComponentAbnormal;
			if (abnormal.ExtendSlipDamageData != null)
				abnormal.ExtendSlipDamageData.Enable = false;
			var data = new ExtendSlipDamageData
			{
				EnvironmentId = EnvironmentId,
				Target = _target.Owner,
				ExtendValue = (float)(Value[eValueNumber.VALUE_1] + 100.0) / 100f,
				Duration = Value[eValueNumber.VALUE_2]
			};
			abnormal.ExtendSlipDamageData = data;
			abnormal.ExtendSlipDamageData.Initialize();
		}

		[global::OpenPCR.Migration("95a10a03-c438-4a1a-9375-6ddaa66462f6")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
		}
	}
}
