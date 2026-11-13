using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Elements.Battle;

namespace Elements
{
	public class ViewBattle : ViewBase, ISingletonField
	{
		public override void StartView() => global::OpenPCR.MigrationPending.Throw("Elements.ViewBattle.StartView()", "behavior migration pending");
		public override void FinishView() => global::OpenPCR.MigrationPending.Throw("Elements.ViewBattle.FinishView()", "behavior migration pending");
		[SerializeField]
		private UnitUiCtrl unitUiCtrl;
		[SerializeField]
		private TransformEffectCtrl transformEffectCtrl;
		[SerializeField]
		private TransformEffectCtrlForBoss transformEffectCtrlForBoss;
		private BattleManager battleManager;
		[global::OpenPCR.ImplicitMigration("ff5b0894-09d4-48c6-a63e-7a510168157c")]
		public UnitUiCtrl UnitUiCtrl => unitUiCtrl;
		[global::OpenPCR.ImplicitMigration("6e16498c-84c5-4d3f-9960-5058137213ad")]
		public TransformEffectCtrl TransformEffectCtrl => transformEffectCtrl;
		[global::OpenPCR.ImplicitMigration("f92ce07f-4739-4be1-a493-7f4ee6f2309e")]
		public TransformEffectCtrlForBoss TransformEffectCtrlForBoss => transformEffectCtrlForBoss;
		public bool IsResultEnable { get; private set; }
		public bool StartFadeInCalled { get; private set; }
		public bool FinishFadeInCalled { get; private set; }

		public ViewBattle()
		{

		}



		[global::OpenPCR.Migration("98915504-b752-4ccd-9bbc-4053263f6fa2")]
		public IEnumerator ShowResultDialog(eBattleCategory _battleCategory, bool _win)
		{
			if (_battleCategory != eBattleCategory.CLAN_BATTLE && _battleCategory != eBattleCategory.TRIAL_CLAN_BATTLE)
				throw new global::OpenPCR.OutOfBoundaryException("ViewBattle.ShowResultDialog: other mode-specific result lifecycle outside clan/training scope.");
			if (_win)
			{
				if (_battleCategory == eBattleCategory.CLAN_BATTLE) dialogManager.OpenBossResult(this);
				else dialogManager.OpenTrialBattleResult(this);
				IsResultEnable = true;
				if (tempData.BattleSkipState != (eBattleSkipState)2 && !battleManager.IsLocalReplay())
					BattleVoiceUtility.PlayWinVoice(battleManager.UnitList, _battleCategory, false);
			}
			else openResultFailed(_battleCategory);
			yield break;
		}

		[global::OpenPCR.Migration("33127b73-1d6e-42d4-88e1-cc86a61f832a")]
		public void PlayFlatoutAfterMove(Action _callback)
		{
			BattleManager manager = battleManager;
			eBattleCategory category = manager.BattleCategory;
			eBattleSkipState skipState = tempData.BattleSkipState;
			bool localReplay = manager.IsLocalReplay();
			switch ((int)category)
			{
				case 9:
				case 11:
				case 12:
				case 16:
				case 20:
				case 25:
				case 26:
				case 27:
				case 28:
				case 29:
				case 31:
				case 32:
				case 35:
				case 47:
				case 48:
				case 50:
				case 51:
				case 52:
				case 53:
				case 57:
				case 58:
				case 66:
					battleManager.IsCurtain = true;
					openResultDialog(_callback);
					return;
				case 7:
					openResultDialog(_callback);
					return;
				case 8:
					return;
				case 0:
				case 1:
				case 10:
				case 56:
					openResultDialog(_callback);
					StartCoroutine(delayedExec(0.5f, () => PlayHXUIAnimation(DialogFlatoutPlayer.FlatoutType.BATTLE_RESULT_WIN_UP_LOOP, () => battleManager.IsCurtain = true)));
					return;
				default:
					throw new global::OpenPCR.OutOfBoundaryException("ViewBattle.PlayFlatoutAfterMove: other mode-specific result animations/callback producers outside clan/training scope.");
			}
		}

		private void openResultDialog(Action _callback)
		{
			switch ((int)battleManager.BattleCategory)
			{
				case 0:
				case 1:
				case 2:
				case 3:
				case 10:
				case 13:
				case 14:
				case 17:
				case 18:
				case 19:
				case 21:
				case 22:
				case 23:
				case 24:
				case 30:
				case 33:
				case 34:
				case 36:
				case 37:
				case 38:
				case 39:
				case 40:
				case 41:
				case 43:
				case 44:
				case 46:
				case 49:
				case 54:
				case 55:
				case 56:
				case 59:
					battleManager.DisplayUnitFrontOfDialog();
					break;
				case 9:
				case 11:
				case 12:
				case 16:
				case 20:
				case 25:
				case 26:
				case 27:
				case 28:
				case 29:
				case 31:
				case 32:
				case 35:
				case 47:
				case 48:
				case 50:
				case 51:
				case 52:
				case 53:
				case 57:
				case 58:
				case 66:
					if (battleManager.BattleResult == eBattleResult.WIN)
						battleManager.DisplayUnitFrontOfDialog();
					else
						battleManager.DisplayEnemyFrontOfDialog();
					break;
				case 15:
				case 60:
					if (battleManager.BattleResult == eBattleResult.WIN) battleManager.DisplayUnitFrontOfDialog();
					break;
			}
			_callback.Call();
		}

		private IEnumerator delayedExec(float _delay, Action _callback)
		{
			yield return new WaitForSeconds(_delay);
			_callback.Call();
		}

		public void PlayHXUIAnimation(DialogFlatoutPlayer.FlatoutType flatout, Action _callback = null)
		{
			throw new global::OpenPCR.OutOfBoundaryException("ViewBattle.PlayHXUIAnimation: scene animation completion producer; preserve callback instead of invoking it immediately.");
		}

		[global::OpenPCR.Migration("dfb252a6-c5c5-47c2-9f9e-d2422150fd8c")]
		public void OpenResultWin(eBattleCategory _category)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.ViewBattle::OpenResultWin(Elements.eBattleCategory):System.Void: periphery timing member; behavior is supplied by the wiring phase");
		}

		private void openResultFailed(eBattleCategory _battleCategory)
		{
			switch (_battleCategory)
			{
				case eBattleCategory.CLAN_BATTLE:
					dialogManager.OpenQuestFailed();
					return;
				case eBattleCategory.TRIAL_BATTLE:
				case eBattleCategory.TRIAL_CLAN_BATTLE:
					dialogManager.OpenTrialBattleResult(this);
					return;
				default:
					throw new global::OpenPCR.OutOfBoundaryException("ViewBattle.openResultFailed: other mode-specific result scene production outside clan/training scope.");
			}
		}
	}
}
