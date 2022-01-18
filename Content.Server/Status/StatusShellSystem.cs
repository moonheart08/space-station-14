using System;
using System.Text.Json.Nodes;
using Content.Server.GameTicking;
using Robust.Server;
using Robust.Server.Player;
using Robust.Server.ServerStatus;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.ViewVariables;

namespace Content.Server.Status;

public class StatusShellSystem : EntitySystem
{
    [Dependency] private readonly IBaseServer _baseServer = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly GameTicker _gameTicker = default!;

    /// <summary>
    ///     Used for thread safety, given <see cref="IStatusHost.OnStatusRequest"/> is called from another thread.
    /// </summary>
    private readonly object _statusShellLock = new();

    public override void Initialize()
    {
        IoCManager.Resolve<IStatusHost>().OnStatusRequest += GetStatusResponse;
    }

    private void GetStatusResponse(JsonNode jObject)
    {
        // This method is raised from another thread, so this better be thread safe!
        lock (_statusShellLock)
        {
            jObject["name"] = _baseServer.ServerName;
            jObject["players"] = _playerManager.PlayerCount;
            jObject["run_level"] = (int) _gameTicker.RunLevel;
            if (_gameTicker.RunLevel >= GameRunLevel.InRound)
            {
                jObject["round_start_time"] = _gameTicker.RoundStartDateTime.ToString("o");
            }
        }
    }
}
