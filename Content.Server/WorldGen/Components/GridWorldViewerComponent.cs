namespace Content.Server.WorldGen.Components;

/// <summary>
/// This is for grids, so that the world around them is loaded.
/// Grids only load the world if there is a WorldViewer on them or if they're set to always on.
/// </summary>
[RegisterComponent]
public sealed class GridWorldViewerComponent : Component
{
    [ViewVariables]
    public HashSet<EntityUid> Viewers = new();
    [ViewVariables(VVAccess.ReadWrite)]
    public int ViewRadius = 64;
    [ViewVariables(VVAccess.ReadWrite)]
    public bool AlwaysLoad = false;
}
