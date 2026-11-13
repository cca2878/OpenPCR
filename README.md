# OpenPCR

OpenPCR 是公主连结的 C# 离线战斗推演引擎，支持公会战、训练场和黎明界，面向 11.7.2 的数据格式。通过战斗请求 JSON 提供原始养成、编成和玩法输入，可直接运行或使用交互式会话。支持范围见 [Core I/O 参考](docs/reference/core-io.md#7-当前支持范围)。

构建需要 [global.json](global.json) 指定的 .NET SDK 和 NuGet 依赖。使用 Public 模式：

```bash
dotnet build OpenPCR.slnx -c Release -p:OpenPcrAuditMode=Public
```

运行模拟前，需要按 [资源准备说明](docs/usage/resources.md) 提供与场景版本一致的 masterdata 和 prefab 数据库。项目不附带游戏资源或账号快照；资源服务器地址由使用者自行指定。

- [文档首页](docs/README.md)
- [构建与测试](docs/development/build-and-test.md)
- [运行与资源部署](docs/usage/running.md)
- [黎明界模拟](docs/usage/labyrinth.md)
- [架构概览](docs/architecture/overview.md)
- [场景编辑](docs/tools/battle-editor.md)与[资源提取](docs/tools/prefab-extractor.md)

`OpenPCR.Engine` 负责战斗执行；`OpenPCR.Growth` 负责准备阶段养成计算；`OpenPCR.BattleInputs` 保存交接产品；`OpenPCR.Host` 组织资源、建场和会话；`OpenPCR.Runtime` 提供运行替身；`OpenPCR.Platform` 保存外部依赖的最小边界。运行期间不从原始账号重新计算养成产品。

源码中的 Migration、ImplicitMigration 和 Optimization Attribute 用 UUID 标识声明；分析器检查 ID 格式、重复使用和优化区域结构，见 [源码检查](docs/development/source-checks.md)。Agent 修改项目前请阅读 [AGENTS.md](AGENTS.md)。

## 许可与项目关系

本项目采用 [PolyForm Noncommercial License 1.0.0](LICENSE)（SPDX：`PolyForm-Noncommercial-1.0.0`）。在许可证允许的非商业目的范围内，可使用、修改和分发本项目；具体条件以许可证原文为准。第三方内容保留其原有条款，详见 [许可说明](docs/reference/licensing.md)。

OpenPCR 是独立、非官方的研究项目，与研究对象的开发者、发行商及运营方无官方关联。完整声明见 [NOTICE.md](NOTICE.md)。
