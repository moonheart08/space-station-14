using Robust.Shared.GameStates;
using Robust.Shared.Physics;
using Robust.Shared.Random;

namespace Content.Shared.FleshyMass;

public abstract partial class SharedFleshyMassSystem : EntitySystem
{
    [Dependency] protected readonly ILogManager _logManager = default!;
    [Dependency] protected readonly IRobustRandom _random = default!;
    [Dependency] protected readonly SharedJointSystem _jointSystem = default!;
    protected ISawmill _sawmill = default!;

    public override void Initialize()
    {
        _sawmill = _logManager.GetSawmill("fleshymass");
        SubscribeLocalEvent<SharedFleshBallComponent, ComponentGetState>(GetFleshBallComponentState);
        SubscribeLocalEvent<SharedFleshBallComponent, ComponentHandleState>(HandleFleshBallComponentState);
        SubscribeLocalEvent<SharedFleshyMassControllerComponent, ComponentGetState>(GetFleshyMassControllerComponentState);
        SubscribeLocalEvent<SharedFleshyMassControllerComponent, ComponentHandleState>(HandleFleshyMassControllerComponentState);
        InitializeMovement();
    }

    public override void Update(float frameTime)
    {
        base.FrameUpdate(frameTime);
        foreach (var fleshBall in EntityQuery<SharedFleshBallComponent>())
        {
             UpdateFleshBall(fleshBall.Owner, fleshBall);
        }
    }

    private void GetFleshyMassControllerComponentState(EntityUid uid, SharedFleshyMassControllerComponent component, ref ComponentGetState args)
    {
        args.State = new FleshyMassControllerComponentState(component);
    }

    private void HandleFleshyMassControllerComponentState(EntityUid uid, SharedFleshyMassControllerComponent component, ref ComponentHandleState args)
    {
        var newState = (FleshyMassControllerComponentState)args.Next!;
        component.FleshBalls = newState.FleshBalls;
    }

    private void GetFleshBallComponentState(EntityUid uid, SharedFleshBallComponent component, ref ComponentGetState args)
    {
        args.State = new FleshBallComponentState(component);
    }

    private void HandleFleshBallComponentState(EntityUid uid, SharedFleshBallComponent component, ref ComponentHandleState args)
    {
        var newState = (FleshBallComponentState) (args.Next!);
        component.State = newState.State;
    }

    public void AddMassToController(EntityUid uid, SharedFleshyMassControllerComponent? controllerComponent = null, TransformComponent? transformComponent = null, bool avoidJointUpdate = false)
    {
        if (!Resolve(uid, ref controllerComponent, ref transformComponent))
            throw new ArgumentException("Tried to add mass to a non-fleshy-mass-controller entity.");

        var newMass = Spawn(controllerComponent.FleshBallPrototype, transformComponent.Coordinates.Offset(_random.NextVector2(0.5f)));
        var ball = Comp<SharedFleshBallComponent>(newMass);
        ball.Controller = uid;
        controllerComponent.FleshBalls.Add(newMass);

        if (!avoidJointUpdate)
            TryRecalculatePartners(newMass, controllerComponent.Owner);
    }
}
