using System.Diagnostics.CodeAnalysis;
using Content.Shared.ExChat.Components;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Utility;

namespace Content.Shared.ExChat.Systems;

/// <summary>
/// This handles chat messages, and sending/receiving them.
/// </summary>
public sealed partial class MessageSystem : EntitySystem
{
    [Dependency] private readonly ILogManager _log = default!;

    private ISawmill _sawmill = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        _sawmill = _log.GetSawmill("chat");
    }

    /// <summary>
    ///     Creates a new chat message without distributing and finalizing it.
    ///     This should be used for special handling and then immediately passed back over.
    ///     It's ill advised to have a chat message linger for multiple frames but it is allowed.
    /// </summary>
    /// <param name="speakerEnt">The entity creating the message.</param>
    /// <param name="msgProto">The prototype for the message.</param>
    /// <param name="message">The actual message text.</param>
    /// <param name="msgEnt">The final message entity, if any.</param>
    /// <param name="speakerXform">The transform of the speaker.</param>
    /// <returns>Operation success.</returns>
    public bool TryCreateSpokenMessage(EntityUid speakerEnt, string msgProto, string message,
        [NotNullWhen(true)] out EntityUid? msgEnt, TransformComponent? speakerXform = null)
    {
        if (!Resolve(speakerEnt, ref speakerXform))
        {
            _sawmill.Error($"Tried to speak as {speakerEnt} with message \"{message}\" ({msgProto}), but speaker did not exist or is transformless.");
            msgEnt = null;
            return false;
        }

        msgEnt = InternalBuildMessage(speakerEnt, null, msgProto, message, speakerXform);
        return true;
    }

    /// <inheritdoc cref="MessageSystem.TryCreateSpokenMessage"/>
    /// <param name="speakerUser">The user creating the message.</param>
    /// <param name="msgProto">The prototype for the message.</param>
    /// <param name="message">The actual message text.</param>
    /// <param name="msgEnt">The final message entity, if any.</param>
    /// <returns>Operation success.</returns>
    public bool TryCreateBroadcastMessage(NetUserId? speakerUser, string msgProto, string message,
        [NotNullWhen(true)] out EntityUid? msgEnt)
    {
        msgEnt = InternalBuildMessage(null, speakerUser, msgProto, message, null);
        return true;
    }

    public bool TrySpeak(EntityUid speaker, string msgProto, string message)
    {
        if (!TryCreateSpokenMessage(speaker, msgProto, message, out var ent))
            return false;

        if (!TrySayMessage(speaker, ent.Value))
        {
            Del(ent.Value); // Clean up.
            return false;
        }

        return true;
    }

    public bool TrySayMessage(EntityUid speaker, EntityUid message)
    {
        var canSayEv = new CanSayMessageAs(speaker, message);
        RaiseLocalEvent(speaker, ref canSayEv, broadcast: true);

        if (canSayEv.Cancelled)
            return false;

        // Okay, we can actually start doing the rest of the lifting.
        var listenersEv = new GetListenersEvent(message, new(), new());
        RaiseLocalEvent(speaker, ref listenersEv, broadcast: true);

        var listenersToRemove = new HashSet<EntityUid>();

        foreach (var listener in listenersEv.EntityListeners)
        {
            var transferEv = new CanTransferMessage(speaker, listener, message);

            RaiseLocalEvent(speaker, ref transferEv, broadcast: true);
            RaiseLocalEvent(listener, ref transferEv);

            if (transferEv.Cancelled)
                listenersToRemove.Add(listener);
        }

        listenersEv.EntityListeners.ExceptWith(listenersToRemove);

        var usersToRemove = new HashSet<NetUserId>();

        foreach (var listener in listenersEv.UserListeners)
        {
            var transferEv = new CanTransferMessageDirect(speaker, listener, message);

            RaiseLocalEvent(speaker, ref transferEv, broadcast: true);

            if (transferEv.Cancelled)
                usersToRemove.Add(listener);
        }

        listenersEv.UserListeners.ExceptWith(usersToRemove);

        foreach (var listener in listenersEv.EntityListeners)
        {
            var ev = new TransferMessage(message);
            RaiseLocalEvent(listener, ev);
        }

        return true;
    }

    private EntityUid InternalBuildMessage(EntityUid? speakerEnt, NetUserId? speakerUser, string msgProto,
        string message, TransformComponent? speakerXform)
    {
        DebugTools.Assert(speakerEnt is not null || speakerUser is not null);

        EntityCoordinates coords;

        if (speakerEnt is { } ent && !Deleted(ent) && speakerXform is not null)
        {
            coords = speakerXform.Coordinates;
        }
        else
        {
            coords = EntityCoordinates.Invalid; // Nullspace.
        }

        var msgEnt = Spawn(msgProto, coords);

        var msg = Comp<ChatMessageComponent>(msgEnt);
        msg.RawMessage = message;
        msg.OriginalSpeaker = speakerEnt;
        msg.OriginalSpeakerPlayer = speakerUser;

        return msgEnt;
    }
}
