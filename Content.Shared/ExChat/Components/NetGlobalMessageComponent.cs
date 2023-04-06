using Robust.Shared.GameStates;

namespace Content.Shared.ExChat.Components;

/// <summary>
/// This is used for messages that get immediately sent over the network,
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class NetGlobalMessageComponent : Component
{

}
