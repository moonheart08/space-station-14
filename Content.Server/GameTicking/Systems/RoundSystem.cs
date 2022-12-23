using System.Linq;
using System.Threading.Tasks;
using Content.Server.Database;
using Content.Server.GameTicking.Components;
using Content.Shared.CCVar;
using Prometheus;
using Robust.Shared.Asynchronous;
using Robust.Shared.Configuration;
using Robust.Shared.Map;

namespace Content.Server.GameTicking.Systems;

/// <summary>
/// RoundSystem handles the overall flow of a "round" within the game.
/// </summary>
public sealed class RoundSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly ILogManager _log = default!;
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly ITaskManager _task = default!;

    private static readonly Counter RoundNumberMetric = Metrics.CreateCounter(
        "ss14_round_number",
        "Round number.");

    private static readonly Gauge RoundLengthMetric = Metrics.CreateGauge(
        "ss14_round_length",
        "Round length in seconds.");

    private ISawmill _sawmill = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        _sawmill = _log.GetSawmill("round");
    }

    public override void Update(float frameTime)
    {
        var round = GetCurrentRound();
        if (round is null)
            return;

        RoundLengthMetric.Inc(frameTime);
    }

    public bool TryStartRound(RoundConfig config = new(), int depth = 0)
    {
        var round = MakeNewRound(config.RoundEntityPrototype);
    }

    private EntityUid MakeNewRound(string? prototype, Guid[] playerIds)
    {
        prototype ??= _cfg.GetCVar<string>(CCVars.GameRoundPrototype);
        var round = Spawn(prototype, MapCoordinates.Nullspace);
        var comp = Comp<RoundComponent>(round);
        comp.RoundId = GetNewRoundId()
        return round;
    }

    public EntityUid? GetCurrentRound()
    {
        var rounds = EntityQuery<RoundComponent>().ToList();
        if (rounds.Count > 1)
        {
            // Oooooooh fuck.
            throw new Exception($"Oh god. There's more than one round. If you're planning to implement multi-round god bless your soul. {string.Join(", ", rounds)}");
        }

        if (rounds.Count == 1)
            return rounds[0].Owner;

        return null;
    }

    private int GetNewRoundId(Guid[] playerIds)
    {
        var serverName = _cfg.GetCVar(CCVars.AdminLogsServerName);
        // TODO FIXME AAAAAAAAAAAAAAAAAAAH THIS IS BROKEN
        // Task.Run as a terrible dirty workaround to avoid synchronization context deadlock from .Result here.
        // This whole setup logic should be made asynchronous so we can properly wait on the DB AAAAAAAAAAAAAH
        // -- I didn't write this PJB did --moony
        var task = Task.Run(async () =>
        {
            var server = await _db.AddOrGetServer(serverName);
            return await _db.AddNewRound(server, playerIds);
        });

        _task.BlockWaitOnTask(task);
        var id = task.GetAwaiter().GetResult();
        RoundNumberMetric.IncTo(id);
        return id;
    }
}

public record struct RoundConfig(string? RoundEntityPrototype = null, bool ForceAllPlayersJoin = false);

