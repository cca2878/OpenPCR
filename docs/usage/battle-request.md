# 战斗请求与结果

所有战斗模拟使用同一份请求 `openpcr-battle/v2`，结果为 `openpcr-battle-result/v2`。一份请求描述一个战斗序列：会战或训练场是一场，黎明界普通关卡是一场，黎明界 EX 是至多三队的整场挑战。

请求由己方 `Player`、设备设置 `Settings`、战斗列表 `Battles` 和恰好一个玩法段组成：`ClanBattle` 或 `Labyrinth`。竞技场与公主竞技场未开放。

字段名参考游戏服务器响应，调用方可以把抓到的响应对象直接放进对应位置。OpenPCR 只声明模拟会读取的字段，照搬对象里的其它字段被忽略；下文列出每段的最小必需子集。只出现在结算请求里的值（`battle_log_id`、`viewer_id`、`enter_id` 等）不是输入。

场景只接受游戏能合法建出的：敌人、时限和背景由玩法段的 ID 从 masterdata 推导，不能在请求里覆盖。

## 结构

```jsonc
{
  "Schema": "openpcr-battle/v2",
  "MasterVersion": "202611031924",          // 十二位资源版本，对应 <resources>/masterdata/<version>_unhashed.db
  "Player":   { },                          // 己方：养成与账号开关
  "Settings": { },                          // 设备本地设置，整份请求共用
  "Battles":  [ { "Party": [100101, 0, 0, 0, 0], "Opening": { "Auto": false, "Speed": 1, "SetUnits": [] }, "Seed": 12345 } ],
  "ClanBattle": { }                         // 或 "Labyrinth": { }
}
```

服务器形状的对象（`Player` 内部、`ClanBattle`、`Labyrinth` 及其成员）使用服务器的 snake_case 名并忽略未声明字段；OpenPCR 自定义的对象（根、`Settings`、`Battles[i]`、`Opening`、`Releases`）使用 PascalCase，拒绝未知字段。任意层的重复键都被拒绝。完整示例见 [examples/battles](../../examples/battles/)。

## Player

