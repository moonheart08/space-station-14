using System.Linq;
using Content.Server.WorldGen.Components;
using Robust.Shared.Map;
using Robust.Shared.Physics;

namespace Content.Server.WorldGen.Systems;

public sealed partial class WorldChunkSystem
{
    private Box2? GetGridViewBounds(EntityUid grid, GridWorldViewerComponent? gridWorldViewerComponent = null)
    {
        if (!Resolve(grid, ref gridWorldViewerComponent))
            return null;
        var mapGrid = Comp<IMapGridComponent>(grid).Grid;
        return mapGrid.LocalBounds.Enlarged(gridWorldViewerComponent.ViewRadius);
    }
}
