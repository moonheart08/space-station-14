using Robust.Shared.Configuration;

namespace Content.Shared.Conditionals;

public sealed class ConditionalUpdateManager
{
    [Dependency] private readonly IConfigurationManager _configurationManager = default!;
    // TODO: Sandbox PR and make this a weakref.
    private readonly Dictionary<string, HashSet<Conditional>> _cvarUpdateDictionary = new();

    public void RegisterConditionalForUpdates(string cvar, Conditional conditional)
    {
        if (!_cvarUpdateDictionary.ContainsKey(cvar))
        {
            _cvarUpdateDictionary.Add(cvar, new HashSet<Conditional>(16));
            var cvarType = _configurationManager.GetCVarType(cvar);

            if (cvarType == typeof(bool))
            {
                _configurationManager.OnValueChanged<bool>(cvar, _ =>
                {
                    foreach (var toUpdate in _cvarUpdateDictionary[cvar])
                    {
                        toUpdate.Update();
                    }
                });
            }
            else
            {
                Logger.Error($"Tried to register a conditional on CVar {cvar}, which is an unsupported type {cvarType}");
                return;
            }
        }

        _cvarUpdateDictionary[cvar].Add(conditional);
    }
}
