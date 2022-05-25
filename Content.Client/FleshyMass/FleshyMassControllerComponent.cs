using Content.Shared.FleshyMass;
using Robust.Shared.GameStates;

namespace Content.Client.FleshyMass;

[RegisterComponent, NetworkedComponent, ComponentReference(typeof(SharedFleshyMassControllerComponent))]
public sealed class FleshyMassControllerComponent : SharedFleshyMassControllerComponent
{

}
