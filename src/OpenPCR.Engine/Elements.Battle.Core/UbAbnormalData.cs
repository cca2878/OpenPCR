using Elements.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class UbAbnormalData : ISingletonField
	{
		public double Value { get; set; }

		private static Yggdrasil<UbAbnormalData> staticSingletonTree;
		private static IBattleManagerForUbAbnormalData staticBattleManager;
		[global::OpenPCR.Migration("21385cee-8e14-40f7-ab0e-2e9abf4fcd49")]
		private IBattleManagerForUbAbnormalData battleManager
		{
			[global::OpenPCR.Migration("688e4606-fd6f-4b85-914e-d33548d98b79")]
			get
			{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.UbAbnormalData::get_battleManager (reachable only through Excluded UbChangeSpeedAction: no first-phase loader)"); }
		}

		[global::OpenPCR.Migration("fc1dac9e-1d8d-4507-97d9-ec37b9be5495")]
		public UbAbnormalData()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.UbAbnormalData::.ctor (reachable only through Excluded UbChangeSpeedAction: no first-phase loader)"); }

		[global::OpenPCR.Migration("f793f933-cf50-47d2-9509-6ea4e0462cc5")]
		public static void StaticRelease()
		{
			staticSingletonTree = null;
			staticBattleManager = null;
		}

		[global::OpenPCR.Migration("830e26b2-59bc-45dc-ae6e-2ee51676711a")]
		public void Exec(UnitCtrl _target)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.UbAbnormalData::Exec (reachable only through Excluded UbChangeSpeedAction: no first-phase loader)"); }
	}
}
