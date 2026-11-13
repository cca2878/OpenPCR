# 提交规范

提交须遵守 [Conventional Commits 1.0.0](https://www.conventionalcommits.org/en/v1.0.0/)。本仓库仅保留项目的提交要求，不附规范全文。

标题使用 `type(scope): description`，scope 可省略。新增功能使用 feat，缺陷修复使用 fix；文档、测试、重构、构建或维护等改动使用对应类型。破坏兼容性的改动须在类型／范围后加 `!`，或在正文尾部提供 `BREAKING CHANGE:` 说明。

按功能拆分提交，标题说明具体变化，必要时在正文交代原因、兼容性影响和验证结果。例如：

```text
fix(host): reject ambiguous effect resource IDs
docs: clarify resource deployment inputs
```

开发、验证和暂存要求见 [AGENTS.md](../../AGENTS.md)。
