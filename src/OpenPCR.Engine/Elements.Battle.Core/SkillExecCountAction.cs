using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class SkillExecCountAction : ActionParameter
	{
		[global::OpenPCR.Migration("033e605f-d4bb-4176-a2bf-1b06eefc1fe9")]
		public SkillExecCountAction() : base()
		{
		}

		[global::OpenPCR.Migration("24f29524-ac20-4a13-bb7f-5d0cb7134880")]
		public override void ExecActionOnWaveStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnWaveStart(_skill, _source, _sourceActionController);
			if (_source.ComponentAbnormal.SkillExecCountDictionary.ContainsKey(base.ActionDetail1))
			{
				_source.ComponentAbnormal.SkillExecCountDictionary[base.ActionDetail1] = 0;
			}
		}

		[global::OpenPCR.Migration("120e7c92-932d-428b-891b-f6cc05585fe8")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("3773a23c-8fe4-4df5-9e3a-6d536a845246")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			if (!_source.ComponentAbnormal.SkillExecCountDictionary.ContainsKey(base.ActionDetail1))
			{
				_source.ComponentAbnormal.SkillExecCountDictionary.Add(base.ActionDetail1, 0);
			}

			_source.ComponentAbnormal.SkillExecCountDictionary[base.ActionDetail1] = Mathf.Min(OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_1]), _source.ComponentAbnormal.SkillExecCountDictionary[base.ActionDetail1] + 1);
		}

		[global::OpenPCR.Migration("4c9babaa-e636-469b-920b-afcf7a5c6e3b")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
		}
	}
}
