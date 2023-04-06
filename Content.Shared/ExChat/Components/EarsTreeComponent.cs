using Content.Shared.Audio;
using Robust.Shared.ComponentTrees;
using Robust.Shared.Physics;

namespace Content.Shared.ExChat.Components;

/// <summary>
/// This is used for ear spatial queries.
/// </summary>
[RegisterComponent]
public sealed class EarsTreeComponent : Component, IComponentTreeComponent<AmbientSoundComponent>
{
    public DynamicTree<ComponentTreeEntry<AmbientSoundComponent>> Tree { get; set; } = default!;
}
