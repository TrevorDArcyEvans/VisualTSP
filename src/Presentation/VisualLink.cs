namespace VisualTSP.Presentation;

using Microsoft.UI.Xaml.Shapes;
using Models;
using Serialisation;

public sealed class VisualLink : Line, IHighlightable
{
    public static Brush DefaultBrush = "Black";

    public Link Link { get; set; } = new();

    public VisualLink()
    {
        Stroke = DefaultBrush;
        StrokeThickness = 4d;
        Tag = 10;
        UpdateToolTip();
        Canvas.SetZIndex(this, -1);
    }

    public VisualLink(JsonLink link) :
        this()
    {
        Link = link.Link;
        X1 = link.X1;
        Y1 = link.Y1;
        X2 = link.X2;
        Y2 = link.Y2;

        Tag = Link.Cost;
        UpdateToolTip();
    }

    public void UpdateToolTip()
    {
        ToolTipService.SetToolTip(this, Tag);
    }
}
