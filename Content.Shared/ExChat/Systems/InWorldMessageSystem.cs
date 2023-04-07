using Content.Shared.ExChat.Components;
using Robust.Shared.Physics;

namespace Content.Shared.ExChat.Systems;

/// <summary>
/// This handles in-world messages and calculating the listener list.
/// </summary>
public sealed class InWorldMessageSystem : EntitySystem
{
    [Dependency] private readonly EarsTreeSystem _earsTree = default!; // The tree listens.
    [Dependency] private readonly SharedTransformSystem _xformSys = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<InWorldMessageComponent, GetListenersEvent>(GetInWorldListeners);
    }

    private void GetInWorldListeners(EntityUid speaker, InWorldMessageComponent component, ref GetListenersEvent args)
    {
        var speakerXform = Transform(speaker);
        var state = new EarsTreeLookupState()
        {
            Speaker = speaker,
            SpeakerXform = speakerXform,
            InView = new(),
            XFormSys = _xformSys,
            EntityManager = EntityManager,
            MaxRange = component.MaxRange,
        };

        var worldPos = _xformSys.GetWorldPosition(speakerXform);
        var queryBox = Box2.CenteredAround(worldPos, new Vector2(component.MaxRange, component.MaxRange));

        _earsTree.QueryAabb(ref state, QueryCallback, speakerXform.MapID, queryBox);
    }

    private static bool QueryCallback(ref EarsTreeLookupState state, in ComponentTreeEntry<EarsComponent> value)
    {
        var (_, xform) = value;

        if (!xform.Coordinates.TryDistance(state.EntityManager, state.XFormSys, state.SpeakerXform.Coordinates,
                out var dist))
            return true;

        if (dist >= state.MaxRange)
            return true;

        state.InView.Add(value.Uid);

        return true;
    }

    private struct EarsTreeLookupState
    {
        public required EntityUid Speaker;
        public required TransformComponent SpeakerXform;
        public required HashSet<EntityUid> InView;
        public required SharedTransformSystem XFormSys;
        public required EntityManager EntityManager;
        public required float MaxRange;
    }
}
