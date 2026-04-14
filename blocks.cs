using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Data;
//using AnchorStyles = System.Windows.Forms.AnchorStyles;

namespace studentNotes;

/*
public class tools{
    public static Color CFromARGBint(int a, int r, int g, int b)
    {
        return Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
    }
}*/


// SOURCE:  https://stackoverflow.com/a/2666702
//          https://pwlodek.blogspot.com/2010/12/matrixanimation-for-wpf.html

public class LinearMatrixAnimation : AnimationTimeline
    {

        public Matrix? From
        {
            set { SetValue(FromProperty, value);}
            get { return (Matrix)GetValue(FromProperty); }
        }
        public static DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(Matrix?), typeof(LinearMatrixAnimation), new PropertyMetadata(null));

        public Matrix? To
        {
            set { SetValue(ToProperty, value); }
            get { return (Matrix)GetValue(ToProperty); }
        }
        public static DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(Matrix?), typeof(LinearMatrixAnimation), new PropertyMetadata(null));

        public LinearMatrixAnimation()
        {            
        }

        public LinearMatrixAnimation(Matrix from, Matrix to, Duration duration)
        {
            Duration = duration;
            From = from;
            To = to;
            var func = new DoubleAnimation();
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            if (animationClock.CurrentProgress == null)
            {
                return null;
            }

            double progress = animationClock.CurrentProgress.Value;
            Matrix from = From ?? (Matrix)defaultOriginValue;
            if (EasingFunction != null)
            {
                progress = EasingFunction.Ease(progress);
            }

            if (To.HasValue)
            {
                Matrix to = To.Value;
                Matrix newMatrix = new Matrix(((to.M11 - from.M11) * progress)+from.M11, 0, 0, ((to.M22 - from.M22) * progress)+from.M22,
                                              ((to.OffsetX - from.OffsetX) * progress) + from.OffsetX, ((to.OffsetY - from.OffsetY) * progress)+ from.OffsetY);
                return newMatrix;
            }

            return Matrix.Identity;
        }

        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new LinearMatrixAnimation();
        }

        public override System.Type  TargetPropertyType
        {
            get { return typeof(Matrix); }
        }

        public IEasingFunction EasingFunction
    {
        get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
        set { SetValue(EasingFunctionProperty, value); }
    }

    public static readonly DependencyProperty EasingFunctionProperty =
        DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(LinearMatrixAnimation),
            new UIPropertyMetadata(null));
    }

/*

Not worth the time

public class MatrixTrans : DependencyObject, IAnimatable
{
    public Matrix matrix;

    public MatrixTrans()
    {
        this.matrix = new Matrix();
        this.AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(MatrixTrans));
    }
    public MatrixTrans(double m11, double m12, double m21, double m22, double offsetx, double offsety)
    {
        this.matrix = new Matrix(m11, m12, m21, m22, offsetx, offsety);
        this.AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(MatrixTrans));
    }
    public MatrixTrans(double Angle, double offsetx, double offsety)
    {
        this.Angle = Angle;
        this.offsetx = offsetx;
        this.offsety = offsety;
        this.AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(MatrixTrans));
    }

    public double offsetx
    {
        get
        {
            return this.matrix.OffsetX == offsetx ? offsetx : this.matrix.OffsetX;
        }
        set
        {
            this.offsetx = value;
            UpdateMatrix();
        }
    }
    public double offsety
    {
        get
        {
            return this.matrix.OffsetY == offsety ? offsety : this.matrix.OffsetY;
        }
        set
        {
            this.offsety = value;
            UpdateMatrix();
        }
    }
    public double Angle
    {
        get
        {
            return Math.Asin(this.matrix.M21) * (180/Math.PI) == Angle ? Angle : Math.Asin(this.matrix.M21) * (180/Math.PI);
        }
        set
        {
            Angle = value;
            UpdateMatrix();
        }
    }
    public DependencyProperty AngleProperty;


    public void UpdateMatrix()
    {
        this.matrix.M11 = Math.Cos(this.Angle);
        this.matrix.M12 = -Math.Sin(this.Angle);
        this.matrix.M21 = Math.Sin(this.Angle);
        this.matrix.M22 = Math.Cos(this.Angle);
        this.matrix.OffsetX = this.offsetx;
        this.matrix.OffsetY = this.offsety;
    }

    public MatrixTransform GetTransform()
    {
        return new MatrixTransform(this.matrix);
    }

    public void BeginAnimation(DependencyProperty dp, AnimationTimeline at)
    {
        
    }

    public void BeginAnimation(DependencyProperty dp, AnimationTimeline at, HandoffBehavior hb)
    {
        
    }

    public void ApplyAnimationClock(DependencyProperty dp, AnimationClock ac)
    {
        
    }

    public void ApplyAnimationClock(DependencyProperty db, AnimationClock ac, HandoffBehavior hb)
    {
        
    }

    public object GetAnimationBaseValue(DependencyProperty db)
    {
        switch (db.Name)
        {
            case "Angle" :{
                    return this.Angle;
                }
            default :{
                return 0.0;
                }
        }
    }

    public bool HasAnimatedProperties()
    {
        return true;
    }
}*/

