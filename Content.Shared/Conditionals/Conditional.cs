using System.Linq;
using Content.Shared.Conditionals.Conditions;

namespace Content.Shared.Conditionals;

public sealed class Conditional
{
    public bool CurrentValue { get; private set; }

    private readonly List<ISimpleCondition>? _conditions = null;
    private readonly bool? _exactValue = null;

    public void Update()
    {
        if (_exactValue is not null)
        {
            CurrentValue = _exactValue.Value;
            return;
        }

        if (_conditions is null)
        {
            Logger.Error("Conditional with no exact value and no conditions attempted to update!");
            return;
        }

        var isSet = true;

        foreach (var condition in _conditions)
        {
            isSet &= condition.Evaluate();
        }

        CurrentValue = isSet;
    }

    public Conditional(bool value)
    {
        _exactValue = value;
        Update();
    }

    public Conditional(List<ISimpleCondition> condition)
    {
        _conditions = condition;
        var updateManager = IoCManager.Resolve<ConditionalUpdateManager>();
        foreach (var cond in _conditions)
        {
            cond.RegisterUpdates(updateManager, this);
        }
        Update();
    }
}
