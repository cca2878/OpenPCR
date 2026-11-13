# 黎明界模拟

黎明界使用通用请求 `openpcr-battle/v2` 的 `Labyrinth` 段；`Player`、`Settings`、`Battles` 与结果的公共部分见 [战斗请求与结果](battle-request.md)。

- 类别 59（普通和困难关卡）：`Battles` 恰好一项，一份请求描述一场战斗。
- 类别 60（EX 关卡）：`Battles` 为一到三项，一份请求描述整场 EX 挑战。OpenPCR 在同一进程里逐队运行，队与队之间的交接按客户端规则在内部完成。

类别由 `quest_id` 决定，`Battles` 的项数与类别不符时报错。账号、持有角色、遗物、NPC 和 seed 由调用方提供；关卡本身决定的数据由 OpenPCR 从部署的 masterdata 推导。

## 字段来源

服务器字段名取自黎明界相关接口的响应（参考 autopcr 的接口模型）。

| 请求字段 | 来源 | 说明 |
| --- | --- | --- |
| `Labyrinth.quest_id` | `labyrinth/resume` 的 `status.quest_id`，或玩家选择的关卡 | 决定类别、敌人、时限和背景 |
| `Labyrinth.difficulty` | `labyrinth/top` 的 `difficulty` | |
| `Labyrinth.alpha_rupee` | `labyrinth/resume` 的 `alpha_rupee` | |
| `Labyrinth.unit_list[]` | `labyrinth/resume` 的 `unit_list[]` | `unit_id`、`unit_type`（USER=1、NPC=2）；NPC 另需 `unit_data` |
| `Labyrinth.relic_list[]` | `labyrinth/resume` 的 `relic_list` | `relic_id`、`effected_count`、`enable`，保持服务器顺序 |
| `Labyrinth.labyrinth_enhance_node_list` | `labyrinth/top` 的同名字段 | 强化节点 ID |
| `Labyrinth.start_state` | 可选，见 [EX 挑战](#ex-挑战) | 从 EX 中途某队开始 |
| `Battles[0].Seed`（类别 59） | `labyrinth/battle_start` 的 `seed` | |
| `Battles[i].Seed`（EX） | `labyrinth/boss_battle_start` 的 `seed_list[i]` | 按队号取用。`seed_list` 和 `battle_log_id_list` 只含非空编队：第 1、2 队为空时只返回一项，`Battles` 也只列出这些队 |
| `Battles[i].Party`、`Opening` | 玩家编成与开场设置 | EX 编成在开局前经 `labyrinth/update_deck` 同步，开局后不能更换 |

`unit_list` 是持有单位的完整列表；遗物和强化效果按持有数量、职能和属性计数，不只看上场的五人。USER 单位的养成取 `Player.unit_list` 中的同 ID 单位，必须存在。NPC 的 `unit_data` 原样照搬服务器对象，按客户端解析规则读取：

- 缺失字段取 0 或 null；`battle_rarity` 为 0 时按 `unit_rarity` 计算；专武槽保持服务器条目，不补占位。
- `bonus_param` 缺失或为 null 时没有加成，为空表 `[]` 时是全零加成，为对象时按服务器字段名（`hp`、`atk`、`hrec_rate` 等）读取。
- `unit_param`、`power` 等计算结果不读取。

NPC 的属性和技能由同版 masterdata 重算，保留服务器提供的 `bonus_param`。NPC 使用未限级的玩法产品，不受会战等级同步影响；同 ID 的 USER 与 NPC 各自独立计算。

## 由关卡推导的数据

客户端建场时，以下内容直接从 masterdata 取值（`LabyrinthUtility.CreateQuestBattleStage`／`CreateExBattleStage`、`setBattleStageEnemy`、`initializeExEnemyList`）：

| 内容 | 取值 |
| --- | --- |
| 类别 | 关卡类型 1、2 为 59，类型 3（EX）为 60 |
| 敌人 | `labyrinth_wave_group_data` 的 `enemy_id_1`～`enemy_id_5`，按顺序去掉 0 |
| 时限 | `labyrinth_quest_data.limit_time` − `labyrinth_difficulty_data.battle_decrease_time` |
| 背景 | 关卡展示表的 `battle_bg_id`（EX 关卡用 EX 展示表） |
| EX 首队敌人记录 | 每个波次敌人一条：`IdentifyNum` 从 1 起，HP 和最大 HP 取 `labyrinth_enemy_parameter.hp`，TP 为 0，等级、稀有度、Rank 取同一行 |

需要预先查看时，可以调用 `LabyrinthStage.Read(resources, masterVersion, questId, difficulty)` 和 `LabyrinthExState.Initial(resources, masterVersion, questId)`。

## 开场设置

每场使用自己的 `Opening`：开场 AUTO、速度 1／2／4，以及开场开启 SET 的本队单位。设置不跨场继承。黎明界不支持战斗中途的玩家操作，`Operations` 必须为空。

## EX 挑战

EX 挑战按开局输入、各队编成和 seed 逐队运行：

1. 第 0 队从关卡推导的首队敌人记录开始。
2. 每队结束后，按本队结算推出下一队开始时的状态（敌人 HP 标准化、已死亡敌人、各队波次结果、己方结果、清除模式、敌方续战）。
3. 本队胜利、已打完最后一队，或下一队编成为空时，整场结束。

`Battles` 少于三项时，缺的队视为空编成。`start_state` 省略时从第 0 队开始；从中途某队做分支推演时，填入该队开始时的状态，`PartyIndex` 指明队号，`Battles` 仍按队号排列。每队结果里的 `Next` 就是下一队的状态，可以直接用作 `start_state` 分支。

`start_state` 的字段：`PartyIndex`、`Enemies`（敌人记录，含 HP、TP、技能计数和皮肤）、`AlreadyDeadEnemies`、`Waves`、`UnitResults`（各队的己方 HP、最大 HP 和 TP）、`ClearTypes`、`EnemyContinuation`（敌方续战，可为 null）。HP、伤害和 viewer ID 为 int64，己方 TP 为 double。完整结构见 [schema](../reference/schemas/battle-request.schema.json) 中的 `LabyrinthExState`。

## 最小请求

可用编辑器预设生成基础养成，并将所列角色声明为持有的 USER 单位。`editor` 的定义见 [BattleEditor](../tools/battle-editor.md)：

```bash
editor create --resources /data/openpcr-resources --master-version 202611031924 --preset labyrinth-basic \
  --units 100101,100201 --quest-id 770110101 --difficulty 2 --output labyrinth.json
```

手写时，`Labyrinth` 段如下（`Player`、`Settings` 见 [战斗请求与结果](battle-request.md)）：

```json
{
  "Schema": "openpcr-battle/v2",
  "MasterVersion": "202611031924",
  "Player": { "...": "..." },
  "Settings": { "...": "..." },
  "Battles": [{ "Party": [100101, 0, 0, 0, 0], "Opening": { "Auto": false, "Speed": 1, "SetUnits": [] }, "Seed": 12345 }],
  "Labyrinth": {
    "quest_id": 770110101, "difficulty": 1, "alpha_rupee": 0,
    "unit_list": [{ "unit_id": 100101, "unit_type": 1 }],
    "relic_list": [], "labyrinth_enhance_node_list": []
  }
}
```

EX 请求把 `quest_id` 换成 EX 关卡，并按队列出 `Battles`：

```json
"Battles": [
  { "Party": [100101, 0, 0, 0, 0], "Opening": { "Auto": false, "Speed": 1, "SetUnits": [] }, "Seed": 12345 },
  { "Party": [100201, 0, 0, 0, 0], "Opening": { "Auto": true, "Speed": 2, "SetUnits": [100201] }, "Seed": 12346 },
  { "Party": [0, 0, 0, 0, 0], "Opening": { "Auto": false, "Speed": 1, "SetUnits": [] }, "Seed": 12347 }
]
```

完整请求见 [labyrinth-normal.json](../../examples/battles/labyrinth-normal.json) 和 [labyrinth-ex.json](../../examples/battles/labyrinth-ex.json)。EX 示例的第 0 队落败后由第 1 队接战，第 2 队为空时整场结束。

## 运行与结果

运行方式与其它玩法相同（CLI `run`、Worker `create`、C# `BattleRunner.Run`／`BattleSequence`），见 [战斗请求与结果](battle-request.md#运行)。EX 是一个序列：Worker 的 `prepare` 只接受单场请求，整场 EX 用 `create` 直接创建；`step` 遇到已结束的队会先切到下一队，`run` 跑完整场，切队时之前的检查点全部释放。

- 类别 59：`Battles[0].Settlement` 是 `openpcr-labyrinth-settlement/v2`（`QuestId`、`RemainTime` 毫秒、`AutoClear`、`Hp`）。
- EX：每队一项，`Settlement` 是 `openpcr-labyrinth-ex-settlement/v2`（队号、是否最终队、敌我状态），`Next` 是下一队状态。整场结束后 `Outcome` 为最后一队后客户端发送的唯一一次结算请求内容：`QuestId`、`Waves`、`Enemies`、`AutoClear`。
- 某队用完帧预算时 `Completed` 为 false，序列停在该队。

### 对应的服务器请求

客户端在每场结束时按下表组装请求体；PostParam 的属性名就是请求字段名，列表原样转为数组。`enter_id`、`block_id` 和 `battle_log_id` 不参与模拟，组装请求时由调用方从自己的会话数据填入。

| 请求 | 字段 | 取自结果 |
| --- | --- | --- |
| `labyrinth/battle_finish`（类别 59） | `enter_id`、`block_id` | 调用方：`labyrinth/top` 或 `enter` 的 `enter_id`，`labyrinth/resume` 的 `block_id` |
| | `quest_id`、`remain_time`、`unit_hp_list[{viewer_id, unit_id, hp}]`、`auto_clear` | `Settlement` 的 QuestId、RemainTime、Hp、AutoClear；`viewer_id` 由调用方把 1 换成玩家 ID |
| `labyrinth/boss_battle_finish`（EX，最后一队后一次） | `enter_id`、`block_id` | 调用方 |
| | `quest_id`、`wave_result_list`、`versus_user_list`、`auto_clear` | `Outcome` 的 QuestId、Waves、Enemies、AutoClear；成员字段名与请求一致，`owner_viewer_id` 同样需要替换 |
| `log/battle_log2`（每场一次，EX 每队一次） | `battle_log_id` | 调用方：类别 59 为 `battle_start` 的 `battle_log_id`，EX 为 `boss_battle_start` 的 `battle_log_id_list[队号]` |
| | `frame_rate`、`battle_log`、`system_id` | 本场 `Battle` 的 FrameRate、Csv（原样，每行以 LF 结尾）、SystemId（136） |

客户端只在日志功能开启时发送 `log/battle_log2`：`Player.ini_setting.battle_log_type` 中任一项为 1。
