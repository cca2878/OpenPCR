# 资源准备与校验

构建源码无需游戏资源。准备和运行战斗前，调用方需提供数据库，并指定资源根目录和版本；联网提取还需指定资源服务器。下文使用 Bash，命令在仓库根目录执行。

## 输入契约

| 输入 | 要求 | 准备责任 |
| --- | --- | --- |
| `masterdata/<version>_unhashed.db` | 明文、可由标准 SQLite 读取；表名和列名为消费代码使用的名称；十二位版本与场景一致 | 调用者提供。项目不包含获取、解密、解压或表名／列名还原工具；仅修改扩展名或文件名不完成转换 |
| `prefab/unit_prefab_native.sqlite3` | 按当前 Host schema 提取，`prefab_meta` 中 `schema_version=10`、`unity_version=6000.0.58f2`，单位树 `UnitPrefab.format_version=1` | 用 [PrefabExtractor](../tools/prefab-extractor.md) 从同一部署的 manifest 和 bundle 生成 |
| `prefab/equipment_skills.json` | `openpcr-equipment-skills/v1`，`manifest_version` 与场景 masterdata 版本相同；只在战斗用到 EX 装备技能（例如黎明界）时读取，缺失或版本不符时明确失败 | 用 [PrefabExtractor](../tools/prefab-extractor.md) 的 `equipment-skills` 子命令从同一 manifest 导出 |
| 战斗请求 | `openpcr-battle/v2`，其 `MasterVersion` 对应部署目录，角色与场景 ID 来自该版数据 | 使用 [BattleEditor](../tools/battle-editor.md) 创建或显式提供 |

masterdata 查询涉及养成、技能、行动、敌人、召唤和玩法上下文，需要提供相应的完整数据库。

当前消费契约见 [Growth 查询](../../src/OpenPCR.Growth/GrowthMasterData.cs)、[MasterInputs](../../src/OpenPCR.Host/MasterInputs.cs)、[运行时 masterdata 装载](../../src/OpenPCR.Host/RuntimeMasterData.cs)及 [Editor 查询](../../tools/OpenPCR.BattleEditor/Catalog.cs)。已有数据库应保留完整表、列、数值类型和行序，缺少数据时应重新核对来源，不应手工补零或添加默认行。

数据契约面向 11.7.2，其它客户端版本需另行验证。会战／训练场示例使用 `202609081902`，黎明界示例使用 `202611031924`；每个请求的 `MasterVersion` 必须与所用资源部署配套。

## 兼容的外部产物

masterdata 数据库格式兼容 [cc004/autopcr](https://github.com/cc004/autopcr) 项目的相关产物。可以参考该项目的数据库准备流程，并使用其完成表名／列名还原后的同版本 SQLite 数据库；相关实现见 [dbmgr.py](https://github.com/cc004/autopcr/blob/main/autopcr/db/dbmgr.py)。

将该数据库复制到 `masterdata/<version>_unhashed.db`，并按下文与同版 prefab 数据库配对、校验。上游产物的文件名不必带 `_unhashed`，应以实际表名／列名是否已还原为准；OpenPCR 使用该后缀作为部署路径约定。

## 配对部署

```bash
resource_root=/data/openpcr-resources
master_version=202609081902
```

将已准备好的 masterdata 和同版资源提取结果部署为：

```text
/data/openpcr-resources/
  masterdata/202609081902_unhashed.db
  prefab/unit_prefab_native.sqlite3
```

每个资源根只有一个 prefab 数据库，因此不同资源部署使用不同根目录。保留调用者自己的原始版本、manifest 与摘要记录，确认两库和场景确实属于同一部署；不要在会话或 worker 运行期间替换文件。

Host 检查部署路径、prefab schema 与 Unity 版本，消费时拒绝缺表、缺字段或必需行。提取器按 manifest 核对 bundle 的大小和 MD5；masterdata 与 prefab 的来源版本是否一致由部署者确认。

## 分层验证

先完成 [Public 构建](../development/build-and-test.md)。下列可选检查仅使用 Python 3 标准库，只读打开数据库，核对完整性、基础字段和 prefab 元数据。完整 schema、其它资源和内容版本仍需通过后续步骤核对：

```bash
python - "$resource_root" "$master_version" <<'PY'
from contextlib import closing
from pathlib import Path
import sqlite3
import sys

root = Path(sys.argv[1]).resolve()
version = sys.argv[2]
if len(version) != 12 or not version.isascii() or not version.isdigit():
    raise ValueError('Specify a twelve-digit deployment version')
checks = {
    f'masterdata/{version}_unhashed.db': [
        'SELECT unit_id FROM unit_data LIMIT 1',
        'SELECT enemy_id FROM enemy_parameter LIMIT 1',
        'SELECT skill_id, skill_cast_time FROM skill_data LIMIT 1',
    ],
    'prefab/unit_prefab_native.sqlite3': ['SELECT prefab_id, data FROM prefab_tree LIMIT 1'],
}
for relative, queries in checks.items():
    with closing(sqlite3.connect((root / relative).as_uri() + '?mode=ro', uri=True)) as db:
        if db.execute('PRAGMA quick_check').fetchall() != [('ok',)]:
            raise ValueError(f'Database integrity check failed: {relative}')
        for query in queries:
            if db.execute(query).fetchone() is None:
                raise ValueError(f'Missing sample data: {relative}: {query}')
        if relative.startswith('prefab/'):
            meta = dict(db.execute('SELECT key, value FROM prefab_meta'))
            if meta.get('schema_version') != '10' or meta.get('unity_version') != '6000.0.58f2':
                raise ValueError('Unsupported prefab metadata')
    print('Readable:', relative)
PY
```

然后查询实际可选资源；这些查询只检查 masterdata 的对应子集：

```bash
dotnet tools/OpenPCR.BattleEditor/bin/Release/net10.0/OpenPCR.BattleEditor.dll catalog \
  --resources "$resource_root" --master-version "$master_version" --kind units --limit 5
```

对与部署版本匹配的场景做完整准备校验，再运行战斗。下面的示例仅用于 202609081902；其它版本先按目录查询结果创建对应场景：

```bash
dotnet tools/OpenPCR.BattleEditor/bin/Release/net10.0/OpenPCR.BattleEditor.dll validate \
  --resources "$resource_root" --input examples/battles/trial.json

dotnet tools/OpenPCR.Runner/bin/Release/net10.0/OpenPCR.Runner.dll run \
  --resources "$resource_root" --request examples/battles/trial.json \
  --max-engine-frames 5000 --csv-dir /tmp/openpcr-trial
```

准备校验检查输入和产品生成，完整运行检查该场景实际经过的战斗路径；两步都应执行。可选资源集成回归见 [构建与测试](../development/build-and-test.md#自备资源的-worker-回归)。
