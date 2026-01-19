namespace VisualTSP.Presentation;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Input;
using Microsoft.UI;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;

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
    var shape = (Shape) sender;
    _oldColour = shape.Fill;
    _oldStroke = shape.Stroke;
    shape.Fill = shape.Stroke = new SolidColorBrush(Colors.Chartreuse);

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
    var draggedCircle = (Ellipse) sender;
    Canvas.SetZIndex(draggedCircle, _currZindex++);
  }

  private void Shape_OnMouseMove(object sender, PointerRoutedEventArgs e)
  {
    if (!_drag)
    {
      return;
    }

    // if dragging, then adjust circle position based on mouse movement
    var draggedCircle = (Ellipse) sender;
    var left = Canvas.GetLeft(draggedCircle);
    var top = Canvas.GetTop(draggedCircle);
    var newPoint = e.GetCurrentPoint(Canvas);
    Canvas.SetLeft(draggedCircle, left + (newPoint.RawPosition.X - _startPoint.RawPosition.X));
    Canvas.SetTop(draggedCircle, top + (newPoint.RawPosition.Y - _startPoint.RawPosition.Y));

    UpdateLine(draggedCircle);

    // save where we end up
    _startPoint = newPoint;
  }

  private void UpdateLine(Ellipse draggedCircle)
  {
    if (draggedCircle == Node1)
    {
      Link12.X1 = Canvas.GetLeft(Node1) + Node1.Width / 2;
      Link12.Y1 = Canvas.GetTop(Node1) + Node1.Height / 2;
    }

    if (draggedCircle == Node2)
    {
      Link12.X2 = Canvas.GetLeft(Node2) + Node2.Width / 2;
      Link12.Y2 = Canvas.GetTop(Node2) + Node2.Height / 2;
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

    var shape = (Shape) sender;
    shape.Fill = _oldColour;
    shape.Stroke = _oldStroke;

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
    var circle = new Ellipse()
    {
      Height = 50,
      Width = 50,
      Fill = new SolidColorBrush(Colors.Fuchsia)
    };
    circle.PointerEntered += Shape_OnMouseEntered;
    circle.PointerPressed += Shape_OnMouseDown;
    circle.PointerMoved += Shape_OnMouseMove;
    circle.PointerReleased += Shape_OnMouseUp;
    circle.PointerExited += Shape_OnMouseExited;
    circle.ContextFlyout = EditNodeMenu;

    Canvas.SetLeft(circle, _currPoint.Position.X);
    Canvas.SetTop(circle, _currPoint.Position.Y);

    Canvas.Add(circle);
  }

  private Shape _startNode;
  private void StartNode(object sender, RoutedEventArgs e)
  {
    _startNode = (Shape) EditNodeMenu.Target;
  }

  private void AddLink(object sender, RoutedEventArgs e)
  {
    throw new System.NotImplementedException();
  }

  private void EditNode(object sender, RoutedEventArgs e)
  {
    throw new System.NotImplementedException();
  }

  private void DeleteNode(object sender, RoutedEventArgs e)
  {
    throw new System.NotImplementedException();
  }

  private void EditLink(object sender, RoutedEventArgs e)
  {
    throw new System.NotImplementedException();
  }

  private void DeleteLink(object sender, RoutedEventArgs e)
  {
    throw new System.NotImplementedException();
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

  private void EditNodeMenu_OnOpening(object sender, object e)
  {
    IsStart = EditNodeMenu.Target == _startNode;
  }
}
