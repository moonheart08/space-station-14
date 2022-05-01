using Content.Shared.Storage;
using Robust.Shared.Map;
using Robust.Shared.Random;

namespace Content.Server.Meteors;

public sealed class AggregateSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly PoissonDiskSampler _sampler = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<AggregateComponent, ComponentStartup>(AggregateStartup);
    }

    private void AggregateStartup(EntityUid uid, AggregateComponent component, ComponentStartup args)
    {
        var points = _sampler.SampleCircle(Vector2.Zero, component.Radius, component.MinimumDistance);
        foreach (var point in points)
        {
            var coords = new EntityCoordinates(uid, point);
            var toSpawn = EntitySpawnCollection.GetSpawns(component.Parts);
            foreach (var prototype in toSpawn)
            {
                var ent = Spawn(prototype, coords);
                if (component.RandomizeRotation)
                    Transform(ent).WorldRotation = _random.NextAngle();
            }
        }
    }

    public void DeAggregate(EntityUid uid)
    {
        if (!HasComp<AggregateComponent>(uid))
            return;
        var xform = Transform(uid);
        foreach (var child in xform.Children)
        {
            child.AttachToGridOrMap();
        }
    }
}
