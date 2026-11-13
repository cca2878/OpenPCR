# OpenPCR 项目规则

本文件适用于本仓库；子目录规则按其目录范围适用。

## 架构与行为

- `src/OpenPCR.Engine/` 是战斗模型与核心执行，`src/OpenPCR.Runtime/` 是统一运行替身，两者保持 C# 9。保持原有的类型边界、访问性、成员签名和布局，不为调用方便新增接口或共享状态。
- `OpenPCR.Platform` 放外部依赖的最小边界，`OpenPCR.Support` 放辅助代码。不支持的入口直接抛错，不用默认值顶替。
- Growth 只在准备阶段计算，BattleInputs 交付产品，Host 负责建场和运行。Engine 不引用 Growth，也不从账号原始数据、数据库或计算器回调重建产品。
- 保持 RNG 消费、浮点运算顺序、时钟、暂停、回调、对象池和协程顺序。省略表现层时，保留战斗逻辑仍会读取的状态和生命周期。
- OpenPCR 是推演引擎：输入和资源路径由调用方显式提供，缺失时报错；不读取墙钟、服务器或本地账号。账号等非游戏内置资源的数据由调用方给出，资源版本是否配套由部署者负责。

## 登记与优化

- Migration、ImplicitMigration、Optimization 标记保存稳定的 UUID v4；已有 ID 不改，也不复用别的登记项的 ID。
- 默认构建模式为 Public；公开构建可显式使用 `-p:OpenPcrAuditMode=Public`。不要禁用分析器、压制 PCR001/PCR005，或定义 OPENPCR_EMIT_AUDIT_METADATA。
- 性能优化写在 OPENPCR_OPTIMIZED 区域内，并保留完整的参照分支；新增成员遵守分析器约束。Release 是参照版，ReleaseOptimized 需要通过对应检查和运行比较。

## 开发与提交

- 修改前先看工作树，保留无关改动。改了工具，顺带测一下它会拒绝的输入。
- 文档和注释写当前的接口、限制和行为，保留必要的来源署名与许可信息。报告验证结果时说明实际跑了什么。
- 提交前按 [构建与测试](docs/development/build-and-test.md) 设置构建模式、运行格式化，检查 diff 并做相关验证。精确暂存，不用 `git add -A` 或 `git add .`。
- 提交遵守 [提交规范](docs/standards/README.md) 的 Conventional Commits。读写同一程序集输出的构建和检查按顺序执行。

命令见 [构建与测试](docs/development/build-and-test.md)，场景与产品契约见 [战斗输入](docs/architecture/battle-inputs.md) 和 [Core I/O 参考](docs/reference/core-io.md)。
