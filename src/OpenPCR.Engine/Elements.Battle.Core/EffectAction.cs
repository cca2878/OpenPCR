using global::Elements;
using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class EffectAction : ActionParameter
	{
		[global::OpenPCR.Migration("c45f5caf-0e05-4ea0-ba92-a1edc9877923")]
		public EffectAction() : base()
		{
		}

		[global::OpenPCR.Migration("a808ed82-8d7e-4dda-aca9-746c41ef522d")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("6ff28068-9663-46a2-9e37-ef6470b956ee")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			foreach (NormalSkillEffect effect in ActionEffectList)
			{
				global::OpenPCR.Runtime.Api.GameObject prefab = _target.Owner.IsLeftDir ? effect.PrefabLeft : effect.Prefab;
				SkillEffectCtrl ctrl = battleEffectPool.GetEffect(prefab, null);
				ctrl.transform.parent = ExceptUIRoot.Transform;
				ctrl.SortTarget = _target.Owner;
				ctrl.InitializeSort();
				ctrl.SetPossitionAppearanceType(effect, _target, _target.Owner, _skill, SkillEffectCtrl.eToadTrackType.NONE);
				ctrl.ExecAppendCoroutine(null, false);
				ctrl.PlaySe(_source.ComponentSound.SoundUnitId, _source.IsLeftDir);
			}
		}

		[global::OpenPCR.Migration("3a3aebfc-fd66-4a99-b337-b5f5785a2be8")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
		}
	}
}
