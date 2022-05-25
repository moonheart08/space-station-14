using Content.Shared.FleshyMass;
using Robust.Shared.GameStates;

namespace Content.Client.FleshyMass;

[RegisterComponent, NetworkedComponent, ComponentReference(typeof(SharedFleshBallComponent))]
public sealed class FleshBallComponent : SharedFleshBallComponent
{

}
