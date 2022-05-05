namespace Content.Server.Autopilot.Components;

[RegisterComponent]
public sealed class RangeSensorComponent : Component
{
    /// <summary>
    /// The maximum range to raytrace when checking sensor readings. Be careful with this,
    /// setting it too high may result in Folivora-induced death.
    /// </summary>
    [DataField("maximumRange")] public float MaximumRange = 128.0f;

    /// <summary>
    /// Whether or not to automatically calibrate the range of the sensor based on it's facing direction and estimated shuttle stop times.
    /// </summary>
    [DataField("autoCalibrateRange")] public bool AutoCalibrateRange = true;

    /// <summary>
    /// Whether to shoot one ray or three. Good for shorter-range wall detection.
    /// </summary>
    [DataField("omnidirectional")] public bool Omnidirectional = false;
    /// <summary>
    /// The "class" of this range sensor.
    /// Used so that you can have both long distance and short-range sensors on the same boat.
    /// </summary>
    [DataField("class", required: true)] public string Class = string.Empty;

    /// <summary>
    /// Rate at which the sensor checks for updates.
    /// </summary>
    [DataField("updateRate")] public float UpdateRate = 0.2f;

    /// <summary>
    /// Frame-time accumulator used for updating.
    /// </summary>
    [DataField("accumulator")] public float Accumulator = 0.0f;

    /// <summary>
    /// The last reading from the sensor. It only raises an event if this changes.
    /// </summary>
    [DataField("priorDistance")] public float? PriorDistance = null;
}
