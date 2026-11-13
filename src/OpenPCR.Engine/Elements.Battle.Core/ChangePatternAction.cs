using global::Elements;
using global::OpenPCR.Runtime.Api;
using Elements.Data;
using global::OpenPCR.Foundation;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class ChangePatternAction : ActionParameter
	{
		[global::OpenPCR.Migration("6288c44e-ee58-4c1b-82a2-f661ff5dc8df")]
		public ChangePatternAction() : base()
		{
		}

		[global::OpenPCR.Migration("8912d7d9-d702-4e5f-9a49-3fb82346d377")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			int actionDetail = base.ActionDetail1;
			if (actionDetail == 1)
			{
				_source.ComponentActionPattern.CreateAttackPattern(ManagerSingleton<MasterDataManager>.Instance.masterUnitSkillData[_source.CharacterUnitId], base.ActionDetail2);
			}
		}

		[global::OpenPCR.Migration("3eacfa5e-68a2-40bc-8999-18456913d9e9")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendTargetNum(_target.Owner, _num);
			endAllBeforeEffect();
			for (int i = 0; i < base.ActionEffectList.Count; i++)
			{
				playActionEffect(_source, _skill, base.ActionEffectList[i]);
			}

			switch (base.ActionDetail1)
			{
				case 1:
					_target.Owner.ComponentActionPattern.ChangeAttackPattern(base.ActionDetail2, _skill.Level, (float)_valueDictionary[eValueNumber.VALUE_1]);
					break;
				case 2:
					_target.Owner.ChangeChargeSkill(base.ActionDetail2, (float)_valueDictionary[eValueNumber.VALUE_1]);
					break;
			}

			switch (base.ActionDetail3)
			{
				case 1:
					_target.Owner.ComponentUnionBurst.SetIsUbDisabledByChangePattern(true);
					break;
				case 0:
					_target.Owner.ComponentUnionBurst.SetIsUbDisabledByChangePattern(false);
					break;
			}
		}

		[global::OpenPCR.Migration("f984d8d8-30da-4873-9f88-1e35f0d55549")]
		private void endAllBeforeEffect()
		{
			for (int i = 0; i < changePatternCurrentSkillEffect.Count; i++)
			{
				SkillEffectCtrl skillEffectCtrl = changePatternCurrentSkillEffect[i];
				skillEffectCtrl.SetTimeToDie(value: true);
				skillEffectCtrl.OnEffectEnd.Call(skillEffectCtrl);
			}

			changePatternCurrentSkillEffect.Clear();
		}

		[global::OpenPCR.Migration("db6ed104-a189-432e-9a68-81edf53bd546")]
		private void playActionEffect(UnitCtrl _source, Skill _skill, NormalSkillEffect _actionEffect)
		{
			GameObject mDOJNMEMHLN = (_source.IsLeftDir ? _actionEffect.PrefabLeft : _actionEffect.Prefab);
			SkillEffectCtrl effect = base.battleEffectPool.GetEffect(mDOJNMEMHLN);
			effect.IsRepeat = true;
			effect.transform.parent = ExceptUIRoot.Transform;
			effect.InitializeSort();
			effect.PlaySe(_source.ComponentSound.SoundUnitId, _source.IsLeftDir);
			effect.SetPossitionAppearanceType(_actionEffect, _source.ComponentMultiPartsBoss.GetFirstParts(_owner: true), _source, _skill);
			effect.ExecAppendCoroutine(_source);
			changePatternCurrentSkillEffect.Add(effect);
		}
	}
}
