using Content.Shared.Placement.SnapModes;
using Robust.Shared.Map;

namespace Content.Shared.Placement;

/// <summary>
/// This handles...
/// </summary>
public abstract class PlacementSystem : EntitySystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {

    }

    public MapCoordinates DoPlacementSnap(MapCoordinates input, EntityUid relativeTo, ISnapMode mode)
    {
        // Allows us to work in a coordinate space that's aligned with the target.
        var relativeCoords = EntityCoordinates.FromMap(relativeTo, input);
        return mode.DoPlacementSnap(relativeCoords).ToMap(EntityManager);
    }
}

