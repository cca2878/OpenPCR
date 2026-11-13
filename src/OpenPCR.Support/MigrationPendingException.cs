using System;

namespace OpenPCR;

/// <summary>目标声明已经迁移，核心实现尚未完成；不能视为范围外或吞掉本异常。</summary>
public sealed class MigrationPendingException : Exception
{
	public string Member { get; }
	public string Reason { get; }

	public MigrationPendingException(string member, string reason)
		: base($"{member} 尚未完成 11.7.2 实现迁移：{reason}")
	{
		Member = member;
		Reason = reason;
	}
}
