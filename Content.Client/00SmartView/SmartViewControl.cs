using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._00SmartView;

public sealed class SmartViewControl : Control
{
    private readonly Color _defaultColor = Color.FromHex("#25252AAA");

    private Control _contents;

    public SmartViewControl(Control contents)
    {
        _contents = contents;
        AddChild(new PanelContainer { PanelOverride = new StyleBoxFlat(_defaultColor)});
        AddChild(new BoxContainer { Name = "ChildContainer", Children = { _contents }});
        Measure(Vector2.Infinity);
        UserInterfaceManager.WindowRoot.AddChild(this);
    }

    public void RecenterView(Vector2 relativePosition)
    {
        if (Parent == null)
            return;

        // Where we want the upper left corner of the window to be
        var corner = Parent!.Size * Vector2.Clamp(relativePosition, Vector2.Zero, Vector2.One) - DesiredSize / 2;

        // Attempt to keep the whole window is visible, regardless of the target position. e.g., if the target for
        // the center is (0,0), this will actually open the window so that the upper left is at (0,0). If the window
        // is bigger than the parent, this will currently prioritize showing the upper left corner.
        var pos = Vector2.Clamp(corner, Vector2.Zero, Parent.Size - DesiredSize);
        LayoutContainer.SetPosition(this, pos);
    }
}
