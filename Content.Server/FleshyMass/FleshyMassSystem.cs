using Content.Shared.FleshyMass;

namespace Content.Server.FleshyMass;

public sealed class FleshyMassSystem : SharedFleshyMassSystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SharedFleshyMassControllerComponent, ComponentStartup>(OnFleshyMassStartup);
    }

    private void OnFleshyMassStartup(EntityUid uid, SharedFleshyMassControllerComponent component, ComponentStartup args)
    {
        AddMassToController(uid, component, null, true);
        AddMassToController(uid, component, null, true);
        AddMassToController(uid, component, null, true);
        AddMassToController(uid, component, null, true);
        AddMassToController(uid, component, null, true);
        AddMassToController(uid, component, null, true);
        AddMassToController(uid, component, null, true);
        AddMassToController(uid, component, null, true);
        component.FleshBalls.Add(uid);

        foreach (var ball in component.FleshBalls)
        {
            TryRecalculatePartners(ball, uid);
        }
    }
}
