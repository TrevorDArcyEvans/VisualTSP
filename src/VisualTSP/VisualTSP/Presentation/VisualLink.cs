namespace VisualTSP.Presentation;

using Microsoft.UI.Xaml.Shapes;

public sealed class VisualLink : Line
{
    public VisualLink()
    {
        //Canvas.ZIndex = "-1";
        X1=25;
        Y1=25;
        X2=45;
        Y2=45;
        Tag = "42";
        Stroke = "Black";
        StrokeThickness = 4d;
    }
}

