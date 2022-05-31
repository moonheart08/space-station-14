using Content.Shared.Actions;
using Content.Shared.Polymorph;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Magic.Events;

public sealed class PolymorphSelfSpellEvent : InstantActionEvent
{
    [DataField("polymorphTarget", customTypeSerializer: typeof(PrototypeIdSerializer<PolymorphPrototype>))]
    public string PolymorphTarget = default!;
}
