namespace OpenPCR;

/// <summary>
/// 表现层空操作，用于结果和副作用只流向渲染、朝向、音效或 UI 的成员，保留原调用签名。
/// 作为成员的唯一语句使用；有返回值时返回 default。
/// </summary>
public static class Presentation
{
	public static void Noop() { }

	public static T Noop<T>() => default!;
}
