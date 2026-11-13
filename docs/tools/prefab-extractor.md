# Prefab 数据库提取器

PrefabExtractor 使用 AssetsTools.NET 读取 Unity bundle 内嵌的 type tree，生成 Host 所需的 `prefab/unit_prefab_native.sqlite3`，并提供 manifest 建库、增量更新和数据库比较。工具可独立运行，无需 Python、Engine或 Unity 运行环境。

```bash
dotnet build tools/OpenPCR.PrefabExtractor -c Release -p:OpenPcrAuditMode=Public
dotnet tools/OpenPCR.PrefabExtractor/bin/Release/net10.0/OpenPCR.PrefabExtractor.dll --list
dotnet tools/OpenPCR.PrefabExtractor/bin/Release/net10.0/OpenPCR.PrefabExtractor.dll \
  --manifest /data/manifest_202608171854.db \
  --bundles /data/bundles/202608171854 \
  --output /data/resources/prefab/unit_prefab_native.sqlite3 --all --jobs 4
```

`--manifest`、`--bundles` 和 `--output` 必须指定。工具默认离线；加上 `--download` 后，才会下载 manifest 中列出但本地缺失的 bundle。缓存文件和下载内容都按 manifest 核对大小与 MD5；新缓存通过临时文件写入，保留其它版本的缓存。联网的 `--download` 与 `--build-manifest` 必须显式提供 HTTPS `--resource-root`；没有预设服务器。离线缓存提取无需填写服务器地址。

使用 `--all` 完整重建数据库，按依赖顺序处理以下 10 类数据源（source）：

| Source | 输入与产物 |
| --- | --- |
| `unit_prefab` | 单位组件 → `prefab_tree` protobuf |
| `spine` | CYSP 骨架 → 骨骼、包围盒、顶点、原始 setup |
| `unit_spine` | SD_NORMAL／SD_SHADOW SkeletonDataAsset 与混合配置 |
| `mode_spine` | SD_MODE_CHANGE 动画、姿态、setup、混合配置 |
| `skill_spine` | SKILL_EFFECT 动画、时间线身份、混合配置 |
| `firearm` | `prefab_tree` 中单位包及依赖包 → 弹体组件 |
| `battle_manager` | BattleManager 标量、父 Transform 链、共用 Spine Transform |
| `cysp` | battle/common_battle 动画时长、原始时间线、damage 事件 |
| `circle_effect` | 环形效果参数与有序子节点 |
| `skill_effect` | 效果组件、粒子、倒计时、tweener、bundle/container 身份 |

局部更新可重复指定 `--only`。只选择一类数据源时，可用 `--keys` 限定键；`unit_prefab` / `firearm` 使用 prefab ID，Spine 类使用对应资源键，效果类使用带 `a/` 前缀的 bundle 名。

`unit_spine` 的普通资源键为皮肤 ID（如 `106361`），暗影资源键为 `shadow_` 加皮肤 ID（如 `shadow_106361`）。它们分别读取 `spine_sdnormal_106361.unity3d` 和 `spine_sdshadow_106361.unity3d`，保留在同一表的独立 bundle 行；不能用普通资源代替缺失的暗影资源。提取器要求这些包已缓存，或显式提供 `--download --resource-root`。
`firearm` 默认从输出库的 `prefab_tree` 选键，所以新库需要先提取单位树，或显式指定键。

```bash
dotnet tools/OpenPCR.PrefabExtractor/bin/Release/net10.0/OpenPCR.PrefabExtractor.dll \
  --manifest /data/manifest_202608171854.db --bundles /data/bundles/202608171854 \
  --output /data/resources/prefab/unit_prefab_native.sqlite3 \
  --only unit_prefab --keys 100101 103301
```

局部更新先通过 SQLite backup 复制已有数据库，在临时库中替换成功解析的键对应的数据；解析结果为零行时，也清除该键的旧数据。缺包、摘要不符、解析失败、重复主键、缺少序列化字段或不支持的外部粒子／tweener 引用，都会使本轮返回非零退出码并保留原输出。

`--all` 从空库重建，并检查必需表非空。全部处理成功后，工具关闭 SQLite 连接，将临时库重命名为目标文件。Host 通过指定资源根目录只读打开该文件。

`--merge` 按第一张表跳过已有范围键，用于补齐数据，不核对已有键的 manifest 来源。

每个键解析时实际读取的 bundle 连同 manifest 中的 MD5 和大小写入 `source_bundle`（数据源、键、bundle 各一行），随该键的数据一起替换。换用新 manifest 时用 `--update` 增量更新：

```bash
dotnet tools/OpenPCR.PrefabExtractor/bin/Release/net10.0/OpenPCR.PrefabExtractor.dll \
  --manifest /data/manifest_<新版本>.db --bundles /data/bundles/<新版本> \
  --output /data/resources/prefab/unit_prefab_native.sqlite3 --update [--only <source>] [--download --resource-root <https-url>]
```

`--update` 要求目标库已存在，不能与 `--all`、`--merge`、`--keys` 同用；省略 `--only` 时处理全部数据源。对新 manifest 产生的每个键，没有记录、记录的 bundle 不在 manifest 中或 MD5／大小不同、或按命名规则应读取的 bundle 不在记录中，就重新提取；已记录但新 manifest 不再产生的键，删除其范围数据。其余键不读取 bundle，因此只下载变化的 bundle。没有范围列的表（`battle_manager_*`）只追加或替换，不随键删除。

解析使用至多 `--jobs` 个并行任务，写入线程按键顺序写出；`zjson` 与 `zproto` 列在解析任务中压缩。工具使用服务器 GC。

