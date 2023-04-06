using Content.Shared.Audio;
using Robust.Shared.ComponentTrees;
using Robust.Shared.Physics;

namespace Content.Shared.ExChat.Components;

/// <summary>
/// This is used for things that can hear in generalized fashion.
/// </summary>
/// <remarks>It's listening. Turn back.</remarks>
[RegisterComponent]
public sealed class EarsComponent : Component, IComponentTreeEntry<EarsComponent>
{
    public EntityUid? TreeUid { get; set; }

    public DynamicTree<ComponentTreeEntry<EarsComponent>>? Tree { get; set; }

    public bool AddToTree => true; // Currently can't disable ears.

    public bool TreeUpdateQueued { get; set; }
}
