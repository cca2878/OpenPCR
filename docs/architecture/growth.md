# C# 养成计算器契约

养成计算器位于 `src/OpenPCR.Growth/`，使用调用者指定的资源和 masterdata 计算准备产品。公式的运算顺序、调用时机和对象关系都是计算契约的一部分。战斗交接及 Core 的职责见 [Core I/O 参考](../reference/core-io.md)。

## 输入

计算器接收强类型的 `GrowthInputs`，由 `new GrowthCalculator(inputs, masterConnection, masterVersion)` 计算。`BattlePlanner` 从 `openpcr-battle/v2` 的服务器形状字段构造输入，字段来源见 [战斗请求与结果](../usage/battle-request.md)。显式 `masterVersion` 必须与 `GrowthInputs.MasterVersion` 相同；数据库只读，路径由调用方显式指定。

| 输入组 | 类型与语义 |
| --- | --- |
| 账号环境 | `TeamLevel`、`ChangeRarityReleased`、`SkillLevelSyncReleased`：请求的 `Releases` 布尔，计算器不读墙钟 |
| 持有单位 | `RawUnit`：`Id`、等级、稀有度、战斗稀有度、Rank、ExceedStage、抗性键、六星解放槽、皮肤、普通／专武／EX／公会战 EX 槽和 UB／Main／EX／Free 有序技能列表 |
| 普通／专武槽 | `RawEquipSlot`：`Id, IsSlot, EnhancementLevel, EnhancementPt, Rank`。列表顺序就是槽顺序；999999、未装备和缺表行按对应 Native 门处理 |
| EX 槽与库存 | `RawExEquipSlot` 的 serial、直接 equipment ID、强化点、副词条；库存 `RawExEquipment` 为 serial、equipment ID、强化点、Rank、副词条。`SubStatusGroupId` 现查 masterdata；副词条允许同种属性重复，逐条加算 |
| 剧情 | 已读剧情 ID 列表，保留顺序；空表代表无已读剧情，不能从 BonusParam 反解 |
| 队伍强化 | 己方 `RawTeamEnhance`：`RawPrincessKnight`（天赋等级、最近强化页节点、队伍技能节点；可为 null）与 `RawUnitRole` 列表；敌方显式 null 是合法输入 |
| 支援 | `RawSupportUnit`：训练场支援的服务器 `unit_data`（UnitDataLight）；技能 ID、装备 ID／强化等级由 masterdata 及原始等级／强化点构造 |

自有单位的 `UnitParam`、`BonusParam`、`Power`、Limited、UniqueDataForView、IsClanSupportOrReplay，以及队伍强化的三张结果字典，均由准备流程产生，不作为输入。计算器也不读取 `frame0`、`unit_instances`、被动或钳制后的属性、运行 HP、伤害上下文和 RNG 轨迹。

支援的 `UnitDataLight.BonusParam` 是服务器提供的支援者剧情属性，由 Native 构造器 `UnitData(UnitDataLight,bool,bool)` 直接传播，须保留原值。自有单位的剧情属性按本账号已读剧情计算；支援的 Base／Equip／EX／Total 仍由计算器生产。

原始 ID、转换后的战斗 ID 与 prefab ID 分别保存。字典键／ID 不一致、转换后重复出战身份、输入版本与显式部署版本不一致、缺必需字段、缺 serial 和 Native 必须解引用的缺表行均失败。数据库内容是否确实属于声明的部署版本，仍由调用者按 [资源准备](../usage/resources.md) 确认。合法 null、空表、0 和缺失不得互换；Native 本来允许缺行返回 0 的访问器保留该行为。

## 输出与调用时机

| API | 输出和生产时机 |
| --- | --- |
| `CalculateUnit(originalId, isClanSupportOrReplay=false)` | 登录态 Unique＋`UpdateLimitedUnitData` 结果 Limited 两份 `UnitData`（保留原始 ID）、分别的不可变 `PreparedUnitInputs` 和转换后身份的 prefab ID；不修改输入 |
| `CalculateSupport(originalId)` | 选中支援在训练场支援列表中的 `UnitParamData`（原始 ID）及其 `UpdateLimitedUnitData(true, true)` 的 Limited |
| `CalculateConvertedLimited(unique, isClanSupportOrReplay)` | `MightConvertRarity6Unit` 复制出的转换身份副本再次刷新 Limited 的结果；无转换时为 null |
| `CalculateLabyrinthNpc(raw, bonus)` | 从原始 NPC 字段和服务器剧情属性生产未限级的玩法产品 |
| `CalculateConvertedLabyrinthNpc(unique)` | NPC 六星转换身份的未限级产品，保留服务器剧情属性；无转换时为 null |
| `CalculateTeam(enemy=false)` | 可空 `TeamEnhanceParameter`，分别生成 talent-level、knight-enhance、role 三张表；role 的键保持 `eRoleParamType`，由 Core 按攻击类型转换；这些值不合入 Total |
| `CalculateEnemy(enemyId)` | 锁定 enemy_parameter 的新参数产品及按位置对应的技能等级；master／skill 资源身份在 Host 绑定 |
| `PrepareParameters(stageEnemyIds)` | 编译 masterdata 的召唤技能关系、各稀有度／Rank 的成长系数、解锁条件及动态敌人产品（召唤敌人，以及以本场敌人为 `target_value_1` 的 extra-effect 敌人）；返回不含原始账号、JSON、数据库、委托或计算器的 `PreparedParameterProducts` |
| `PrepareAccountGrowth()` | 返回固定的稀有度切换门和已解析 EX serial→装备身份；Core 账号子集由该产品装配 |

