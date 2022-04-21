using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Content.Server.Damage.Components;
using Content.Server.Weapon.Melee.Components;
using Content.Server.Wieldable.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;

namespace Content.Server.GuideGenerator;

public sealed class DamageTableJsonGenerator
{
    public static void PublishJson(StreamWriter file)
    {
        var prototype = IoCManager.Resolve<IPrototypeManager>();

        var ents =
            prototype
                .EnumeratePrototypes<EntityPrototype>()
                .Select(x => new DamageTableEntry(x))
                .Where(x => !x.NoDamage())
                .ToDictionary(x => x.Id, x => x);

        var serializeOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters =
            {
                new UniversalJsonConverter<ReagentEffect>(),
            }
        };

        file.Write(JsonSerializer.Serialize(ents, serializeOptions));
    }

    public static Dictionary<string, float> AdjustDamageDict(Dictionary<string, FixedPoint2> dict)
    {
        return dict.Select(x => (x.Key, Value: x.Value.Float())).ToDictionary(x => x.Key, x => x.Value);
    }
}

public sealed class DamageTableEntry
{
    [JsonPropertyName("id")] public string Id { get; }
    [JsonPropertyName("name")] public string Name { get; }
    [JsonPropertyName("standardDamage")] public Dictionary<string, float>? StandardDamage { get; }
    [JsonPropertyName("wieldDamage")] public Dictionary<string, float>? WieldDamage { get; }
    [JsonPropertyName("thrownDamage")] public Dictionary<string, float>? ThrownDamage { get; }

    public DamageTableEntry(string id, string name, Dictionary<string, float> standardDamage, Dictionary<string, float> wieldDamage, Dictionary<string, float> thrownDamage)
    {
        Id = id;
        Name = name;
        StandardDamage = standardDamage;
        WieldDamage = wieldDamage;
        ThrownDamage = thrownDamage;
    }

    public DamageTableEntry(EntityPrototype proto)
    {
        Id = proto.ID;
        Name = proto.Name;

        MeleeWeaponComponent? melee = null;
        IncreaseDamageOnWieldComponent? wield = null;
        DamageOtherOnHitComponent? thrown = null;

        foreach (var (_, component) in proto.Components)
        {
            switch (component)
            {
                case MeleeWeaponComponent m:
                    melee = m;
                    break;
                case IncreaseDamageOnWieldComponent w:
                    wield = w;
                    break;
                case DamageOtherOnHitComponent h:
                    thrown = h;
                    break;
            }
        }

        if (melee is not null)
            StandardDamage = DamageTableJsonGenerator.AdjustDamageDict(melee.Damage.DamageDict);
        if (wield is not null && melee is not null)
            WieldDamage = DamageTableJsonGenerator.AdjustDamageDict(DamageSpecifier.ApplyModifierSet(melee.Damage, wield.Modifiers).DamageDict);
        if (thrown is not null)
            ThrownDamage = DamageTableJsonGenerator.AdjustDamageDict(thrown.Damage.DamageDict);
    }

    public bool NoDamage()
    {
        return (StandardDamage is null || StandardDamage.Values.All(x => x == 0)) &&
               (WieldDamage is null || WieldDamage.Values.All(x => x == 0)) &&
               (ThrownDamage is null || ThrownDamage.Values.All(x => x == 0));
    }
}
