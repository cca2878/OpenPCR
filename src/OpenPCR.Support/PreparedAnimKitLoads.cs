using System;

namespace OpenPCR;

/// <summary>Explicit numerical animation resource producer. Loading does not run a second battle engine.</summary>
/// <remarks>
/// One process-wide slot: Engine call sites pass no session owner to key on, so this relies on the
/// single-active-session rule that BattleSession enforces through <see cref="IsInitialized"/>.
/// </remarks>
public static class PreparedAnimKitLoads
{
	private static Func<int, int, int, int, object, int, object>? producer;
	private static Action<object, int, int>? animation;
	public static bool IsInitialized => producer is not null;
	public static void Bind(Func<int, int, int, int, object, int, object> create, Action<object, int, int>? requireAnimation = null)
	{
		if (producer is not null) throw new InvalidOperationException("An animation producer is already bound.");
		producer = create ?? throw new ArgumentNullException(nameof(create));
		animation = requireAnimation;
	}
	public static object Create(int type, int skin, int controllerSkin, int enemyColor, object parent, int prefabSkin)
		=> (producer ?? throw new InvalidOperationException("Missing prepared animation resource producer."))(type, skin, controllerSkin, enemyColor, parent, prefabSkin)
		   ?? throw new InvalidOperationException("Animation producer returned null.");
	public static void LoadAnimation(object controller, int type, int index) =>
		(animation ?? throw new InvalidOperationException("Missing prepared animation bundle input."))(controller, type, index);
	public static void Release() { producer = null; animation = null; }
}
