/* Ilja Gusevs
   Kristina Yasenovich */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace DrawingWithTutorial
{
    public partial class Tutorial : Window
    {
        private bool mediaPlayerIsPlaying = false;
        private bool userIsDraggingSlider = false;
        private bool reverseTime = false; // lblProgressStatus karogs

        public Tutorial()
        {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();

            this.KeyDown += new KeyEventHandler(MediaPlayerProject_KeyDown);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if ((mePlayer.Source != null) && (mePlayer.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
            {
                sliderProgress.Minimum = 0;
                sliderProgress.Maximum = mePlayer.NaturalDuration.TimeSpan.TotalSeconds;
                sliderProgress.Value = mePlayer.Position.TotalSeconds;
            }
        }

        private void Play_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (mePlayer != null) && (mePlayer.Source != null);
        }

        // Plays video tutorial by clickig on "Tutorial" button
        private void Play_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mePlayer.Source = new Uri(@"C:\Users\Илья\Desktop\Media\Tutorial.mp4", UriKind.Absolute);
            mePlayer.Play();
            mediaPlayerIsPlaying = true;
            mePlayer.Width = 800;
            mePlayer.Height = 600;

        }

        // Starts play video
        private void play_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Play();
            mediaPlayerIsPlaying = true;
        }

        // Pause video tutorial
        private void Pause_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        private void Pause_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mePlayer.Pause();
            mediaPlayerIsPlaying = false;
        }

        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = mediaPlayerIsPlaying;
        }

        // Stops video tutorial
        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            mePlayer.Stop();
            mediaPlayerIsPlaying = false;
        }


        // Shortcuts for media player
        private void MediaPlayerProject_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (mediaPlayerIsPlaying)
                {
                    mePlayer.Pause();
                    mediaPlayerIsPlaying = false;
                }
                else
                {
                    mePlayer.Play();
                    mediaPlayerIsPlaying = true;
                }
            }
        }

        // Rewinds backward
        private void fastBckwd_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Position -= TimeSpan.FromSeconds(5);
        }

        // Rewinds forward
        private void fastFwd_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Position += TimeSpan.FromSeconds(5);
        }

        // Progress bar for video time
        private void sliderProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            userIsDraggingSlider = true;
        }

        // Progress bar shows video current position
        private void sliderProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            userIsDraggingSlider = false;
            mePlayer.Position = TimeSpan.FromSeconds(sliderProgress.Value);
        }

        // Progress bar shows video time 
        private void sliderProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            timeUpdate();
        }

        // Changes media player volume on mousewheel
        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            mePlayer.Volume += (e.Delta > 0) ? 0.05 : -0.05;
        }

        private void lblProgressStatus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            reverseTime = !reverseTime;
            timeUpdate();
        }

        // Video time 
        private void timeUpdate()
        {
            if (!reverseTime)
                lblProgressStatus.Text = TimeSpan.FromSeconds(sliderProgress.Value).ToString(@"hh\:mm\:ss");
            else
                lblProgressStatus.Text = "-"
                    + TimeSpan.FromSeconds(mePlayer.NaturalDuration.TimeSpan.TotalSeconds
                    - mePlayer.Position.TotalSeconds).ToString(@"hh\:mm\:ss");
        }

        // Changes back to main window with drawing
        private void backTo_Click(object sender, RoutedEventArgs e)
        {
            MainWindow objWindow1 = new MainWindow();
            this.Visibility = Visibility.Hidden; // Two windows can't be open at the same time
            objWindow1.Show();
            mePlayer.Stop();
        }

        private void lblProgressStatus_MouseLeave(object sender, MouseEventArgs e)
        {
            // tad kad pele vairs nav virsuu lblProgressStatus teksta blokam
        }

        #region ScaleValue Depdency Property

        public static readonly DependencyProperty ScaleValueProperty
            = DependencyProperty.Register("ScaleValue",
                typeof(double),
                typeof(Tutorial),
                new UIPropertyMetadata(1.0,
                    new PropertyChangedCallback(OnScaleValueChanged),
                    new CoerceValueCallback(OnCoerceScaleValue)));

        private static object OnCoerceScaleValue(DependencyObject o, object value)
        {
            Tutorial mainWindow = o as Tutorial;

            if (mainWindow != null)
                return mainWindow.OnCoerceScaleValue((double)value);
            else return value;
        }

        private static void OnScaleValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)

        {
            Tutorial mainWindow = o as Tutorial;

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
            double yScale = ActualHeight / 600f; // Main windows sizes
            double xScale = ActualWidth / 800f;
            double value = Math.Min(xScale, yScale);
            ScaleValue = (double)OnCoerceScaleValue(tutorialWindow, value);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            mePlayer.StretchDirection = StretchDirection.UpOnly;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.SizeChanged += Window_SizeChanged;
        }

        #endregion
    }
}