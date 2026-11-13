using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;
namespace Elements.Battle
{
	public class BonusAppearEffect : MonoBehaviour
	{
		public BonusAppearEffect()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Scene-owned bonus animation production; its callback controls the battle pause gate.");
		}
		[global::OpenPCR.Migration("0f319684-ede8-4355-a1df-ca96ae6c06f9")]
		public void Play(Dictionary<BattleManager.eBonusType, List<BattleManager.BonusData>> _data, Action _callback, List<UnitCtrl> _unitList, List<UnitCtrl> _enemyList)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Bonus appearance animation/callback producer is unresolved; completion must not be invoked synchronously.");
		}
	}
}
