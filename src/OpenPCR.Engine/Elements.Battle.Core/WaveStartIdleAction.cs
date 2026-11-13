using global::Elements;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class WaveStartIdleAction : ActionParameter
	{
		[global::OpenPCR.Migration("b756dc39-28bd-49d4-8290-10a3664385f5")]
		public WaveStartIdleAction() : base()
		{
		}

		[global::OpenPCR.Migration("f874adae-4bcd-4d3f-937c-5b3ebb46a37d")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("042ac822-3f78-4ffb-b12f-a4f7de5dd935")]
		public override void ExecActionOnWaveStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			var units = _source.IsOther ? battleManager.EnemyList : battleManager.UnitList;
			var peers = units.FindAll(e => e != _source && (long)e.ComponentParameter.Hp > 0L && e.ComponentSound.SoundUnitId != 105201 && e.ComponentSound.SoundUnitId != 114701);
			if (peers.Count == 0)
			{
				_source.ComponentSearchArea.SkillAreaWidthList[_skill.SkillId] = _source.ComponentSearchArea.SearchAreaSize;
				return;
			}

			_source.ComponentState.WaveStartIdle = true;
			_source.transform.SetLocalPosX(_source.IsOther ? 1400f : -1400f);
			_source.GetCurrentSpineCtrl().CurColor = new Color(1f, 1f, 1f, 0f);
			_sourceActionController.AppendCoroutine(updateIdle(_source), ePauseType.SYSTEM, _source);
		}

		[global::OpenPCR.Migration("4b8f1aba-69f2-4124-b468-a3ebb0d8fa7d")]
		private IEnumerator updateIdle(UnitCtrl _source)
		{
			_source.IsMoveSpeedForceZero = true;
			_source.SetLeftDirection(_source.IsOther);
			double value = Value[eValueNumber.VALUE_1];
			float delay = battleManager.IsBossBattle ? battleManager.GetBossUnit().UnitAppearDelay : 0f;
			float time = delay + (float)value;
			UnitComponentState state = _source.ComponentState;
			while (true)
			{
				time -= _source.DeltaTimeForPause;
				if (battleManager.GameState == eBattleGameState.WAIT_WAVE_END)
				{
					_source.GetCurrentSpineCtrl().CurColor = Color.white;
					_source.IsMoveSpeedForceZero = false;
					yield break;
				}

				if (time < 0f)
					break;
				yield return null;
			}

			_source.IsMoveSpeedForceZero = false;
			_source.SetLeftDirection(_source.IsOther);
			state.WaveStartIdle = false;
			state.SetState(ActionState.WALK, 0, 0, false);
		}

		[global::OpenPCR.Migration("b27d60fb-3187-43d7-9693-c784b84a1ebf")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			BattleAnimKitController currentSpineCtrl = _source.GetCurrentSpineCtrl();
			if (currentSpineCtrl.CurColor.a == 0f)
			{
				currentSpineCtrl.CurColor = Color.white;
			}
		}
	}
}
