namespace VisualTSP;

public partial class App : Application
{
  /// <summary>
  /// Initializes the singleton application object. This is the first line of authored code
  /// executed, and as such is the logical equivalent of main() or WinMain().
  /// </summary>
  public App()
  {
    InitializeComponent();
  }

  protected Window? MainWindow { get; private set; }
  protected IHost? Host { get; private set; }

  protected async override void OnLaunched(LaunchActivatedEventArgs args)
  {
      var builder = this.CreateBuilder(args)
          .Configure(host => host.UseNavigation());
    MainWindow = builder.Window;
    MainWindow.SetWindowIcon();

    Host = await builder.NavigateAsync<MainPage>();
  }
}
