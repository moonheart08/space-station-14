using Robust.Shared.GameObjects;

namespace Content.Server.Explosion.Components
{
    [RegisterComponent]
    public sealed class TriggerOnCollideComponent : Component
    {
        [DataField("minimumTriggerMass")]
        public float MinimumTriggerMass = 0.0f;
    }
}
