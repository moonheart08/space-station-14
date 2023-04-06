using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.ExChat;

/// <summary>
///     Raised when a message is being said by a given speaker.
/// </summary>
/// <param name="Speaker"></param>
/// <param name="Message">The message entity.</param>
[ByRefEvent]
public record struct CanSayMessageAs(EntityUid Speaker, EntityUid Message, bool Cancelled = false);

public record struct TransferMessage(EntityUid Message);

[NetSerializable, Serializable]
public sealed class TransferMessageNetworked : EntityEventArgs
{
    public EntityUid Message;
}

public record struct ReadMessageAs(EntityUid Reader, string Message);

[ByRefEvent]
public record struct CanTransferMessage(EntityUid Sender, EntityUid Receiver, EntityUid Message, bool Cancelled = false);

[ByRefEvent]
public record struct CanTransferMessageDirect(EntityUid Sender, NetUserId Receiver, EntityUid Message, bool Cancelled = false);

public record struct CloneChatMessage(EntityUid Message, EntityUid NewMessage);

[ByRefEvent]
public record struct GetListenersEvent(EntityUid Message, HashSet<EntityUid> EntityListeners, HashSet<NetUserId> UserListeners);
