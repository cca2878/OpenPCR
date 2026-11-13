using System.Collections.Generic;

namespace OpenPCR.Host;

/// <summary>Explicit deployment version and resource identities; no snapshot or runtime combat state.</summary>
internal record BattleResourceRequest(string MasterVersion, IReadOnlyList<UnitResourceRequest> Units);
