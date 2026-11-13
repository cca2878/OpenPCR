using Elements.Data;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using global::OpenPCR.Foundation;

namespace Elements
{
	public class ApiManager : ManagerSingleton<ApiManager>
	{
		[global::OpenPCR.Migration("461ea41d-ddfa-4296-a028-9dda05815dca")]
		public void AddLabyrinthBossBattleFinishPostParam(int _enterId, int _blockId, int _questId, List<TowerWaveResultInfo> _waveResultList, List<DungeonQueryUnit> _versusUserList, int _autoClear, Action<LabyrinthBossBattleFinishReceiveParam> _callback = null)
		{
			global::OpenPCR.PreparedBattleResultOutput.CaptureLabyrinthEx(this, new global::OpenPCR.PreparedBattleResultOutput.LabyrinthExFinish(_enterId, _blockId, _questId, _waveResultList.ToArray(), _versusUserList.ToArray(), _autoClear), _callback);
		}

		[global::OpenPCR.Migration("94767cbd-4937-40d0-a24a-56c3f792b188")]
		public void AddLabyrinthBattleFinishPostParam(int _enterId, int _blockId, int _questId, int _remainTime, List<UnitHpInfo> _unitHpList, int _autoClear, Action<LabyrinthBattleFinishReceiveParam> _callback = null)
		{
			global::OpenPCR.PreparedBattleResultOutput.CaptureLabyrinth(this, new global::OpenPCR.PreparedBattleResultOutput.LabyrinthFinish(_enterId, _blockId, _questId, _remainTime, _unitHpList.ToArray(), _autoClear), _callback);
		}

		[global::OpenPCR.ImplicitMigration("c387be95-81ed-44cb-841d-b62241aadc09")]
		public int TaskCount => throw new global::OpenPCR.OutOfBoundaryException("Elements.ApiManager::TaskCount");

		public ApiManager()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.ApiManager constructor: Native initializes network task queue and DateTime.Now throttle state before singleton base; network bootstrap producer outside static battle host");
		}

		[global::OpenPCR.Migration("629eebb5-d47e-4fb2-964a-c883717d128e")]
		public void Exec(Action _finishConnectApi, Action<int> _setCallbackOnResultCode = null, bool _disableBlockConnect = false)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.ApiManager::Exec(System.Action, System.Action<int>, bool)");
		}
		[global::OpenPCR.Migration("1747371a-e0ee-4274-bb9e-a69593c0e97e")]
		public void AddClanBattleFinishPostParam(System.Int32 _clanId, System.Int32 _clanBattleId, System.Int32 _lapNum, System.Int32 _phase, System.Int32 _orderNum, Elements.ClanBattleFinishUnit _userUnit, System.Int64 _bossHp, System.Int64 _bossDamage, System.Int32 _remainTime, System.Int64 _totalDamage, System.Int32 _battleLogId, System.String _battleLog, System.Int32 _isAuto, List<Elements.UnitUnionBurstTimeline> _timeline, System.Int32 _battleTime, System.Int32 _startRemainTime, Action<Elements.ClanBattleFinishReceiveParam> _callback = null)
		{
			global::OpenPCR.PreparedBattleResultOutput.CaptureClan(this, new global::OpenPCR.PreparedBattleResultOutput.ClanBattleFinish(_clanId, _clanBattleId, _lapNum, _phase, _orderNum, _battleLogId, _userUnit, _bossHp, _bossDamage, _remainTime, _totalDamage, _battleLog, _isAuto, _timeline.ToArray(), _battleTime, _startRemainTime), _callback);
		}

		[global::OpenPCR.Migration("d6c7c6ee-d33e-41b3-957f-d9ec2c78dd87")]
		public void AddClanBattleRehearsalFinishPostParam(System.Int32 _clanId, System.Int32 _clanBattleId, System.Int32 _lapNum, System.Int32 _phase, System.Int32 _orderNum, Elements.ClanBattleFinishUnit _userUnit, System.Int64 _bossHp, System.Int64 _bossDamage, System.Int32 _remainTime, System.Int64 _totalDamage, System.Int32 _battleLogId, System.Int32 _isActualBossStatus, List<Elements.UnitUnionBurstTimeline> _timeline, System.Int32 _battleTime, System.Int32 _startRemainTime, System.Int32 _isAuto, System.Int32 _stockIndex, Action<Elements.ClanBattleRehearsalFinishReceiveParam> _callback = null)
		{
			global::OpenPCR.PreparedBattleResultOutput.CaptureClan(this, new global::OpenPCR.PreparedBattleResultOutput.ClanRehearsalFinish(_clanId, _clanBattleId, _lapNum, _phase, _orderNum, _battleLogId, _userUnit, _bossHp, _bossDamage, _remainTime, _totalDamage, _isActualBossStatus, _isAuto, _stockIndex, _timeline.ToArray(), _battleTime, _startRemainTime), _callback);
		}

		[global::OpenPCR.Migration("f607dfc3-f9c3-4822-96b0-604dfc1e8317")]
		public void AddClanBattleTrainingFinishPostParam(System.Int32 _clanId, System.Int32 _trainingId, System.Int32 _clanBattleMode, System.Int32 _phase, System.Int32 _orderNum, Elements.ClanBattleFinishUnit _userUnit, System.Int64 _bossHp, System.Int64 _bossDamage, System.Int32 _remainTime, System.Int64 _totalDamage, System.Int32 _battleLogId, System.Int32 _isAuto, List<Elements.UnitUnionBurstTimeline> _timeline, System.Int32 _battleTime, System.Int32 _startRemainTime, Action<Elements.ClanBattleTrainingFinishReceiveParam> _callback = null)
		{
			global::OpenPCR.PreparedBattleResultOutput.CaptureClan(this, new global::OpenPCR.PreparedBattleResultOutput.ClanTrainingFinish(_clanId, _trainingId, _clanBattleMode, _phase, _orderNum, _battleLogId, _userUnit, _bossHp, _bossDamage, _remainTime, _totalDamage, _isAuto, _timeline.ToArray(), _battleTime, _startRemainTime), _callback);
		}

		[global::OpenPCR.Migration("dc214440-6aa1-43ea-a1e0-f335774cbf34")]
		public void AddTrialBattleTopPostParam(Action<Elements.TrialBattleTopReceiveParam> _callback = null)
		{
			if (_callback != null) throw new global::OpenPCR.OutOfBoundaryException("Offline refresh output does not produce API responses.");
			global::OpenPCR.PreparedBattleResultOutput.RequestRefresh(this);
		}

		[global::OpenPCR.Migration("47059dc0-131e-48b9-b5a4-ce5a3abc4f4e")]
		public void AddTrialClanBattleFinishPostParam(System.Int32 _clanBattleId, System.Int32 _phase, System.Int32 _orderNum, System.Int32 _battleLogId, Elements.TrialBattleFinishUnit _battleFinishUnit, System.Int64 _totalDamage, Action<Elements.TrialClanBattleFinishReceiveParam> _callback = null)
		{
			if (_callback != null) throw new global::OpenPCR.OutOfBoundaryException("Offline finish output does not produce API responses.");
			global::OpenPCR.PreparedBattleResultOutput.Capture(this, _clanBattleId, _phase, _orderNum, _battleLogId, _battleFinishUnit, _totalDamage);
		}

		protected override void onInit()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.ApiManager::onInit():System.Void: runtime capability is unavailable.");
		}

	}
}