public abstract class Element
{
    protected virtual UIElement element {get; set;}
    public Grid box;
    protected double rotation = 0.0;
    protected Point position;
    protected Size size;
    public UIElement Inherit
    {
        get
        {
            return this.box;
        }
    }
    protected bool debug = false;

    public void ElementSetup(Size size)
    {
        SetBox(size);
        this.box.Children.Add(this.element);
    }

    private void SetBox(Size size)
    {
        Grid.SetColumn(this.element, 0);
        Grid.SetRow(this.element, 0);
        this.box = new Grid();
        this.box.ColumnDefinitions.Add(new ColumnDefinition());
        this.box.RowDefinitions.Add(new RowDefinition());
        this.box.Width = size.Width;
        this.box.Height = size.Height;
        this.box.Background = new SolidColorBrush(Colors.Transparent);
        this.box.RenderTransform = new MatrixTransform(1, 0, 0, 1, this.position.X, this.position.Y);
    }

    public void SetGridVerAlign(VerticalAlignment valign)
    {
        this.box.VerticalAlignment = valign;
    }
    public void SetGridHorAlign(HorizontalAlignment halign)
    {
        this.box.HorizontalAlignment = halign;
    }
    public void SetGridAlign(HorizontalAlignment halign, VerticalAlignment valign)
    {
        this.box.HorizontalAlignment = halign;
        this.box.VerticalAlignment = valign;
    }

    public void SetGridRow(int row)
    {
        Grid.SetRow(this.box, row);
    }
    public void SetGridColumn(int col)
    {
        Grid.SetColumn(this.box, col);
    }

    public void SetGridPos(int col, int row)
    {
        Grid.SetColumn(this.box, col);
        Grid.SetRow(this.box, row);
    }

    public void SetGridRowSpan(int span)
    {
        Grid.SetRowSpan(this.box, span);
    }
    public void SetGridColumnSpan(int span)
    {
        Grid.SetColumnSpan(this.box, span);
    }
    public void SetGridSpan(int col, int row)
    {
        Grid.SetColumnSpan(this.box, col);
        Grid.SetRowSpan(this.box, row);
    }

    public double GetAngle()
    {
        return this.rotation;
    }

    public void SetAngle(double ang)
    {
        this.rotation = ang;
        RotateTransform transform = new RotateTransform(this.rotation);
        this.element.RenderTransform = transform;
    }
    public void SetOffset(int x, int y)
    {
        this.position.X = x;
        this.position.Y = y;
        this.box.RenderTransform = new MatrixTransform(1, 0, 0, 1, this.position.X, this.position.Y);
    }

    public void SetRotationAnchor(double x, double y)
    {
        this.element.RenderTransformOrigin = new Point(x, y);
    }

