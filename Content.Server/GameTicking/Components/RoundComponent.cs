namespace Content.Server.GameTicking.Components;

/// <summary>
/// This is used for marking an entity as controlling the round.
/// </summary>
/// <remarks>Please do not use this outside of it's very specific intended purpose or you will explode the server.</remarks>
[RegisterComponent]
public sealed class RoundComponent : Component
{
    [DataField("roundStartTime")]
    public TimeSpan RoundStartTime;

    [DataField("roundId")]
    public int RoundId;
}
