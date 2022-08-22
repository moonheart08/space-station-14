using Content.Shared.Whitelist;

namespace Content.Server.OuterRim.Refinery;

/// <summary>
/// This is used for converting LoaderTryLoadEvents into InteractUsingEvents for legacy systems.
/// </summary>
[RegisterComponent]
public sealed class OuterRimLoaderInteractionSimComponent : Component
{
    /// <summary>
    /// What entities are allowed to actually get a fake interaction handled.
    /// </summary>
    [DataField("whitelist", required: true, readOnly: true)]
    public EntityWhitelist Whitelist = default!;

    [DataField("doInteract", readOnly: true)]
    public bool DoInteract = true;

    [DataField("doAfterInteract", readOnly: true)]
    public bool DoAfterInteract = false;
}
