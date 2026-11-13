using Common;
using global::OpenPCR.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Elements.Data;
using global::OpenPCR.Runtime.Api;

namespace Elements
{
	public class TutorialManager : ManagerSingleton<TutorialManager>
	{

		private static TutorialDefine.eTutorial currentTutorial;

		public static bool IsActive
		{
			[global::OpenPCR.Migration("cc9e6ab2-b37c-45f3-b8be-30d633866485")]
			get
			{
				if (ManagerSingleton<TutorialManager>.Instance == null) return false;
				if (!ManagerSingleton<TutorialManager>.Instance.gameObject.activeSelf) return false;
				return currentTutorial != TutorialDefine.eTutorial.NONE;
			}
		}

		private static bool isTutorialExec(TutorialDefine.eTutorial _tutorialId)
		{
			if (ManagerSingleton<TutorialManager>.Instance == null) return false;
			if (!ManagerSingleton<TutorialManager>.Instance.gameObject.activeSelf) return false;
			return currentTutorial == _tutorialId;
		}

		[global::OpenPCR.ImplicitMigration("4c013a5f-aca9-4719-9d75-c47ffbd8764b")]
		public static bool IsStartTutorial => isTutorialExec(TutorialDefine.eTutorial.GAME_START);

		public TutorialManager()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.TutorialManager::.ctor()", "body migration pending");
		}
		protected override void onInit() => global::OpenPCR.MigrationPending.Throw("Elements.TutorialManager.onInit()", "behavior migration pending");

	}
}
