namespace Content.Server.Holodeck;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed class HoloProjectedComponent : Component
{
    [DataField("holoProjector")]
    public EntityUid HoloProjector;
}
