namespace VisualTSP.Presentation;

using Microsoft.UI;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;

public sealed partial class VisualNode : UserControl
{
    public VisualNode()
    {
        InitializeComponent();
        ToolTipService.SetToolTip(this, Name.Text);
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
