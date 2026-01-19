namespace VisualTSP.Presentation;

public sealed partial class VisualNode : UserControl, IHighlightable
{
    public VisualNode()
    {
        InitializeComponent();
        ToolTipService.SetToolTip(this, Name.Text);
    }

    public Brush Fill
    {
        get
        {
            return Shape.Fill;
        }

        set
        {
            Shape.Fill = value;
        }
    }

    public Brush Stroke
    {
        get
        {
            return Shape.Stroke;
        }
        set
        {
            Shape.Stroke = value;
        }
    }
}
