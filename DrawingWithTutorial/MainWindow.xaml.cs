/* Ilja Gusevs
   Kristina Yasenovich */

using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DrawingWithTutorial
{
    public partial class MainWindow : Window
    {
        public string strokeColor = "black";
        public string fillColor = "transparent";
        public string filePath;

        public int strokeSize = 2;
        public int brushSwitch = 2;
        public bool isHighlighted = false;
        public bool isEllipse = false;
        public bool isRectangle = false;

        Point currentPoint = new Point();
        public Ellipse elip = new Ellipse();
        public Rectangle rec = new Rectangle();

        string messageBoxText = "Do you want to save changes?";
        string captionNew = "New File";
        string captionOpen = "Open File";
        MessageBoxButton button = MessageBoxButton.YesNoCancel;
        MessageBoxImage icon = MessageBoxImage.Warning;

        public MainWindow()
        {

            InitializeComponent();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl + S shortcut for saving
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                SaveFile();
            }
        }

        private void menuNew_Click(object sender, RoutedEventArgs e)
        {
            NewFile();
        }

        private void menuOpen_Click(object sender, RoutedEventArgs e)
        {

            OpenFileMessageBox();
        }

        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
        }

        private void Canvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Draws a line when ellipse or rectangle are not selected
            if (!isEllipse && !isRectangle)
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                    currentPoint = e.GetPosition(canvas);
            }
            // Draws an elipse or rectangle when selected with selected strokeColor 
            else
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                    currentPoint = e.GetPosition(canvas);

                var converter = new System.Windows.Media.BrushConverter();
                var brush = (Brush)converter.ConvertFromString(strokeColor);

                elip = new Ellipse
                {

                    Stroke = brush,
                    StrokeThickness = strokeSize
                };

                rec = new Rectangle
                {
                    Stroke = brush,
                    StrokeThickness = strokeSize
                };

                canvas.Children.Add(elip);
                canvas.Children.Add(rec);
            }
        }

        void pencil_Click(object sender, RoutedEventArgs e)
        {
            HighlightMenu(sender);
            TogglePen();
        }

        void fill_Click(object sender, RoutedEventArgs e)
        {
            fillCanvas();
        }

        void Clear()
        {
            // Clears canvas with all drawn lines and background
            canvas.Children.Clear();
            canvas.Background = new SolidColorBrush(Colors.Transparent);
        }

        void clear_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }


        private void brushSize_Click(object sender, RoutedEventArgs e)
        {
            switch (brushSwitch)
            {
                // Cases with brush sizes. For each stroke size changes brush size icon 
                case 1:

                    brushSize.Icon = new System.Windows.Controls.Image
                    {
                        Source = new BitmapImage(new Uri("Images/smaller.png", UriKind.Relative))
                    };
                    strokeSize = 2;
                    brushSwitch += 1;
                    break;

                case 2:

                    brushSize.Icon = new System.Windows.Controls.Image
                    {
                        Source = new BitmapImage(new Uri("Images/small.png", UriKind.Relative))
                    };
                    strokeSize = 6;
                    brushSwitch += 1;
                    break;

                case 3:

                    brushSize.Icon = new System.Windows.Controls.Image
                    {
                        Source = new BitmapImage(new Uri("Images/big.png", UriKind.Relative))
                    };
                    strokeSize = 12;
                    brushSwitch = 1;
                    break;
            }

        }

        // Cases for background color
        private void fillCanvas()
        {
            switch (fillColor)
            {
                case "white":
                    canvas.Background = new SolidColorBrush(Colors.White);
                    break;

                case "black":
                    canvas.Background = new SolidColorBrush(Colors.Black);
                    break;

                case "gray":
                    canvas.Background = new SolidColorBrush(Colors.Gray);
                    break;

                case "red":
                    canvas.Background = new SolidColorBrush(Colors.Red);
                    break;

                case "yellow":
                    canvas.Background = new SolidColorBrush(Colors.Yellow);
                    break;

                case "blue":
                    canvas.Background = new SolidColorBrush(Colors.Blue);
                    break;
                default:
                    canvas.Background = new SolidColorBrush(Colors.Transparent);
                    break;
            }
        }


        #region Colors

        // All color definitions for pencil and for background 
        private void colorBlack_Click(object sender, RoutedEventArgs e)
        {
            ColorHighlight(sender);
            strokeColor = "black";
            fillColor = "black";

        }

        private void colorGrey_Click(object sender, RoutedEventArgs e)
        {
            ColorHighlight(sender);
            strokeColor = "gray";
            fillColor = "gray";
        }

        private void colorWhite_Click(object sender, RoutedEventArgs e)
        {
            ColorHighlight(sender);
            strokeColor = "white";
            fillColor = "white";
        }

        private void colorRed_Click(object sender, RoutedEventArgs e)
        {
            ColorHighlight(sender);
            strokeColor = "red";
            fillColor = "red";
        }

        private void colorBlue_Click(object sender, RoutedEventArgs e)
        {
            ColorHighlight(sender);
            strokeColor = "blue";
            fillColor = "blue";
        }

        private void colorYellow_Click(object sender, RoutedEventArgs e)
        {
            ColorHighlight(sender);
            strokeColor = "yellow";
            fillColor = "yellow";
        }

        #endregion


        #region Shapes and text
        private void ellipse_Click(object sender, RoutedEventArgs e)
        {
            HighlightShape(sender);
            isRectangle = false;
        }

        private void square_Click(object sender, RoutedEventArgs e)
        {
            HighlightShape(sender);
            isEllipse = false;
        }

        private void text_Click(object sender, RoutedEventArgs e)
        {
            ////Adds text box to canvas with defined width and height
            //TextBox tb = new TextBox
            //{
            //    Width = 100
            //    Height = 50,
            //};
            //this.canvas.Children.Add(tb);
            //tb.Focus();

        }

        #endregion

        private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!isEllipse && !isRectangle)
            {
                // If ellipse or rectangle are not selected then draws with a line on canvas by following a mouse 
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Line line = new Line();

                    var converter = new System.Windows.Media.BrushConverter();
                    var brush = (Brush)converter.ConvertFromString(strokeColor);

                    line.Stroke = brush;
                    line.X1 = currentPoint.X;
                    line.Y1 = currentPoint.Y;
                    line.X2 = e.GetPosition(canvas).X;
                    line.Y2 = e.GetPosition(canvas).Y;
                    line.StrokeThickness = strokeSize;
                    currentPoint = e.GetPosition(canvas);

                    canvas.Children.Add(line);
                }
            }
                else if (isEllipse)
                {
                    // If ellipse is selected draws an ellipse on canvas by following a mouse 
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {

                        double minX = Math.Min(e.GetPosition(canvas).X, currentPoint.X);
                        double minY = Math.Min(e.GetPosition(canvas).Y, currentPoint.Y);
                        double maxX = Math.Max(e.GetPosition(canvas).X, currentPoint.X);
                        double maxY = Math.Max(e.GetPosition(canvas).Y, currentPoint.Y);

                        Canvas.SetTop(elip, minY);
                        Canvas.SetLeft(elip, minX);

                        double height = maxY - minY;
                        double width = maxX - minX;

                        elip.Height = Math.Abs(height);
                        elip.Width = Math.Abs(width);
                    }

                }
            else if (isRectangle)
            {
                // If rectangle is selected draws a rectangle on canvas by following a mouse
                if (e.LeftButton == MouseButtonState.Pressed)
                {

                    double minX = Math.Min(e.GetPosition(canvas).X, currentPoint.X);
                    double minY = Math.Min(e.GetPosition(canvas).Y, currentPoint.Y);
                    double maxX = Math.Max(e.GetPosition(canvas).X, currentPoint.X);
                    double maxY = Math.Max(e.GetPosition(canvas).Y, currentPoint.Y);

                    Canvas.SetTop(rec, minY);
                    Canvas.SetLeft(rec, minX);

                    double height = maxY - minY;
                    double width = maxX - minX;

                    rec.Height = Math.Abs(height);
                    rec.Width = Math.Abs(width);

                }
            }

        }

        public void TogglePen()
        {
            // Property IsHitTestVisible tells if pen toggle is on(true) or off(false)

            if (canvas.IsHitTestVisible == false)
            {
                canvas.IsHitTestVisible = true;
            }
            else
            {
                canvas.IsHitTestVisible = false;
            }
        }

        #region Message boxes, save file and open file

        public void OpenFileMessageBox()
        {
            MessageBoxResult result = MessageBox.Show(messageBoxText, captionOpen, button, icon);

            // Process message box results
            switch (result)
            {
                /* Before file will be opened program offers to save the existing file(Yes)
                or just open new file without saving(No),
                option 'cancel' closes message box */

                case MessageBoxResult.Yes:
                    SaveFile();
                    OpenFile();
                    break;
                case MessageBoxResult.No:
                    OpenFile();
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }

        public void NewFile()
        {
            MessageBoxResult result = MessageBox.Show(messageBoxText, captionNew, button, icon);

            // Process message box results
            switch (result)
            {
                /* Before canvas will be cleaned program offers to save the existing(Yes)
                or just cleans canvas without saving(No),
                option 'cancel' closes message box */

                case MessageBoxResult.Yes:
                    canvas.Children.Clear();
                    canvas.Background = new SolidColorBrush(Colors.Transparent);
                    SaveFile();
                    break;
                case MessageBoxResult.No:
                    canvas.Children.Clear();
                    canvas.Background = new SolidColorBrush(Colors.Transparent);
                    break;
                case MessageBoxResult.Cancel:
                    break;
            }
        }

        public void SaveFile()
        {
            try
            {
                // Saves canvas in JPN or PNG format
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "JPG (*.jpg)|*.jpg|PNG (*.png)|*.png";

                Nullable<bool> result = sfd.ShowDialog();
                string fileName = "";

                if (result == true)
                {
                    fileName = sfd.FileName;
                    Size size = canvas.RenderSize;
                    RenderTargetBitmap rtb = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);
                    rtb.Render(canvas);
                    PngBitmapEncoder png = new PngBitmapEncoder();
                    png.Frames.Add(BitmapFrame.Create(rtb));
                    using (Stream stm = File.Create(fileName))
                    {
                        png.Save(stm);
                    }
                }
            }
            catch { };

        }
        public void OpenFile()
        {
            // User can open file in png format and continue drawing
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "JPG (*.jpg)|*.jpg|PNG (*.png)|*.png|All Files (*.*)|*.*";
            Nullable<bool> result = open.ShowDialog();

            if (result == true)
            {
                string filename = open.FileName;
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(@filename, UriKind.Relative));
                canvas.Background = brush;
            }

            canvas.Children.Clear();
        }

        #endregion

        public void HighlightShape(object sender)
        {
            // Highlights ellipse and rectangle buttons if they are selected
            if (!isEllipse && !isRectangle)
            {
                // Clears previously set backgrounds
                ellipse.Background = null;
                ellipse.BorderBrush = null;

                rectangle.Background = null;
                rectangle.BorderBrush = null;

                MenuItem mi = sender as MenuItem;
                var converter = new System.Windows.Media.BrushConverter();
                mi.Background = (SolidColorBrush)converter.ConvertFromString("#3D26A0DA");
                mi.BorderBrush = (SolidColorBrush)converter.ConvertFromString("#FF26A0DA");

                isEllipse = true;
                isRectangle = true;
            }
            else
            {
                ellipse.Background = null;
                ellipse.BorderBrush = null;
                isEllipse = false;

                rectangle.Background = null;
                rectangle.BorderBrush = null;
                isRectangle = false;
            }
        }

        public void HighlightMenu(object sender)
        {
            // Highlights pen button if it is selected
            if (!isHighlighted)
            {
                // Clears previously set backgrounds
                pencil.Background = null;
                pencil.BorderBrush = null;
                

                MenuItem mi = sender as MenuItem;
                var converter = new System.Windows.Media.BrushConverter();
                mi.Background = (SolidColorBrush)converter.ConvertFromString("#3D26A0DA");
                mi.BorderBrush = (SolidColorBrush)converter.ConvertFromString("#FF26A0DA");
                isHighlighted = true;
            }
            else
            {
                pencil.Background = null;
                pencil.BorderBrush = null;

                isHighlighted = false;
            }
        }

        public void ColorHighlight(object sender)
        {
            // Highlights for color buttons if one of them is selected
            colorBlack.Background = null;
            colorBlack.BorderBrush = null;
            colorGrey.Background = null;
            colorGrey.BorderBrush = null;
            colorWhite.Background = null;
            colorWhite.BorderBrush = null;
            colorBlue.Background = null;
            colorBlue.BorderBrush = null;
            colorRed.Background = null;
            colorRed.BorderBrush = null;
            colorYellow.Background = null;
            colorYellow.BorderBrush = null;

            MenuItem mi = sender as MenuItem;
            var converter = new System.Windows.Media.BrushConverter();
            mi.Background = (SolidColorBrush)converter.ConvertFromString("#3D26A0DA");
            mi.BorderBrush = (SolidColorBrush)converter.ConvertFromString("#FF26A0DA");

        }

        private void tutorial_Click(object sender, RoutedEventArgs e)
        {
            // Opens new window with tutorial video
            Tutorial objWindow1 = new Tutorial();
            this.Visibility = Visibility.Hidden; // Two windows can't be open at the same time
            objWindow1.Show();
        }

        #region ScaleValue Depdency Property

        public static readonly DependencyProperty ScaleValueProperty
            = DependencyProperty.Register("ScaleValue",
                typeof(double),
                typeof(MainWindow),
                new UIPropertyMetadata(1.0,
                    new PropertyChangedCallback(OnScaleValueChanged),
                    new CoerceValueCallback(OnCoerceScaleValue)));

        private static object OnCoerceScaleValue(DependencyObject o, object value)
        {
            MainWindow mainWindow = o as MainWindow;

            if (mainWindow != null)
                return mainWindow.OnCoerceScaleValue((double)value);
            else return value;
        }

        private static void OnScaleValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)

        {
            MainWindow mainWindow = o as MainWindow;

            if (mainWindow != null)
                mainWindow.OnScaleValueChanged((double)e.OldValue, (double)e.NewValue);

        }

        protected virtual double OnCoerceScaleValue(double value)
        {
            if (double.IsNaN(value))
                return 1.0f;

            value = Math.Max(0.1, value);
            return value;
        }


        protected virtual void OnScaleValueChanged(double oldValue, double newValue)
        {

        }

        public double ScaleValue
        {
            get
            {
                return (double)GetValue(ScaleValueProperty);
            }
            set
            {
                SetValue(ScaleValueProperty, value);
            }
        }

        private void MainGrid_SizeChanged(object sender, EventArgs e)
        {
            CalculateScale();
        }

        private void CalculateScale()
        {
            double yScale = ActualHeight / 600f; // galvena loga izmers
            double xScale = ActualWidth / 800f;
            double value = Math.Min(xScale, yScale);
            ScaleValue = (double)OnCoerceScaleValue(myMainWindow, value);
        }

        #endregion
    }
}
