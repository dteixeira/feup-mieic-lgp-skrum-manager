using Kinect.Gestures;
using Kinect.Pointers;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.Interaction;
using PageTransitions;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ServiceLib.DataService;
using GenericControlLib;

namespace WPFApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        private static MainWindow instance;

        /// <summary>
        /// Returns a reference to the singleton MainWindow instance.
        /// </summary>
        public static MainWindow Instance
        {
            get { return MainWindow.instance; }
        }

        private ApplicationController backdata;
        private const int MOUSEEVENTF_LEFTDOWN = 0x00000002;
        private const int MOUSEEVENTF_LEFTUP = 0x00000004;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x00000020;
        private const int MOUSEEVENTF_MIDDLEUP = 0x00000040;
        private const int MOUSEEVENTF_MOVE = 0x00000001;
        private const int MOUSEEVENTF_ABSOLUTE = 0x00008000;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x00000008;
        private const int MOUSEEVENTF_RIGHTUP = 0x00000010;
        private const int MOUSEEVENTF_WHEEL = 0x00000800;
        private const int MOUSEEVENTF_XDOWN = 0x00000080;
        private const int MOUSEEVENTF_XUP = 0x00000100;

        public PageTransitionType Transition
        {
            set { Dispatcher.Invoke(new Action(() => { this.pageTransitionControl.TransitionType = value; })); }
        }

        public NavigationControl Navigation
        {
            get { return this.NavigationLayer; }
        }

        public void WindowBlur(Boolean blur)
        {
            if (blur)
            {
                this.BlurLayer.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.BlurLayer.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            MainWindow.instance = this;
            this.backdata = ApplicationController.Instance;

            // Setup navigation controls.
            this.Navigation.UpBarText = null;
            this.Navigation.DownBarText = null;
            this.Navigation.LeftBarText = null;
            this.Navigation.RightBarText = "MENU INICIAL";
            this.Navigation.NavigationEvent += new NavigationControl.NavigationHandler(NavigationEventHandler);

            // Register for callbacks if sensor is ready and
            // start the sensor.
            if (this.backdata.KinectSensor.FoundSensor())
            {
                this.backdata.KinectSensor.Gestures.KinectGestureRecognized += new EventHandler<KinectGestureEventArgs>(this.GestureRegognized);
                this.backdata.KinectSensor.Pointers.KinectPointerMoved += new EventHandler<KinectPointerEventArgs>(this.PointerMoved);
                this.backdata.KinectSensor.Sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(this.runtime_SkeletonFrameReady);
                this.backdata.KinectSensor.StartSensor();
            }
        }

        private void NavigationEventHandler(NavigationDirection direction)
        {
            KinectGestureEventArgs userGeneratedSignal = new KinectGestureEventArgs(KinectGestureType.UserGenerated, backdata.TrackingId);
            switch (direction)
            {
                case NavigationDirection.Up:
                    MainWindow.Instance.GestureRegognized(this.backdata.PagesUp[backdata.CurrentPage], userGeneratedSignal);
                    break;
                case NavigationDirection.Right:
                    MainWindow.Instance.GestureRegognized(this.backdata.PagesRight[backdata.CurrentPage], userGeneratedSignal);
                    break;
                case NavigationDirection.Left:
                    MainWindow.Instance.GestureRegognized(this.backdata.PagesLeft[backdata.CurrentPage], userGeneratedSignal);
                    break;
                case NavigationDirection.Down:
                    MainWindow.Instance.GestureRegognized(this.backdata.PagesDown[backdata.CurrentPage], userGeneratedSignal);
                    break;
            }
        }

        /// <summary>
        /// Creates new application pages.
        /// </summary>
        /// <param name="page">Type of page to be created</param>
        /// <returns>New page.</returns>
        private UserControl CreatePage(ApplicationPages page)
        {
            switch (page)
            {
                case ApplicationPages.sKrum:
                    return null;
                case ApplicationPages.MainPage:
                    return new MainPage();
                case ApplicationPages.ProjectsPage:
                    return new ProjectsPage();
                case ApplicationPages.TaskBoardPage:
                    return new TaskBoardPage();
                case ApplicationPages.UsersPage:
                    return new UsersPage();
                case ApplicationPages.UserStatsPage:
                    return new UserStatsPage();
                default:
                    return null;
            }
        }

        /// <summary>
        /// Callback for "gesture recognized" events.
        /// </summary>
        /// <param name="sender">Object that triggered the event</param>
        /// <param name="e">Event arguments</param>
        public void GestureRegognized(object sender, KinectGestureEventArgs e)
        {
            // User Generated Signal.
            if (e.GestureType == KinectGestureType.UserGenerated)
            {
                this.Window_Every.Background = Brushes.Transparent;
                this.Logo.Visibility = Visibility.Collapsed;
                this.Transition = PageTransitionType.Fade;
                var page = this.CreatePage((ApplicationPages)sender);
                this.pageTransitionControl.ShowPage(page);
                return;
            }

            // Engage new skeleton.
            if (e.GestureType == KinectGestureType.WaveRightHand && backdata.TrackingId == -1)
            {
                this.Cursor = Cursors.None;
                this.backdata.TrackingId = e.TrackingId;
                this.backdata.KinectSensor.StartTrackingSkeleton(backdata.TrackingId);
                this.RightOpen.Visibility = Visibility.Visible;
                this.RightClosed.Visibility = Visibility.Collapsed;
                this.UpperBar.Background = (Brush)Application.Current.FindResource("KinectOnBarBrush");
            }

            // Disengage previous skeleton.
            else if (e.GestureType == KinectGestureType.WaveLeftHand && backdata.TrackingId == e.TrackingId)
            {
                this.backdata.TrackingId = -1;
                this.backdata.KinectSensor.StopTrackingSkeleton();
                this.RightOpen.Visibility = Visibility.Collapsed;
                this.RightClosed.Visibility = Visibility.Collapsed;
                this.UpperBar.Background = (Brush)Application.Current.FindResource("KinectOffBarBrush");
            }

            else if (this.backdata.TrackingId != -1)
            {
                // Process event modifications.
                switch (e.GestureType)
                {
                    case KinectGestureType.SwipeRightToLeft:
                        if (this.backdata.PagesRight.ContainsKey(this.backdata.CurrentPage))
                        {
                            this.Window_Every.Background = Brushes.Transparent;
                            this.Logo.Visibility = Visibility.Collapsed;
                            this.Transition = PageTransitionType.SlideRight;
                            UserControl page = this.CreatePage(this.backdata.PagesRight[this.backdata.CurrentPage]);
                            this.pageTransitionControl.ShowPage(page);
                        }
                        break;

                    case KinectGestureType.SwipeLeftToRight:
                        if (this.backdata.PagesLeft.ContainsKey(this.backdata.CurrentPage))
                        {
                            this.Window_Every.Background = Brushes.Transparent;
                            this.Logo.Visibility = Visibility.Collapsed;
                            this.Transition = PageTransitionType.SlideLeft;
                            UserControl page = this.CreatePage(this.backdata.PagesLeft[this.backdata.CurrentPage]);
                            this.pageTransitionControl.ShowPage(page);
                        }
                        break;

                    case KinectGestureType.CircleLeftHand:
                        if (this.backdata.PagesDown.ContainsKey(this.backdata.CurrentPage))
                        {
                            this.Window_Every.Background = Brushes.Transparent;
                            this.Logo.Visibility = Visibility.Collapsed;
                            this.Transition = PageTransitionType.SlideUp;
                            UserControl page = this.CreatePage(this.backdata.PagesDown[this.backdata.CurrentPage]);
                            this.pageTransitionControl.ShowPage(page);
                        }
                        break;

                    case KinectGestureType.CircleRightHand:
                        if (this.backdata.PagesUp.ContainsKey(this.backdata.CurrentPage))
                        {
                            this.Window_Every.Background = Brushes.Transparent;
                            this.Logo.Visibility = Visibility.Collapsed;
                            this.Transition = PageTransitionType.SlideDown;
                            UserControl page = this.CreatePage(this.backdata.PagesUp[this.backdata.CurrentPage]);
                            this.pageTransitionControl.ShowPage(page);
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Used to automatically disengage out of frame skeletons.
        /// </summary>
        /// <param name="sender">Object that triggered the event</param>
        /// <param name="e">Event arguments</param>
        public void runtime_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            bool receivedData = false;
            using (SkeletonFrame SFrame = e.OpenSkeletonFrame())
            {
                if (SFrame == null)
                {
                    // The image processing took too long. More than 2 frames behind.
                }
                else
                {
                    backdata.Skeletons = new Skeleton[SFrame.SkeletonArrayLength];
                    SFrame.CopySkeletonDataTo(backdata.Skeletons);
                    receivedData = true;
                }
            }
            if (receivedData)
            {
                // No skeleton engaged, draw all.
                if (backdata.TrackingId != -1)
                {
                    var skeleton = backdata.Skeletons.FirstOrDefault(s => s.TrackingId == backdata.TrackingId);
                    if (skeleton == null)
                    {
                        this.GestureRegognized(this, new KinectGestureEventArgs(KinectGestureType.WaveLeftHand, backdata.TrackingId));
                    }
                }
            }
        }

        /// <summary>
        /// Callback called when the pointers move or change state.
        /// </summary>
        /// <param name="sender">Object that triggered the event</param>
        /// <param name="e">Pointer change arguments</param>
        public void PointerMoved(object sender, KinectPointerEventArgs e)
        {
            uint flag = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE;

            // Position right hand.
            if (e.RightHand.HandEventType == InteractionHandEventType.GripRelease)
            {
                backdata.Gripping = false;
                flag = flag | MOUSEEVENTF_LEFTUP;
                RightOpen.Visibility = Visibility.Visible;
                RightClosed.Visibility = Visibility.Collapsed;
            }
            else if (e.RightHand.HandEventType == InteractionHandEventType.Grip)
            {
                backdata.Gripping = true;
                flag = flag | MOUSEEVENTF_LEFTDOWN;
                RightOpen.Visibility = Visibility.Collapsed;
                RightClosed.Visibility = Visibility.Visible;
            }

            if (backdata.Gripping)
            {
                flag |= MOUSEEVENTF_LEFTDOWN;
            }

            //TODO nao deixar sair do ecra a imagem
            Canvas.SetLeft(RightOpen, (e.RightHand.X * this.RenderSize.Width) - (RightOpen.RenderSize.Width / 2));
            Canvas.SetTop(RightOpen, (e.RightHand.Y * this.RenderSize.Height) - (RightOpen.RenderSize.Height / 2));

            Canvas.SetLeft(RightClosed, (e.RightHand.X * this.RenderSize.Width) - (RightClosed.RenderSize.Width / 2));
            Canvas.SetTop(RightClosed, (e.RightHand.Y * this.RenderSize.Height) - (RightClosed.RenderSize.Height / 2));

            //Call the imported function with the cursor's current position
            uint X = (uint)(e.RightHand.X * 65535);
            uint Y = (uint)(e.RightHand.Y * 65535);
            mouse_event(flag, X, Y, 0, 0);
        }
    }
}