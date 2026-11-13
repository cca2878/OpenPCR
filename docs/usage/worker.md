# Worker JSON Lines 协议

Runner 的 `worker` 模式在单个进程中串行执行命令，每个完整帧边界处理一个请求。请求格式见 [战斗请求与结果](battle-request.md)，准备产品所有权见 [战斗输入](../architecture/battle-inputs.md)，部署前置见 [资源准备](resources.md)。先完成 [Public 构建](../development/build-and-test.md)。

```bash
dotnet tools/OpenPCR.Runner/bin/Release/net10.0/OpenPCR.Runner.dll worker
```

## 消息格式

stdin 的每一行是一个完整 JSON 对象；stdout 每行是一个响应。进度及其它诊断在 stderr，客户端须持续消费 stderr 或让其继承终端，避免缓冲区塞满。EOF 释放当前会话、检查点和准备句柄并退出；`shutdown` 返回响应后退出。

```json
{"id":1,"command":"status"}
```

尚未创建会话时，响应为：

```json
{"id":1,"ok":false,"error":{"code":"InvalidOperationException","message":"Create a session first.","faulted":false}}
```

`id` 必须是字符串或数字，由客户端保证请求对应关系；worker 原样返回，不负责去重。消息无法解析或尚未取得 ID 时响应的 `id` 可以为 null。消息中任意层的重复对象键均被拒绝。`command` 和需要时的 `arguments` 均区分大小写。外层参数使用下表的小写形式；`request` 是完整的 `openpcr-battle/v2` 对象（不是文件路径），C# record 的 JSON 字段保持 PascalCase。

成功响应是 `{"id":1,"ok":true,"result":...}`。`result` 随命令变化，进度类返回中有 `EngineFrame`、`BattleFrame`、`TimeRemaining` 和 `Result`。判断是否完成时，检查 `status` 的 `runCompleted`、`exhausted`，以及 `result` 的 `Completed`。

错误的 `code` 是异常类型名，`message` 用于诊断；`faulted=false` 表示会话未被标记为故障；准备句柄是否仍有效，取决于该请求是否已将其消费。`faulted=true` 的会话应先 `dispose` 再重新创建；不要在同一故障对象上继续运行或恢复。

一个会话运行请求描述的整个战斗序列。会战和黎明界普通关卡是一场；黎明界 EX 是至多三队，当前队结束且还有下一队时，`step` 先切到下一队再推进，`run` 一直跑到整场结束或某队用完帧预算；切到下一队时，之前的检查点全部释放。`battle-result`、`inputs`、`terminal-trace`、`capture`／`restore` 作用于当前场。黎明界的 AUTO、速度和 SET 写在请求里，会话不接受 `replace-operations` 和 UB 探索命令。

`terminal-trace` 返回独立的 `openpcr-battle-terminal/v1`，包含当前 Completed 和原结果边界按发生顺序记录的 Events。`request-captured` 表示请求数据已生产，`send-requested` 表示原发送门控已到达；EX 的 `finish-returned` 表示续战／己方产品已写入，训练场另有 `refresh-requested`。事件中的 Point 保留当时的会话 UpdateIndex、原 BattleFrame、Wave、GameState、Result 和 MiliLimitTime。UpdateIndex 是会话更新序号；它不等同于外供 Unity frameCount。读取不推进战斗，也不替代发送门控。检查点恢复同时恢复事件序列。

## 创建和准备命令

| command | arguments | result／约束 |
| --- | --- | --- |
| `create` | `request` 或 `preparedId`（二选一）；`options`；可选 `clock` | 给 `request` 时准备并创建整个序列，从第一场开始；给 `preparedId` 时消费本进程的准备句柄并建场。返回状态 |
| `prepare` | `request`；`resources`：资源根字符串 | 准备一场战斗，返回 `preparedId`、`masterVersion`；黎明界 EX 是序列，不能用 `prepare` |
| `drop-prepared` | `preparedId` | 释放未消费的句柄，返回 `dropped` |

`options` 的完整形式为：

```json
{"Resources":"/data/openpcr-resources","MaxEngineFrames":5000}
```

`MaxEngineFrames` 是每场的宿主帧预算。开场 AUTO、SET 和会战的战斗中操作都来自请求的 `Battles[i]`；运行中可用 `replace-operations` 替换未来操作。

同一进程只有一个活动会话，准备与运行不能重叠。无会话时可准备多个独立产品，再逐个创建、运行和释放。句柄通过参数检查并被取走后，即使建场失败也不退回，需重新准备。`dispose` 释放活动会话和全部检查点，保留未消费的准备句柄；`shutdown` 和 EOF 释放全部状态。资源文件须在进程使用期间保持不变。