`PreparedUnitInputs` 含 eParamType **1..17** 的战斗被动前 Total（HP Int64、其余 Int32），及独立 Int64 受伤回 TP HP／DEF／MDEF。产品也保留 BaseParam、EquipParam、BonusParam、原始槽、技能列表、皮肤和身份关系。`Power` 是显示输出，本计算器的 Core 交接不包含战力。

Limited 按 Native 从 Unique 复制指定的技能／EX 列表，保持其它规定的共享关系，使用 Cb EX 槽、等级封顶与技能同步，再重算参数。UB 的同步没有正等级门；Main 前两个和 EX 第一个有正等级门；未解锁技能不会被强制开启。`IsClanSupportOrReplay` 是生产机制实参，不能从“借人”业务标签随意推断。

受伤 TP 的三项从对应出战 Unique 独立生产，保留 promotion bonus 的 Rank 回退循环，不含专武、剧情或 EX。这些值与普通 Total 使用不同的计算规则，需分别提供。

`OpenPCR.BattleInputs.PreparedParameterProducts.CalculateSummon` 在原 Native 消费点用实际 owner 和出生实参装配新产品，保留 Native 的 UB→Main→EX→SP→Free 首个匹配顺序、技能等级作为基础成长等级的规则、稀有度／Rank 编码及回落、装备继承引用语义。`equipment=true` 时 EquipParam／BonusParam 继承 owner 的实际引用；Core 仍拥有动态出生、行动模式、RF／SP 选择、被动和生命周期。这些动态产品仍在 Native 规定的调用时点生成，准备阶段只编译其所需规则。

## 建场交接

Host 按原始 ID 将自有单位放入 `UserData.UnitParameterDictionary`，按 Native 位置装配编成与训练场支援列表。`UnitUiCtrl.Initialize` 完成 `AlterUnitDataLevelLimit`、六星转换与站位排序。`UnitParameter.UpdateLimitedUnitData` 按（UniqueData ID，支援／回放标志）读取预先准备的 Limited 产品，并核对稀有度与 Rank。敌人由玩法 ID 推导，副敌由处理器在原消费点经 `CreateUnitParameterFromEnemyParameter` 取产品。

Host 将环境／场景、参数准备与运行操作分开，见 [战斗输入](battle-inputs.md)。黎明界另有 `CalculateLabyrinthNpc`（服务器 NPC `unit_data` 与 `bonus_param`）、`CalculateLabyrinthEnemy` 与 `PrepareLabyrinthMode`。外供参数模板与捕获参数产品不进入请求协议。

## 项目与交接边界

只读 masterdata 的原始行与查询索引按规范化部署路径在进程内复用；资源版本位于文件名中，缓存只保留最近一个数据库身份。每张表共享列索引，行保存不可变的原始值，相同类型和位模式的数值共用一个装箱对象（`SharedBoxes`）；缓存不持有账号、计算器、数据库连接或可变单位产品。

Worker 存活期间，部署的只读资源必须保持不变；更新前先停止相关 Worker，完成更新后再启动。缓存不检测运行期间的写入、删除或替换。可写数据库和内存数据库使用计算器实例内缓存，保持相同的养成公式、数值转换和产品复制规则。

`OpenPCR.Engine` 维护核心执行模型，保持现有类型边界、签名及执行顺序。`OpenPCR.Growth` 引用 Engine 的共同类型和 `OpenPCR.BattleInputs`，不引用 Host。Host 组织准备和执行，Engine 不引用 Growth。

准备阶段计算单位、队伍强化、EX 身份投影和动态参数规则。`BattlePreparer` 在返回准备产品前释放 `GrowthInputs` 和 `GrowthCalculator`；运行提供器只捕获 `PreparedParameterProducts` 与资源装配器。请求 JSON 不进入产品对象图；计算器的 `Dispose` 清除原始输入和数据库访问器。

召唤规则由完整 masterdata 技能／动作关系建立，覆盖数据库中可构造的单位定义，不从实际战斗出现清单反推。运行工厂只读取规则产品、实际 owner 参数与调用实参；成长系数按 Native 浮点顺序求值，出生等级与基础成长所用技能等级分别处理。缺失定义、稀有度／Rank 规则或 owner 匹配明确失败。每次创建新可变单位／技能，并保持装备／剧情继承引用；动态敌人复制产品并隔离参数实例。

队伍强化产品在战斗初始化前生成；Core 仍负责定位属性映射、实际阵容才能统计、被动应用和生命周期。TeamLevel、ViewerId、设置、准备时点及解锁／日志环境是显式场景环境产品，不要求全部为养成公式的派生数值。EX 属性计算所需完整库存只在 Growth；Engine 仅取得已解析装备身份。

上述划分说明参数和账号依赖的交接方式。验证时还需分别核对 Native 类型契约、实际资源使用路径，以及产品使用时点与原生计算时点的差异。

## 验证与限制

公开的 Growth.Tests 提供无需资源的 masterdata 缓存测试；嵌套输入记录的校验（缺字段拒绝及合法零值、null、空表和 light 支援形状）在 Host.Tests 中针对 v2 字段测试。参数数值和完整战斗回归需要另行指定数据版本及输入样本，其结论仅覆盖实际执行的账号、支援载荷和调用路径。

```bash
dotnet run --no-build -c Release --project tests/OpenPCR.Growth.Tests
```