manifest 也可由 C# 单独生成；版本必须明确指定，不自动选择线上最新版：

```bash
dotnet tools/OpenPCR.PrefabExtractor/bin/Release/net10.0/OpenPCR.PrefabExtractor.dll \
  --build-manifest 202608171854 --output /data/manifest_202608171854.db \
  --resource-root https://resources.example.invalid
```

manifest 按深度优先顺序展开，同一 URL 后出现的行覆盖先前的行。支持 MD5 寻址和独立短 pool key 两种行格式；生成失败时保留已有 manifest。版本自动查询和旧缓存自动清理不在此工具的功能范围内。

## EX 装备技能

`equipment-skills` 子命令将 EX 装备技能导出为独立的 `prefab/equipment_skills.json`：

```bash
dotnet tools/OpenPCR.PrefabExtractor/bin/Release/net10.0/OpenPCR.PrefabExtractor.dll equipment-skills \
  --manifest /data/manifest_202611031924.db --bundles /data/bundles/202611031924 \
  --output /data/resources/prefab/equipment_skills.json
```

工具从 manifest 列出的 `all_equipment_skill_*.unity3d` 提取 `ExSkillScriptable`，保留两条击退曲线、EffectType、资源路径、bundle SHA-256 和 manifest 摘要。格式为 `openpcr-equipment-skills/v1`，Unity 版本为 `6000.0.58f2`。输出已存在、缺资源、非法选项、损坏 bundle 或 manifest 版本错误时失败。联网需提供 `--download --resource-root <URL>`；Host 核对部署文件的 schema、版本、资源身份和曲线支持范围。

## 数据契约与验证

提取器直接链接 Host 的 [unit_prefab.proto](../../src/OpenPCR.Host/Schema/unit_prefab.proto) 与 [spine_animation.proto](../../src/OpenPCR.Host/Schema/spine_animation.proto)。[Tables.json](../../tools/OpenPCR.PrefabExtractor/Tables.json) 定义 SQL 表的列名、主键、范围、索引与浮点位宽。`prefab_meta` 必须包含 `schema_version=10`、`unity_version=6000.0.58f2`；版本不匹配时须用同版 manifest 和 bundle 重建，不能只改版本标记。

| 列 | 存储格式 |
| --- | --- |
| `prefab_tree.data` | `UnitPrefab` protobuf，`format_version=1`；`SlowEffect.to` 带存在标记，保留显式零值 |
| `cysp_pose.timelines`、`mode_spine_pose.timelines` | `zproto`：`SpineAnimation` protobuf 的 zlib 流，`format_version=1` |
| `spine_pose_setup.data`、`mode_spine_setup.data`、`skill_spine_timeline.properties` | `zjson`：UTF-8 JSON 的 zlib 流 |
| 其它 JSON 列 | UTF-8 文本 |

Host 按列类型检查 BLOB／文本，拒绝未标记或不支持的产品版本。`SpineAnimation` 按资源顺序保留全部时间轴帧、数值和曲线，不缩放数值；附件名与事件字符串的 null 由存在标记区分。浮点字段采用 packed repeated 存储，由标准 writer 保留负零。

`UnitPrefab` 使用 `CodedOutputStream` 按 schema 顺序写出，以位模式判断浮点默认值，保留标准 C# writer 会省略的 `-0f`；读取使用标准 parser。新增单位树字段时修改共享 schema 与 `UnitPrefabBuilder`，再按原 bundle 版本用 `--only unit_prefab` 更新，可用 `--compare` 核对差异。

Unity Single 提升为 double；SQLite 浮点存为小端 f32／f64 BLOB，JSON double 使用 G17 和显式 `-0.0`。PPtr 保留有符号 64 位身份。提取器只输出原始数据，世界变换、AABB、动画混合和秒到帧换算由消费者执行。bundle 的序列化版本 `0.0.0` 按 `6000.0.58f2` 解释，其它明确版本必须等于该版本。

```bash
OpenPcrAuditMode=Public dotnet run -c Release --project tests/OpenPCR.PrefabExtractor.Tests
dotnet tools/OpenPCR.PrefabExtractor/bin/Release/net10.0/OpenPCR.PrefabExtractor.dll \
  --compare /data/reference/unit_prefab_native.sqlite3 \
  --output /data/resources/prefab/unit_prefab_native.sqlite3
```

测试覆盖 manifest 格式与遍历顺序、整数身份、浮点位模式、CYSP 边界、空范围替换、重复主键、离线缺包、损坏缓存，以及解析失败时保留原输出的行为。比较命令只读扫描全部已定义表，要求行数、主键和全部列一致：
protobuf/BLOB 逐字节比较，`zproto` 列解压后逐字节比较，`zjson` 列先解压再按 JSON 比较，JSON 忽略空白与对象键顺序，保留数组顺序及负零差异。
JSON 数学整数按精确数值比较，包括小数点或指数写法；双方均为非整数时，按有限 double 比较，以兼容不同语言对同一 double 的十进制表示（如 `0.1` 和 `0.10000000000000001`）。
JSON 对象键不得重复；非整数超出非零有限 double 范围时拒绝比较。退出码 0 表示一致。
数据库文件的页布局、JSON 文本排版及 zlib 字节流不要求一致。

工具按输入键顺序写出，完整建库使用 prefab ID 升序；比较命令仍会报告归属列的任何差异。
CYSP 段解析失败会使本轮提取失败，保留原输出数据库。

AssetsTools.NET 的 bundle/type-tree 读取方式见其[官方文档](https://github.com/nesrak1/AssetsTools.NET/wiki/Getting-Started%3A-Bundle-file-reading)。
