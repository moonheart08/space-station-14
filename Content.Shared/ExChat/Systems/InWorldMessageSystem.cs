using Content.Shared.ExChat.Components;

namespace Content.Shared.ExChat.Systems;

/// <summary>
/// This handles in-world messages and calculating the listener list.
/// </summary>
public sealed class InWorldMessageSystem : EntitySystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<InWorldMessageComponent, GetListenersEvent>(GetInWorldListeners);
    }

    private void GetInWorldListeners(EntityUid uid, InWorldMessageComponent component, ref GetListenersEvent args)
    {

    }
}
