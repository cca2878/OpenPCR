using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System.Linq;
using Elements.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using global::OpenPCR.Foundation;
using global::OpenPCR.Foundation.Audio;

namespace Elements
{
	public class ViewManager : ManagerSingleton<ViewManager>
	{

		[SerializeField]
		private UIRoot curUIRoot;
		private float activeScreenWidth;

		public float ActiveScreenWidth { get => activeScreenWidth; private set => activeScreenWidth = value; }

		public float ActiveScreenHeight => throw new global::OpenPCR.OutOfBoundaryException("Elements.ViewManager::get_ActiveScreenHeight():System.Single: runtime capability is unavailable.");
		[SerializeField]
		public PartsBattleSkipAnimation PartsBattleSkipAnime;
		public bool IsSnsBattlePause;
		[global::OpenPCR.ImplicitMigration("94f7d7b2-5a1b-4082-95f0-416829773ef3")]
		public UIRoot CurUIRoot => curUIRoot;

		public eViewId NextViewId { get; private set; }
		public eViewId BeforeViewId { get; private set; }

		[global::OpenPCR.ImplicitMigration("1d4012ac-c8a9-4740-b616-5bbf9bcb7f0d")]
		public float ActiveScreenWidthInUIRoot => throw new global::OpenPCR.OutOfBoundaryException("Elements.ViewManager::ActiveScreenWidthInUIRoot");
		[global::OpenPCR.ImplicitMigration("00c41e8e-3677-46e5-86bf-e967b074bb50")]
		public float ActiveScreenHeightInUIRoot => throw new global::OpenPCR.OutOfBoundaryException("Elements.ViewManager::ActiveScreenHeightInUIRoot");

		public ViewManager()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.ViewManager::.ctor()", "body migration pending");
		}

		[global::OpenPCR.Migration("dd87b6e9-11eb-49f9-94a3-886db1cc7ff8")]
		public void ChangeOrReturnToView(eViewId _nextViewId)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.ViewManager::ChangeOrReturnToView(Elements.eViewId)");
		}

		public void OpenNoticeWindow(eTextId _setTextId, params object[] _args)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.ViewManager::OpenNoticeWindow(Elements.eTextId, params object[])");
		}
		protected override void onInit() => global::OpenPCR.MigrationPending.Throw("Elements.ViewManager.onInit()", "behavior migration pending");

	}
}
