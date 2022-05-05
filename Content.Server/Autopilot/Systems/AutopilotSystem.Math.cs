using Content.Server.Shuttles.Components;

namespace Content.Server.Autopilot.Systems;

public sealed partial class AutopilotSystem
{
    public float EstimateStoppingDistance(EntityUid grid, Vector2 direction, PhysicsComponent? physicsComponent = null, ShuttleComponent? shuttleComponent = null)
    {
        var opposing = new Angle(180).RotateVec(direction);
    }
}
