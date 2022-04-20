using Robust.Shared.Map;

namespace Content.Server.WorldGen.Components;

[RegisterComponent]
public sealed class WorldChunkComponent : Component
{
    public (MapId, Vector2i) Coordinates;

}
