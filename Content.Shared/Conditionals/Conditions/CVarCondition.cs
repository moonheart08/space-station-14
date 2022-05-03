using Robust.Shared.Configuration;

namespace Content.Shared.Conditionals.Conditions;

[DataDefinition]
public sealed class CVarCondition : ISimpleCondition
{
    [DataField("cvar", required: true)] public string CVar { get; } = default!;
    [DataField("comparison")] public ComparisonKind ComparisonKind { get; } = ComparisonKind.Equals;

    [DataField("value")] public object Value { get; } = true; // Defaulted this way so people can shorthand checking if a bool is true.
    public bool Evaluate()
    {
        var configManager = IoCManager.Resolve<IConfigurationManager>();

        var type = configManager.GetCVarType(CVar);

        if (type == typeof(bool))
        {
            switch (ComparisonKind)
            {
                case ComparisonKind.Equals:
                    return configManager.GetCVar<bool>(CVar).Equals(Value);
                    break;
                case ComparisonKind.NotEquals:
                    return !configManager.GetCVar<bool>(CVar).Equals(Value);
                    break;
                default:
                    throw new ArgumentException($"Unsupported comparison on a bool: {ComparisonKind}");
            }
        }

        throw new ArgumentException($"Unsupported CVar type: {type}");
    }

    public void RegisterUpdates(ConditionalUpdateManager updateManager, Conditional cond)
    {
        updateManager.RegisterConditionalForUpdates(CVar, cond);
    }
}

public enum ComparisonKind
{
    Equals,
    NotEquals,
    GreaterThan,
    GreaterEquals,
    LessThan,
    LessEquals,
    Contains,
}
