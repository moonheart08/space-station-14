using Robust.Shared.Map;

namespace Content.Shared.Placement.SnapModes;

[DataDefinition]
public sealed class RoundedSnapMode : ISnapMode
{
    [DataField("positionsPerTile"), ViewVariables(VVAccess.ReadWrite)]
    public int PositionsPerTile = 1;

    public EntityCoordinates DoPlacementSnap(EntityCoordinates input)
    {
        return new EntityCoordinates(
            input.EntityId,
            MathF.Round(input.X * PositionsPerTile) / PositionsPerTile, // This has the effect of evenly dividing a tile to have N points at which it'll snap.
            MathF.Round(input.Y * PositionsPerTile) / PositionsPerTile
        );
    }
}
