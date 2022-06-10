using Content.Shared.Decals;
using Content.Shared.Rotation;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Utility;
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

        private readonly Dictionary<(string, Direction), (Texture, RSI.State.Direction)> _cachedTextures = new(64);

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

                var gridUid = _mapManager.GetGridEuid(gridId);
                var xform = xformQuery.GetComponent(gridUid);

                var worldMatrix = _transform.GetWorldMatrix(xform, xformQuery);

                foreach (var (_, decals) in zIndexDictionary)
                {
                    foreach (var (_, decal) in decals)
                    {
                        var dir = decal.Angle.GetDir();
                        if (!_cachedTextures.TryGetValue((decal.Id, dir), out var textureDirPair))
                        {
                            var sprite = GetDecalSprite(decal.Id);
                            var state = _sprites.RsiStateLike(sprite);
                            var rsiDir = dir.Convert(state.Directions);
                            var texture = state.GetFrame(rsiDir, 0);
                            _cachedTextures[(decal.Id, dir)] = (texture, rsiDir);
                            textureDirPair = (texture, rsiDir);
                        }

                        GetDrawMatrix(textureDirPair.Item2, Matrix3.CreateTransform(decal.Coordinates, decal.Angle), out var drawMatrix);
                        Matrix3.Multiply(ref worldMatrix, ref drawMatrix, out var finalMatrix);
                        handle.SetTransform(finalMatrix);
                        handle.DrawTexture(textureDirPair.Item1, Vector2.Zero, decal.Color);
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

        private void GetDrawMatrix(RSI.State.Direction dir, Matrix3 localMatrix, out Matrix3 drawMatrix)
        {
            if (dir == RSI.State.Direction.South)
                drawMatrix = localMatrix;
            else
            {
                Matrix3.Multiply(ref RSIDirectionMatrices[(int)dir], ref localMatrix, out drawMatrix);
            }
        }

        private static readonly Matrix3[] RSIDirectionMatrices = {
            // Yes, this is stolen straight from SpriteComponent.
            // array order chosen such that this array can be indexed by casing an RSI direction to an int
            Matrix3.Identity, // should probably just avoid matrix multiplication altogether if the direction is south.
            Matrix3.CreateRotation(-Direction.North.ToAngle()),
            Matrix3.CreateRotation(-Direction.East.ToAngle()),
            Matrix3.CreateRotation(-Direction.West.ToAngle()),
            Matrix3.CreateRotation(-Direction.SouthEast.ToAngle()),
            Matrix3.CreateRotation(-Direction.SouthWest.ToAngle()),
            Matrix3.CreateRotation(-Direction.NorthEast.ToAngle()),
            Matrix3.CreateRotation(-Direction.NorthWest.ToAngle())
        };
    }
}
