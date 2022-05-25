using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.FleshyMass;

/// <summary>
/// Controls the fleshy mass, functioning as a bookkeeping tool for all of it's individual parts.
/// </summary>
[Friend(typeof(SharedFleshyMassSystem))]
public abstract class SharedFleshyMassControllerComponent : Component
{
    [DataField("attachedFleshBalls")]
    public HashSet<EntityUid> FleshBalls = new(); // yumby

    /// <summary>
    /// An adjuster used to avoid the eye being able to outrun the mass early on.
    /// This is adjusted by distance from the edge of the mass. Closer you are to the boundary the higher the extra velocity.
    /// </summary>
    [DataField("extraVisualVelocity")]
    public float ExtraVisualVelocity = 1.0f;

    [DataField("fleshBallPrototype", required: true)]
    public string FleshBallPrototype = default!;
}

[NetSerializable, Serializable]
public sealed class FleshyMassControllerComponentState : ComponentState
{
    public HashSet<EntityUid> FleshBalls;

    public FleshyMassControllerComponentState(SharedFleshyMassControllerComponent component)
    {
        FleshBalls = component.FleshBalls;
    }
}
