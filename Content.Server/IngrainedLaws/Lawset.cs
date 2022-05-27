using Robust.Shared.Serialization;

namespace Content.Server.IngrainedLaws;

[NetSerializable, Serializable]
public sealed class LawSet
{
    public SortedDictionary<int, (LawConfig, string)> Laws = new();
}

public struct LawConfig
{
    public PointStyle LawPointStyle = PointStyle.Indexed;

    public LawConfig()
    {
    }
}

public enum PointStyle
{
    /// <summary>
    /// No number assigned.
    /// </summary>
    Bulleted,
    /// <summary>
    /// Use the index of this law in the law dictionary.
    /// </summary>
    Indexed,
    /// <summary>
    /// Should have a glitched out index.
    /// </summary>
    Ion,
}
