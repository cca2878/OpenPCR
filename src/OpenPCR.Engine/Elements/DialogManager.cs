namespace Elements
{
	[global::System.Serializable]
	public class DialogManager : global::Elements.ManagerSingleton<DialogManager>
	{

		[global::OpenPCR.Migration("0882680d-a79d-4581-b6c4-486fbd30672e")]
		public void OpenFlatoutPlayer(global::Elements.DialogFlatoutPlayer.FlatoutType _type, global::System.Action _closeCallback, int _unitId = 0, int _faceId = 0, global::System.Action _showCallback = null)
		{
			if (_type != DialogFlatoutPlayer.FlatoutType.TIME_UP || _unitId != 0 || _faceId != 0 || _showCallback != null)
				throw new global::OpenPCR.OutOfBoundaryException("Prepared terminal dialog supports only the TIME_UP caller without unit/face/show output.");
			global::OpenPCR.PreparedTerminalDialog.CloseTimeUp(this, _closeCallback);
		}


		public void OpenBossResult(ViewBattle _viewBattle)
		{
			throw new global::OpenPCR.OutOfBoundaryException("DialogManager.OpenBossResult: Native Open(BOSS_RESULT), Initialize(view), Show scene production; lifecycle effects have not been excluded as pure output.");
		}
		public void OpenTrialBattleResult(ViewBattle _viewBattle)
		{
			throw new global::OpenPCR.OutOfBoundaryException("DialogManager.OpenTrialBattleResult: Native Open(TRIAL_BATTLE_RESULT), Initialize(view), Show scene production; no fabricated completion.");
		}
		public void OpenQuestFailed()
		{
			throw new global::OpenPCR.OutOfBoundaryException("DialogManager.OpenQuestFailed: Native Open(QUEST_FAILED), Initialize, Show scene production; lifecycle effects have not been excluded as pure output.");
		}

		private global::Elements.Battle.IBattleManagerForDialogManager battleManager;

		[global::OpenPCR.Migration("1f7af3f5-e3f2-4b74-8119-3d29ce3d7c8a")]
		public void SetBattleManager(global::Elements.Battle.IBattleManagerForDialogManager _battleManager)
		{
			battleManager = _battleManager;
		}

		[global::OpenPCR.Migration("9ae34832-7069-47ae-a9d5-dba1a1914781")]
		public void ReleaseBattleManager() => battleManager = null;


		[global::OpenPCR.Migration("8a51d3f4-9b85-4f3f-80f4-522df1960b93")]
		public global::Elements.DialogFlatoutPlayer OpenFlatoutPlayer(global::Elements.DialogFlatoutPlayer.FlatoutType _type, int _sortingOrder, int _unitId = 0, int _faceId = 0, int _sortOffset = 0, float _resizeScale = 1f)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.DialogManager::OpenFlatoutPlayer(Elements.DialogFlatoutPlayer+FlatoutType,System.Int32,System.Int32,System.Int32,System.Int32,System.Single):Elements.DialogFlatoutPlayer: periphery timing member; behavior is supplied by the wiring phase");
		}

		[global::OpenPCR.Migration("88ed24b6-7054-4038-b486-0f0aaf3ef68a")]
		public global::Elements.DialogFlatoutPlayer OpenFlatoutPlayer(global::Elements.DialogFlatoutPlayer.FlatoutType _type, int _unitId = 0, int _faceId = 0, int _sortOffset = 0, float _resizeScale = 1f)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.DialogManager::OpenFlatoutPlayer(Elements.DialogFlatoutPlayer+FlatoutType,System.Int32,System.Int32,System.Int32,System.Single):Elements.DialogFlatoutPlayer: periphery timing member; behavior is supplied by the wiring phase");
		}

		public DialogManager()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.DialogManager::.ctor():System.Void", "Native consumer declaration; behavior evidence pending");
		}

		protected override void onInit()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.DialogManager::onInit():System.Void", "Native consumer declaration; behavior evidence pending");
		}

		[global::OpenPCR.Migration("414b08b8-f136-436d-8f77-74f1c876f915")]
		public bool IsUse(global::Elements.eDialogId _dialogId)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.DialogManager::IsUse(Elements.eDialogId):System.Boolean: periphery input member; behavior is supplied by the wiring phase");
		}

		[global::OpenPCR.Migration("3a22843c-5efa-487e-a1f3-d9e9a3198e0b")]
		public bool ForceCloseOne(global::Elements.eDialogId _dialogId, bool _isQuickHide = false)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.DialogManager::ForceCloseOne(Elements.eDialogId,System.Boolean):System.Boolean: periphery input member; behavior is supplied by the wiring phase");
		}
	}
}
