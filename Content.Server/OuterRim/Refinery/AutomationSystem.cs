using Content.Server.Popups;
using Content.Server.Tools;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Dynamics;
using Robust.Shared.Player;

namespace Content.Server.OuterRim.Refinery;

/// <summary>
/// This handles automation machinery.
/// </summary>
public sealed class AutomationSystem : EntitySystem
{
    [Dependency] private readonly ToolSystem _toolSystem = default!; // this system is a real tool.
    [Dependency] private readonly EntityLookupSystem _lookupSystem = default!; // competes with the above.
    [Dependency] private readonly PopupSystem _popupSystem = default!;

    private readonly ISawmill _sawmill = Logger.GetSawmill(nameof(AutomationSystem));

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<OuterRimLoaderComponent, StartCollideEvent>(OnLoaderCollide);
        SubscribeLocalEvent<OuterRimTargetedComponent, InteractUsingEvent>(OnTargetedInteractUsing);
    }

    private void OnTargetedInteractUsing(EntityUid uid, OuterRimTargetedComponent component, InteractUsingEvent args)
    {
        if (!_toolSystem.HasQuality(args.Used, "Prying"))
            return;

        var xform = Transform(uid);

        var entities = _lookupSystem.GetEntitiesInRange(
            xform.Coordinates.Offset(xform.LocalRotation.RotateVec(component.Offset)), 0.1f);

        ref var targetIdx = ref component.PileIndex;
        ref var targetPile = ref component.TargetPile;
        ref var targetEntity = ref component.TargetEntity;

        if (!entities.SetEquals(targetPile))
        {
            // This somewhat annoying logic is so that we roughly keep order even if the contents of the target pile changes, sorta.
            // Makes sure we don't accidentally loop back around to the previous target early.
            targetPile = new(entities);
            if (targetEntity is { } t)
            {
                var idx = targetPile.FindIndex(x => x == t);
                targetIdx = idx == -1 ? 0 : idx;
            }
            else
            {
                targetIdx = 0;
            }
        }

        if (targetPile.Count == 0)
        {
            _popupSystem.PopupEntity("There was nothing to bind the loader to.", uid, Filter.Pvs(uid));
            targetEntity = null;
            return; // oops, no targets.
        }

        targetIdx++;

        if (targetPile.Count >= targetIdx)
            targetIdx = 0;

        targetEntity = targetPile[targetIdx];

        _popupSystem.PopupEntity($"You bind {Identity.Name(uid, EntityManager)} to {Identity.Name(targetEntity.Value, EntityManager)}.", uid, Filter.Pvs(uid));
    }

    private void OnLoaderCollide(EntityUid uid, OuterRimLoaderComponent component, StartCollideEvent args)
    {
        Logger.Debug("EA");
        if (args.OurFixture.ID != component.LoadingFixture)
            return;

        if (args.OtherFixture.Body.BodyType == BodyType.Static)
            return;

        Logger.Debug("AE");

        if (!TryComp<OuterRimTargetedComponent>(uid, out var targeted))
        {
            _sawmill.Warning($"Tried to use a loader with no target, offending entity is {ToPrettyString(uid)}");

            return;
        }

        if (targeted.TargetEntity is not { } target)
            return; // No target, do nothing.


        var used = args.OtherFixture.Body.Owner;

        var loaderEv = new LoaderTryLoadEvent(used, uid);

        RaiseLocalEvent(target, ref loaderEv);

        if (loaderEv.Handled)
            return; // Okay, we're done.

        // Legacy path!

        var interactUsing = new InteractUsingEvent(uid, used, target, Transform(target).Coordinates);

        RaiseLocalEvent(target, interactUsing);

        if (interactUsing.Handled)
            // ReSharper disable once RedundantJumpStatement
            return; // Technically redundant, but makes it clear we should care if more is added.
    }
}

[ByRefEvent]
public struct LoaderTryLoadEvent
{
    public readonly EntityUid LoadedEntity;
    public readonly EntityUid Loader;
    public bool Handled;

    public LoaderTryLoadEvent(EntityUid loadedEntity, EntityUid loader) : this()
    {
        LoadedEntity = loadedEntity;
        Loader = loader;
    }
}
