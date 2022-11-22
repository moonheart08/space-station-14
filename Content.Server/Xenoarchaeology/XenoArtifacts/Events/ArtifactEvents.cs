namespace Content.Server.Xenoarchaeology.XenoArtifacts.Events;

/// <summary>
///     Invokes when artifact was successfully activated.
///     Used to start attached effects.
/// </summary>
/// <param name="Activator">Entity that activated the artifact if any.</param>
public readonly record struct ArtifactActivatedEvent(EntityUid? Activator);

/// <summary>
///     Force to randomize artifact triggers.
/// </summary>
/// <param name="RandomSeed">An RNG seed that can be used to control random events for the given node.</param>
public readonly record struct ArtifactNodeEnteredEvent(int RandomSeed);
