using Content.Shared.Decals;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Decals
{
    public sealed class DecalOverlay : Overlay
    {
        private readonly DecalSystem _decals;
        private readonly SharedTransformSystem _transform;
        private readonly SpriteSystem _sprites;
        private readonly IEntityManager _entManager;
        private readonly IMapManager _mapManager;
        private readonly IPrototypeManager _prototypeManager;

        public override OverlaySpace Space => OverlaySpace.WorldSpaceBelowEntities;

        private readonly Dictionary<string, (Texture texture, DecalKind kind)> _cachedData = new(64);

        public DecalOverlay(
            DecalSystem decals,
            SharedTransformSystem transforms,
            SpriteSystem sprites,
            IEntityManager entManager,
            IMapManager mapManager,
            IPrototypeManager prototypeManager)
        {
            _decals = decals;
            _transform = transforms;
            _sprites = sprites;
            _entManager = entManager;
            _mapManager = mapManager;
            _prototypeManager = prototypeManager;
        }

        protected override void Draw(in OverlayDrawArgs args)
        {
            // Shouldn't need to clear cached textures unless the prototypes get reloaded.
            var handle = args.WorldHandle;
            var xformQuery = _entManager.GetEntityQuery<TransformComponent>();

            foreach (var (gridId, zIndexDictionary) in _decals.DecalRenderIndex)
            {
                if (zIndexDictionary.Count == 0) continue;

                var xform = xformQuery.GetComponent(gridId);

                handle.SetTransform(_transform.GetWorldMatrix(xform, xformQuery));

                foreach (var (_, decals) in zIndexDictionary)
                {
                    foreach (var (_, decal) in decals)
                    {
                        if (!_cachedData.TryGetValue(decal.Id, out var data))
                        {
                            var sprite = GetDecalSprite(decal.Id);
                            data.texture = _sprites.Frame0(sprite);
                            data.kind = GetDecalKind(decal.Id);
                            _cachedData[decal.Id] = data;
                        }

                        handle.UseShader();

                        if (decal.Angle.Equals(Angle.Zero))
                            handle.DrawTexture(data.texture, decal.Coordinates, decal.Color);
                        else
                            handle.DrawTexture(data, decal.Coordinates, decal.Angle, decal.Color);
                    }
                }
            }
        }

        public SpriteSpecifier GetDecalSprite(string id)
        {
            if (_prototypeManager.TryIndex<DecalPrototype>(id, out var proto))
                return proto.Sprite;
            else
            {
                Logger.Error($"Unknown decal prototype: {id}");
                return new SpriteSpecifier.Texture(new ResourcePath("/Textures/noSprite.png"));
            }
        }

        public DecalKind GetDecalKind(string id)
        {
            if (_prototypeManager.TryIndex<DecalPrototype>(id, out var proto))
                return proto.Kind;
            else
            {
                Logger.Error($"Unknown decal prototype: {id}");
                return DecalKind.Normal;
            }
        }
    }
}
