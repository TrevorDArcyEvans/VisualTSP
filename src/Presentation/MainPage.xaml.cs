namespace VisualTSP.Presentation;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Web;
using Windows.UI.Input;
using Microsoft.UI;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using Models;
using Newtonsoft.Json;
using Serialisation;
using Solvers;
using Path = Path;

public sealed partial class MainPage : INotifyPropertyChanged
{
    public MainPage()
    {
        InitializeComponent();

        PropertyChanged += OnPropertyChanged;
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
        MousePos.Text = $"({_currPoint.Position.X:0}, {_currPoint.Position.Y:0})@{((CompositeTransform)Surface.RenderTransform).ScaleX}";

        if (_isLinking)
        {
            _linkPreview.X2 = _currPoint.Position.X;
            _linkPreview.Y2 = _currPoint.Position.Y;
        }
    }

    private void Canvas_OnPointerDown(object sender, PointerRoutedEventArgs e)
    {
        if (!_isLinking)
        {
            return;
        }

        // cancel link creation
        _isLinking = false;
        Surface.Children.Remove(_linkPreview);
    }

    #region Shape

    private bool _drag;
    private PointerPoint _startPoint;

    private Brush _oldColour;
    private Brush _oldStroke;

    private void Shape_OnMouseEntered(object sender, PointerRoutedEventArgs e)
    {
        var node = (IHighlightable)sender;
        _oldColour = node.Fill;
        _oldStroke = node.Stroke;
        node.Fill = node.Stroke = new SolidColorBrush(Colors.Chartreuse);

        // remove canvas CSM otherwise we get it displayed along with shape CSM
        Surface.ContextFlyout = null;
    }

    private void Shape_OnMouseDown(object sender, PointerRoutedEventArgs e)
    {
        var node = (VisualNode)sender;

        if (_isLinking)
        {
            _isLinking = false;
            Surface.Children.Remove(_linkPreview);

            var link = new VisualLink()
            {
                Link = new Link
                {
                    Start = _linkStart.Node.Id,
                    End = node.Node.Id,
                },
                X1 = Canvas.GetLeft(_linkStart) + _linkStart.ActualWidth / 2,
                Y1 = Canvas.GetTop(_linkStart) + _linkStart.ActualHeight / 2,
                X2 = Canvas.GetLeft(node) + node.ActualWidth / 2,
                Y2 = Canvas.GetTop(node) + node.ActualHeight / 2
            };
            ConnectListeners(link);
            Surface.Children.Add(link);

            return;
        }

        // start dragging
        _drag = true;

        // save start point of dragging
        _startPoint = e.GetCurrentPoint(Surface);
    }

    private void Shape_OnMouseMove(object sender, PointerRoutedEventArgs e)
    {
        if (!_drag)
        {
            return;
        }

        // if dragging, then adjust circle position based on mouse movement
        var node = (VisualNode)sender;
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

        var item = (IHighlightable)sender;
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
        var ct = (CompositeTransform)Surface.RenderTransform;
        ct.ScaleX += ZoomInc;
        ct.ScaleY += ZoomInc;
    }

