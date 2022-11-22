using Content.Server.Light.Components;
using Content.Shared.Destructible;
using Content.Shared.Light;
using Content.Shared.Throwing;
using Robust.Shared.Player;

namespace Content.Server.Light.EntitySystems
{
    public sealed class LightBulbSystem : EntitySystem
    {
        [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
        [Dependency] private readonly SharedAudioSystem _audio = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<LightBulbComponent, ComponentInit>(OnInit);
            SubscribeLocalEvent<LightBulbComponent, LandEvent>(HandleLand);
            SubscribeLocalEvent<LightBulbComponent, BreakageEventArgs>(OnBreak);
        }

        private void OnInit(EntityUid uid, LightBulbComponent bulb, ComponentInit args)
        {
            SetState(uid, bulb.State, bulb);
        }

        private void HandleLand(EntityUid uid, LightBulbComponent bulb, LandEvent args)
        {
            PlayBreakSound(uid, bulb);
            SetState(uid, LightBulbState.Broken, bulb);
        }

        private void OnBreak(EntityUid uid, LightBulbComponent component, BreakageEventArgs args)
        {
            SetState(uid, LightBulbState.Broken, component);
        }

        /// <summary>
        ///     Set a new state for a light bulb (broken, burned) and raise event about change
        /// </summary>
        public void SetState(EntityUid uid, LightBulbState state, LightBulbComponent? bulb = null)
        {
            if (!Resolve(uid, ref bulb))
                return;

            bulb.State = state;
            UpdateAppearance(uid, bulb);
        }

        public void PlayBreakSound(EntityUid uid, LightBulbComponent? bulb = null)
        {
            if (!Resolve(uid, ref bulb))
                return;

            _audio.Play(bulb.BreakSound, Filter.Pvs(uid), uid);
        }

        private void UpdateAppearance(EntityUid uid, LightBulbComponent? bulb = null,
            AppearanceComponent? appearance = null)
        {
            if (!Resolve(uid, ref bulb, ref appearance, logMissing: false))
                return;

            // try to update appearance and color
            _appearance.SetData(uid, LightBulbVisuals.State, bulb.State, appearance);
            _appearance.SetData(uid, LightBulbVisuals.Color, bulb.Configs[bulb.ActiveConfig], appearance);
        }
    }
}