    public void AnimRotation(object sender, RoutedEventArgs e, DoubleAnimation da)
    {
        this.element.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, da);
    }

    public void AnimTransform(object sender, RoutedEventArgs e, LinearMatrixAnimation da)
    {
        this.box.RenderTransform.BeginAnimation(MatrixTransform.MatrixProperty, da);
    }

    public void DebugBackgroundSwitch()
    {
        this.debug = !this.debug;
        if (this.debug)
        {
            this.box.Background = new SolidColorBrush(Color.FromArgb((byte)0x88,(byte)0xFF,(byte)0x00,(byte)0xFF));
        }
        else
        {
            this.box.Background = new SolidColorBrush(Colors.Transparent);
        }
    }
}

public class Text : Element
{
    TextBlock el;
    protected double rotation = 0.0;
    protected Point position;
    protected Size size;
    protected override UIElement element {get{return el;}set{if(value is TextBlock){el = (TextBlock)value;}else{throw new Exception();}}}
    public Visibility Visibility
    {
        get
        {
            return this.el.Visibility;
        }
        set
        {
            this.el.Visibility = value;
        }
    }

    public Text(string text = "", bool isbold = false, int fsize = 10, string ffamily = "Liberation Sans")
    {
        this.el = new TextBlock();
        this.position = new Point(0,0);

        this.SetRotationAnchor(0.5, 0.5);
        this.el.Text = text;
        this.el.FontSize = fsize;
        this.el.Background = new SolidColorBrush(Colors.Transparent);
        //this.el.BorderBrush = new SolidColorBrush(Colors.Transparent);
        this.el.FontWeight = isbold ? FontWeights.Bold : FontWeights.Normal;
        this.el.FontFamily = new FontFamily(ffamily);

        this.size = MeasureString(text);

        this.ElementSetup(this.size);

        //this.OnLoad(TestAnim);
    }

    public void OnLoad(RoutedEventHandler foo)
    {
        this.el.Loaded += foo;
    }

    public string GetContent()
    {
        return this.el.Text;
    }

    public void SetContent(string text)
    {
        this.size = MeasureString(text);
        this.el.Text = text;
        this.ElementResize();
    }

    public void ElementResize()
    {
        this.box = new Grid();
        this.box.ColumnDefinitions.Add(new ColumnDefinition());
        this.box.RowDefinitions.Add(new RowDefinition());
        this.box.Width = size.Width;
        this.box.Height = size.Height;
        this.box.Background = new SolidColorBrush(Colors.Transparent);
        this.box.RenderTransform = new MatrixTransform(1, 0, 0, 1, this.position.X, this.position.Y);
    }
    
    public void SetAlignment(TextAlignment alignment)
    {
        this.el.TextAlignment = alignment;
    }

    public void SetBackground(Brush brush)
    {
        this.el.Background = brush;
    }

    public void SetForeground(Brush brush)
    {
        this.el.Foreground = brush;
    }

    private Size MeasureString(string candidate)
{
    var formattedText = new FormattedText(
        candidate,
        CultureInfo.GetCultureInfo("en-us"),
        FlowDirection.LeftToRight,
        new Typeface(this.el.FontFamily, this.el.FontStyle, this.el.FontWeight, this.el.FontStretch),
        this.el.FontSize,
        Brushes.Black,
        new NumberSubstitution(),
        VisualTreeHelper.GetDpi(this.el).PixelsPerDip);

    return new Size(formattedText.Width, formattedText.Height);
}

    public void TestAnim(object sender, RoutedEventArgs e)
    {
        DoubleAnimation anim1 = new DoubleAnimation(360, 0, new Duration(TimeSpan.FromSeconds(1)));
        anim1.RepeatBehavior = RepeatBehavior.Forever;
        LinearMatrixAnimation anim2 = new LinearMatrixAnimation(new Matrix(1,1,1,1,0,0), new Matrix(1,1,1,1,0,0), new Duration(TimeSpan.FromSeconds(1)));
        anim2.RepeatBehavior = RepeatBehavior.Forever;

        this.AnimRotation(sender, e, anim1);
        this.AnimTransform(sender, e, anim2);
    }

    public void AnimColor(object sender, RoutedEventArgs e, ColorAnimation da)
    {
        this.el.Background.BeginAnimation(SolidColorBrush.ColorProperty, da);
    }
}