`create` 可附加 `arguments.clock`，其值为完整 [BattleClockInput](../../src/OpenPCR.Host/Inputs/BattleClockInput.cs) 对象（`openpcr-battle-clock/v1`），不能传文件路径。时钟放在消息顶层或准备命令中会被拒绝。省略时使用默认 60 Hz；null、重复键及不完整输入被拒绝。使用准备句柄时，时钟校验失败保留原句柄；通过校验并取走句柄后的建场失败仍遵循上述消费规则。序列耗尽使会话 faulted，失败发生在该次 Update 的计数推进之前；输入不会自动延长。检查点保存的是同一不可变序列上的会话游标。详情及短序列示例见 [Update 时钟](../architecture/battle-inputs.md#宿主时钟与诊断重放)。

## 运行、操作和检查点

| command | arguments | result／约束 |
| --- | --- | --- |
| `status` | 无 | `{faulted, progress, completed, exhausted, index, runCompleted}`：当前场的进度与状态、当前场序号、整个序列是否结束；故障时只返回 `{faulted:true}` |
| `step` | `frames`：正整数 | 最多推进指定宿主帧数，返回状态；仍受每场预算限制 |
| `run` | 无 | 运行到序列结束或当前场预算耗尽，返回状态 |
| `result` | 无 | `openpcr-battle-result/v2` 结果：已结束的场与当前场，见 [结果](battle-request.md#结果)；序列未结束时 `Completed` 为 false |
| `battle-result` | 无 | 当前场的 [BattleResult](../../src/OpenPCR.Host/Inputs/BattleResult.cs)，包含 CSV 和 RNG |
| `sequence-state` | 无 | 黎明界 EX 当前队开始时的 `LabyrinthExState`；其它玩法拒绝 |
| `terminal-trace` | 无 | 当前场的 `openpcr-battle-terminal/v1`，见上文 |
| `inputs` | 无 | [PlayerInputState](../../src/OpenPCR.Host/Inputs/PlayerOperation.cs)，包含合法战斗单位 ID、时钟游标和未来操作 |
| `replace-operations` | `operations`：完整的新未来操作数组 | 原子替换全部尚未执行的操作，不是追加；返回更新后的输入视图 |
| `capture` | 无 | `{checkpointId, objectCount, progress}`；只创建本进程内存句柄，不序列化 Native 对象图 |
| `restore` | `checkpointId` | 恢复同一活动会话的图、RNG、操作和游标，返回状态；允许在正常结算后恢复早期检查点 |
| `drop-checkpoint` | `checkpointId` | 释放句柄，返回 `dropped`；之后不能恢复 |
| `dispose` | 无 | `{disposed:true}`；无会话时也可调用 |
| `shutdown` | 无 | `{shutdown:true}`，随后进程以 0 退出 |

操作元素形如：

```json
{"Anchor":"BattleClock","At":10,"Kind":"auto_mode","UnitId":0,"Value":true}
```

`Anchor` 为 `BattleFrame` 或 `BattleClock`，按更新前的计数精确匹配。`BattleFrame` 是战斗帧号；`BattleClock` 是倒计时差除以战斗步长后舍入得到的计数，两者都以帧计。

`Kind` 支持 `auto_mode`、`ub_set_mode`、`manual_ub`；`auto_mode` 的 `UnitId` 必须为 0，`ub_set_mode` 使用 `UnitId` 与 `Value`，`manual_ub` 使用 `UnitId` 且 `Value` 为 false。

从 `inputs` 的 `PlayerUnitIds` 和 `MinimumBattleFrame`／`MinimumBattleClock` 获取当前允许的目标与下界，保持数组内的操作顺序。已过去的时点、无效目标和非法批次会被拒绝；空数组清空未来操作。已结算／耗尽预算后，应先恢复早期检查点再编辑。

## UB 决策探索

| command | arguments | 行为 |
| --- | --- | --- |
| `begin-ub-exploration` | 无 | 清空未来操作并启用显式 UB 决策，返回探索状态 |
| `ub-status` | 无 | 返回 [UbExplorationStatus](../../src/OpenPCR.Host/UbDecision.cs) |
| `advance-ub-decision` | `frames`：正整数 | 在给定宿主帧预算内寻找下一个合法决策；`Decision` 可以为 null，应同时检查完成／耗尽状态 |
| `choose-ub` | `decisionId`；`unitId` | 提交当前决策；单位必须来自 `Decision.UnitIds`，0 表示本帧不使用 UB |
| `end-ub-exploration` | 无 | 返回普通步进，未来操作队列为空 |

探索开启时不能使用普通 `step`／`run` 或编辑操作。`capture` 只允许在已经取得 UB 决策的边界调用；返回的检查点仍通过普通 `restore` 使用。`decisionId` 属于当前决策，不能缓存后跨决策使用；探测计数等诊断元数据不随战斗检查点回退。

## 可运行示例

[worker_session.py](../../examples/worker_session.py) 是仅依赖 Python 3 标准库的最小客户端。它从显式文件读入完整请求，使用 `prepare` → `create` → `capture` → `run` → `result` → `restore` → 再次运行，比较完整结果、CSV 和 RNG，然后释放句柄并退出。脚本设置逐响应超时，并让诊断继承 stderr。

```bash
python examples/worker_session.py \
  --runner tools/OpenPCR.Runner/bin/Release/net10.0/OpenPCR.Runner.dll \
  --resources /data/openpcr-resources --request examples/battles/trial.json \
  --max-engine-frames 5000
```

该示例要求 202609081902 的对应部署。成功输出包括 `Completed: true` 和 `CheckpointReplayEqual: true`，表示该场景两次运行结果一致。

也可以从请求文件读取完整 JSON 对象，通过 `create` 直接准备并建场：

```python
request = json.loads(Path('examples/battles/trial.json').read_text(encoding='utf-8'))
send('create', {
    'request': request,
    'options': {'Resources': '/data/openpcr-resources', 'MaxEngineFrames': 5000},
})
```

`send` 的封装及响应校验见示例脚本。更完整的成功、拒绝、重复准备及状态释放回归见 [公开 Host.Tests](../development/build-and-test.md#自备资源的-worker-回归)。