| 字段 | 必需 | 来源 | 说明 |
| --- | --- | --- | --- |
| `user_info.team_level` | 是 | `load/index` | 玩家等级 |
| `unit_list[]` | 是 | `load/index` | 出战用到的持有单位；只读原始养成字段，见下表 |
| `read_story_ids` | 是 | `load/index` | 已读剧情，决定角色剧情加成 |
| `unit_role_list` | 是 | `load/index` | 职能强化；可为空表 |
| `ini_setting.battle_log_type` | 是 | `load/index` | 日志类型开关，值为 1 开启，缺项为 0 |
| `ini_setting.log_barrier` | 是 | `load/index` | `coefficient`、`threshold`，对数盾参数 |
| `errm` | 是 | `load/index` | TP 回复上限（EnergyRecoveryRateMax） |
| `Releases` | 是 | 由调用方得出 | 四个布尔开关，见 [推导开关](#推导开关) |
| `princess_knight_info` | 否 | `load/index` | 天赋与队伍技能；省略时没有天赋加成 |
| `user_ex_equip[]` | 装了 EX 装备时 | `load/index` | 按 `serial_id` 解析单位 EX 装备槽 |

`unit_list[]` 的单位字段：

| 字段 | 必需 | 说明 |
| --- | --- | --- |
| `id`、`unit_rarity`、`unit_level`、`promotion_level` | 是 | |
| `union_burst`、`main_skill`、`ex_skill`、`free_skill` | 是 | 技能条目 `{skill_id, skill_level, slot_number}`，可为空表 |
| `equip_slot` | 是 | `{is_slot, id, enhancement_level, enhancement_pt, rank}`；`is_slot` 接受布尔或 0／1 |
| `battle_rarity`、`exceed_stage`、`resist_status_id`、`resist_variation_id` | 否 | 缺省为 0 |
| `unique_equip_slot`、`ex_equip_slot`、`cb_ex_equip_slot` | 否 | 缺省为空 |
| `skin_data`、`unlock_rarity_6_item` | 否 | 缺省为 null |

`unit_param`、`power`、总属性等服务器计算结果不读取；OpenPCR 从原始字段和 masterdata 重新计算。`bonus_param` 只对黎明界 NPC 和训练场支援有意义（见各玩法段）。

## 推导开关

几项 Native 条件在请求里写成布尔值。调用方按下表从接口值得出它们；OpenPCR 内部再构造 Native 读取的值，消费方式与客户端相同。

| 输入 | 调用方从接口得出 | 影响 |
| --- | --- | --- |
| `Releases.ChangeRarity` | `load/index` 的 `chr ≥ 1` 且服务器时间 ≥ `chr` | 5 星单位是否按非 0 的 `battle_rarity` 计算 |
| `Releases.SkillLevelSync` | `cbslf ≥ 1` 且服务器时间 ≥ `cbslf` | 会战限级时 UB 等级是否同步 |
| `Releases.SlipDamageLogBarrier` | `sdlb ≥ 1` 且服务器时间 ≥ `sdlb` | 普攻以外的伤害是否受对数盾约束 |
| `Releases.KnightEnhance` | `content_release_data` 中 system 129 的开放条件：当前为主线 2-6 通关，即 `home/index` 的 `quest_list` 里 11002006 的 `clear_flg > 0` | 天赋编成加成与天赋弱点倍率 |

服务器时间取响应 `data_headers.servertime`。三个开放时刻在内部写成“当前时刻 2、开放时刻 1（开放）或 3（未开放）”；`KnightEnhance` 为 true 时构造一条 11002006 的通关记录，否则通关表为空。system 129 还要求玩家等级不低于 `content_release_data.team_level`，`KnightEnhance` 与等级矛盾时报错。

其余由请求推导的值：

| 内部值 | 取值 |
| --- | --- |
| 日志开关 | `battle_log_type` 中值为 1 的类型开启，任一开启时日志功能开启 |
| 己方 viewer ID | 固定为 1，训练场支援的 owner 为 2；viewer ID 只用作归属键，不参与计算 |
| AUTO 与速度 | `Opening.Auto` 为开场 AUTO；`Speed` 为 1 时关闭加速，2 为加速，4 为加速并选择四倍速 |
| 四倍速许可 | 黎明界允许；会战按 `quad_speed`；训练场不允许 |
| 本场帧率 | `Speed = 4` 且允许四倍速时为 30，否则为 `Settings.FrameRate` |
| UB SET 模式 | 恒为开启（客户端的界面便利开关）；`Opening.SetUnits` 是开场 SET 的单位 |

## Settings

设备本地设置，没有对应的服务器字段。除 `PlayMultiTargetAppearance` 外均须显式提供。

| 字段 | 说明 |
| --- | --- |
| `FrameRate` | 设备的战斗帧率设置，30 或 60 |
| `ApplyOtherUserSkins` | 是否显示其他玩家角色的皮肤 |
| `ResetSkillReserveEveryTime` | 每场重置 SET 预约 |
| `AdvantageAndBreakdownBonusShows`、`MultiTargetFirst`、`TalentFormationBonusFirst` | 战斗设置页的对应开关 |
| `PlayMultiTargetAppearance` | 本场是否播放多目标登场动画，缺省为 false |

## Battles

| 字段 | 必需 | 说明 |
| --- | --- | --- |
| `Party` | 是 | 5 格单位 ID，0 为空位。会战填 `Player.unit_list` 中的单位，训练场支援格填 1；黎明界填 `Labyrinth.unit_list` 中的单位 |
| `Opening` | 是 | `{Auto, Speed, SetUnits}`：开场 AUTO、速度 1／2／4、开场开启 SET 的本队单位 |
| `Seed` | 是 | 本场 seed，取服务器开战响应下发的值（黎明界见 [字段来源](labyrinth.md#字段来源)） |
| `Operations` | 否 | 战斗中的玩家操作，仅会战／训练场；格式见 [Worker 协议](worker.md#运行操作和检查点) |
| `Opponent` | — | 预留给竞技场，须省略 |

至少一格非空（EX 的第 1、2 队可以全空，表示不出战）。

## ClanBattle

会战（类别 9）和训练场（类别 32）。

| 字段 | 必需 | 说明 |
| --- | --- | --- |
| `clan_battle_id`、`phase`、`order_num` | 是 | 场景：期次、阶段、BOSS 序号 1～5 |
| `kind` | 是 | `clan`（会战）或 `trial`（训练场） |
| `lap_num` | 否 | 仅 `clan`。按周目范围选地图行；省略时取该阶段 `lap_num_from` 最大的行。早期期次（1001～1010 第 1 阶段、1067 第 2 阶段）的场景随周目变化，模拟这些期次时填写 |
| `boss_hp` | 否 | 仅 `clan`。`{max_hp, current_hp}`，`0 < current_hp ≤ max_hp`；省略为满血 |
| `quad_speed` | 否 | 仅 `clan`。允许四倍速（演习且最高速度 4） |
| `infinity_hp` | 否 | 仅 `trial`。BOSS 不死模式 |
| `support` | 否 | 仅 `trial`。`{unit_id, unit_data}`，`unit_data` 取 `trial_battle/support_unit_list` 的 `support_unit_list[].unit_data`；`Party` 中以 1 标记支援格 |

由 masterdata 推导：

| 内容 | 取值 |
| --- | --- |
| 地图行 | `clan_battle_2_map_data` 中该期次、阶段的行，按上面的 `lap_num` 规则选取 |
| 敌人 | 地图行 `wave_group_id_<order_num>` → `wave_group_data.enemy_id_1..5`，去掉 0 |
| 时限 | `clan_battle_period` 中该期次 `period` 最小行的 `limit_time` |
| 背景 | 地图行 `boss_id_<order_num>` → `clan_battle_2_boss_data.background` |
| BOSS 特效难度 | 地图行的 `aura_effect` |

会战的正式战与模拟战在战斗中没有区别。单人会战模式只决定客户端读哪张地图表，请求统一按普通地图表建场。

## Labyrinth

黎明界的字段来源、NPC 与 EX 规则见 [黎明界模拟](labyrinth.md)。

## 结果

```jsonc
{
  "Schema": "openpcr-battle-result/v2",
  "Completed": true,                  // 整个序列已结束
  "Battles": [ { "Index": 0, "Battle": { }, "Settlement": { }, "Next": null } ],
  "Outcome": null                     // 多场玩法的汇总；黎明界 EX 为最终结算请求
}
```

`Battles[i].Battle` 是每场的公共结果：

| 字段 | 说明 |
| --- | --- |
| `Completed` | 本场的 Native 结束流程已完成 |
| `EngineFrame`、`BattleFrame`、`TimeRemaining` | 宿主步进次数、战斗帧、剩余时间（秒） |
| `Result` | `WIN`、`LOSE`、`TIME_OVER` 等 |
| `Units` | 首次启用 Update 时已建场单位的终态 `{UnitId, Enemy, Hp, MaxHp, Energy}`，不含之后出生的召唤单位 |
| `FrameRate`、`SystemId` | 本场帧率与处理器 ID（会战 107、训练场 909、黎明界 136），客户端随战斗日志上传的两个值 |
| `Csv` | 原始战斗日志 |
| `RandomState` | 读取时的 RNG 状态 |
| `ScriptExceptions` | 按发生顺序记录的游戏脚本异常；无异常时省略。处理规则见 [Core I/O](../reference/core-io.md#6-输出契约) |

`Settlement` 是本场结算，本场未完成时为 null：

| 玩法 | 内容 |
| --- | --- |
| ClanBattle | `ClanBattleId`、`Phase`、`OrderNum`、`TotalDamage`、`Hp[{UnitId, ViewerId, Hp}]`、`Damage[{UnitId, ViewerId, Damage, Rarity}]` |
| Labyrinth 普通关卡 | `openpcr-labyrinth-settlement/v2`：`QuestId`、`RemainTime`（毫秒）、`AutoClear`、`Hp` |
| Labyrinth EX | `openpcr-labyrinth-ex-settlement/v2`：本队的队号、是否最终队、敌我状态与可空的 `FinishRequest` |

`Next` 只用于 EX：下一队开始时的状态，可以作为 `Labyrinth.start_state` 从该队分支。某场用完帧预算时，该场 `Completed` 为 false、`Settlement` 为 null，序列停止。

结果中的 viewer ID 是内部归属键（己方 1、支援 2）。是否上传、如何组装服务器请求由调用方决定。

## 运行

| 方式 | 用法 |
| --- | --- |
| CLI | `OpenPCR.Runner run --resources <根> --request <请求> --max-engine-frames <N> [--csv-dir <目录>] [--clock <时钟>]`：stdout 为结果 JSON，stderr 为进度；`--csv-dir` 为每场写 `battle-<序号>.csv`。完成返回 0，输入或运行错误返回 1，帧预算耗尽返回 3 |
| Worker | `prepare`／`create`，见 [Worker 协议](worker.md) |
| C# | `BattleRequest.Read(path)` → `BattleRunner.Run(request, options)`；逐场控制用 `BattleSequence.Start(request, options)`；单场也可 `BattlePreparer.Prepare` 后 `BattleSession.Create(inputs, options, execution)` |

`--max-engine-frames` 是每场的宿主帧预算，不是战斗倒计时。

```csharp
var request = BattleRequest.Read("examples/battles/trial.json");
BattleRunResult result = BattleRunner.Run(request, new BattleRunOptions(resources, 20000));

using var sequence = BattleSequence.Start(request, new BattleRunOptions(resources, 20000));
sequence.Current.Step(400);
using var checkpoint = sequence.Current.Capture();
sequence.RunToEnd();
```

## Schema

[battle-request.schema.json](../reference/schemas/battle-request.schema.json) 是请求的 JSON Schema（draft 2020-12），[battle-clock.schema.json](../reference/schemas/battle-clock.schema.json) 是可选的显式 Update 时钟。两者由读取器使用的记录类型生成：`OpenPCR.Runner schema --write <目录>` 重新生成，`schema --check <目录>` 确认文件与代码一致（多出的 schema 文件也报错）。masterdata 引用和字段间的一致性由读取器检查。
