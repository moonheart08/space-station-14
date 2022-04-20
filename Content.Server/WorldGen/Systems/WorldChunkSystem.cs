using Content.Server.WorldGen.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Map;

namespace Content.Server.WorldGen.Systems;

public sealed partial class WorldChunkSystem : EntitySystem
{
    [Dependency] private readonly IMapManager _mapManager = default!;

    public const int ChunkSize = 128;

    private readonly HashSet<MapId> _loadingEnabledMaps = new HashSet<MapId>();

    public override void Initialize() {
        SubscribeLocalEvent<GridInitializeEvent>(OnGridInitialized);
        SubscribeLocalEvent<ViewSubscriberAddedEvent>(OnViewSubscriberAdded);
        SubscribeLocalEvent<ViewSubscriberRemovedEvent>(OnViewSubscriberRemoved);
        SubscribeLocalEvent<WorldViewerComponent, ComponentShutdown>(OnWorldViewerRemoved);
    }

    #region Subscriptions
    private void OnGridInitialized(GridInitializeEvent ev)
    {
        // Allow the grid to see the world.
        AddComp<GridWorldViewerComponent>(ev.EntityUid);
    }

    private void OnViewSubscriberAdded(ViewSubscriberAddedEvent ev)
    {
        AddComp<WorldViewerComponent>(ev.View);
        var parentGrid = Transform(ev.View).GridID;
        if (parentGrid == GridId.Invalid)
            return;

        var gridEuid = _mapManager.GetGridEuid(parentGrid);
        if (!TryComp<GridWorldViewerComponent>(gridEuid, out var gridViewer))
            return;

        // Make sure the grid recognizes it has a viewer on it.
        gridViewer.Viewers.Add(ev.View);
    }

    private void OnViewSubscriberRemoved(ViewSubscriberRemovedEvent ev)
    {
        RemComp<WorldViewerComponent>(ev.View);

        var parentGrid = Transform(ev.View).GridID;
        if (parentGrid == GridId.Invalid)
            return;

        var gridEuid = _mapManager.GetGridEuid(parentGrid);
        if (!TryComp<GridWorldViewerComponent>(gridEuid, out var gridViewer))
            return;

        // Make sure the grid recognizes it has a viewer removed.
        gridViewer.Viewers.Remove(ev.View);
    }

    private void OnWorldViewerRemoved(EntityUid uid, WorldViewerComponent component, ComponentShutdown args)
    {
        var parentGrid = Transform(uid).GridID;
        if (parentGrid == GridId.Invalid)
            return;

        var gridEuid = _mapManager.GetGridEuid(parentGrid);
        if (!TryComp<GridWorldViewerComponent>(gridEuid, out var gridViewer))
            return;

        // Make sure the grid recognizes it has a viewer removed.
        gridViewer.Viewers.Remove(uid);
    }

    #endregion

    public void AddLoadedMap(MapId map)
    {
        _loadingEnabledMaps.Add(map);
    }

    public void RemoveLoadedMap(MapId map)
    {
        _loadingEnabledMaps.Remove(map);
        //TODO: Teardown like deleting chunks.
    }

    private void GetRelevantViewers(MapId map, ref HashSet<EntityUid> viewers, ref HashSet<EntityUid> gridViewers, EntityQuery<TransformComponent> xformQuery)
    {
        foreach (var viewerComp in EntityQuery<WorldViewerComponent>())
        {
            var viewer = viewerComp.Owner;

            if (viewerComp.WorldViewRadius == 0)
                continue;

            var xform = xformQuery.GetComponent(viewer);

            if (xform.MapID != map)
                continue;

            // If we pass all of these, then we're irrelevant.
            if (viewerComp.IrrelevantOnGrid
                && xform.GridID != GridId.Invalid
                && HasComp<GridWorldViewerComponent>(_mapManager.GetGridEuid(xform.GridID)))
                continue;

            viewers.Add(viewer);
        }

        foreach (var viewerComp in EntityQuery<GridWorldViewerComponent>())
        {
            var viewer = viewerComp.Owner;

            if (!(xformQuery.GetComponent(viewer).MapID != map))
                continue;

            if (viewerComp.AlwaysLoad)
            {
                gridViewers.Add(viewer);
                continue;
            }

            if (viewerComp.Viewers.Count == 0 || viewerComp.ViewRadius == 0)
                continue;

            gridViewers.Add(viewer);
        }
    }

}
