# 战斗场景 JSON 编辑工具

BattleEditor 用于创建、修改和校验 `openpcr-battle/v2` 请求（格式见 [战斗请求与结果](../usage/battle-request.md)）。生成的文件可直接交给 Runner／Host。校验和数据准备复用 Host／Growth；战斗运行及操作输入由 Runner／Host 单独处理。

```bash
dotnet build tools/OpenPCR.BattleEditor -c Release -p:OpenPcrAuditMode=Public
dotnet tools/OpenPCR.BattleEditor/bin/Release/net10.0/OpenPCR.BattleEditor.dll --help
# 可选：在当前 Bash 会话中为下文命令定义短名称。
editor() { dotnet tools/OpenPCR.BattleEditor/bin/Release/net10.0/OpenPCR.BattleEditor.dll "$@"; }
```

下文以 `editor` 代指 `dotnet tools/OpenPCR.BattleEditor/bin/Release/net10.0/OpenPCR.BattleEditor.dll`，在仓库根目录执行。资源根目录和版本需要指定，工具不会自动查找或下载资源。

## 查找并创建

```bash
editor catalog --resources /data/openpcr-resources --master-version 202609081902 \
  --kind units --query 日和莉
editor catalog --resources /data/openpcr-resources --master-version 202609081902 \
  --kind enemies --id 401010101
editor unit-info --resources /data/openpcr-resources --master-version 202609081902 --id 100101

editor create --resources /data/openpcr-resources --master-version 202609081902 \
  --preset trial-basic --units 100101,100201 --battle-id 1001 --phase 1 --order 1 \
  --output /tmp/battle.json
editor inspect --input /tmp/battle.json
```

`catalog` 支持 `units`、`enemies`、`backgrounds`、`clan-maps`、`equipment`、`stories`、`labyrinth-enemies`、`labyrinth-quests`、`labyrinth-relics`；可用 `--query`、`--id`、`--limit`（默认 30，范围 1–1000）过滤。角色目录排除转换副本，`unit-info` 返回原始角色的稀有度、Rank／装备、技能和专武定义。背景目录的查询范围为 `clan_battle_2_map_data.map_bg`。`clan-maps --id` 按活动 ID 查询两种模式的阶段和周目区间。`enemies` 使用 `enemy_parameter.enemy_id`，`labyrinth-enemies` 使用独立的 `labyrinth_enemy_parameter.enemy_id`。黎明界任务目录返回类别、时限及 wave group；遗物目录返回名称、印记和效果组。

