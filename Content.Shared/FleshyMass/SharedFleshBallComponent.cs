using Robust.Shared.Serialization;

namespace Content.Shared.FleshyMass;

[Friend(typeof(SharedFleshyMassSystem))]
public abstract class SharedFleshBallComponent : Component
{
    [DataField("state")] public FleshBallState State;

    /// <summary>
    /// This is used in state FootUp to control which way it's moving and what it's target velocity is.
    /// We do NOT netsync this. Client has authority, in fact. The mass is weighty enough that this doesn't matter much.
    /// </summary>
    [DataField("movementVector")]
    public Vector2 MovementVector = Vector2.One * 5;
    [DataField("controller")]
    public EntityUid Controller;
    [DataField("jointLength")]
    public float JointLength = 1.25f;
}

[NetSerializable, Serializable]
public sealed class FleshBallComponentState : ComponentState
{
    public FleshBallState State;
    public EntityUid Controller;

    public FleshBallComponentState(SharedFleshBallComponent component)
    {
        State = component.State;
        Controller = component.Controller;
    }
}


public enum FleshBallState : byte
{
    /// <summary>
    /// Signals that this ball is functioning like a foot, and holding position.
    /// </summary>
    FootDown = 0,
    /// <summary>
    /// Indicates that this ball is functioning like a foot, and moving forwards toward the moving direction.
    /// </summary>
    FootUp,
    /// <summary>
    /// Indicates this ball is virtual, and controls the mass.
    /// This is used so that the controller cannot go too far from the body.
    /// </summary>
    Controller,
}
