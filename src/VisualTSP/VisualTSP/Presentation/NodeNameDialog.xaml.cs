namespace VisualTSP.Presentation;

public partial class NodeNameDialog
{
    private readonly VisualNode _node;

    public NodeNameDialog(VisualNode node) :
        this()
    {
        _node = node;
        DisplayName.Text = _node.DisplayName.Text;
    }

    public NodeNameDialog()
    {
        InitializeComponent();
        DisplayName.Focus(FocusState.Keyboard);
    }
    
    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        IsPrimaryButtonEnabled = DisplayName.Text.Length > 0;
    }

    private void OnOkClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        _node.DisplayName.Text = DisplayName.Text;
        _node.UpdateToolTip();
    }
}