public class TextB : Element
{
    TextBox el;
    protected override UIElement element {get{return el;}set{if(value is TextBox){el = (TextBox)value;}else{throw new Exception();}}}
    public Visibility Visibility
    {
        get
        {
            return this.el.Visibility;
        }
        set
        {
            this.el.Visibility = value;
        }
    }

    public TextB(int width, int height, string text = "", bool isbold = false, int fsize = 10, bool doWrap = false, string ffamily = "Liberation Sans")
    {
        this.el = new TextBox();
        this.position = new Point(0,0);

        this.SetRotationAnchor(0.5, 0.5);
        this.el.Text = text;
        this.el.FontSize = fsize;
        this.el.Background = new SolidColorBrush(Colors.Transparent);
        //this.el.BorderBrush = new SolidColorBrush(Colors.Transparent);
        this.el.FontWeight = isbold ? FontWeights.Bold : FontWeights.Normal;
        this.el.FontFamily = new FontFamily(ffamily);
        this.el.BorderThickness = new Thickness(0);
        if (doWrap)
        {
            this.el.TextWrapping = TextWrapping.Wrap;
            this.el.AcceptsReturn = true;
            this.el.AcceptsTab = true;
        }

        this.size = new Size(width, height == 0 ? MeasureString("Test.txt").Height : height);

        this.ElementSetup(this.size);

    }

    public string GetContent()
    {
        return this.el.Text;
    }

    public void SetContent(string text)
    {
        this.el.Text = text;
    }

    public void OnLoad(RoutedEventHandler foo)
    {
        this.el.Loaded += foo;
    }
    
    public void SetAlignment(TextAlignment alignment)
    {
        this.el.TextAlignment = alignment;
    }

    public void SetBackground(Brush brush)
    {
        this.el.Background = brush;
    }

    public void SetForeground(Brush brush)
    {
        this.el.Foreground = brush;
    }
    private Size MeasureString(string candidate)
    {
        var formattedText = new FormattedText(
            candidate,
            CultureInfo.GetCultureInfo("en-us"),
            FlowDirection.LeftToRight,
            new Typeface(this.el.FontFamily, this.el.FontStyle, this.el.FontWeight, this.el.FontStretch),
            this.el.FontSize,
            Brushes.Black,
            new NumberSubstitution(),
            VisualTreeHelper.GetDpi(this.el).PixelsPerDip);

        return new Size(formattedText.Width, formattedText.Height);
    }
}

public class DropdownList : Element
{
    ComboBox el;
    protected override UIElement element {get{return el;}set{if(value is ComboBox){el = (ComboBox)value;}else{throw new Exception();}}}
    public Visibility Visibility
    {
        get
        {
            return this.el.Visibility;
        }
        set
        {
            this.el.Visibility = value;
        }
    }

    public DropdownList(int width, int height, Brush brush, int itemHeight, string[] items, RoutedEventHandler[] item_actions)
    {
        this.el = new ComboBox();
        this.size = new Size(width, height);
        this.position = new Point(0,0);
        for(int i = 0; i < items.Length; i++)
        {
            var temp = new ComboBoxItem();
            temp.Content = items[i];
            temp.Selected += item_actions[i];
            this.el.Items.Add(temp);
        }

        this.ElementSetup(this.size);

        this.SetRotationAnchor(0.5, 0.5);

        this.el.Width = this.size.Width;
        this.el.Background = brush;
        //this.el.ShowGridLines = true;
        //this.el.BorderBrush = new SolidColorBrush(Colors.Transparent);
        //this.OnLoad(TestAnim);
    }
    
    public void SetBackground(Brush brush)
    {
        this.el.Background = brush;
    }

    public void SetForeground(Brush brush)
    {
        this.el.Foreground = brush;
    }

}

public class ListEl : Element
{
    Grid el;
    ScrollViewer viewer;
    protected override UIElement element {get{return el;}set{if(value is Grid){el = (Grid)value;}else{throw new Exception();}}}
    public Visibility Visibility
    {
        get
        {
            return this.el.Visibility;
        }
        set
        {
            this.el.Visibility = value;
        }
    }

