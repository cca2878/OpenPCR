using global::Elements;
using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;
using global::OpenPCR.AnimKit.Scene;

namespace Elements
{
	public class AnimKitManager : ManagerSingleton<AnimKitManager>
	{
		public AnimKitManager()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.AnimKitManager::.ctor()", "body migration pending");
		}

		public static int SelectResourceIndexFromUnitIdWeaponId(eAnimKitBinaryAnimationId _binaryAnimationId, int _unitId, int _motionType)
		{
			int result = -1;
			switch (_binaryAnimationId)
			{
				case eAnimKitBinaryAnimationId.COMMON_BATTLE:
					if (_motionType == 0)
					{
						if (!UnitUtility.IsEnemyUnit(_unitId))
						{
							result = _unitId;
						}
					}
					else
					{
						result = _motionType;
					}

					break;
				case eAnimKitBinaryAnimationId.COMMON_ROOM:
				case eAnimKitBinaryAnimationId.UNIQUE_ROOM:
				case eAnimKitBinaryAnimationId.EKD:
				case eAnimKitBinaryAnimationId.OMP:
				case eAnimKitBinaryAnimationId.UEK:
				case eAnimKitBinaryAnimationId.NYX:
				case eAnimKitBinaryAnimationId.PKB:
				case eAnimKitBinaryAnimationId.CGG:
				case eAnimKitBinaryAnimationId.MHP:
				case eAnimKitBinaryAnimationId.NOP:
				case eAnimKitBinaryAnimationId.BLB_DRAMA:
				case eAnimKitBinaryAnimationId.WAC:
				case eAnimKitBinaryAnimationId.LSV:
				case eAnimKitBinaryAnimationId.DSB:
					result = ((_motionType != 0) ? _motionType : _unitId);
					break;
				case eAnimKitBinaryAnimationId.BATTLE:
				case eAnimKitBinaryAnimationId.ROOM_ITEM:
				case eAnimKitBinaryAnimationId.ROOM_ITEM_UNIQUE:
				case eAnimKitBinaryAnimationId.EVENT:
				case eAnimKitBinaryAnimationId.MAP:
				case eAnimKitBinaryAnimationId.MINIGAME:
				case eAnimKitBinaryAnimationId.TRAVEL_TOP_ROOM_UNIT:
				case eAnimKitBinaryAnimationId.TRAVEL_TOP_BATTLE_UNIT:
				case eAnimKitBinaryAnimationId.TRAVEL_TOP_DRAMA_ROOM_UNIT:
				case eAnimKitBinaryAnimationId.TRAVEL_TOP_DRAMA_BATTLE_UNIT:
				case eAnimKitBinaryAnimationId.TRAVEL_EX_DRAMA_ROOM_UNIT:
				case eAnimKitBinaryAnimationId.TRAVEL_EX_DRAMA_BATTLE_UNIT:
					result = _unitId;
					break;
				case eAnimKitBinaryAnimationId.POSING:
				case eAnimKitBinaryAnimationId.SMILE:
				case eAnimKitBinaryAnimationId.DEFEAT:
				case eAnimKitBinaryAnimationId.RUN_JUMP:
				case eAnimKitBinaryAnimationId.NO_WEAPON:
				case eAnimKitBinaryAnimationId.RACE:
				case eAnimKitBinaryAnimationId.DEAR:
					result = ((_motionType == 0) ? _unitId : 0);
					break;
				case eAnimKitBinaryAnimationId.LOADING:
					result = ((_unitId != 900000 && _motionType != 0) ? _motionType : _unitId);
					break;
			}

			return result;
		}
		protected override void onInit() => global::OpenPCR.MigrationPending.Throw("Elements.AnimKitManager::onInit():System.Void", "behavior migration pending");

	}
}
