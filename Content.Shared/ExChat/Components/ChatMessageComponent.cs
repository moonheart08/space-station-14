using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Players;

namespace Content.Shared.ExChat.Components;

/// <summary>
/// This is used for chat messages, and contains core info about a sent message.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ChatMessageComponent : Component
{
    /// <summary>
    ///     The raw, original chat message. You should only use this for logging or similar.
    /// </summary>
    [AutoNetworkedField]
    public string RawMessage = string.Empty;

    /// <summary>
    ///     The original speaker of the message, if any. This is not necessarily where it was heard from!
    /// </summary>
    [AutoNetworkedField]
    public EntityUid? OriginalSpeaker = null;

    /// <summary>
    ///     The original speaking player, if any.
    /// </summary>
    [AutoNetworkedField]
    public NetUserId? OriginalSpeakerPlayer = null;

    /// <summary>
    ///     Echoing speakers, this can be for example intercomms or radios. Prevents message dupe.
    /// </summary>
    [AutoNetworkedField]
    public HashSet<EntityUid>? EchoSpeakers = null;
}
