using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Elements;
using OpenPCR;

internal static class ClanOutputTests
{
	internal static void Run(Action<bool, string> check)
	{
		var owner = (ApiManager)RuntimeHelpers.GetUninitializedObject(typeof(ApiManager));
		int callbacks = 0;
		var units = new ClanBattleFinishUnit();
		var timeline = new List<UnitUnionBurstTimeline> { new UnitUnionBurstTimeline() };
		void Reject(Action action)
		{
			try { action(); }
			catch (InvalidOperationException) { check(true, "Rejected invalid finish sequence."); return; }
			throw new Exception("Invalid finish sequence was accepted.");
		}
		Action formal = () => owner.AddClanBattleFinishPostParam(11, 12, 13, 14, 15, units, 16, 17, 18, 19, 20, "original csv", 21, timeline, 22, 23, _ => callbacks++);
		Reject(formal);
		PreparedBattleResultOutput.Bind(owner);
		try
		{
			Reject(() => PreparedBattleResultOutput.Complete(owner, 9));
			formal();
			Reject(formal);
			Reject(() => PreparedBattleResultOutput.RequestRefresh(owner));
			Reject(() => PreparedBattleResultOutput.Complete(owner, 32));
			check(PreparedBattleResultOutput.Read(owner) is null, "Unsealed finish became observable.");
			timeline.Clear();
			PreparedBattleResultOutput.Complete(owner, 9);
			var finish = (PreparedBattleResultOutput.ClanBattleFinish)PreparedBattleResultOutput.Read(owner)!;
			check(finish.ClanId == 11 && finish.ClanBattleId == 12 && finish.LapNum == 13 && finish.Phase == 14 && finish.OrderNum == 15 && ReferenceEquals(finish.Units, units), "Formal clan identity or original unit payload changed.");
			check(finish.BossHp == 16 && finish.BossDamage == 17 && finish.RemainTime == 18 && finish.TotalDamage == 19 && finish.BattleLogId == 20 && finish.BattleLog == "original csv" && finish.IsAuto == 21 && finish.BattleTime == 22 && finish.StartRemainTime == 23, "Formal finish arguments changed.");
			check(((UnitUnionBurstTimeline[])finish.Timeline).Length == 1 && callbacks == 0, "Timeline was not copied or an API response was fabricated.");
			Reject(() => PreparedBattleResultOutput.Complete(owner, 9));
		}
		finally { PreparedBattleResultOutput.Release(owner); }
		check(PreparedBattleResultOutput.Read(owner) is null, "Released finish remains bound.");
		PreparedBattleResultOutput.Bind(owner);
		try
		{
			owner.AddClanBattleRehearsalFinishPostParam(31, 32, 33, 34, 35, units, 36, 37, 38, 39, 40, 41, timeline, 42, 43, 44, 45, _ => callbacks++);
			PreparedBattleResultOutput.Complete(owner, 9);
			var finish = (PreparedBattleResultOutput.ClanRehearsalFinish)PreparedBattleResultOutput.Read(owner)!;
			check(finish.ClanId == 31 && finish.ClanBattleId == 32 && finish.LapNum == 33 && finish.Phase == 34 && finish.OrderNum == 35 && ReferenceEquals(finish.Units, units), "Rehearsal identity or original unit payload changed.");
			check(finish.BossHp == 36 && finish.BossDamage == 37 && finish.RemainTime == 38 && finish.TotalDamage == 39 && finish.BattleLogId == 40 && finish.IsActualBossStatus == 41 && finish.BattleTime == 42 && finish.StartRemainTime == 43 && finish.IsAuto == 44 && finish.StockIndex == 45, "Rehearsal finish arguments changed.");
		}
		finally { PreparedBattleResultOutput.Release(owner); }
		PreparedBattleResultOutput.Bind(owner);
		try
		{
			owner.AddClanBattleTrainingFinishPostParam(51, 52, 53, 54, 55, units, 56, 57, 58, 59, 60, 61, timeline, 62, 63, _ => callbacks++);
			PreparedBattleResultOutput.Complete(owner, 9);
			var finish = (PreparedBattleResultOutput.ClanTrainingFinish)PreparedBattleResultOutput.Read(owner)!;
			check(finish.ClanId == 51 && finish.TrainingId == 52 && finish.ClanBattleMode == 53 && finish.Phase == 54 && finish.OrderNum == 55 && ReferenceEquals(finish.Units, units), "Training identity or original unit payload changed.");
			check(finish.BossHp == 56 && finish.BossDamage == 57 && finish.RemainTime == 58 && finish.TotalDamage == 59 && finish.BattleLogId == 60 && finish.IsAuto == 61 && finish.BattleTime == 62 && finish.StartRemainTime == 63, "Training finish arguments changed.");
		}
		finally { PreparedBattleResultOutput.Release(owner); }
		PreparedBattleResultOutput.Bind(owner);
		try
		{
			PreparedBattleResultOutput.Capture(owner, 1, 2, 3, 4, new object(), 5);
			Reject(() => PreparedBattleResultOutput.Complete(owner, 32));
			PreparedBattleResultOutput.RequestRefresh(owner);
			PreparedBattleResultOutput.Complete(owner, 32);
			check(PreparedBattleResultOutput.Read(owner) is PreparedBattleResultOutput.TrialFinish && callbacks == 0, "Trial protocol or callback boundary changed.");
		}
		finally { PreparedBattleResultOutput.Release(owner); }
	}
}