    public UIElement Inherit
    {
        get
        {
            return this.viewer;
        }
    }

    protected int list_len = 0;
    protected int ItemHeight = 40;

    public ListEl(int width, int height, Brush brush, int itemHeight)
    {
        this.el = new Grid();
        this.viewer = new ScrollViewer();
        this.size = new Size(width, height);
        this.position = new Point(0,0);
        this.ItemHeight = itemHeight;

        this.ElementSetup(this.size);

        this.SetRotationAnchor(0.5, 0.5);

        this.el.Width = this.size.Width;
        this.viewer.Height = this.size.Height;
        this.viewer.Width = this.size.Width;
        this.viewer.Content = this.box;
        this.viewer.Background = brush;
        this.el.Background = brush;
        //this.el.ShowGridLines = true;
        //this.el.BorderBrush = new SolidColorBrush(Colors.Transparent);
        //this.OnLoad(TestAnim);
    }

    public void AddItem(Element el)
    {
        el.SetGridPos(0, this.list_len);
        this.el.RowDefinitions.Add(new RowDefinition());
        this.el.RowDefinitions[list_len].MaxHeight = this.ItemHeight;
        this.list_len += 1;
        this.el.Children.Add(el.Inherit);
        this.el.UpdateLayout();
    }

    public int Len()
    {
        return this.el.RowDefinitions.ToArray().Length;
    }

    public void SetAlignment(HorizontalAlignment halign, VerticalAlignment valign)
    {
        this.box.HorizontalAlignment = halign;
        this.box.VerticalAlignment = valign;
    }

    public void OnLoad(RoutedEventHandler foo)
    {
        this.el.Loaded += foo;
    }

    public void SetGridRow(int row)
    {
        Grid.SetRow(this.viewer, row);
    }
    public void SetGridColumn(int col)
    {
        Grid.SetColumn(this.viewer, col);
    }

    public void SetGridPos(int col, int row)
    {
        Grid.SetColumn(this.viewer, col);
        Grid.SetRow(this.viewer, row);
    }

    public void SetGridRowSpan(int span)
    {
        Grid.SetRowSpan(this.viewer, span);
    }
    public void SetGridColumnSpan(int span)
    {
        Grid.SetColumnSpan(this.viewer, span);
    }
    public void SetGridSpan(int col, int row)
    {
        Grid.SetColumnSpan(this.viewer, col);
        Grid.SetRowSpan(this.viewer, row);
    }

    public void SetBackground(Brush brush)
    {
        this.el.Background = brush;
    }

    public void AnimColor(object sender, RoutedEventArgs e, ColorAnimation da)
    {
        this.el.Background.BeginAnimation(SolidColorBrush.ColorProperty, da);
    }
}

public class DrawRect : Element
{
    Rectangle el;
    protected override UIElement element {get{return el;}set{if(value is Rectangle){el = (Rectangle)value;}else{throw new Exception();}}}
    public Visibility Visibility
    {
        get
        {
            return this.el.Visibility;
        }
        set
        {
            this.el.Visibility = value;
        }
    }

    public DrawRect(int width, int height, Color color)
    {
        this.el = new Rectangle();
        this.size = new Size(width, height);
        this.position = new Point(0,0);

        this.ElementSetup(this.size);

        this.SetRotationAnchor(0.5, 0.5);

        this.el.Width = width;
        this.el.Height = height;
        this.el.Fill = new SolidColorBrush(color);
    }

    public void Rotate(double ang)
    {
        this.SetAngle((this.rotation + ang) % 360.0);
        this.el.UpdateLayout();
    }

    public void OnLoad(RoutedEventHandler foo)
    {
        this.el.Loaded += foo;
    }

    public void TestAnim(object sender, RoutedEventArgs e)
    {
        var anim1 = new DoubleAnimation(360, 0, new Duration(TimeSpan.FromSeconds(1)));
        anim1.RepeatBehavior = RepeatBehavior.Forever;

        this.AnimRotation(sender, e, anim1);
    }

    public void AnimColor(object sender, RoutedEventArgs e, ColorAnimation da)
    {
        this.el.Fill.BeginAnimation(SolidColorBrush.ColorProperty, da);
    }
}

