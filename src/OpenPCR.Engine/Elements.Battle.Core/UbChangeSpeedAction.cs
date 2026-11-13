using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using Elements.Battle;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class UbChangeSpeedAction : ActionParameter
	{
		private enum eChangeSpeedType
		{
			SLOW = 1,
			HASTE,
			PARALYSIS,
			FREEZE,
			CHAINED,
			SLEEP,
			STUN,
			STONE,
			FAINT
		}

		private class eChangeSpeedType_DictComparer : IEqualityComparer<eChangeSpeedType>
		{
			public bool Equals(eChangeSpeedType _x, eChangeSpeedType _y)
			{
				return _x == _y;
			}

			public int GetHashCode(eChangeSpeedType _obj)
			{
				return (int)_obj;
			}
		}

		private static readonly Dictionary<eChangeSpeedType, eAbnormalState> abnormalStateDic = new Dictionary<eChangeSpeedType, eAbnormalState>(new eChangeSpeedType_DictComparer())
		{
			{
				eChangeSpeedType.SLOW,
				eAbnormalState.SLOW
			},
			{
				eChangeSpeedType.HASTE,
				eAbnormalState.HASTE
			},
			{
				eChangeSpeedType.PARALYSIS,
				eAbnormalState.PARALYSIS
			},
			{
				eChangeSpeedType.FREEZE,
				eAbnormalState.FREEZE
			},
			{
				eChangeSpeedType.CHAINED,
				eAbnormalState.CHAINED
			},
			{
				eChangeSpeedType.SLEEP,
				eAbnormalState.SLEEP
			},
			{
				eChangeSpeedType.STUN,
				eAbnormalState.STUN
			},
			{
				eChangeSpeedType.STONE,
				eAbnormalState.STONE
			},
			{
				eChangeSpeedType.FAINT,
				eAbnormalState.FAINT
			}
		};
		private Dictionary<eChangeSpeedType, GameObject> effectDictionary { get; set; }

		[global::OpenPCR.Migration("bdbad58f-60a0-49dd-b6df-b49e498dcbc0")]
		public UbChangeSpeedAction() : base()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.UbChangeSpeedAction::.ctor (action_type 41: no first-phase loader)"); }
	}
}
