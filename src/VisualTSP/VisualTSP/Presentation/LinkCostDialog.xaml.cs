namespace VisualTSP.Presentation;

public partial class LinkCostDialog
{
    private readonly VisualLink _link;

    public LinkCostDialog(VisualLink link) :
        this()
    {
        _link = link;
        Cost.Text = _link.Tag.ToString();
    }

    public LinkCostDialog()
    {
        InitializeComponent();
        Cost.Focus(FocusState.Keyboard);
    }
    
    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        IsPrimaryButtonEnabled = int.TryParse(Cost.Text, out _);
    }

    private void OnOkClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var value = int.Parse(Cost.Text);
        _link.Tag = _link.Link.Cost = value;
        _link.UpdateToolTip();
    }
}
