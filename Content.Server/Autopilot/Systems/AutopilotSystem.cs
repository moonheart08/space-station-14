using Robust.Server.GameObjects;
using Robust.Shared.Map;

namespace Content.Server.Autopilot.Systems;

public sealed partial class AutopilotSystem : EntitySystem
{
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly PhysicsSystem _physicsSystem = default!;

    public override void Initialize()
    {
        InitializeSensors();

    }

    public override void Update(float frameTime)
    {
        UpdateSensors(frameTime);
    }
}
