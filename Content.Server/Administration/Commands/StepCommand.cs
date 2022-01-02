using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Server.Administration.Commands;

[AdminCommand(AdminFlags.Debug)]
public class StepCommand : IConsoleCommand
{
    public string Command => "step";

    public string Description => "Steps an entity with the given vector, accounting for it's rotation.";

    public string Help => "step <uid> <x> <y>";

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length < 3 || !float.TryParse(args[1], out var posX) || !float.TryParse(args[2], out var posY) || !EntityUid.TryParse(args[0], out var ent))
        {
            shell.WriteError(Help);
            return;
        }
        var transform = IoCManager.Resolve<IEntityManager>().GetComponent<TransformComponent>(ent);
        var wasAnchored = transform.Anchored;
        transform.Anchored = false;
        transform.LocalPosition += transform.LocalRotation.RotateVec(new Vector2(posX, posY));
        transform.Anchored = wasAnchored;
    }
}
