using Content.Shared.Singularity.Components;
using Content.Shared.VacuumDecay;
using Robust.Client.Graphics;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;

namespace Content.Client.VacuumDecay;

public sealed class VacuumDecayOverlay : Overlay
{
    [Dependency] private readonly IEntityManager _entMan = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    /// <summary>
    ///     Maximum number of distortions that can be shown on screen at a time.
    ///     If this value is changed, the shader itself also needs to be updated.
    /// </summary>
    public const int MaxCount = 5;

    public override OverlaySpace Space => OverlaySpace.WorldSpace;
    public override bool RequestScreenTexture => true;

    private readonly ShaderInstance _shader;

    public VacuumDecayOverlay()
    {
        IoCManager.InjectDependencies(this);
        _shader = _prototypeManager.Index<ShaderPrototype>("VacuumDecay").Instance().Duplicate();
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (ScreenTexture == null || args.Viewport.Eye == null)
            return;

        // Has to be correctly handled because of the way intensity/falloff transform works so just do it.
        _shader?.SetParameter("renderScale", args.Viewport.RenderScale);

        var position = new Vector2[MaxCount];
        var radius = new float[MaxCount];
        var pixelationTable = new float[8];

        var mapId = args.Viewport.Eye.Position.MapId;

        foreach (var distortion in _entMan.EntityQuery<VacuumDecayComponent>())
        {
            var mapPos = _entMan.GetComponent<TransformComponent>(distortion.Owner).MapPosition;
            if (mapPos.MapId != mapId)
                continue;

        }

        var tempCoords = args.Viewport.WorldToLocal(Vector2.Zero);
        tempCoords.Y = args.Viewport.Size.Y - tempCoords.Y;
        position[0] = tempCoords;
        tempCoords = args.Viewport.WorldToLocal(new Vector2(128, 0));
        tempCoords.Y = args.Viewport.Size.Y - tempCoords.Y;
        position[1] = tempCoords;
        radius[0] = 128 * EyeManager.PixelsPerMeter;
        radius[1] = 128 * EyeManager.PixelsPerMeter;
        for (var i = 0; i < 8; i++)
        {
            pixelationTable[i] = MathF.Pow(0.5f, i);
        }

        var count = 2;

        if (count == 0)
            return;

        _shader?.SetParameter("count", count);
        _shader?.SetParameter("position", position);
        _shader?.SetParameter("radius", radius);
        _shader?.SetParameter("falloffPower", pixelationTable);
        _shader?.SetParameter("pixelsPerEntry", EyeManager.PixelsPerMeter*args.Viewport.RenderScale.Length);
        _shader?.SetParameter("SCREEN_TEXTURE", ScreenTexture);

        var worldHandle = args.WorldHandle;
        worldHandle.UseShader(_shader);
        worldHandle.DrawRect(args.WorldAABB, Color.White);
        worldHandle.UseShader(null);
    }
}
