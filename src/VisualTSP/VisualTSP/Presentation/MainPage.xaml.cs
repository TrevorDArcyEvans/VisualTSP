using Windows.UI.Core;

namespace VisualTSP.Presentation;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Input;
using Microsoft.UI;
using Microsoft.UI.Xaml.Input;

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
        MousePos.Text = $"({_currPoint.Position.X:0}, {_currPoint.Position.Y:0})@{((CompositeTransform) Canvas.RenderTransform).ScaleX}";
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
        Canvas.ContextFlyout = null;
    }

    private void Shape_OnMouseDown(object sender, PointerRoutedEventArgs e)
    {
        // start dragging
        _drag = true;

        // save start point of dragging
        _startPoint = e.GetCurrentPoint(Canvas);

        // move selected circle to the top of the Z order
        var draggedCircle = (VisualNode) sender;
        Canvas.SetZIndex(draggedCircle, _currZindex++);
    }

    private void Shape_OnMouseMove(object sender, PointerRoutedEventArgs e)
    {
        if (!_drag)
        {
            return;
        }

        // if dragging, then adjust circle position based on mouse movement
        var draggedCircle = (VisualNode) sender;
        var left = Canvas.GetLeft(draggedCircle);
        var top = Canvas.GetTop(draggedCircle);
        var newPoint = e.GetCurrentPoint(Canvas);
        Canvas.SetLeft(draggedCircle, left + (newPoint.RawPosition.X - _startPoint.RawPosition.X));
        Canvas.SetTop(draggedCircle, top + (newPoint.RawPosition.Y - _startPoint.RawPosition.Y));

        UpdateLine(draggedCircle);

        // save where we end up
        _startPoint = newPoint;
    }

    private void UpdateLine(VisualNode node)
    {
        // have to use ActualWidth/Height since composite control
        if (node == Node1)
        {
            Link12.X1 = Canvas.GetLeft(Node1) + Node1.ActualWidth / 2;
            Link12.Y1 = Canvas.GetTop(Node1) + Node1.ActualHeight / 2;
        }

        if (node == Node2)
        {
            Link12.X2 = Canvas.GetLeft(Node2) + Node2.ActualWidth / 2;
            Link12.Y2 = Canvas.GetTop(Node2) + Node2.ActualHeight / 2;
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

        var node = (IHighlightable) sender;
        node.Fill = _oldColour;
        node.Stroke = _oldStroke;

        // restore canvas CSM
        Canvas.ContextFlyout = AddNodeMenu;
    }

    #endregion

    #region Zoom

    private const double ZoomInc = 0.1;

    private void Zoom_In(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform) Canvas.RenderTransform;
        ct.ScaleX += ZoomInc;
        ct.ScaleY += ZoomInc;
    }

    private void Zoom_Fit(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform) Canvas.RenderTransform;
        ct.ScaleX = 1.0;
        ct.ScaleY = 1.0;
    }

    private void Zoom_Out(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform) Canvas.RenderTransform;
        ct.ScaleX -= ZoomInc;
        ct.ScaleY -= ZoomInc;
    }

    #endregion

    #region Translate

    private const double TranslateInc = 10d;

    private void Translate_Up(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform) Canvas.RenderTransform;
        ct.TranslateY -= TranslateInc;
    }

    private void Translate_Down(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform) Canvas.RenderTransform;
        ct.TranslateY += TranslateInc;
    }

    private void Translate_Left(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform) Canvas.RenderTransform;
        ct.TranslateX -= TranslateInc;
    }

    private void Translate_Right(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform) Canvas.RenderTransform;
        ct.TranslateX += TranslateInc;
    }

    private void Translate_Reset(object sender, RoutedEventArgs e)
    {
        var ct = (CompositeTransform) Canvas.RenderTransform;
        ct.TranslateX = 0;
        ct.TranslateY = 0;
    }

    #endregion

    private void AddNode(object sender, RoutedEventArgs e)
    {
        var node = new VisualNode();
        node.PointerEntered += Shape_OnMouseEntered;
        node.PointerPressed += Shape_OnMouseDown;
        node.PointerMoved += Shape_OnMouseMove;
        node.PointerReleased += Shape_OnMouseUp;
        node.PointerExited += Shape_OnMouseExited;
        node.ContextFlyout = EditNodeMenu;

        Canvas.SetLeft(node, _currPoint.Position.X);
        Canvas.SetTop(node, _currPoint.Position.Y);

        Canvas.Add(node);
    }

    #region StartNode

    private VisualNode _startNode;

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

    private void EditNodeMenu_OnOpening(object sender, object e)
    {
        IsStart = EditNodeMenu.Target == _startNode;
    }

    private void StartNode(object sender, RoutedEventArgs e)
    {
        _startNode = (VisualNode) EditNodeMenu.Target;
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
        Canvas.Children.Remove(node);
    }

    private void DeleteAssociatedLinks(VisualNode node)
    {
        if (node == Node1 || node == Node2)
        {
            Canvas.Children.Remove(Link12);
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
        Canvas.Children.Remove(link);
    }
}
