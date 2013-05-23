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
using SharedTypes;

namespace WPFApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, ApplicationWindow
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

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
            set { Dispatcher.Invoke(new Action(() => { this.PageTransitionControl.TransitionType = value; })); }
        }

        public NavigationControl Navigation
        {
            get { return this.NavigationLayer; }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.backdata = ApplicationController.Instance;
            this.backdata.ApplicationWindow = this;

            // Setup navigation controls.
            this.backdata.CurrentPage = new RootPage(null);
            this.backdata.CurrentPage.SetupNavigation();
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

        private void NavigationEventHandler(PageChangeDirection direction)
        {
            // Try to move only if theres a current page.
            if(this.backdata.CurrentPage != null) {

                // Try to move only if a valid transition is defined.
                PageChange pageChange = this.backdata.CurrentPage.PageChangeTarget(direction);
                if(pageChange != null) {
                    
                    // Release previous page.
                    this.backdata.CurrentPage.UnloadPage();
                    this.backdata.CurrentPage = null;

                    // Create and setup current page.
                    ITargetPage targetPage = this.CreatePage(pageChange);
                    targetPage.SetupNavigation();
                    this.backdata.CurrentPage = targetPage;

                    // Animate transition.
                    this.WindowEvery.Background = Brushes.Transparent;
                    this.Logo.Visibility = Visibility.Collapsed;
                    this.Transition = PageTransitionType.Fade;
                    this.PageTransitionControl.ShowPage((UserControl)targetPage);
                }
            }
        }

        /// <summary>
        /// Creates new application pages.
        /// </summary>
        /// <param name="page">Type of page to be created</param>
        /// <returns>New page.</returns>
        private ITargetPage CreatePage(PageChange page)
        {
            switch (page.Page)
            {
                case ApplicationPages.BacklogPage:
                    return null;
                case ApplicationPages.MainPage:
                    return new MainPageLib.MainPage(page.Context);
                case ApplicationPages.MeetingPage:
                    return null;
                case ApplicationPages.PeopleManagementPage:
                    return null;
                case ApplicationPages.PersonStatisticsPage:
                    return null;
                case ApplicationPages.PersonTaskBoardPage:
                    return null;
                case ApplicationPages.ProjectBacklogPage:
                    return null;
                case ApplicationPages.ProjectConfigurationPage:
                    return null;
                case ApplicationPages.ProjectManagementPage:
                    return null;
                case ApplicationPages.ProjectStatisticsPage:
                    return null;
                case ApplicationPages.ProjectTeamManagementPage:
                    return null;
                case ApplicationPages.RootPage:
                    return null;
                case ApplicationPages.TaskBoardPage:
                    return new TaskBoardPageLib.TaskBoardPage(page.Context);
                default:
                    return null;
            }
        }

        // TODO REFACTOR.
        /// <summary>
        /// Callback for "gesture recognized" events.
        /// </summary>
        /// <param name="sender">Object that triggered the event</param>
        /// <param name="e">Event arguments</param>
        public void GestureRegognized(object sender, KinectGestureEventArgs e)
        {
            /*// User Generated Signal.
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
            if (e.GestureType == KinectGestureType.WaveRightHand && backdata.TrackingID == -1)
            {
                this.Cursor = Cursors.None;
                this.backdata.TrackingID = e.TrackingId;
                this.backdata.KinectSensor.StartTrackingSkeleton(backdata.TrackingID);
                this.RightOpen.Visibility = Visibility.Visible;
                this.RightClosed.Visibility = Visibility.Collapsed;
                this.UpperBar.Background = (Brush)Application.Current.FindResource("KinectOnBarBrush");
            }

            // Disengage previous skeleton.
            else if (e.GestureType == KinectGestureType.WaveLeftHand && backdata.TrackingID == e.TrackingId)
            {
                this.backdata.TrackingID = -1;
                this.backdata.KinectSensor.StopTrackingSkeleton();
                this.RightOpen.Visibility = Visibility.Collapsed;
                this.RightClosed.Visibility = Visibility.Collapsed;
                this.UpperBar.Background = (Brush)Application.Current.FindResource("KinectOffBarBrush");
            }

            else if (this.backdata.TrackingID != -1)
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
            }*/
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
                if (backdata.TrackingID != -1)
                {
                    var skeleton = backdata.Skeletons.FirstOrDefault(s => s.TrackingId == backdata.TrackingID);
                    if (skeleton == null)
                    {
                        this.GestureRegognized(this, new KinectGestureEventArgs(KinectGestureType.WaveLeftHand, backdata.TrackingID));
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

        /// <summary>
        /// Sets the current navigation bars.
        /// </summary>
        /// <param name="directions">Maps every possible transition direction
        /// to the target page's name</param>
        public void SetupNavigation(System.Collections.Generic.Dictionary<PageChangeDirection, string> directions)
        {
            foreach (PageChangeDirection key in directions.Keys)
            {
                switch (key)
                {
                    case PageChangeDirection.Up:
                        this.Navigation.UpBarText = directions[key];
                        break;
                    case PageChangeDirection.Right:
                        this.Navigation.RightBarText = directions[key];
                        break;
                    case PageChangeDirection.Left:
                        this.Navigation.LeftBarText = directions[key];
                        break;
                    case PageChangeDirection.Down:
                        this.Navigation.DownBarText = directions[key];
                        break;
                }
            }
        }

        /// <summary>
        /// Fades or clears the main window (for popups).
        /// </summary>
        /// <param name="fade">If true fade, otherwise clear.</param>
        public void SetWindowFade(bool fade)
        {
            if (fade)
            {
                this.BlurLayer.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.BlurLayer.Visibility = System.Windows.Visibility.Hidden;
            }
        }
    }
}