public class nButton : Element
{
    Button el;
    protected override UIElement element {get{return el;}set{if(value is Button){el = (Button)value;}else{throw new Exception();}}}
    public Visibility Visibility
    {
        get
        {
            return this.el.Visibility;
        }
        set
        {
            this.el.Visibility = value;
        }
    }
    public string metadata = "";

    public nButton(int width, int height, Brush brush, RoutedEventHandler onclick, string text = "")
    {
        this.el = new Button();
        this.size = new Size(width, height);
        this.position = new Point(0,0);

        this.ElementSetup(this.size);

        SetRotationAnchor(0.5, 0.5);

        this.el.Width = width;
        this.el.Height = height;
        this.el.Background = brush;
        this.el.Content = new TextBlock();

        //this.el.MouseEnter += (object sender, MouseEventArgs e)=>{Console.WriteLine("Mouse Over Button!");};
        //this.el.MouseLeave += (object sender, MouseEventArgs e)=>{Console.WriteLine($"Mouse Left Button!");};

        this.el.Content = text;

        this.el.FontFamily = new FontFamily("Liberation Sans");
        this.el.FontSize = 20;
        this.el.FontWeight = FontWeights.Bold;
        this.el.Foreground = new SolidColorBrush(Colors.White);

        this.el.BorderBrush = brush;
        this.el.Focusable = false;
        this.el.BorderThickness = new Thickness(0);
        this.el.Padding = new Thickness(0);
        this.el.Margin = new Thickness(0);

        this.el.Click += onclick;
    }

    public void SetMetadata(string data)
    {
        this.el.DataContext = data;
    }

    public string GetMetadata()
    {
        return (string)this.el.DataContext;
    }

    public void OnLoad(RoutedEventHandler foo)
    {
        this.el.Loaded += foo;
    }

    public void SetAlignment(HorizontalAlignment halign, VerticalAlignment valign)
    {
        this.box.HorizontalAlignment = halign;
        this.box.VerticalAlignment = valign;
    }

    public void AnimColor(object sender, RoutedEventArgs e, ColorAnimation da)
    {
        this.el.Background.BeginAnimation(SolidColorBrush.ColorProperty, da);
    }

    public void SetBackground(Brush brush)
    {
        this.el.Background = brush;
    }

    public void SetForeground(Brush brush)
    {
        this.el.Foreground = brush;
    }

    public void SetBorder(Brush brush, int thickness)
    {
        this.el.BorderBrush = brush;
        this.el.BorderThickness = new Thickness(thickness);
    }
}

public class FileListItem : Element
{
    Grid el;
    protected override UIElement element {get{return el;}set{if(value is Button){el = (Grid)value;}else{throw new Exception();}}}
    public Visibility Visibility
    {
        get
        {
            return this.el.Visibility;
        }
        set
        {
            this.el.Visibility = value;
        }
    }
    protected Text filename;
    protected nButton load;
    protected nButton delete;

    public FileListItem(int width, int height, Brush brush, RoutedEventHandler onloadclick, RoutedEventHandler ondelclick, Brush borderbrush, Brush buttonbrush, Brush buttontextbrush, string filename = "")
    {
        this.el = new Grid();
        this.size = new Size(width, height);
        this.position = new Point(0,0);

        this.ElementSetup(this.size);

        SetRotationAnchor(0.5, 0.5);

        this.el.Width = width;
        this.el.Height = height;
        this.el.Background = brush;

        this.el.ColumnDefinitions.Add(new ColumnDefinition());
        this.el.ColumnDefinitions[0].MaxWidth = width * 0.6;
        this.el.ColumnDefinitions.Add(new ColumnDefinition());
        this.el.ColumnDefinitions[1].MaxWidth = width * 0.2;
        this.el.ColumnDefinitions.Add(new ColumnDefinition());
        this.el.ColumnDefinitions[2].MaxWidth = width * 0.2;

        this.filename = new Text(filename, false, 25);
        this.filename.SetForeground(new SolidColorBrush(Colors.White));
        this.filename.SetGridPos(0,0);
        this.filename.SetForeground(buttontextbrush);
        this.load = new nButton((int)(width * 0.2), height-2, brush, onloadclick, "load");
        this.load.SetGridPos(1,0);
        this.load.SetBorder(borderbrush, 2);
        this.load.SetBackground(buttonbrush);
        this.load.SetForeground(buttontextbrush);
        this.load.SetMetadata(filename);
        this.delete = new nButton((int)(width * 0.2), height-2, brush, ondelclick, "delete");
        this.delete.SetGridPos(2,0);
        this.delete.SetBorder(borderbrush, 2);
        this.delete.SetBackground(buttonbrush);
        this.delete.SetForeground(buttontextbrush);
        this.delete.SetMetadata(filename);

        this.el.Children.Add(this.filename.Inherit);
        this.el.Children.Add(this.load.Inherit);
        this.el.Children.Add(this.delete.Inherit);
    }

