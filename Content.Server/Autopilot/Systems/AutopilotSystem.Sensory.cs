using Content.Server.Autopilot.Components;
using Content.Shared.Physics;
using JetBrains.Annotations;
using Robust.Shared.Map;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Utility;

namespace Content.Server.Autopilot.Systems;

public sealed partial class AutopilotSystem
{
    private void InitializeSensors()
    {

    }

    private void UpdateSensors(float frameTime)
    {
        foreach (var (rangeSensor, xform) in EntityQuery<RangeSensorComponent, TransformComponent>())
        {
            if (xform.GridID == GridId.Invalid)
                continue; // don't even bother.

            rangeSensor.Accumulator += frameTime;

            if (rangeSensor.Accumulator < rangeSensor.UpdateRate)
                continue;

            rangeSensor.Accumulator -= rangeSensor.UpdateRate;
            var ray = new CollisionRay(xform.WorldPosition, xform.WorldRotation.ToVec(),
                (int) (CollisionGroup.Opaque | CollisionGroup.Impassable));
            var intersections = _physicsSystem.IntersectRay(xform.MapID, ray, rangeSensor.MaximumRange);
            var intersection = intersections.FirstOrNull();
            if (intersection is null)
            {
                if (rangeSensor.PriorDistance != null)
                {
                    var ev = new RangeSensorStateChangeEvent(rangeSensor.Owner, rangeSensor.Class, null);
                    RaiseLocalEvent(_mapManager.GetGridEuid(xform.GridID));
                }
                continue;
            }

            if ( )
        }
    }
}

[PublicAPI]
public readonly ref struct RangeSensorStateChangeEvent
{
    public readonly EntityUid HitSensor;
    public readonly string Class;
    public readonly float? Distance;

    public RangeSensorStateChangeEvent(EntityUid hitSensor, string @class, float? distance)
    {
        HitSensor = hitSensor;
        Class = @class;
        Distance = distance;
    }
}
