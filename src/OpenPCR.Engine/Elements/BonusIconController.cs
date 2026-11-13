using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Elements.Battle;

namespace Elements
{
	public class BonusIconController : MonoBehaviour, ISingletonField
	{

		[global::OpenPCR.Migration("c32b7733-2d01-4c71-bcce-a1d29d582e69")]
		public void DeleteStatus(int _index)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.BonusIconController::DeleteStatus(System.Int32):System.Void: periphery output member; behavior is supplied by the wiring phase");
		}
		private static Yggdrasil<BonusIconController> staticSingletonTree;
		private static IBattleManagerForAbromalStateIconController staticBattleManager;

		public BonusIconController()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.BonusIconController::.ctor()", "body migration pending");
		}

		[global::OpenPCR.Migration("2e01f9a6-129e-486d-8f15-d9eb7cf964d2")]
		public static void StaticRelease()
		{
			staticSingletonTree = null;
			staticBattleManager = null;
		}
	}
}
