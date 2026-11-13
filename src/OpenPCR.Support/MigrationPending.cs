namespace OpenPCR;

/// <summary>Explicit structural-migration stub helpers.  Every overload
/// throws; no default value can accidentally become a behavior implementation.</summary>
public static class MigrationPending
{
	public static T Throw<T>(string member, string reason) =>
		throw new MigrationPendingException(member, reason);

	public static void Throw(string member, string reason) =>
		throw new MigrationPendingException(member, reason);
}
