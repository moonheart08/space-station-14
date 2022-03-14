using Content.Server.GameTicking;
using Content.Shared.Administration;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.Map;

namespace Content.Server.Administration.Commands;

[AdminCommand(AdminFlags.Admin)]
public sealed class SetLobbyViewpoint : IConsoleCommand
{
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    public string Command => "setlobbyviewpoint";
    public string Description => Loc.GetString("set-lobby-viewpoint-command-description", ("command", Command));
    public string Help => Loc.GetString("set-lobby-viewpoint-command-help-text", ("command", Command));

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        var gameTicker = EntitySystem.Get<GameTicker>();

        if (args.Length is not 3)
        {

        }
        if (!int.TryParse(args[0], out var mid) || _mapManager.GetMapEntityId(new MapId(mid)) is not {Valid: true} eid)
        {
            shell.WriteError(Loc.GetString("shell-argument-map-id-invalid", ("index", 1)));
            return;
        }

        if (!float.TryParse(args[1], out var wx))
        {
            shell.WriteError(Loc.GetString("shell-argument-number-invalid", ("index", 2)));
            return;
        }

        if (!float.TryParse(args[1], out var wy))
        {
            shell.WriteError(Loc.GetString("shell-argument-number-invalid", ("index", 3)));
            return;
        }

        gameTicker.SetLobbyViewpoint(new MapCoordinates(wx, wy, new MapId(mid)));
    }
}
