namespace Content.Server.OuterRim.Refinery;

/// <summary>
/// This is used for allowing entities to be assigned a target based on adjacency, which can be cycled through.
/// </summary>
[RegisterComponent, Access(typeof(AutomationSystem))]
public sealed class OuterRimTargetedComponent : Component
{
    [DataField("offset")] public Vector2 Offset = new(0, 1);

    /// <summary>
    /// The entity we are currently targeting.
    /// </summary>
    [DataField("targetEntity"), ViewVariables(VVAccess.ReadWrite)]
    public EntityUid? TargetEntity = null;

    /// <summary>
    /// The "pile" of entities we're looking
    /// </summary>
    [ViewVariables] public List<EntityUid> TargetPile = new();

    public int PileIndex = 0;
}