    private void Zoom_Fit(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform)Surface.RenderTransform;
        ct.ScaleX = 1.0;
        ct.ScaleY = 1.0;
    }

    private void Zoom_Out(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform)Surface.RenderTransform;
        ct.ScaleX -= ZoomInc;
        ct.ScaleY -= ZoomInc;
    }

    #endregion

    #region Translate

    private const double TranslateInc = 10d;

    private void Translate_Up(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform)Surface.RenderTransform;
        ct.TranslateY -= TranslateInc;
    }

    private void Translate_Down(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform)Surface.RenderTransform;
        ct.TranslateY += TranslateInc;
    }

    private void Translate_Left(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform)Surface.RenderTransform;
        ct.TranslateX -= TranslateInc;
    }

    private void Translate_Right(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform)Surface.RenderTransform;
        ct.TranslateX += TranslateInc;
    }

    private void Translate_Reset(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform)Surface.RenderTransform;
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

    #region SetStartNode + SetEndNode

    public Guid StartNode
    {
        get;

        set
        {
            if (value.Equals(field))
            {
                return;
            }

            // remove previous highlight
            UnhighlightStartNode();

            field = value;

            // will update node highlight
            OnPropertyChanged();
        }
    }

    public Guid EndNode
    {
        get;

        set
        {
            if (value.Equals(field))
            {
                return;
            }

            // remove previous highlight
            UnhighlightEndNode();

            field = value;

            // will update node highlight
            OnPropertyChanged();
        }
    }

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
        IsStart = ((VisualNode)EditNodeMenu.Target).Node.Id == StartNode;
        IsEnd = ((VisualNode)EditNodeMenu.Target).Node.Id == EndNode;
    }

    private void SetStartNode(object sender, RoutedEventArgs e)
    {
        StartNode = ((VisualNode)EditNodeMenu.Target).Node.Id;
    }

    private void SetEndNode(object sender, RoutedEventArgs e)
    {
        EndNode = ((VisualNode)EditNodeMenu.Target).Node.Id;
    }

    private void HighlightStartNode()
    {
        var visNode = Surface.Children
            .OfType<VisualNode>()
            .SingleOrDefault(x => x.Node.Id == StartNode);
        visNode?.Start.Visibility = Visibility.Visible;
    }

    private void UnhighlightStartNode()
    {
        if (StartNode == Guid.Empty)
        {
            return;
        }

        var visNode = Surface.Children
            .OfType<VisualNode>()
            .SingleOrDefault(x => x.Node.Id == StartNode);
        visNode?.Start.Visibility = Visibility.Collapsed;
    }

    private void HighlightEndNode()
    {
        var visNode = Surface.Children
            .OfType<VisualNode>()
            .SingleOrDefault(x => x.Node.Id == EndNode);
        visNode?.End.Visibility = Visibility.Visible;
    }

    private void UnhighlightEndNode()
    {
        if (EndNode == Guid.Empty)
        {
            return;
        }

        var visNode = Surface.Children
            .OfType<VisualNode>()
            .SingleOrDefault(x => x.Node.Id == EndNode);
        visNode?.End.Visibility = Visibility.Collapsed;
    }

    #endregion

    private bool _isLinking;
    private VisualNode _linkStart;
    private Line _linkPreview;

    private void AddLink(object sender, RoutedEventArgs e)
    {
        _isLinking = true;
        _linkStart = (VisualNode)EditNodeMenu.Target;
        _linkPreview = new Line()
        {
            Stroke = "Black",
            StrokeThickness = 4d,
            X1 = _currPoint.Position.X,
            Y1 = _currPoint.Position.Y,
            X2 = _currPoint.Position.X,
            Y2 = _currPoint.Position.Y
        };
        Surface.Children.Add(_linkPreview);
    }

    private async void EditNode(object sender, RoutedEventArgs e)
    {
        var node = (VisualNode)EditNodeMenu.Target;
        var dlg = new NodeNameDialog(node)
        {
            XamlRoot = XamlRoot
        };
        await dlg.ShowAsync();
    }

    private void DeleteNode(object sender, RoutedEventArgs e)
    {
        var node = (VisualNode)EditNodeMenu.Target;
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
        var link = (VisualLink)EditLinkMenu.Target;
        var dlg = new LinkCostDialog(link)
        {
            XamlRoot = XamlRoot
        };
        await dlg.ShowAsync();
    }

    private void DeleteLink(object sender, RoutedEventArgs e)
    {
        var link = (VisualLink)EditLinkMenu.Target;
        Surface.Children.Remove(link);
    }

    #region Serialisation/Deserialisation

    private Network _network = new();

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
        var jsonNetwork = JsonConvert.DeserializeObject<JsonNetwork>(json);

        // everything else will be updated when we start solvers
        _network.Id = jsonNetwork.Id;
        _network.Name = jsonNetwork.Name;

        LoadNetwork(jsonNetwork);

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
        Surface.Children.Clear();

        Surface.Children.AddRange(links);
        Surface.Children.AddRange(nodes);

        // reset nodes so events will definitely get fired
        StartNode = EndNode = Guid.Empty;

        StartNode = network.Start;
        EndNode = network.End;
    }

    private async void OnSave(object sender, RoutedEventArgs e)
    {
        var savePicker = new Windows.Storage.Pickers.FileSavePicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
            SuggestedFileName = "New Document"
        };
        savePicker.FileTypeChoices.Add("Travelling Salesman Problem (*.tsp)", (List<string>)[".tsp"]);
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

        var network = new JsonNetwork(StartNode, EndNode)
        {
            Name = fileName,
            Nodes = nodes,
            Links = links
        };

        var json = JsonConvert.SerializeObject(network, Formatting.Indented);

        return json;
    }

    #endregion

    #region Calculation

    private bool IsSolving { get; set; }

    private void OnRestart(object sender, RoutedEventArgs e)
    {
    }

    private void OnNextStep(object sender, RoutedEventArgs e)
    {
    }

    private void OnPlayPause(object sender, RoutedEventArgs e)
    {
        IsSolving = !IsSolving;

        if (!IsSolving)
        {
            // stop solvers
            return;
        }

        // update nodes, links & start+end nodes
        _network.Nodes = Surface.Children.OfType<VisualNode>().Select((x => x.Node)).ToList();
        _network.Links = Surface.Children.OfType<VisualLink>().Select((x => x.Link)).ToList();
        _network.Start = StartNode;
        _network.End = EndNode;

        RunGreedySolver();
    }

    #region simulated annealing

    public float SimAnneal_Temperature
    {
        get;

        set
        {
            if (value.Equals(field))
            {
                return;
            }

            field = value;
            OnPropertyChanged();
        }
    } = 100;

    public float SimAnneal_Distance
    {
        get;

        set
        {
            if (value.Equals(field))
            {
                return;
            }

            field = value;
            OnPropertyChanged();
        }
    } = 155;

    #endregion

    #region greedy solver

    public int Greedy_Distance
    {
        get;

        set
        {
            if (value.Equals(field))
            {
                return;
            }

            field = value;
            OnPropertyChanged();
        }
    } = 255;

    public bool Greedy_Show
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

    private void RunGreedySolver()
    {
        var greedy = new Greedy(_network);
        var greedyRoute = greedy.Solve();
        Greedy_Distance = greedyRoute.Sum(x => x.Cost);
    }

    #endregion

    public int InitialTemperature
    {
        get;

        set
        {
            if (value.Equals(field))
            {
                return;
            }

            field = value;
            OnPropertyChanged();
        }
    } = 100;

    public int RawDecaySpeed
    {
        get;

        set
        {
            if (value.Equals(field))
            {
                return;
            }

            field = value;
            OnPropertyChanged();
        }
    } = 0;

    public double DecaySpeed
    {
        get;

        set
        {
            if (value.Equals(field))
            {
                return;
            }

            field = value;
            OnPropertyChanged();
        }
    } = 0.99693;

    #endregion

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(RawDecaySpeed))
        {
            DecaySpeed = (98470 + RawDecaySpeed) / 100000d;
        }

        if (e.PropertyName == nameof(StartNode))
        {
            HighlightStartNode();
        }

        if (e.PropertyName == nameof(EndNode))
        {
            HighlightEndNode();
        }
    }
}
