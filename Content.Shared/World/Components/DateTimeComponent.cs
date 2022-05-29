using Robust.Shared.Serialization;

namespace Content.Shared.World.Components;

/// <summary>
/// Contains orbital period, day/night, and other information about the map's time of day.
/// </summary>
public abstract class DateTimeComponent : Component
{
    [DataField("")]
    public TimeSpan OrbitalPeriod = default!;
}

/// <summary>
/// Contains network state for DateTimeComponent.
/// </summary>
[NetSerializable, Serializable]
public sealed class DateTimeComponentState : ComponentState
{
    public DateTimeComponentState(DateTimeComponent component)
    {

    }
}
