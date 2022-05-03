namespace Content.Shared.Conditionals.Conditions;

[DataDefinition]
public sealed class AndConditionGroup : ISimpleCondition
{
    [DataField("not")]
    public bool Inverted { get; }

    [DataField("conditions", required: true)]
    public readonly List<ISimpleCondition> Conditions = default!;

    public bool Evaluate()
    {
        var result = true;

        foreach (var condition in Conditions)
        {
            result &= condition.Evaluate();
        }

        return (result || Conditions.Count == 0) ^ Inverted;
    }

    public void RegisterUpdates(ConditionalUpdateManager updateManager, Conditional cond)
    {
        foreach (var condition in Conditions)
        {
            condition.RegisterUpdates(updateManager, cond);
        }
    }
}
