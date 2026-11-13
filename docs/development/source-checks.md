# 源码登记与检查边界

Migration、ImplicitMigration 和 Optimization Attribute 各保存一个稳定的随机 UUID v4，用于标识源码声明。

```bash
dotnet build OpenPCR.slnx -c Release -p:OpenPcrAuditMode=Public
```

公开分析器检查两个编译视图中的 ID 格式和可见的重复 ID、优化区域及成员覆盖。PCR001 表示登记或配置错误；PCR005 表示优化结构错误。请不要禁用检查器、压制这些诊断或定义 OPENPCR_EMIT_AUDIT_METADATA。

审计元数据符号不能通过项目编译选项或文件内的 `#define` 定义，参照视图和优化视图都会检查。

Public 模式检查源码结构；战斗行为仍需通过测试和运行结果比较来验证。

修改已有登记的正文时保留其 UUID；新增独立登记时生成新的随机 UUID v4，例如 `python -c "import uuid; print(uuid.uuid4())"` 或 C# 的 `Guid.NewGuid().ToString("D")`。不要从类型名、来源、内容摘要或旧 ID 推导新 ID，也不要把其它声明的 ID 复制过来。重命名或移动已有登记时保留原 ID。

提交贡献时请附上相关测试和实际验证范围。开发环境的 Public 模式设置见 [构建与测试](build-and-test.md#开发环境中的模式)。

ReleaseOptimized 只允许分析器认可的 OPENPCR_OPTIMIZED 区域。替换正文应保留整个参照正文，新成员必须独立登记并符合访问性和成员种类约束。优化版与参照版的行为一致性通过相关测试和运行结果比较来验证。

检查按预处理语法和属性绑定识别登记，Unicode 标识符转义和类型别名不改变规则。属性上的登记可覆盖单个完整访问器正文的替换；新增属性的显式访问器随属性一并检查。指向隐式字段或事件访问器的标记不能代替合法的优化区域登记。
