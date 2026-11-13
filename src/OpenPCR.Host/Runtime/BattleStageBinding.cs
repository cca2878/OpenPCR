using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Elements;
using Elements.Data;

namespace OpenPCR.Host;

/// <summary>Prepared account/stage inputs; combat state and frame clocks are not imported.</summary>
internal sealed class BattleStageBinding : IDisposable
{
	public TempData Temp { get; }
	public TempData.BattleStage Stage => Temp.BattleStageData;
	internal BattleStageBinding(BattleScene input, IReadOnlyList<UnitParameter> enemies, PreparedTeamGrowth growth)
	{
		if (Singleton<TempData>.Instance is not null) throw new InvalidOperationException("A battle stage is already active.");
		Temp = (TempData)RuntimeHelpers.GetUninitializedObject(typeof(TempData));
		Temp.BattleStageData = new TempData.BattleStage
		{
			CurrentBattleCategory = (eBattleCategory)input.Category,
			BattleTimeLimit = input.TimeLimit,
			BattleLogId = input.BattleLogId,
			Seed = input.Seed,
			CurrentBattleWaves = new List<List<UnitParameter>> { enemies.ToList() },
			CurrentBattleBackgrounds = input.Backgrounds.ToArray(),
			UnitTeamEnhanceInfo = growth.Unit,
			EnemyTeamEnhanceInfo = growth.Enemy
		};
		if (Stage.CurrentBattleCategory == eBattleCategory.CLAN_BATTLE)
		{
			var inputBoss = input.Clan!;
			var boss = (BossInfo)RuntimeHelpers.GetUninitializedObject(typeof(BossInfo));
			RuntimeBinding.Set(boss, "MaxHp", (global::System.Int64)inputBoss.MaxHp);
			RuntimeBinding.Set(boss, "CurrentHp", (global::System.Int64)inputBoss.CurrentHp);
			Stage.CurrentClanBattleBossInfo = boss;
		}
		RuntimeBinding.Set(Temp, "unitDamageInfoList", new List<UnitDamageInfo>());
		RuntimeBinding.Set(Temp, "enemyDamageInfoList", new List<UnitDamageInfo>());
		RuntimeBinding.Set(Temp, "enemyColorIdList", new List<int>());
		RuntimeBinding.Set(Temp, "unitUnionBurstTimelineList", new List<UnitUnionBurstTimeline>());
		RuntimeBinding.Instance(typeof(Singleton<>), typeof(TempData), Temp);
	}
	public void Dispose()
	{
		if (ReferenceEquals(Singleton<TempData>.Instance, Temp)) Temp.DestroyInstance();
	}
}
