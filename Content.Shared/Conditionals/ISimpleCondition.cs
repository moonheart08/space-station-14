namespace Content.Shared.Conditionals;

[ImplicitDataDefinitionForInheritors]
public interface ISimpleCondition
{
    public bool Evaluate();

    public void RegisterUpdates(ConditionalUpdateManager updateManager, Conditional cond);
}
