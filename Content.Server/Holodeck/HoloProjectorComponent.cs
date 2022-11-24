namespace Content.Server.Holodeck;

/// <summary>
/// This is used for the holoprojector, providing bookkeeping
/// </summary>
[RegisterComponent]
public sealed class HoloProjectorComponent : Component
{
    public const int CheckedPerSecond = 256;
    public int RollingCheckIndex = 0;

    [DataField("activelyProjected")]
    public List<EntityUid> ActivelyProjected = new();

    [DataField("projectionConfigs")]
    public List<string> ProjectionConfigs = new();

    /// <summary>
    /// The offset from the projector to use.
    /// </summary>
    [DataField("offset")]
    public Vector2 Offset = Vector2.Zero;
}
