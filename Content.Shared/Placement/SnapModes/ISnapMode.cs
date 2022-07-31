using Robust.Shared.Map;

namespace Content.Shared.Placement.SnapModes;

public interface ISnapMode
{
    public EntityCoordinates DoPlacementSnap(EntityCoordinates input);
}
