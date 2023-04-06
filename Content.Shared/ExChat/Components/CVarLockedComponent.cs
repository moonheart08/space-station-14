using Robust.Shared.GameStates;

namespace Content.Shared.ExChat.Components;

/// <summary>
/// This is used for chat channels locked behind a boolean cvar.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed class CVarLockedComponent : Component
{
    [DataField("cvar", required: true), AutoNetworkedField]
    public string CVar = default!;

    [DataField("adminBypass", required: true), AutoNetworkedField]
    public bool AdminBypass = true;
}
