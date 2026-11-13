# 构建与测试

以下命令在仓库根目录执行，SDK 版本见 `global.json`。默认构建模式为 `Public`，以下命令也显式指定该模式。构建无需游戏数据。

## 构建

```bash
dotnet build OpenPCR.slnx -c Release -p:OpenPcrAuditMode=Public
dotnet build OpenPCR.slnx -c ReleaseOptimized -p:OpenPcrAuditMode=Public
```

两种配置的输出分别位于各项目的 `bin/Release` 和 `bin/ReleaseOptimized`。优化结构约束见 [源码检查](source-checks.md)；结构检查不能替代行为比较。

## 开发环境中的模式

构建、格式化和 IDE 默认使用 `Public`。如需在命令行指定模式，使用 `-p:OpenPcrAuditMode=Public`；格式化可使用 `OpenPcrAuditMode=Public dotnet format OpenPCR.slnx --no-restore`。

## 不需要资源的测试

先完成 Release 构建：

```bash
dotnet run --no-build -c Release --project tests/OpenPCR.Transcription.Tests
dotnet run --no-build -c Release --project tests/OpenPCR.Runtime.Tests
dotnet run --no-build -c Release --project tests/OpenPCR.Checkpoint.Tests
dotnet run --no-build -c Release --project tests/OpenPCR.Host.Tests
dotnet run --no-build -c Release --project tests/OpenPCR.Growth.Tests
dotnet run --no-build -c Release --project tests/OpenPCR.PrefabExtractor.Tests
dotnet run --no-build -c Release --project tests/OpenPCR.Bindings.Tests
python tools/tests/test_battle_editor.py -v
python tools/tests/test_labyrinth_editor.py -v
dotnet tools/OpenPCR.Runner/bin/Release/net10.0/OpenPCR.Runner.dll schema --check docs/reference/schemas
```

两个 Python 编辑器测试只运行无资源用例，跳过资源集成部分；`schema --check` 确认 `docs/reference/schemas/` 与代码生成的 schema 一致，改动请求或时钟的记录类型后用 `schema --write docs/reference/schemas` 重新生成。其它项目使用合成数据或数值测试样本。编辑器的资源集成参数见 [编辑器说明](../tools/battle-editor.md)。

Host 默认包含活动发布、协程激活、动画分派、视频播放边界、结算输出及多目标演出的显式开关契约测试；`-- --list` 列出用例组，`-- --only <组名>[,<组名>...]` 只运行指定的组（`-- --activity` 等同于 `--only SceneActivity`）。一个断言失败后其余用例组照常运行，结束时汇总失败项并以退出码 1 返回。测试项目是控制台程序，应使用 `dotnet run`；`dotnet test` 不是这些断言的运行入口。

Bindings.Tests 绑定 Host、Growth 和 BattleInputs 源码里的每个 `RuntimeBinding.Set`／`Get`／`Call` 调用点，按接收者类型解析字面量成员名（含私有成员）；成员改名或删除后这里报出文件和行号。运行时拼出的成员名不在检查范围内，输出里单独计数。源码从仓库根读取，输出目录不在仓库内时用 `-- --root <仓库根>` 指定。

## 自备资源的 Worker 回归

先完成同一配置的 Runner 和 Host.Tests 构建。在仓库根目录执行：

```bash
dotnet run --no-build -c Release --project tests/OpenPCR.Host.Tests -- \
  <resources> --preparation-worker \
  tools/OpenPCR.Runner/bin/Release/net10.0/OpenPCR.Runner.dll \
  examples/battles/trial.json
```

资源必须匹配所提供的场景；示例固定使用 202609081902。用例覆盖准备句柄、建场、非法请求、产品隔离、完整运行，以及检查点恢复前后的结果一致性。默认无参数测试不搜索资源或运行这些集成用例。

验证优化版时，将测试命令的 `-c Release` 和 Runner 路径中的 `Release` 一起改为 `ReleaseOptimized`。除分别运行用例外，还应对相同场景、操作和资源运行两版 Runner，逐字节比较 CSV，并比较 `run` 结果 JSON 中每场的 `RandomState` 和 `Settlement`。不要只按耗时或总伤害判断等价。场景协议与 Worker 示例见 [Worker](../usage/worker.md)。

## 格式化

```bash
OpenPcrAuditMode=Public dotnet format OpenPCR.slnx --no-restore
```

检查格式而不写文件可加 `--verify-no-changes`。格式化后检查 diff，并确认没有项目加载错误。不要自动应用会改变模型成员命名、访问性、布局、签名或数值顺序的建议。

资源部署和模拟命令见 [运行指南](../usage/running.md)，模块关系见 [架构概览](../architecture/overview.md)。
