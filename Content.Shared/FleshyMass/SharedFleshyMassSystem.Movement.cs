using System.Linq;
using Content.Shared.Arcade;
using Robust.Shared.Physics;
using Robust.Shared.Utility;

namespace Content.Shared.FleshyMass;

public abstract partial class SharedFleshyMassSystem
{
    protected void InitializeMovement()
    {
        SubscribeLocalEvent<SharedFleshBallComponent, MoveEvent>(OnFleshBallMove);
    }

    private void UpdateFleshBall(EntityUid uid, SharedFleshBallComponent component)
    {
        if (!TryComp<PhysicsComponent>(uid, out var physicsComponent))
            return;

        if (!TryComp<SharedFleshyMassControllerComponent>(component.Controller, out var controllerComponent))
            return;

        if (component.State == FleshBallState.FootUp)
        {
            physicsComponent.ApplyLinearImpulse(component.MovementVector + (component.MovementVector.Normalized * controllerComponent.ExtraVisualVelocity));
            physicsComponent.BodyStatus = BodyStatus.InAir;
        } else if (component.State == FleshBallState.FootDown)
        {
            physicsComponent.ApplyLinearImpulse(-(component.MovementVector - (component.MovementVector.Normalized * controllerComponent.ExtraVisualVelocity)));
            physicsComponent.BodyStatus = BodyStatus.InAir;
        } else if (component.State == FleshBallState.Controller)
        {
            physicsComponent.ApplyLinearImpulse(component.MovementVector);
            physicsComponent.BodyStatus = BodyStatus.InAir;
        }
    }

