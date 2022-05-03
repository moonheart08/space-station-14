namespace Content.Shared.Conditionals.Conditions;

[DataDefinition]
public sealed class PlayerCountCondition : ISimpleCondition
{
    [DataField("minPlayers")] public int MinPlayers =

    public bool Evaluate()
    {
        throw new NotImplementedException();
    }

    public void RegisterUpdates(ConditionalUpdateManager updateManager, Conditional cond)
    {
        throw new NotImplementedException();
    }
}
