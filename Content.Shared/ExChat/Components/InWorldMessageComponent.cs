using Robust.Shared.GameStates;

namespace Content.Shared.ExChat.Components;

/// <summary>
///     This is used for messages that are "in world" so to speak and originate from a particular location.
/// </summary>
/// <remarks>This component expects the message to not have been spawned in null-space like global messages are.</remarks>
[RegisterComponent, NetworkedComponent]
public sealed class InWorldMessageComponent : Component
{
    /// <summary>
    ///     The maximum hearing range for this message. Listeners further from here will be excluded.
    /// </summary>
    [DataField("maxRange", required: true)]
    public float MaxRange;
}
