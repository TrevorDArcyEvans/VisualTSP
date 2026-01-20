namespace VisualTSP.Presentation;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Web;
using Windows.UI.Input;
using Microsoft.UI;
using Microsoft.UI.Xaml.Input;
using Newtonsoft.Json;
using Serialisation;

public sealed partial class MainPage : INotifyPropertyChanged
{
    public MainPage()
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private PointerPoint _currPoint;

    private void Canvas_OnPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        _currPoint = e.GetCurrentPoint(null);
        MousePos.Text = $"({_currPoint.Position.X:0}, {_currPoint.Position.Y:0})@{((CompositeTransform) Surface.RenderTransform).ScaleX}";
    }

    #region Shape

    private bool _drag;
    private PointerPoint _startPoint;

    // we already have 4 circle, so topmost in Z order will be 5
    // NOTE: this will eventually overflow but is good enough for a demo
    private int _currZindex = 5;

    private Brush _oldColour;
    private Brush _oldStroke;

    private void Shape_OnMouseEntered(object sender, PointerRoutedEventArgs e)
    {
        var node = (IHighlightable) sender;
        _oldColour = node.Fill;
        _oldStroke = node.Stroke;
        node.Fill = node.Stroke = new SolidColorBrush(Colors.Chartreuse);

        // remove canvas CSM otherwise we get it displayed along with shape CSM
        Surface.ContextFlyout = null;
    }

    private void Shape_OnMouseDown(object sender, PointerRoutedEventArgs e)
    {
        // start dragging
        _drag = true;

        // save start point of dragging
        _startPoint = e.GetCurrentPoint(Surface);

        // move selected circle to the top of the Z order
        var node = (VisualNode) sender;
        Canvas.SetZIndex(node, _currZindex++);
    }

    private void Shape_OnMouseMove(object sender, PointerRoutedEventArgs e)
    {
        if (!_drag)
        {
            return;
        }

        // if dragging, then adjust circle position based on mouse movement
        var node = (VisualNode) sender;
        var left = Canvas.GetLeft(node);
        var top = Canvas.GetTop(node);
        var newPoint = e.GetCurrentPoint(Surface);
        Canvas.SetLeft(node, left + (newPoint.RawPosition.X - _startPoint.RawPosition.X));
        Canvas.SetTop(node, top + (newPoint.RawPosition.Y - _startPoint.RawPosition.Y));

        UpdateLinks(node);

        // save where we end up
        _startPoint = newPoint;
    }

    private void UpdateLinks(VisualNode node)
    {
        // have to use ActualWidth/Height since composite control
        var linksStarts = Surface.Children
            .OfType<VisualLink>()
            .Where(x => x.Link.Start == node.Node.Id);
        foreach (var link in linksStarts)
        {
            link.X1 = Canvas.GetLeft(node) + node.ActualWidth / 2;
            link.Y1 = Canvas.GetTop(node) + node.ActualHeight / 2;
        }

        var linksEnds = Surface.Children
            .OfType<VisualLink>()
            .Where(x => x.Link.End == node.Node.Id);
        foreach (var link in linksEnds)
        {
            link.X2 = Canvas.GetLeft(node) + node.ActualWidth / 2;
            link.Y2 = Canvas.GetTop(node) + node.ActualHeight / 2;
        }
    }

    private void Shape_OnMouseUp(object sender, PointerRoutedEventArgs e)
    {
        // stop dragging
        _drag = false;
    }

    private void Shape_OnMouseExited(object sender, PointerRoutedEventArgs e)
    {
        // stop dragging
        _drag = false;

        var item = (IHighlightable) sender;
        item.Fill = _oldColour;
        item.Stroke = _oldStroke;

        // restore canvas CSM
        Surface.ContextFlyout = AddNodeMenu;
    }

    #endregion

    #region Zoom

    private const double ZoomInc = 0.1;

    private void Zoom_In(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform) Surface.RenderTransform;
        ct.ScaleX += ZoomInc;
        ct.ScaleY += ZoomInc;
    }

    private void Zoom_Fit(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform) Surface.RenderTransform;
        ct.ScaleX = 1.0;
        ct.ScaleY = 1.0;
    }

    private void Zoom_Out(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform) Surface.RenderTransform;
        ct.ScaleX -= ZoomInc;
        ct.ScaleY -= ZoomInc;
    }

    #endregion

    #region Translate

    private const double TranslateInc = 10d;

    private void Translate_Up(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform) Surface.RenderTransform;
        ct.TranslateY -= TranslateInc;
    }

    private void Translate_Down(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform) Surface.RenderTransform;
        ct.TranslateY += TranslateInc;
    }

    private void Translate_Left(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform) Surface.RenderTransform;
        ct.TranslateX -= TranslateInc;
    }

    private void Translate_Right(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform) Surface.RenderTransform;
        ct.TranslateX += TranslateInc;
    }

    private void Translate_Reset(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform) Surface.RenderTransform;
        ct.TranslateX = 0;
        ct.TranslateY = 0;
    }

    #endregion

    private void AddNode(object sender, RoutedEventArgs e)
    {
        var node = new VisualNode();
        ConnectListeners(node);

        Canvas.SetLeft(node, _currPoint.Position.X);
        Canvas.SetTop(node, _currPoint.Position.Y);

        Surface.Add(node);
    }

    private void ConnectListeners(VisualNode node)
    {
        node.PointerEntered += Shape_OnMouseEntered;
        node.PointerPressed += Shape_OnMouseDown;
        node.PointerMoved += Shape_OnMouseMove;
        node.PointerReleased += Shape_OnMouseUp;
        node.PointerExited += Shape_OnMouseExited;
        node.ContextFlyout = EditNodeMenu;
    }

    private void ConnectListeners(VisualLink link)
    {
        link.PointerEntered += Shape_OnMouseEntered;
        link.PointerExited += Shape_OnMouseExited;
        link.ContextFlyout = EditLinkMenu;
    }

    #region StartNode +EndNode

    private Guid _startNode;
    private Guid _endNode;

    public bool IsStart
    {
        get;

        set
        {
            if (value == field)
            {
                return;
            }

            field = value;
            OnPropertyChanged();
        }
    }

    public bool IsEnd
    {
        get;

        set
        {
            if (value == field)
            {
                return;
            }

            field = value;
            OnPropertyChanged();
        }
    }

    private void EditNodeMenu_OnOpening(object sender, object e)
    {
        IsStart = ((VisualNode) EditNodeMenu.Target).Node.Id == _startNode;
        IsEnd = ((VisualNode) EditNodeMenu.Target).Node.Id == _endNode;
    }

    private void StartNode(object sender, RoutedEventArgs e)
    {
        _startNode = ((VisualNode) EditNodeMenu.Target).Node.Id;
    }

    private void EndNode(object sender, RoutedEventArgs e)
    {
        _endNode = ((VisualNode) EditNodeMenu.Target).Node.Id;
    }

    #endregion

    private void AddLink(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private async void EditNode(object sender, RoutedEventArgs e)
    {
        var node = (VisualNode) EditNodeMenu.Target;
        var dlg = new NodeNameDialog(node)
        {
            XamlRoot = XamlRoot
        };
        await dlg.ShowAsync();
    }

    private void DeleteNode(object sender, RoutedEventArgs e)
    {
        var node = (VisualNode) EditNodeMenu.Target;
        DeleteAssociatedLinks((node));
        Surface.Children.Remove(node);
    }

    private void DeleteAssociatedLinks(VisualNode node)
    {
        var links = Surface.Children
            .OfType<VisualLink>()
            .Where(x => x.Link.Start == node.Node.Id || x.Link.End == node.Node.Id);
        foreach (var link in links)
        {
            Surface.Children.Remove(link);
        }
    }

    private async void EditLink(object sender, RoutedEventArgs e)
    {
        var link = (VisualLink) EditLinkMenu.Target;
        var dlg = new LinkCostDialog(link)
        {
            XamlRoot = XamlRoot
        };
        await dlg.ShowAsync();
    }

    private void DeleteLink(object sender, RoutedEventArgs e)
    {
        var link = (VisualLink) EditLinkMenu.Target;
        Surface.Children.Remove(link);
    }

    #region Serialisation/Deserialisation

    private async void OnOpen(object sender, RoutedEventArgs e)
    {
        var picker = new Windows.Storage.Pickers.FileOpenPicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
        };
        picker.FileTypeFilter.Add(".tsp");
        picker.FileTypeFilter.Add("*");
        var file = await picker.PickSingleFileAsync();
        if (file == null)
        {
            return;
        }

        // spaces are html escaped as %20 etc
        var filePath = HttpUtility.UrlDecode(file.Path);

        // add extension if user hasn't
        filePath = Path.ChangeExtension(filePath, ".tsp");

        // update file variable
        file = await StorageFile.GetFileFromPathAsync(filePath);

        var json = await FileIO.ReadTextAsync(file);
        var network = JsonConvert.DeserializeObject<JsonNetwork>(json);

        LoadNetwork(network);

        var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
        appView.Title = $"{Package.Current.DisplayName} - {file.DisplayName}";
    }

    private void LoadNetwork(JsonNetwork network)
    {
        var nodes = network.Nodes.Select(x =>
        {
            var visNode = new VisualNode(x);
            ConnectListeners(visNode);
            return visNode;
        });
        var links = network.Links.Select(x =>
        {
            var visLink = new VisualLink(x);
            ConnectListeners(visLink);
            return visLink;
        });
        _startNode = network.Start;
        _endNode = network.End;

        Surface.Children.Clear();

        Surface.Children.AddRange(links);
        Surface.Children.AddRange(nodes);
    }

    private async void OnSave(object sender, RoutedEventArgs e)
    {
        var savePicker = new Windows.Storage.Pickers.FileSavePicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "New Document"
        };
        savePicker.FileTypeChoices.Add("Travelling Salesman Problem (*.tsp)", (List<string>) [".tsp"]);
        var file = await savePicker.PickSaveFileAsync();
        if (file == null)
        {
            return;
        }

        // Prevent updates to the remote version of the file until
        // we finish making changes and call CompleteUpdatesAsync.
        CachedFileManager.DeferUpdates(file);
        if (!File.Exists(file.Path))
        {
            // spaces are html escaped as %20 etc
            var filePath = HttpUtility.UrlDecode(file.Path);

            // add extension if user hasn't
            filePath = Path.ChangeExtension(filePath, ".tsp");

            // update file variable
            file = await StorageFile.GetFileFromPathAsync(filePath);

            // create file in target folder
            var folder = Path.GetDirectoryName(file.Path);
            var storFolder = await StorageFolder.GetFolderFromPathAsync(folder);
            await storFolder.CreateFileAsync(file.Name, CreationCollisionOption.ReplaceExisting);
        }

        var json = Serialize(file.DisplayName);

        // write to file
        await FileIO.WriteTextAsync(file, json);

        // Let Windows know that we're finished changing the file so
        // the other app can update the remote version of the file.
        // Completing updates may require Windows to ask for user input.
        var status = await CachedFileManager.CompleteUpdatesAsync(file);
        if (status != Windows.Storage.Provider.FileUpdateStatus.Complete)
        {
            throw new FileNotFoundException($"File {file.Name} couldn't be saved.");
        }
    }

    private string Serialize(string fileName)
    {
        var links = Surface.Children.OfType<VisualLink>().Select(x => new JsonLink(x)).ToList();
        var nodes = Surface.Children.OfType<VisualNode>().Select(x => new JsonNode(x)).ToList();

        var network = new JsonNetwork(_startNode, _endNode)
        {
            Name = fileName,
            Nodes = nodes,
            Links = links
        };

        var json = JsonConvert.SerializeObject(network, Formatting.Indented);
        var newNetwork = JsonConvert.DeserializeObject<JsonNetwork>(json);
        var newJson = JsonConvert.SerializeObject(newNetwork, Formatting.Indented);

        return json;
    }

    #endregion
}
