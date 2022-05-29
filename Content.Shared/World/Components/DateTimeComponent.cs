using System.Linq;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.World.Components;

/// <summary>
/// Contains orbital period, day/night, and other information about the map's time of day.
/// </summary>
public abstract class DateTimeComponent : Component
{
    [DataField("orbitalPeriod")]
    public TimeSpan OrbitalPeriod = default!;

    [DataField("timePeriods")]
    public List<TimePeriod> TimePeriods = default!;
}

/// <summary>
/// Contains network state for DateTimeComponent.
/// </summary>
[NetSerializable, Serializable]
public sealed class DateTimeComponentState : ComponentState
{
    public TimeSpan OrbitalPeriod = default!;
    public List<TimePeriod> TimePeriods = default!;

    public DateTimeComponentState(DateTimeComponent component)
    {
        OrbitalPeriod = component.OrbitalPeriod;
        TimePeriods = component.TimePeriods.ToList(); // Clone it so we don't corrupt state.
    }
}

[DataDefinition]
public struct OrbitalConfiguration
{
    [DataField("orbitalPeriod")]
    public TimeSpan OrbitalPeriod;
    [DataField("dayPeriod")]
    public TimeSpan DayPeriod;
    [DataField("nightPeriod")]
    public TimeSpan NightPeriod;

    public OrbitalConfiguration(TimeSpan orbitalPeriod, TimeSpan dayPeriod, TimeSpan nightPeriod)
    {
        OrbitalPeriod = orbitalPeriod;
        DayPeriod = dayPeriod;
        NightPeriod = nightPeriod;
    }
};

/// <summary>
/// An event fired off when the time of day changes to another period.
/// </summary>
/// <remarks>
/// MUST be network serializable, MUST be shared code!
/// </remarks>
[NetSerializable, Serializable]
public abstract class TimeChangeEvent : EntityEventArgs, IEquatable<TimeChangeEvent>
{
    public abstract bool Equals(TimeChangeEvent? other);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TimeChangeEvent) obj);
    }

    public abstract override int GetHashCode();
}

[NetSerializable, Serializable, DataDefinition]
public sealed class DayTimeChangeEvent : TimeChangeEvent
{
    public readonly EntityUid Map;

    public DayTimeChangeEvent(EntityUid map)
    {
        Map = map;
    }

    public override bool Equals(TimeChangeEvent? other)
    {
        return other is DayTimeChangeEvent e && e.Map == Map; // We store no information.
    }

    public override int GetHashCode()
    {
        return Map.GetHashCode();
    }
}

/// <summary>
/// Event fired when the time changes to night.
/// </summary>
[NetSerializable, Serializable, DataDefinition]
public sealed class NightTimeChangeEvent : TimeChangeEvent
{
    public readonly EntityUid Map;

    public NightTimeChangeEvent(EntityUid map)
    {
        Map = map;
    }

    public override bool Equals(TimeChangeEvent? other)
    {
        return other is NightTimeChangeEvent e && e.Map == Map;
    }

    public override int GetHashCode()
    {
        return Map.GetHashCode();
    }
}


[DataDefinition]
public struct TimePeriod
{
    [DataField("timeSpan")]
    public TimeSpan TimeSpan;
    [DataField("timeChangedEvent")]
    public TimeChangeEvent TimeChangedEvent;

    public TimePeriod(TimeSpan timeSpan, TimeChangeEvent timeChangedEvent)
    {
        TimeSpan = timeSpan;
        TimeChangedEvent = timeChangedEvent;
    }
}