    public string GetLoadMetadata()
    {
        return this.load.GetMetadata();
    }

    public string GetDeleteMetadata()
    {
        return this.delete.GetMetadata();
    }

    public void OnLoad(RoutedEventHandler foo)
    {
        this.el.Loaded += foo;
    }

    public void SetAlignment(HorizontalAlignment halign, VerticalAlignment valign)
    {
        this.box.HorizontalAlignment = halign;
        this.box.VerticalAlignment = valign;
    }

    public void AnimColor(object sender, RoutedEventArgs e, ColorAnimation da)
    {
        this.el.Background.BeginAnimation(SolidColorBrush.ColorProperty, da);
    }

    public void SetBackground(Brush brush)
    {
        this.el.Background = brush;
    }

    public void SetForeground(Brush brush)
    {
        this.filename.SetForeground(brush);
        this.load.SetForeground(brush);
        this.delete.SetForeground(brush);
    }
}

public struct SizeBounds
{
    public int minWidth;
    public int maxWidth;
    public int minHeight;
    public int maxHeight;

    public SizeBounds(int minw, int maxw, int minh, int maxh)
    {
        this.minWidth = minw;
        this.maxWidth = maxw;
        this.minHeight = minh;
        this.maxHeight = maxh;
    }
}

public class GridEl
{
    private Grid grid;
    private double rotation = 0.0;
    private Point position;
    private Size size;
    private bool debug = false;
    private MatrixTransform matrix;
    public Visibility Visibility
    {
        get
        {
            return this.grid.Visibility;
        }
        set
        {
            this.grid.Visibility = value;
        }
    }

    public GridEl(int width, int height, int rows, int cols, Brush background = null, HorizontalAlignment halign = HorizontalAlignment.Center, VerticalAlignment valign = VerticalAlignment.Center)
    {
        this.grid = new Grid();
        this.grid.Background = background;
        this.position = new Point(0,0);
        this.matrix = new MatrixTransform(1, 0, 0, 1, this.position.X, this.position.Y);
        this.grid.Width = width;
        this.grid.Height = height;
        this.grid.HorizontalAlignment = halign;
        this.grid.VerticalAlignment = valign;
        this.grid.ClipToBounds = false;
        for(int i = 0; i < cols; i++)
        {
            this.grid.ColumnDefinitions.Add(new ColumnDefinition());
        }
        for(int i = 0; i < rows; i++)
        {
            this.grid.RowDefinitions.Add(new RowDefinition());
        }
    }

    public GridEl(SizeBounds bounds, int rows, int cols, Brush background = null, HorizontalAlignment halign = HorizontalAlignment.Center, VerticalAlignment valign = VerticalAlignment.Center)
    {
        this.grid = new Grid();
        this.grid.Background = background;

        this.position = new Point(0,0);
        this.matrix = new MatrixTransform(1, 0, 0, 1, this.position.X, this.position.Y);
        this.grid.Width = this.size.Width;
        this.grid.Height = this.size.Height;

        this.grid.MinWidth = bounds.minWidth;
        this.grid.MaxWidth = bounds.maxWidth;
        this.grid.MinHeight = bounds.minHeight;
        this.grid.MaxHeight = bounds.maxHeight;

        this.grid.HorizontalAlignment = halign;
        this.grid.VerticalAlignment = valign;
        this.grid.ClipToBounds = false;
        for(int i = 0; i < cols; i++)
        {
            this.grid.ColumnDefinitions.Add(new ColumnDefinition());
        }
        for(int i = 0; i < rows; i++)
        {
            this.grid.RowDefinitions.Add(new RowDefinition());
        }
    }

