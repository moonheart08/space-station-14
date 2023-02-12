using Content.Shared.Administration;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Client.Guidebook;

public partial class GuideEntry
{
    [DataField("requiresAdmin")]
    public bool RequiresAdmin = false;
}
