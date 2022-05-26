using System.Threading;
using Content.Server.Administration.Commands;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Mind.Commands;
using Content.Server.Mind.Components;
using Content.Server.Players;
using Content.Shared.Body.Components;
using Content.Shared.Database;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Timer = Robust.Shared.Timing.Timer;

namespace Content.Server.Administration.Systems;

public sealed partial class AdminVerbSystem
{

    private void AddTrickVerbs(GetVerbsEvent<Verb> args)
    {
        if (!EntityManager.TryGetComponent<ActorComponent?>(args.User, out var actor))
            return;

        var player = actor.PlayerSession;

        if (_groupController.CanCommand(player, "rejuvenate"))
        {
            Verb rejuvenate = new()
            {
                Category = VerbCategory.Tricks,
                IconTexture = "/Textures/Interface/VerbIcons/rejuvenate.svg.192dpi.png",
                Act = () => RejuvenateCommand.PerformRejuvenate(args.Target),
                Impact = LogImpact.Medium,
                Message = "Rejuvenate the object."
            };
            args.Verbs.Add(rejuvenate);
        }

        // Quick control mob.
        if (_groupController.CanCommand(player, "controlmob") &&
            args.User != args.Target)
        {
            Verb controlMob = new() {
                Text = Loc.GetString("control-mob-verb-get-data-text"),
                Category = VerbCategory.Tricks,
                IconTexture = "/Textures/Interface/VerbIcons/sentient.svg.192dpi.png",
                Act = () =>
                {
                    if (!EntityManager.HasComponent<MindComponent>(args.Target))
                        MakeSentientCommand.MakeSentient(args.Target, EntityManager);
                    player.ContentData()?.Mind?.TransferTo(args.Target, ghostCheckOverride: true);
                },
                Impact = LogImpact.High,
                Message = "Control this object, making it sentient if it isn't already."
            };
            args.Verbs.Add(controlMob);
        }
    }
}
