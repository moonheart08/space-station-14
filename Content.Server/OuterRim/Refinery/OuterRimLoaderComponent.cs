namespace Content.Server.OuterRim.Refinery;

/// <summary>
/// This is used for loading objects into a machine, or rather interacting with the machine using the object.
/// </summary>
[RegisterComponent, Access(typeof(AutomationSystem))]
public sealed class OuterRimLoaderComponent : Component
{
    /// <summary>
    /// The fixture to use for detecting entities that should get loaded into the target.
    /// </summary>
    [DataField("loadingFixture", required: true, readOnly: true)]
    public string LoadingFixture = default!;
}
