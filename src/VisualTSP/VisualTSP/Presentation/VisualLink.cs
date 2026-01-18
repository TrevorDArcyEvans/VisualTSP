namespace VisualTSP.Presentation;

using Microsoft.UI;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;

public sealed class VisualLink : Line
{
    public VisualLink()
    {
        //Canvas.ZIndex = "-1";
        X1 = 25;
        Y1 = 25;
        X2 = 45;
        Y2 = 45;
        Tag = "42";
        Stroke = "Black";
        StrokeThickness = 4d;
        PointerEntered += Shape_OnMouseEntered;
        PointerExited += Shape_OnMouseExited;
        ToolTipService.SetToolTip(this, Tag);
    }

    private Brush _oldColour;
    private Brush _oldStroke;

    private void Shape_OnMouseEntered(object sender, PointerRoutedEventArgs e)
    {
        var shape = (Shape) sender;
        _oldColour = shape.Fill;
        _oldStroke = shape.Stroke;
        shape.Fill = shape.Stroke = new SolidColorBrush(Colors.Chartreuse);
    }

    private void Shape_OnMouseExited(object sender, PointerRoutedEventArgs e)
    {
        var shape = (Shape) sender;
        shape.Fill = _oldColour;
        shape.Stroke = _oldStroke;
    }
}