    public void SetZIndex(int index)
    {
        Panel.SetZIndex(this.grid, index);
    }

    public void SetColumnMaxWidth(int index, int width)
    {
        this.grid.ColumnDefinitions[index].MaxWidth = width;
    }
    public void SetRowMaxHeight(int index, int height)
    {
        this.grid.RowDefinitions[index].MaxHeight = height;
    }

    public Grid GetGrid()
    {
        return this.grid;
    }

    public void AddChild(UIElement el)
    {
        this.grid.Children.Add(el);
    }

    public void AddChild(UIElement[] el)
    {
        for(int i = 0; i < el.Length; i++){
            this.grid.Children.Add(el[i]);
        }
    }
    public void AddChild(Panel el)
    {
        this.grid.Children.Add(el);
    }

    public void AddChild(Panel[] el)
    {
        for(int i = 0; i < el.Length; i++){
            this.grid.Children.Add(el[i]);
        }
    }
    public void ResetChildren()
    {
        this.grid.Children.Clear();
    }

    public void SetGridRow(int row)
    {
        Grid.SetRow(this.grid, row);
    }
    public void SetGridColumn(int col)
    {
        Grid.SetColumn(this.grid, col);
    }

    public void SetGridPos(int col, int row)
    {
        Grid.SetColumn(this.grid, col);
        Grid.SetRow(this.grid, row);
    }

    public void SetGridRowSpan(int span)
    {
        Grid.SetRowSpan(this.grid, span);
    }
    public void SetGridColumnSpan(int span)
    {
        Grid.SetColumnSpan(this.grid, span);
    }
    public void SetGridSpan(int col, int row)
    {
        Grid.SetColumnSpan(this.grid, col);
        Grid.SetRowSpan(this.grid, row);
    }

    public double GetAngle()
    {
        return this.rotation;
    }

    public void SetAngle(double ang)
    {
        this.rotation = ang;
        RotateTransform transform = new RotateTransform(this.rotation);
        this.grid.RenderTransform = transform;
    }
    public void SetOffset(int x, int y)
    {
        this.position.X = x;
        this.position.Y = y;
        this.matrix = new MatrixTransform(1, 0, 0, 1, this.position.X, this.position.Y);
        this.grid.RenderTransform = this.matrix;
    }

    public void SetRotationAnchor(double x, double y)
    {
        this.grid.RenderTransformOrigin = new Point(x, y);
    }

    public void OnLoad(RoutedEventHandler foo)
    {
        this.grid.Loaded += foo;
    }

    public void AnimRotation(object sender, RoutedEventArgs e, DoubleAnimation da)
    {
        this.matrix.BeginAnimation(RotateTransform.AngleProperty, da);
    }

    public void AnimTransform(object sender, RoutedEventArgs e, LinearMatrixAnimation da)
    {   
        MatrixTransform mat = new MatrixTransform(this.matrix.Value);
        this.grid.RenderTransform = mat;
        mat.BeginAnimation(MatrixTransform.MatrixProperty, da);
    }

    public void DebugBackgroundSwitch()
    {
        this.debug = !this.debug;
        if (this.debug)
        {
            this.grid.Background = new SolidColorBrush(Color.FromArgb((byte)0x88,(byte)0xFF,(byte)0x00,(byte)0xFF));
        }
        else
        {
            this.grid.Background = new SolidColorBrush(Colors.Transparent);
        }
    }

    public void AnimColor(object sender, RoutedEventArgs e, ColorAnimation da)
    {
        this.grid.Background.BeginAnimation(SolidColorBrush.ColorProperty, da);
    }

    public void ShowGrid()
    {
        this.grid.ShowGridLines = true;
    }
}