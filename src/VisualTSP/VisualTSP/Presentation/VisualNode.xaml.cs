namespace VisualTSP.Presentation;

using Models;

public sealed partial class VisualNode : UserControl, IHighlightable
{
    public Node Node { get; set; } = new() {Name = "New Node"};

    public VisualNode()
    {
        InitializeComponent();
        UpdateToolTip();
    }

    public VisualNode(JsonNode node) :
        this()
    {
        Node = node.Node;
        Canvas.SetLeft(this, node.Left);
        Canvas.SetTop(this, node.Top);
        
        DisplayName.Text = Node.Name;
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
