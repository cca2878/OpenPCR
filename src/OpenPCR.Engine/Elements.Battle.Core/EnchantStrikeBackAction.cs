using global::Elements;
using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using Elements.Battle;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public partial class EnchantStrikeBackAction : ActionParameter
	{
		private ActionParameter targetAction { get; set; }

		public enum eStrikeBackEffectType
		{
			TOY = 1,
			OMEME = 2,
			OMEME_PLUS = 3,
			LIGHT_SHIELD = 4
		}

		public class eStrikeBackEffectType_DictComparer : IEqualityComparer<eStrikeBackEffectType>
		{
			[global::OpenPCR.Migration("255bfa0b-d851-4fa7-88b9-6e08f78c5101")]
			public eStrikeBackEffectType_DictComparer() : base()
			{
			}

			[global::OpenPCR.Migration("2219c8e2-da02-4f96-a45f-3b42df4452d5")]
			public bool Equals(eStrikeBackEffectType _x, eStrikeBackEffectType _y)
			{
				return _x == _y;
			}

			[global::OpenPCR.Migration("fb6b297d-91db-486f-bbcc-8d5daa6bbf5f")]
			public int GetHashCode(eStrikeBackEffectType _obj)
			{
				return (int)_obj;
			}
		}

		[global::OpenPCR.Migration("362b105e-4556-4358-be0f-870c26387bcd")]
		public EnchantStrikeBackAction() : base()
		{
		}

		[global::OpenPCR.Migration("5aa50378-63ec-400a-9b0b-377b025a6b96")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			if (ActionDetail3 != 0)
			{
				targetAction = _skill.ActionParameters.Find(action => action.ActionId == ActionDetail3);
				targetAction.IgnoreTriggerCausedByTrigger = true;
			}

			if (ActionDetail2 <= 4)
				BattleResourceManager.Instance.LoadLegacyStrikeBackEffect(ActionDetail2);
			else
				BattleResourceManager.Instance.LoadStrikeBackEffect(ActionDetail2);
		}

		[global::OpenPCR.Migration("fc35dce8-5621-4eb3-a761-f41a62e1268f")]
		public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			base.ReadyAction(_source, _sourceActionController, _skill);
			ActionParameter action = targetAction;
			if (action != null)
			{
				action.CancelByIfForAll = true;
			}
		}

		[global::OpenPCR.Migration("194fd7c2-0c54-4960-883f-dc6638ed4d6b")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			SkillEffectCtrl skillEffectCtrl = null;
			CircleEffectController circleEffectController = null;
			var strikeBackDictionary = _target.Owner.ComponentAbnormal.StrikeBackDictionary;
			bool flag = true;
			if (strikeBackDictionary.ContainsKey((eStrikeBackEffectType)base.ActionDetail2))
			{
				flag = !strikeBackDictionary[(eStrikeBackEffectType)base.ActionDetail2].SkillEffect.IsPlaying;
			}

			if (flag)
			{
				var effectPool = battleEffectPool;
				BattleResourceManager resources = Singleton<BattleResourceManager>.Instance;
				GameObject prefab = (_target.Owner.IsLeftDir ? resources.StrikeBackEffectLeftDic : resources.StrikeBackEffectDic)[ActionDetail2];
				skillEffectCtrl = effectPool.GetEffect(prefab);
				skillEffectCtrl.transform.parent = ExceptUIRoot.Transform;
				skillEffectCtrl.InitializeSort();
				skillEffectCtrl.PlaySe(_source.ComponentSound.SoundUnitId, _source.IsLeftDir);
				circleEffectController = skillEffectCtrl as CircleEffectController;
				skillEffectCtrl.SetPossitionAppearanceType(new NormalSkillEffect { EffectTarget = eEffectTarget.ALL_TARGET, TargetBone = eTargetBone.CENTER, TrackType = ((!circleEffectController.FollowXY) ? eTrackType.BOTTOM : eTrackType.BONE), TrackDimension = ((!circleEffectController.FollowXY) ? eTrackDimension.NONE : eTrackDimension.XY) }, _target.Owner.ComponentMultiPartsBoss.GetFirstParts(_owner: true), _source, _skill, circleEffectController.FollowXY ? SkillEffectCtrl.eToadTrackType.CENTER : SkillEffectCtrl.eToadTrackType.NONE);
				skillEffectCtrl.ExecAppendCoroutine((_skill.BlackOutTime > 0f) ? _source : null);
				circleEffectController.SetChildrenInactive();
			}
			else
			{
				skillEffectCtrl = strikeBackDictionary[(eStrikeBackEffectType)base.ActionDetail2].SkillEffect;
				circleEffectController = skillEffectCtrl as CircleEffectController;
			}

			StrikeBackDataSet value = null;
			if (!strikeBackDictionary.TryGetValue((eStrikeBackEffectType)base.ActionDetail2, out value))
			{
				value = new StrikeBackDataSet
				{
					DataList = new List<StrikeBackData>()
				};
				strikeBackDictionary.Add((eStrikeBackEffectType)base.ActionDetail2, value);
			}

			value.SkillEffect = skillEffectCtrl;
			int count = circleEffectController.Children.Count;
			int num = Mathf.Min(BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_3]), count - value.DataList.Count);
			circleEffectController.AddEffect(num);
			for (int i = 0; i < num; i++)
			{
				StrikeBackData strikeBackData = new StrikeBackData
				{
					StrikeBackType = (StrikeBackData.eStrikeBackType)base.ActionDetail1,
					Damage = BattleUtil.DoubleToLong(_valueDictionary[eValueNumber.VALUE_1]),
					EffectType = (eStrikeBackEffectType)base.ActionDetail2,
					ActionEffectList = base.ActionEffectList,
					Skill = _skill,
					Source = _source,
					IsEnvironmentAbnormalState = IsEnvironmentAbnormalState,
					TargetAction = targetAction
				};
				strikeBackData.DataSet = value;
				value.DataList.Add(strikeBackData);
				_target.Owner.ComponentAbnormal.OnChangeState.Call(eStateIconType.STRIKE_BACK, arg2: true);
			}
		}

		[global::OpenPCR.Migration("091b9c60-ad09-43eb-889e-51431bd282a3")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
		}
	}
}
