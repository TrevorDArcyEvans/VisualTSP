namespace VisualTSP.Presentation;

public partial class MainViewModel : ObservableObject
{
  [ObservableProperty]
  private string? name = "tde";

  public MainViewModel()
  {
    Title = "Main";
  }

  public string? Title { get; }
}
