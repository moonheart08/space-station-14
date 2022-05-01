using Content.Shared.Storage;

namespace Content.Server.Meteors;

[RegisterComponent]
public sealed class AggregateComponent : Component
{
    [DataField("radius")] public float Radius = 1;
    [DataField("minimumDistance")] public float MinimumDistance = 1;
    [DataField("randomizeRotation")] public bool RandomizeRotation = false;
    [DataField("parts", required: true)] public List<EntitySpawnEntry> Parts = default!;
}
