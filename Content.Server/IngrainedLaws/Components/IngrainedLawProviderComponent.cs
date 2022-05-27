using Content.Shared.Whitelist;

namespace Content.Server.IngrainedLaws.Components;

/// <summary>
/// Provides laws to anything capable of receiving them that matches the whitelist.
/// </summary>
[RegisterComponent]
public sealed class IngrainedLawProviderComponent : Component
{
    /// <summary>
    /// The laws to provide and their associated whitelist.
    /// </summary>
    [DataField("lawSets", required: true)]
    public List<LawWhitelistPair> LawSets = new();
}

[DataDefinition]
public sealed class LawWhitelistPair
{
    [DataField("whitelist", required: true)]
    public EntityWhitelist Whitelist = default!;

    [DataField("lawSet", required: true)]
    public LawSet LawSet = default!;
}

public enum ProvisioningScope
{
    Station,
    Global
}

