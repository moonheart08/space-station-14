using Robust.Shared.Serialization;

namespace Content.Shared.VacuumDecay;

/// <summary>
/// This is used for...
/// </summary>
public sealed  class VacuumDecayComponent : Component
{

}

/// <summary>
/// Contains network state for VacuumDecayComponent.
/// </summary>
[Serializable, NetSerializable]
public sealed class VacuumDecayComponentState : ComponentState
{
    public VacuumDecayComponentState(VacuumDecayComponent component)
    {

    }
}
