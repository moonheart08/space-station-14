using Content.Shared.World.Components;
using Robust.Shared.Timing;

namespace Content.Shared.World.Systems;

/// <summary>
/// Handles the local time for a map. This means orbital period, day/night period, all that jazz.
/// </summary>
public abstract class DateTimeSystem : EntitySystem
{
    [Dependency] protected readonly IGameTiming _gameTiming = default!;

    /// <inheritdoc/>
    public override void Update(float frameTime)
    {
        foreach (var dateTime in EntityQuery<DateTimeComponent>())
        {

        }
    }
}
