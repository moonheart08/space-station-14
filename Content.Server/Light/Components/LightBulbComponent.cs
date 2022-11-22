using Content.Server.Light.EntitySystems;
using Content.Shared.Light;
using Robust.Shared.Audio;

namespace Content.Server.Light.Components
{
    /// <summary>
    ///     Component that represents a light bulb. Can be broken, or burned, which turns them mostly useless.
    /// </summary>
    [RegisterComponent, Access(typeof(LightBulbSystem))]
    public sealed class LightBulbComponent : Component
    {
        [DataField("visualConfigs", required: true)]
        public Dictionary<string, LightBulbVisualConfig> Configs = default!;

        [DataField("activeConfig", required: true)]
        public string ActiveConfig = default!;

        [DataField("bulb")]
        public LightBulbType Type = LightBulbType.Tube;

        [DataField("startingState")]
        public LightBulbState State = LightBulbState.Normal;

        [DataField("breakSound")]
        public SoundSpecifier BreakSound = new SoundCollectionSpecifier("GlassBreak");
    }

    [DataDefinition]
    public sealed class LightBulbVisualConfig
    {
        [DataField("name", required: true)]
        public string Name = default!;

        [DataField("color")]
        public Color Color = Color.White;

        [DataField("BurningTemperature")]
        public int BurningTemperature = 1400;

        [DataField("lightEnergy")]
        public float LightEnergy = 0.8f;

        [DataField("lightRadius")]
        public float LightRadius = 10;

        [DataField("lightSoftness")]
        public float LightSoftness = 1;

        [DataField("PowerUse")]
        public int PowerUse = 60;
    }
}
