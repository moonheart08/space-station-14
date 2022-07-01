using Content.Shared.GameTicking;
using Robust.Shared.Map;

namespace Content.Shared.Datum;

/// <summary>
/// This handles managing datums, entities that are used solely for data storage purposes.
/// Datums do not currently support surviving between rounds.
/// </summary>
public sealed class DatumSystem : EntitySystem
{
    [Dependency] private readonly IMapManager _mapManager = default!;

    [ViewVariables]
    private EntityUid _datumStorageMap = EntityUid.Invalid;

    /// <inheritdoc/>
    public override void Initialize()
    {
        _datumStorageMap = _mapManager.GetMapEntityId(_mapManager.CreateMap()); // Just create it. If we're the client, we need our own clientside storage map, if we're the server, this is already unique.
        SubscribeLocalEvent<GameRunLevelChangedEvent>(GameEnterPreRoundLobbyHandler);
        SubscribeLocalEvent<RoundRestartCleanupEvent>(RoundRestartCleanupHandler);
    }

    public overrid

    private void GameEnterPreRoundLobbyHandler(GameRunLevelChangedEvent msg)
    {

    }

    private void RoundRestartCleanupHandler(RoundRestartCleanupEvent ev)
    {
        // Just build a new map. Unfortunately we cannot reuse the map between rounds at the moment, restricting the ability to have cross-round datums.
        _datumStorageMap = _mapManager.GetMapEntityId(_mapManager.CreateMap());
    }

    public EntityUid SpawnDatum(string? prototype = null)
    {
        return Spawn(prototype, new EntityCoordinates(_datumStorageMap, 0, 0));
    }
}
