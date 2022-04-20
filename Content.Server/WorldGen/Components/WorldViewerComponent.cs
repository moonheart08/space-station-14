namespace Content.Server.WorldGen.Components;

[RegisterComponent]
public sealed class WorldViewerComponent : Component
{
    [DataField("viewRadius")]
    [ViewVariables(VVAccess.ReadWrite)]
    public int WorldViewRadius = 64;

    /// <summary>
    /// Makes it so the viewer uses up no time when standing on a grid, because the grid can load the world for it.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public bool IrrelevantOnGrid = true;
}
