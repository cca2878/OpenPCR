using System;

namespace OpenPCR;

/// <summary>
/// 调用到了抽取边界之外的成员。
/// </summary>
/// <remarks>
/// 它表示「该成员在目标 Native 里有实现，但不在当前静态迁移范围内」。
/// 核心待迁移实现使用 MigrationPendingException，不能以本异常掩盖缺口。
/// 此入口不属于当前支持的战斗范围；不要捕获后继续执行。
/// </remarks>
public sealed class OutOfBoundaryException : Exception
{
	/// <summary>被调用的成员，形如 <c>命名空间.类型.成员</c>。</summary>
	public string Member { get; }

	public OutOfBoundaryException(string member)
		: base($"{member} 不在当前支持的战斗范围内。")
	{
		Member = member;
	}
}
