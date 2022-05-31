using Content.Server.Magic.Events;
using Content.Server.Polymorph.Systems;

namespace Content.Server.Magic.Systems;

/// <summary>
/// This handles various polymorph spells, i.e. become gorilla, become locker, etc.
/// </summary>
public sealed class PolymorphMagicSystem : EntitySystem
{
    [Dependency] private readonly PolymorphableSystem _polymorphableSystem = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<PolymorphSelfSpellEvent>(PolymorphSelfSpell);
    }

    private void PolymorphSelfSpell(PolymorphSelfSpellEvent ev)
    {
        _polymorphableSystem.PolymorphEntity(ev.Performer, ev.PolymorphTarget);
    }
}
