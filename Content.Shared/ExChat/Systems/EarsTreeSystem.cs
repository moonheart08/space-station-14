using Content.Shared.Audio;
using Content.Shared.ExChat.Components;
using Robust.Shared.ComponentTrees;
using Robust.Shared.Physics;

namespace Content.Shared.ExChat.Systems;

/// <summary>
/// This handles the ear lookup tree.
/// </summary>
public sealed class EarsTreeSystem : ComponentTreeSystem<EarsTreeComponent, EarsComponent>
{
    protected override bool DoFrameUpdate => false;
    protected override bool DoTickUpdate => true;
    protected override int InitialCapacity => 256;
    protected override bool Recursive => true;

    protected override Box2 ExtractAabb(in ComponentTreeEntry<EarsComponent> entry, Vector2 pos, Angle rot)
        => new (pos - 1, pos + 1);

    protected override Box2 ExtractAabb(in ComponentTreeEntry<EarsComponent> entry)
    {
        if (entry.Component.TreeUid == null)
            return default;

        var pos = XformSystem.GetRelativePosition(
            entry.Transform,
            entry.Component.TreeUid.Value,
            GetEntityQuery<TransformComponent>());

        return ExtractAabb(in entry, pos, default);
    }
}
