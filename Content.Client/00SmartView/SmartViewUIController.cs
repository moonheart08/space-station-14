using Robust.Client.UserInterface.Controllers;

namespace Content.Client._00SmartView;

public sealed class SmartViewUIController : UIController
{
    /// <summary>
    /// Fired when a smart view is closed.
    /// </summary>
    public event Action? OnViewClosed;

    /// <summary>
    /// Fired when a smart view is windowed.
    /// </summary>
    public event Action? OnViewWindowed;

    /// <summary>
    /// Fired when a smart view is opened.
    /// </summary>
    public event Action? OnViewOpened;

    public List<SmartViewControl> Views = new();
}