`create` 要求选择版本、预设和 1–5 名原始自有角色。场景由玩法 ID 决定：会战预设用 `--battle-id`、`--phase`、`--order`（省略时沿用预设的 1001／1／1），黎明界预设必须给 `--quest-id` 和 `--difficulty`。敌人、时限和背景由场景推导，不是选项。可同时传入下文的养成／场景参数。预设随工具嵌入，安装后不依赖示例文件路径。黎明界预设 `labyrinth-basic` 见[黎明界](#黎明界)；以下两种用于训练场与会战：

| 预设 | 初始配置 |
| --- | --- |
| `trial-basic` | 训练场（`kind: trial`），1001／phase 1／order 1 |
| `clan-rehearsal-basic` | 会战（`kind: clan`），1001／lap 1／phase 1／order 1，BOSS `max_hp` 10000000／`current_hp` 8000000，允许四倍速 |

三种预设共用以下基础配置：

| 项目 | 初始值 |
| --- | --- |
| 账号与角色 | 账号等级 100、角色等级 100、星级 3、Rank 1 |
| 技能 | 基础 UB、前两个 Main、第一个 EX 中 masterdata 定义为非零的技能，等级与角色等级一致 |
| 其它养成 | 无普通装备、专武、EX 装备、剧情、队伍强化或支援 |
| 开关 | `ChangeRarity` 开放，`SkillLevelSync`、`SlipDamageLogBarrier`、`KnightEnhance` 未开放；`errm` 100 |
| 战斗 | seed 12345，开场手动、1 倍速、无 SET；开启全部日志类型 |

其它设置来自 [trial](../../examples/battles/trial.json)／[clan-rehearsal](../../examples/battles/clan-rehearsal.json)／[labyrinth-normal](../../examples/battles/labyrinth-normal.json) 示例。使用前应通过 `inspect` 检查，并按目标场景修改。

## 修改与预览

```bash
# 添加角色并放入第五槽；成长定义按所选版本读取。
editor unit-add --input /tmp/battle.json --resources /data/openpcr-resources --output /tmp/battle.json \
  --id 100301 --slot 5 --level 100 --rarity 4 --rank 3 --skills 100 \
  --equipment all --enhance 1

# 修改已持有角色。修改等级不会隐式修改原始技能等级。
editor unit-edit --input /tmp/battle.json --resources /data/openpcr-resources --output /tmp/battle.json \
  --id 100101 --level 90 --skills 90

# 槽位按输入顺序，不足五槽补 0；0 为空，1 只能引用已有支援。
editor party --input /tmp/battle.json --resources /data/openpcr-resources --output /tmp/battle.json \
  --units 100101,0,100301

# 只预览，校验和产品准备照常进行，不写文件。
editor scene --input /tmp/battle.json --resources /data/openpcr-resources \
  --seed 61281661 --dry-run true
editor scene --input /tmp/battle.json --resources /data/openpcr-resources --output /tmp/edited.json \
  --seed 61281661 --team-level 120 --fps 60 --skill-sync true

editor stories --input /tmp/battle.json --resources /data/openpcr-resources --output /tmp/battle.json \
  --ids 1001002,1001003
editor validate --input /tmp/battle.json --resources /data/openpcr-resources
```

`unit-add`／`unit-edit` 支持 `--level`、`--rarity`、`--battle-rarity`、`--rank`、`--skills`、`--equipment none|all`、`--enhance`、`--slot 1..5`。

新角色沿用上表基础养成；只有新增时未指定 `--skills` 才取该角色等级。修改技能等级保留原有技能 ID、槽号和等级为 0 的未开启技能。

Rank、稀有度及装备强化档位须存在于当前 masterdata。`all` 从该角色所选 Rank 的槽位生成普通装备，并从对应强化表取累计点数；`none` 清空普通装备。已有普通装备时切换 Rank 必须显式选择 `--equipment`，防止保留旧 Rank 的装备；其它专武、EX 和数据不随 Rank 修改。

`all` 按 masterdata 的六个槽位顺序输出；`999999` 未开放占位保留在原槽，`is_slot=false`、强化等级／点数为 0。`--enhance` 只修改已装备槽；`unit-edit --equipment all` 可按当前 Rank 重新生成六槽配置。

`unit-remove --id` 删除持有数据；必须先从阵容移出该角色。`party` 不删除账号养成，`--battle N` 选择编辑第几场（默认 0）；移出支援格 1 后清除 `ClanBattle.support`。新建不自动生成服务器支援 payload。

`scene` 支持：

| 参数 | 修改内容 |
| --- | --- |
| `--battle N` | 选择下列逐场参数编辑的场次，默认 0 |
| `--seed`、`--auto`、`--speed 1\|2\|4`、`--set-units IDS\|none` | 本场 seed 与开场设置 |
| `--team-level`、`--fps` | 玩家等级、设备帧率 30／60 |
| `--rarity-change`、`--skill-sync`、`--slip-damage-barrier`、`--knight-enhance` | `Player.Releases` 的四个布尔开关 |
| `--battle-id`、`--phase`、`--order`、`--lap` | 会战场景 |
| `--boss-hp`、`--boss-current-hp`、`--quad-speed` | 会战（`kind: clan`）专用；HP 为 Int64 |
| `--infinite-hp` | 训练场专用 |

敌人、背景和时限由玩法 ID 推导，功能开放条件使用布尔开关；日志 ID 由调用方管理，不作为模拟输入。

## 高级字段

未提供专用命令的养成／设置可通过 JSON Pointer 显式修改，仍使用同一协议校验和准备器。例如：

```bash
editor set --input /tmp/battle.json --resources /data/openpcr-resources --output /tmp/battle.json \
  --path /Player/Releases/ChangeRarity --value false
editor set --input /tmp/battle.json --resources /data/openpcr-resources --output /tmp/battle.json \
  --path /Player/unit_list/0/unit_level --value 120
```

`--value` 是 JSON 值，字符串和对象须遵守 shell 的引号规则。路径区分大小写，`~0`／`~1` 分别代表 `~`／`/`；数组只能修改已有索引，替换整个数组使用数组所在路径。对象可新增库存 serial 等叶字段，父路径必须存在。OpenPCR 自定义对象拒绝未知字段，服务器形状的对象忽略未读取字段；重复属性与编辑器不支持的战斗操作均拒绝。编辑保留账号、专武、EX、强化和支援数据。

有相互依赖的字段时，使用 `set --patch changes.json` 一次修改后统一校验。文件格式为 `[{"Path":"/Battles/0/Seed","Value":123},{"Path":"/ClanBattle/phase","Value":2}]`。每项只允许 `Path`／`Value`；按顺序应用，不允许空列表。例如添加训练场支援时，可在同一批中同时修改 `/ClanBattle/support` 与 `/Battles/0/Party`。任一项或最终请求失败，整批都不写入。

## 输出与验证边界

除帮助文本外，成功命令向 stdout 输出 JSON `{ "Ok": true, "Result": ... }`；错误向 stderr 输出 `{ "Ok": false, "Error": ..., "Message": ... }`。退出码 0 为成功，1 为拒绝／错误。写入／预览结果包含摘要和 JSON Pointer `ChangedPaths`，`inspect` 只读当前输入。

写入前先运行 `BattleRequest` 的完整输入校验，再按玩法运行 `BattlePreparer.Prepare` 或 `LabyrinthBattlePreparer.Prepare`，核对场景所需的 masterdata 与资源。通过后才在目标目录写临时文件并原子替换，编辑支持输入输出同一路径。`--output` 明确指定替换目标；`--dry-run true` 进行相同校验与准备但不写文件。失败不替换现有文件。`validate` 的成功表示请求解析、上述资源检查与产品准备通过；它不运行战斗，不替代整场回归或客户端验证。

输出文件直接交给现有入口：

```bash
dotnet build tools/OpenPCR.Runner -c Release -p:OpenPcrAuditMode=Public
dotnet tools/OpenPCR.Runner/bin/Release/net10.0/OpenPCR.Runner.dll run \
  --resources /data/openpcr-resources --request /tmp/battle.json \
  --max-engine-frames 20000 --csv-dir /tmp/battle-logs
```

以下集成测试覆盖 CLI 创建、编辑、非法输入、原地写入和输出文件的实际运行：

```bash
dotnet build tools/OpenPCR.BattleEditor -c Release -p:OpenPcrAuditMode=Public
dotnet build tools/OpenPCR.Runner -c Release -p:OpenPcrAuditMode=Public
python tools/tests/test_battle_editor.py --resources /data/openpcr-resources --master-version 202609081902 -v
```

## 黎明界

可用 `create --preset labyrinth-basic` 新建类别 59 请求，或从 [类别 59 示例](../../examples/battles/labyrinth-normal.json)、[EX 示例](../../examples/battles/labyrinth-ex.json) 复制，再用通用命令编辑账号、设置和编成。字段来源见 [黎明界模拟](../usage/labyrinth.md)。

```bash
editor create --resources /data/openpcr-resources --master-version 202611031924 --preset labyrinth-basic \
  --units 100101,100201 --quest-id 770110101 --difficulty 2 --output /tmp/labyrinth.json
```

`labyrinth-basic` 把所列角色声明为持有的 USER 单位（`Labyrinth.unit_list`）并组成 `Battles[0].Party`，遗物、强化节点为空，`alpha_rupee` 为 0。

```bash
editor inspect --input examples/battles/labyrinth-ex.json
editor scene --input examples/battles/labyrinth-normal.json --resources /data/openpcr-resources \
  --seed 12345 --output /tmp/labyrinth.json
editor labyrinth --input /tmp/labyrinth.json --resources /data/openpcr-resources \
  --difficulty 2 --alpha-rupee 100 --relics /tmp/relics.json --output /tmp/labyrinth.json
editor labyrinth --input /tmp/labyrinth.json --resources /data/openpcr-resources \
  --party-auto true --party-speed 4 --party-set 100101 --output /tmp/labyrinth.json
editor labyrinth --input examples/battles/labyrinth-ex.json --resources /data/openpcr-resources \
  --ex-party 1 --party-units 100201,100101 --party-speed 2 --party-set none --ex-seeds 1,2,3 --output /tmp/ex.json
```

`labyrinth` 的选项：

| 选项 | 作用 |
| --- | --- |
| `--quest-id`、`--difficulty`、`--alpha-rupee`、`--reinforcement-nodes IDS\|none` | 改对应字段 |
| `--unit-sources`、`--relics`、`--ex-state` | 读取 JSON 文件，整体替换 `unit_list`、`relic_list`、`start_state`；`--ex-state` 的文件内容为 `null` 时清除起始状态 |
| `--party-units`、`--party-auto`、`--party-speed 1\|2\|4`、`--party-set IDS\|none` | 改一场的编成与开场设置；EX 用 `--ex-party 0\|1\|2` 选队，默认第 0 场 |
| `--ex-seeds A[,B[,C]]` | 按队写 seed，`Battles` 不足时补空编成的队 |

`party --battle N` 与 `unit-edit --slot` 同样改对应场的 `Party`。在类别 59 和 EX 之间切换要同时改关卡与 `Battles` 的项数，用 `set --patch` 一次完成。JSON Pointer 从请求根开始，例如 `/Labyrinth/alpha_rupee`、`/Battles/1/Opening/Speed`。

编成只能用 `Labyrinth.unit_list` 声明的 ID，不接受支援格 1。`unit-add`／`unit-edit`／`unit-remove` 只改 `Player.unit_list`，持有列表不会随编成或账号自动补齐。NPC 通过 `--unit-sources` 提供 `{unit_id, unit_type: 2, unit_data}`，`unit_data` 为服务器对象，不从角色展示表生成。

黎明界 CLI 测试可指定构建配置，资源需包含示例所用的 `202611031924` masterdata 与 format-1 prefab 产品：

```bash
python tools/tests/test_labyrinth_editor.py --resources /data/openpcr-resources -v
```
