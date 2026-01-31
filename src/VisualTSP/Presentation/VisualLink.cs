namespace VisualTSP.Presentation;

using Microsoft.UI.Xaml.Shapes;
using Models;
using Serialisation;

public sealed class VisualLink : Line, IHighlightable
{
    public static Brush DefaultStroke = "Black";
    public const double DefaultStrokeThickness = 4d;

    public Link Link { get; set; } = new();

    public VisualLink()
    {
        Stroke = DefaultStroke;
        StrokeThickness = DefaultStrokeThickness;
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

    public JsonLink ToJsonLink()
    {
        return new JsonLink
        {
            Link = Link,
            X1 = (int) X1,
            Y1 = (int) Y1,
            X2 = (int) X2,
            Y2 = (int) Y2
        };
    }

    public void UpdateToolTip()
    {
        ToolTipService.SetToolTip(this, Tag);
    }
}
