using Content.Shared.Whitelist;

namespace Content.Server.IngrainedLaws.Components;

[RegisterComponent]
public sealed class StationLawProvisionComponent : Component
{
    [DataField("cachedProvisioners")]
    public HashSet<EntityUid> Provisioners = new();

    [DataField("stationDefaultLaws")]
    public List<LawWhitelistPair> StationDefaultLaws = new();


}
