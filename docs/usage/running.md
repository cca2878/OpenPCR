# 运行与资源部署

先完成 [构建](../development/build-and-test.md)。运行输入、资源路径和版本由调用者指定。命令中的 `<resources>` 和 `<version>` 是占位符，执行前替换为实际路径和版本。

## 部署资源

在指定的资源根目录中按以下路径放置数据库：

```text
<resources>/
  masterdata/<version>_unhashed.db
  prefab/unit_prefab_native.sqlite3
```

masterdata、prefab 和场景应来自同一版本。程序只读打开数据库，检查文件路径、prefab schema 和 Unity 版本；缺失文件或不支持的格式会报错。文件内容的版本一致性由部署者确认，重命名文件不能转换其版本。

资源不随项目提供；[资源准备与校验](resources.md) 说明 masterdata 的前置格式、两库配对和逐层验证。[PrefabExtractor](../tools/prefab-extractor.md) 从调用者的 bundle 和 manifest 生成 prefab 数据库；项目没有 masterdata 解密或表名还原工具。网络操作必须显式提供 HTTPS 地址。

## 创建和运行场景

请求格式见 [战斗请求与结果](battle-request.md)，完整编辑选项见 [BattleEditor](../tools/battle-editor.md)。

```bash
dotnet tools/OpenPCR.BattleEditor/bin/Release/net10.0/OpenPCR.BattleEditor.dll create \
  --resources <resources> --master-version <version> --preset trial-basic \
  --units 100101 --battle-id 1001 --phase 1 --order 1 --output /tmp/battle.json

dotnet tools/OpenPCR.Runner/bin/Release/net10.0/OpenPCR.Runner.dll run \
  --resources <resources> --request /tmp/battle.json \
  --max-engine-frames 20000 --csv-dir /tmp/battle-logs
```

单位 ID 和会战场景必须存在于所选版本的数据中；敌人、时限和背景由场景推导。Runner 在 stdout 输出 `openpcr-battle-result/v2` 结果，`--csv-dir` 为每场写 `battle-<序号>.csv`。示例 ID 使用 202609081902 数据集；使用其它版本时，先通过 BattleEditor 的 `catalog` 查询可用 ID。

## 交互会话

```bash
dotnet tools/OpenPCR.Runner/bin/Release/net10.0/OpenPCR.Runner.dll worker
```

Worker 从 stdin 读取 JSON Lines，通过 stdout 返回同 ID 的响应，诊断输出到 stderr。主要命令包括 prepare、create、step、run、status、result、inputs、capture、restore、drop-checkpoint、dispose 和 shutdown；还提供 UB 决策探索。完整示例见 [Worker 协议](worker.md)。同一进程只支持一个活动会话；检查点是当前进程的内存句柄。

## 可选资源集成测试

当前 BattleEditor 集成用例要求固定的 202609081902 数据集，其中包含特定单位、敌人、背景和 Rank 38 装备槽断言：

```bash
python tools/tests/test_battle_editor.py \
  --resources <resources> --master-version 202609081902 -v
```

两个参数必须同时提供，资源不存在或版本不支持时会报错。回归结果仅覆盖所选数据集和用例。
