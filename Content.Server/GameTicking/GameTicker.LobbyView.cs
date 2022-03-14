using Content.Shared.GameTicking;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Server.GameTicking;

public sealed partial class GameTicker
{
    [Dependency] private readonly ViewSubscriberSystem _viewSubscriberSystem = default!;
    [ViewVariables] private EntityUid? _lobbyViewpointEntity = null;

    public void SetLobbyViewpoint(MapCoordinates coords)
    {
        var newEnt = Spawn(null, coords);
        var eye = EnsureComp<EyeComponent>(newEnt);
        eye.DrawFov = false;
        if (_lobbyViewpointEntity is not null)
        {
            foreach (var session in _playerManager.ServerSessions)
            {
                _viewSubscriberSystem.RemoveViewSubscriber(_lobbyViewpointEntity.Value, session);
            }
        }

        _lobbyViewpointEntity = newEnt;

        foreach (var session in _playerManager.ServerSessions)
        {
            _viewSubscriberSystem.AddViewSubscriber(_lobbyViewpointEntity.Value, session);
        }

        RaiseNetworkEvent(new LobbyCameraSetEvent(_lobbyViewpointEntity), Filter.Broadcast());
    }

    public void SetLobbyViewpoint(EntityCoordinates coords)
    {
        var newEnt = Spawn(null, coords);
        var eye = EnsureComp<EyeComponent>(newEnt);
        eye.DrawFov = false;
        if (_lobbyViewpointEntity is not null)
        {
            foreach (var session in _playerManager.ServerSessions)
            {
                _viewSubscriberSystem.RemoveViewSubscriber(_lobbyViewpointEntity.Value, session);
            }
        }

        _lobbyViewpointEntity = newEnt;

        foreach (var session in _playerManager.ServerSessions)
        {
            _viewSubscriberSystem.AddViewSubscriber(_lobbyViewpointEntity.Value, session);
        }

        RaiseNetworkEvent(new LobbyCameraSetEvent(_lobbyViewpointEntity), Filter.Broadcast());
    }
}