    private void OnFleshBallMove(EntityUid uid, SharedFleshBallComponent component, ref MoveEvent args)
    {
        if (Deleted(component.Controller)) // might not be loaded yet.
            return;

        if (!TryRecalculatePartners(uid, component.Controller, component))
        {
            if (component.State == FleshBallState.Controller) // we may not be set up yet due to networking.
                return;

            _sawmill.Error($"Was unable to reattach {ToPrettyString(uid)} to it's controller, {ToPrettyString(component.Controller)}");
            QueueDel(uid); // Well fuck. TryRecalculatePartners will go to great lengths to try and make sure it has two partners. If it really doesn't, we're dead.
            return;
        }
        if (!TryComp(uid, out JointComponent? joints))
        {
            if (component.State == FleshBallState.Controller) // we may not be set up yet due to networking.
                return;

            _sawmill.Error($"Was unable to reattach {ToPrettyString(uid)} to it's controller, {ToPrettyString(component.Controller)}, even after joint correction succeeded!");
            QueueDel(uid);
            return;
        }

        if (!TryComp<SharedFleshyMassControllerComponent>(component.Controller, out var controller))
        {
            _sawmill.Error($"The controller for {ToPrettyString(uid)} got deleted.");
            QueueDel(uid);
            return;
        }




        var anyBehindMovementPlane = false;
        var anyAheadMovementPlane = false;
        foreach (var joint in joints.GetJoints)
        {
            var xformA = Transform(joint.BodyAUid);
            var xformB = Transform(joint.BodyBUid);

            var vectorTo = (xformA.WorldPosition - xformB.WorldPosition).Normalized;
            var moveNormalized = component.MovementVector.Normalized;
            var facingAngle = vectorTo.GetRelativeFacingAngle(moveNormalized);
            if (facingAngle.EqualsApprox(Angle.FromDegrees(180)) || facingAngle.EqualsApprox(Angle.FromDegrees(0)) || HasComp<SharedFleshyMassControllerComponent>(joint.BodyAUid) || HasComp<SharedFleshyMassControllerComponent>(joint.BodyBUid))
                continue; // probably stuck in us, or the controller in which case we don't care.
            anyBehindMovementPlane |= facingAngle > Angle.FromDegrees(90);
            anyAheadMovementPlane |= facingAngle <= Angle.FromDegrees(90);
            _sawmill.Debug($"{facingAngle.Degrees}");
        }

        _sawmill.Debug($"{anyBehindMovementPlane}, {anyAheadMovementPlane}");

        switch (component.State)
        {
            case FleshBallState.FootDown:
                if ((Transform(component.Controller).WorldPosition - Transform(component.Owner).WorldPosition).GetRelativeFacingAngle(component.MovementVector) <= Angle.FromDegrees(60))
                    SetState(component, FleshBallState.FootUp); // start heading forward.
                break;
            case FleshBallState.FootUp:
                if ((Transform(component.Controller).WorldPosition - Transform(component.Owner).WorldPosition).GetRelativeFacingAngle(component.MovementVector) >= Angle.FromDegrees(120))
                    SetState(component, FleshBallState.FootDown); // we're on the ground now.
                break;
            case FleshBallState.Controller:
                var centerOfMass =
                    controller.FleshBalls.Select(x => Transform(x).WorldPosition)
                        .Aggregate(Vector2.Zero, (x, y) => x + y) / controller.FleshBalls.Count;
                Transform(controller.Owner).WorldPosition = centerOfMass; // TODO: Remove when it's made controllable.
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(component.State), $"Unexpected component state. Did you forget to update {nameof(OnFleshBallMove)}?");
        }
    }

    protected bool TryRecalculatePartners(
        EntityUid fleshBall,
        EntityUid controller,
        SharedFleshBallComponent? fleshBallComponent = null,
        TransformComponent? fleshBallXform = null,
        PhysicsComponent? fleshBallPhysics = null,
        JointComponent? fleshBallJoints = null,
        SharedFleshyMassControllerComponent? controllerComponent = null,
        bool retryAtController = false)
    {
        if (!Resolve(fleshBall, ref fleshBallComponent, ref fleshBallXform, ref fleshBallPhysics, ref fleshBallJoints))
            throw new ArgumentException("The given entity was not actually a flesh ball!", nameof(fleshBall));

        if (!Resolve(controller, ref controllerComponent))
            throw new ArgumentException("The given entity was not actually a fleshy mass controller!", nameof(fleshBall));

        if (retryAtController)
        {
            Transform(fleshBall).WorldPosition = Transform(controller).WorldPosition; // Send us to the controller.
        }

        var possibleJointCount = 0;

        foreach (var otherBall in controllerComponent.FleshBalls)
        {
            if (otherBall == fleshBall)
                continue;

            if (Deleted(otherBall))
                continue; // might not be loaded yet.

            var otherXform = Transform(otherBall);

            if ((otherXform.WorldPosition - fleshBallXform.WorldPosition).Length <= fleshBallComponent.JointLength)
                possibleJointCount += 1;
        }

        // Make sure we don't accidentally throw away all of our existing joints.
        switch (possibleJointCount)
        {
            case < 1 when fleshBallJoints.JointCount >= 1:
                return true;
            case < 1 when fleshBallJoints.JointCount < 1 && !retryAtController:
                return TryRecalculatePartners(fleshBall, controller, fleshBallComponent, fleshBallXform, fleshBallPhysics,
                    fleshBallJoints, controllerComponent, true);
            case < 1 when fleshBallJoints.JointCount < 1:
                return false;
        }

        _jointSystem.ClearJoints(fleshBallPhysics);
        var jointCount = 0;

        foreach (var otherBall in controllerComponent.FleshBalls)
        {
            if (otherBall == fleshBall)
                continue;

            if (Deleted(otherBall))
                continue; // might not be loaded yet.

            var otherXform = Transform(otherBall);

            if ((otherXform.WorldPosition - fleshBallXform.WorldPosition).Length <= fleshBallComponent.JointLength)
            {
                var distJoint = _jointSystem.CreateDistanceJoint(fleshBall, otherBall);
                distJoint.MaxLength = fleshBallComponent.JointLength + 0.1f;
                distJoint.MinLength = 0.3f;
                distJoint.CollideConnected = true;
                jointCount += 1;
            }
        }

        DebugTools.Assert(jointCount >= 1);

        return true;
    }

    protected void SetState(SharedFleshBallComponent component, FleshBallState state)
    {
        component.State = state;
    }
}
