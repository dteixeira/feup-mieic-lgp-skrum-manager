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

        private delegate void TryTransitionPageChangeDirectionHandler(PageChangeDirection direction, PageTransitionType transition);
        private delegate void TryTransitionPageChangeHandler(PageChange direction, PageTransitionType transition);

        public PageTransitionType Transition
        {
            set { Dispatcher.Invoke(new Action(() => { this.PageTransitionControl.TransitionType = value; })); }
        }

        public NavigationControl Navigation
        {
            get { return this.NavigationLayer; }
        }

        public System.Windows.Threading.Dispatcher WindowDispatcher
        {
            get { return this.Dispatcher; }
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

        public void TryTransition(PageChange change, PageTransitionType transition = PageTransitionType.Fade)
        {
            if (this.Dispatcher.Thread == System.Threading.Thread.CurrentThread)
            {
                if (change != null)
                {
                    // Release previous page.
                    this.backdata.CurrentPage.UnloadPage();
                    this.backdata.CurrentPage = null;

                    // Create and setup current page.
                    ITargetPage targetPage = this.CreatePage(change);
                    targetPage.SetupNavigation();
                    this.backdata.CurrentPage = targetPage;

                    // Animate transition.
                    this.WindowEvery.Background = Brushes.Transparent;
                    this.Logo.Visibility = Visibility.Collapsed;
                    this.Transition = transition;
                    this.PageTransitionControl.ShowPage((UserControl)targetPage);
                }
            }
            else
            {
                this.Dispatcher.BeginInvoke(new TryTransitionPageChangeHandler(TryTransition), new PageChange[] { change });
            }
        }

        public void TryTransition(PageChangeDirection direction, PageTransitionType transition = PageTransitionType.Fade)
        {
            if (this.Dispatcher.Thread == System.Threading.Thread.CurrentThread)
            {
                // Try to move only if theres a current page.
                if (this.backdata.CurrentPage != null)
                {
                    // Try to move only if a valid transition is defined.
                    PageChange pageChange = this.backdata.CurrentPage.PageChangeTarget(direction);
                    if (pageChange != null)
                    {

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
                        this.Transition = transition;
                        this.PageTransitionControl.ShowPage((UserControl)targetPage);
                    }
                }
            }
            else
            {
                this.Dispatcher.BeginInvoke(new TryTransitionPageChangeDirectionHandler(TryTransition), new PageChangeDirection[] { direction });
            }
        }

        private void NavigationEventHandler(PageChangeDirection direction)
        {
            // Try to make a transition.
            this.TryTransition(direction);
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
                    return new BacklogPageLib.BacklogPage(page.Context);
                case ApplicationPages.MainPage:
                    return new MainPageLib.MainPage(page.Context);
                case ApplicationPages.MeetingPage:
                    return new MeetingPageLib.MeetingPage(page.Context);
                case ApplicationPages.PeopleManagementPage:
                    return new PeopleManagementPageLib.PeopleManagementPage(page.Context);
                case ApplicationPages.PersonStatisticsPage:
                    return null;
                case ApplicationPages.PersonTaskBoardPage:
                    return null;
                case ApplicationPages.ProjectBacklogPage:
                    return new ProjectBacklogPageLib.ProjectBacklogPage(page.Context);
                case ApplicationPages.ProjectManagementPage:
                    return new ProjectManagementPageLib.ProjectManagementPage(page.Context);
                case ApplicationPages.ProjectStatisticsPage:
                    return new ProjectStatisticsPageLib.ProjectStatisticsPage(page.Context);
                case ApplicationPages.ProjectTeamManagementPage:
                    return new ProjectTeamManagementPageLib.ProjectTeamManagementPage(page.Context);
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
            // Engage new skeleton.
            if (e.GestureType == KinectGestureType.WaveRightHand && backdata.TrackingID == -1)
            {
                Mouse.OverrideCursor = Cursors.None;
                this.backdata.TrackingID = e.TrackingId;
                this.backdata.KinectSensor.StartTrackingSkeleton(backdata.TrackingID);
                this.RightOpen.Visibility = Visibility.Visible;
                this.RightClosed.Visibility = Visibility.Collapsed;
                this.UpperBar.Background = (Brush)Application.Current.FindResource("KinectOnBarBrush");
            }

            // Disengage previous skeleton.
            else if (e.GestureType == KinectGestureType.WaveLeftHand && backdata.TrackingID == e.TrackingId)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
                this.backdata.TrackingID = -1;
                this.backdata.KinectSensor.StopTrackingSkeleton();
                this.RightOpen.Visibility = Visibility.Collapsed;
                this.RightClosed.Visibility = Visibility.Collapsed;
                this.UpperBar.Background = (Brush)Application.Current.FindResource("KinectOffBarBrush");
            }

            // Recognize transition gestures.
            else if (this.backdata.TrackingID != -1)
            {
                // Process event modifications.
                switch (e.GestureType)
                {
                    case KinectGestureType.SwipeRightToLeft:
                        this.TryTransition(PageChangeDirection.Right, PageTransitionType.SlideRight);
                        break;

                    case KinectGestureType.SwipeLeftToRight:
                        this.TryTransition(PageChangeDirection.Left, PageTransitionType.SlideLeft);
                        break;

                    case KinectGestureType.CircleLeftHand:
                        this.TryTransition(PageChangeDirection.Down, PageTransitionType.SlideDown);
                        break;

                    case KinectGestureType.CircleRightHand:
                        this.TryTransition(PageChangeDirection.Up, PageTransitionType.SlideUp);
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
                this.kinectLayer.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                this.BlurLayer.Visibility = System.Windows.Visibility.Hidden;
                this.kinectLayer.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void UpperBar_Config_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            /*PopupFormControlLib.FormWindow form = new PopupFormControlLib.FormWindow();
            PopupFormControlLib.PasswordBoxPage page = new PopupFormControlLib.PasswordBoxPage();
            page.PageTitle = "Password do Projecto";
            page.PageName = "password";
            form.FormPages.Add(page);
            ApplicationController.Instance.ApplicationWindow.SetWindowFade(true);
            form.ShowDialog();
            ApplicationController.Instance.ApplicationWindow.SetWindowFade(false);
            MessageBox.Show((string)form["password"].PageValue);*/

            //Tester for the project/people selector

            PopupSelectionControlLib.SelectionWindow form1 = new PopupSelectionControlLib.SelectionWindow();
            PopupSelectionControlLib.UserSelectionPage page1 = new PopupSelectionControlLib.UserSelectionPage();
            page1.PageTitle = "Escolha do Utilizador";
            form1.FormPage = page1;
            ApplicationController.Instance.ApplicationWindow.SetWindowFade(true);
            form1.ShowDialog();
            ApplicationController.Instance.ApplicationWindow.SetWindowFade(false);
        }

        private void UpperBar_Close_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}