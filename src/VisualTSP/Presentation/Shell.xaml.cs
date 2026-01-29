namespace VisualTSP.Presentation;

public sealed partial class Shell : IContentControlProvider
{
  public Shell()
  {
    InitializeComponent();
  }

  public ContentControl ContentControl => Splash;
}
