namespace VisualTSP.Presentation;

using Microsoft.UI.Xaml.Shapes;

public sealed class VisualLink : Line, IHighlightable
{
    public VisualLink()
    {
        Stroke = "Black";
        StrokeThickness = 4d;
        Tag = "42";
        UpdateToolTip();
    }
    
    public void UpdateToolTip()
    {
        ToolTipService.SetToolTip(this, Tag);
    }
}
