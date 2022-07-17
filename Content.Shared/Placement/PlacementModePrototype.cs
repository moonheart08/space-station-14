using Robust.Shared.Prototypes;

namespace Content.Shared.Placement;

/// <summary>
/// This is a prototype for...
/// </summary>
[Prototype("placementMode")]
public sealed class PlacementModePrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; } = default!;

    [DataField("name", required: true)]
    public string Name { get; } = default!;
}
