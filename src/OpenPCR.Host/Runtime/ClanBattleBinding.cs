using System;
using System.IO;
using System.Runtime.CompilerServices;
using Elements;

namespace OpenPCR.Host;

/// <summary>Prepared lobby/account inputs consumed by the existing clan processor.</summary>
internal sealed class ClanBattleBinding : IDisposable
{
	public ClanBattleTempData Temp { get; }
	internal ClanBattleBinding(BattleClan input, UserData account)
	{
		if (Singleton<ClanBattleTempData>.Instance is not null) throw new InvalidOperationException("Clan battle inputs are already owned.");
		Temp = (ClanBattleTempData)RuntimeHelpers.GetUninitializedObject(typeof(ClanBattleTempData));
		Temp.SupportUnitId = 0;
		Temp.SupportOwnerId = 0;
		Temp.IsRehearsal = input.IsRehearsal;
		Temp.IsTrainingBattle = input.IsTrainingBattle;
		Temp.BattleId = input.BattleId;
		Temp.BattleLapNum = input.LapNum;
		Temp.BattlePhase = input.Phase;
		Temp.BossOrderNum = input.OrderNum;
		Temp.BattleClanBattleMode = input.Mode;
		Temp.BattleTrainingId = input.TrainingId;
		Temp.IsActualBossStatus = input.ActualBossStatus;
		Temp.SelectedStockIndex = input.StockIndex;
		var top = (ClanBattleTopInfo)RuntimeHelpers.GetUninitializedObject(typeof(ClanBattleTopInfo));
		top.ClanBattleMode = input.TopMode;
		Temp.ClanBattleTopInfo = top;
		var clan = (UserClan)RuntimeHelpers.GetUninitializedObject(typeof(UserClan));
		RuntimeBinding.Set(clan, "ClanId", (global::System.Int32)input.ClanId);
		RuntimeBinding.Set(account, "UserClan", clan);
		RuntimeBinding.Instance(typeof(Singleton<>), typeof(ClanBattleTempData), Temp);
	}
	public void Dispose()
	{
		if (ReferenceEquals(Singleton<ClanBattleTempData>.Instance, Temp)) Temp.DestroyInstance();
	}
}
