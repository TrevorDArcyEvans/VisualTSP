namespace VisualTSP.Presentation;

public sealed partial class VisualNode : UserControl, IHighlightable
{
    public VisualNode()
    {
        InitializeComponent();
        UpdateToolTip();
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

    public void UpdateToolTip()
    {
        ToolTipService.SetToolTip(this, DisplayName.Text);
    }
}
