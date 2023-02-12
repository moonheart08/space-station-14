using System.Linq;
using Content.Client.Administration.Managers;
using Content.Client.Guidebook;
using Content.Shared.Administration;
using Content.Shared.Administration.Events;
using Content.Shared.GameTicking;
using Robust.Shared.Network;
using Robust.Shared.Utility;

namespace Content.Client.Administration.Systems
{
    public sealed partial class AdminSystem : EntitySystem
    {
        public event Action<List<PlayerInfo>>? PlayerListChanged;

        private Dictionary<NetUserId, PlayerInfo>? _playerList;
        public IReadOnlyList<PlayerInfo> PlayerList
        {
            get
            {
                if (_playerList != null) return _playerList.Values.ToList();

                return new List<PlayerInfo>();
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            InitializeOverlay();
            SubscribeNetworkEvent<FullPlayerListEvent>(OnPlayerListChanged);
            SubscribeNetworkEvent<PlayerInfoChangedEvent>(OnPlayerInfoChanged);
            SubscribeLocalEvent<GetGuidesEvent>(OnGetGuides);
        }

        private void OnGetGuides(GetGuidesEvent ev)
        {
            foreach (var (key, guide) in ev.Guides.ShallowClone())
            {
                if (guide.RequiresAdmin && !_adminManager.IsActive())
                    ev.Guides.Remove(key);
            }
        }

        public override void Shutdown()
        {
            base.Shutdown();
            ShutdownOverlay();
        }

        private void OnPlayerInfoChanged(PlayerInfoChangedEvent ev)
        {
            if(ev.PlayerInfo == null) return;

            if (_playerList == null) _playerList = new();

            _playerList[ev.PlayerInfo.SessionId] = ev.PlayerInfo;
            PlayerListChanged?.Invoke(_playerList.Values.ToList());
        }

        private void OnPlayerListChanged(FullPlayerListEvent msg)
        {
            _playerList = msg.PlayersInfo.ToDictionary(x => x.SessionId, x => x);
            PlayerListChanged?.Invoke(msg.PlayersInfo);
        }
    }
}
