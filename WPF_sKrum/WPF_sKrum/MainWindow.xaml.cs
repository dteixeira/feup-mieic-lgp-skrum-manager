using GenericControlLib;
using Kinect.Gestures;
using Kinect.Pointers;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.Interaction;
using ServiceLib.DataService;
using SharedTypes;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace sKrum
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

        private DateTime LastGestureTimestamp { get; set; }

        private object SensorTiltLock { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.backdata = ApplicationController.Instance;
            this.backdata.ApplicationWindow = this;
            this.LastGestureTimestamp = DateTime.Now;

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

            // Registers key up handler to manage the sensor tilt.
            this.SensorTiltLock = new object();
            this.KeyUp += MainWindow_KeyUp;
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            // Tilt should only be available if sensor is active.
            if (this.backdata.KinectSensor.FoundSensor())
            {
                if (e.Key == Key.Up || e.Key == Key.Down)
                {
                    System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(TiltSensor));
                    thread.Start(e);
                }
                e.Handled = true;
            }
        }

        public void TiltSensor(object obj)
        {
            // A new tilt command cannot be issued if previous tilt is still locked;
            KeyEventArgs e = (KeyEventArgs)obj;
            // at least 1200ms should pass between tilts.
            if (System.Threading.Monitor.TryEnter(this.SensorTiltLock))
            {
                try
                {
                    // Increase the tilt.
                    if (e.Key == Key.Up)
                    {
                        if (ApplicationController.Instance.KinectSensor.Sensor.ElevationAngle <= 20)
                        {
                            ApplicationController.Instance.KinectSensor.Sensor.ElevationAngle += 5;
                        }
                    }
                    else if (e.Key == Key.Down)
                    {
                        // Decrease the tilt.
                        if (ApplicationController.Instance.KinectSensor.Sensor.ElevationAngle >= -20)
                        {
                            ApplicationController.Instance.KinectSensor.Sensor.ElevationAngle -= 5;
                        }
                    }
                    System.Threading.Thread.Sleep(1200);
                }
                catch (Exception)
                {
                    // No exception handling needed, just catching.
                }
                finally
                {
                    System.Threading.Monitor.Exit(this.SensorTiltLock);
                }
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
                    this.Transition = transition;
                    this.PageTransitionControl.ShowPage((UserControl)targetPage);
                    if (change.Page == ApplicationPages.PersonTaskBoardPage || change.Page == ApplicationPages.PersonStatisticsPage)
                    {
                        this.UpperBar_PersonName.Text = ((Person)change.Context).Name;
                        this.GridPersonName.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        this.UpperBar_PersonName.Text = "";
                        this.GridPersonName.Visibility = System.Windows.Visibility.Collapsed;
                    }
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
                        this.Transition = transition;
                        this.PageTransitionControl.ShowPage((UserControl)targetPage);
                        if (pageChange.Page == ApplicationPages.PersonTaskBoardPage || pageChange.Page == ApplicationPages.PersonStatisticsPage)
                        {
                            this.UpperBar_PersonName.Text = ((Person)pageChange.Context).Name;
                            this.GridPersonName.Visibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            this.UpperBar_PersonName.Text = "";
                            this.GridPersonName.Visibility = System.Windows.Visibility.Collapsed;
                        }
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
                    return new ProjectStatisticsPageLib.PersonalStatisticsPage(page.Context);

                case ApplicationPages.PersonTaskBoardPage:
                    return new TaskBoardPageLib.PersonalTaskBoardPage(page.Context);

                case ApplicationPages.ProjectBacklogPage:
                    return new ProjectBacklogPageLib.ProjectBacklogPage(page.Context);

                case ApplicationPages.ProjectManagementPage:
                    return new ProjectManagementPageLib.ProjectManagementPage(page.Context);

                case ApplicationPages.ProjectStatisticsPage:
                    return new ProjectStatisticsPageLib.ProjectStatisticsPage(page.Context);

                case ApplicationPages.ProjectTeamManagementPage:
                    return new ProjectTeamManagementPageLib.ProjectTeamManagementPage(page.Context);

                case ApplicationPages.RootPage:
                    return new RootPage(page.Context);

                case ApplicationPages.TaskBoardPage:
                    return new TaskBoardPageLib.TaskBoardPage(page.Context);

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
            if (backdata.TrackingID == -1)
            {
                // Engage new skeleton.
                if (e.GestureType == KinectGestureType.WaveRightHand)
                {
                    Mouse.OverrideCursor = Cursors.None;
                    this.backdata.TrackingID = e.TrackingId;
                    this.backdata.UserHandedness = KinectGestureUserHandedness.RightHanded;
                    this.backdata.KinectSensor.StartTrackingSkeleton(backdata.TrackingID, backdata.UserHandedness);
                    this.LastGestureTimestamp = DateTime.Now;
                    this.RightOpen.Visibility = Visibility.Visible;
                    this.RightClosed.Visibility = Visibility.Collapsed;
                    this.LeftClosed.Visibility = System.Windows.Visibility.Collapsed;
                    this.LeftOpen.Visibility = System.Windows.Visibility.Collapsed;
                    this.UpperBar.Background = (Brush)Application.Current.FindResource("KinectOnBarBrush");
                }
                else if (e.GestureType == KinectGestureType.WaveLeftHand)
                {
                    Mouse.OverrideCursor = Cursors.None;
                    this.backdata.TrackingID = e.TrackingId;
                    this.backdata.UserHandedness = KinectGestureUserHandedness.LeftHanded;
                    this.backdata.KinectSensor.StartTrackingSkeleton(backdata.TrackingID, backdata.UserHandedness);
                    this.LastGestureTimestamp = DateTime.Now;
                    this.RightOpen.Visibility = Visibility.Collapsed;
                    this.RightClosed.Visibility = Visibility.Collapsed;
                    this.LeftClosed.Visibility = Visibility.Collapsed;
                    this.LeftOpen.Visibility = Visibility.Visible;
                    this.UpperBar.Background = (Brush)Application.Current.FindResource("KinectOnBarBrush");
                }
            }

            // Disengage previous skeleton.
            else if (backdata.TrackingID == e.TrackingId)
            {
                // Prevent several gestures triggered very close.
                DateTime now = DateTime.Now;
                if (LastGestureTimestamp.AddSeconds(1) > now)
                {
                    return;
                }
                else
                {
                    LastGestureTimestamp = now;
                }

                if ((e.GestureType == KinectGestureType.WaveLeftHand && this.backdata.UserHandedness == KinectGestureUserHandedness.RightHanded) ||
                    (e.GestureType == KinectGestureType.WaveRightHand && this.backdata.UserHandedness == KinectGestureUserHandedness.LeftHanded))
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                    this.backdata.TrackingID = -1;
                    this.backdata.KinectSensor.StopTrackingSkeleton();
                    this.RightOpen.Visibility = Visibility.Collapsed;
                    this.RightClosed.Visibility = Visibility.Collapsed;
                    this.LeftOpen.Visibility = Visibility.Collapsed;
                    this.LeftClosed.Visibility = Visibility.Collapsed;
                    this.UpperBar.Background = (Brush)Application.Current.FindResource("KinectOffBarBrush");
                }
                else
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
            if (backdata.Gripping)
            {
                flag |= MOUSEEVENTF_LEFTDOWN;
            }

            // Render hand cursor for right handed users.
            if (this.backdata.UserHandedness == KinectGestureUserHandedness.RightHanded)
            {
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

                // Normalize pointer coordinates.
                double normalizedX = Math.Max(0, Math.Min(e.RightHand.X, 1.0));
                double normalizedY = Math.Max(0, Math.Min(e.RightHand.Y, 1.0));

                Canvas.SetLeft(RightOpen, (normalizedX * this.RenderSize.Width) - (RightOpen.RenderSize.Width / 2));
                Canvas.SetTop(RightOpen, (normalizedY * this.RenderSize.Height) - (RightOpen.RenderSize.Height / 2));

                Canvas.SetLeft(RightClosed, (normalizedX * this.RenderSize.Width) - (RightClosed.RenderSize.Width / 2));
                Canvas.SetTop(RightClosed, (normalizedY * this.RenderSize.Height) - (RightClosed.RenderSize.Height / 2));

                // Call the imported function with the cursor's current position
                uint x = (uint)(normalizedX * 65535);
                uint y = (uint)(normalizedY * 65535);
                mouse_event(flag, x, y, 0, 0);
            }
            // Render hand cursor for left handed users.
            else
            {
                if (e.LeftHand.HandEventType == InteractionHandEventType.GripRelease)
                {
                    backdata.Gripping = false;
                    flag = flag | MOUSEEVENTF_LEFTUP;
                    LeftOpen.Visibility = Visibility.Visible;
                    LeftClosed.Visibility = Visibility.Collapsed;
                }
                else if (e.LeftHand.HandEventType == InteractionHandEventType.Grip)
                {
                    backdata.Gripping = true;
                    flag = flag | MOUSEEVENTF_LEFTDOWN;
                    LeftOpen.Visibility = Visibility.Collapsed;
                    LeftClosed.Visibility = Visibility.Visible;
                }

                // Normalize pointer coordinates.
                double normalizedX = Math.Max(0, Math.Min(e.LeftHand.X, 1.0));
                double normalizedY = Math.Max(0, Math.Min(e.LeftHand.Y, 1.0));

                Canvas.SetLeft(LeftOpen, (normalizedX * this.RenderSize.Width) - (LeftOpen.RenderSize.Width / 2));
                Canvas.SetTop(LeftOpen, (normalizedY * this.RenderSize.Height) - (LeftOpen.RenderSize.Height / 2));

                Canvas.SetLeft(LeftClosed, (normalizedX * this.RenderSize.Width) - (LeftClosed.RenderSize.Width / 2));
                Canvas.SetTop(LeftClosed, (normalizedY * this.RenderSize.Height) - (LeftClosed.RenderSize.Height / 2));

                // Call the imported function with the cursor's current position
                uint x = (uint)(normalizedX * 65535);
                uint y = (uint)(normalizedY * 65535);
                mouse_event(flag, x, y, 0, 0);
            }
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

        public void ShowNotificationMessage(string message, TimeSpan duration)
        {
            this.NotificationMessage.NotificationText = message;
            this.NotificationMessage.VisibleTimespan = duration;
            this.NotificationMessage.StartAnimation();
        }

        private void UpperBar_SelectProj_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Select a project.
            ApplicationController.Instance.ApplicationWindow.SetWindowFade(true);
            PopupSelectionControlLib.SelectionWindow projectForm = new PopupSelectionControlLib.SelectionWindow();
            PopupSelectionControlLib.ProjectSelectionPage projectPage = new PopupSelectionControlLib.ProjectSelectionPage();
            projectPage.PageTitle = "Escolha um Projecto";
            projectForm.FormPage = projectPage;
            projectForm.ShowDialog();
            if (projectForm.Success)
            {
                ServiceLib.DataService.Project project = (ServiceLib.DataService.Project)projectForm.Result;
                if (project.Password != null)
                {
                    PopupFormControlLib.FormWindow form = new PopupFormControlLib.FormWindow();
                    PopupFormControlLib.PasswordBoxPage passwordPage = new PopupFormControlLib.PasswordBoxPage { PageName = "password", PageTitle = "Password do Projecto" };
                    form.FormPages.Add(passwordPage);
                    form.ShowDialog();
                    if (form.Success)
                    {
                        string password = (string)form["password"].PageValue;
                        if (password != null && password != "")
                        {
                            DataServiceClient client = new DataServiceClient();
                            bool login = client.LoginProject(project.ProjectID, password);
                            client.Close();
                            ApplicationController.Instance.ApplicationWindow.SetWindowFade(false);
                            if (login)
                            {
                                ApplicationController.Instance.CurrentProject = project;
                                this.UpperBar_ProjectName.Text = project.Name;
                                ApplicationController.Instance.AdminLogin = false;
                                ApplicationController.Instance.ApplicationWindow.TryTransition(new PageChange { Context = null, Page = ApplicationPages.MainPage });
                            }
                            else
                            {
                                ApplicationController.Instance.ApplicationWindow.ShowNotificationMessage("Não tem permissões para aceder a este projecto.", new TimeSpan(0, 0, 3));
                            }
                        }
                        else
                        {
                            ApplicationController.Instance.ApplicationWindow.ShowNotificationMessage("Não tem permissões para aceder a este projecto.", new TimeSpan(0, 0, 3));
                        }
                    }
                    else
                    {
                        ApplicationController.Instance.ApplicationWindow.ShowNotificationMessage("Não tem permissões para aceder a este projecto.", new TimeSpan(0, 0, 3));
                    }
                }
                else
                {
                    ApplicationController.Instance.CurrentProject = project;
                    this.UpperBar_ProjectName.Text = project.Name;
                    ApplicationController.Instance.AdminLogin = false;
                    ApplicationController.Instance.ApplicationWindow.TryTransition(new PageChange { Context = null, Page = ApplicationPages.MainPage });
                }
            }
            ApplicationController.Instance.ApplicationWindow.SetWindowFade(false);
        }

        private void sKrum_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.TryTransition(new PageChange { Context = null, Page = ApplicationPages.RootPage });
        }

        private void GridPersonName_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Select a project.
            ApplicationController.Instance.ApplicationWindow.SetWindowFade(true);
            PopupSelectionControlLib.SelectionWindow userForm = new PopupSelectionControlLib.SelectionWindow();
            PopupSelectionControlLib.UserSelectionPage userPage = new PopupSelectionControlLib.UserSelectionPage(true);
            userPage.PageTitle = "Escolha uma Pessoa";
            userForm.FormPage = userPage;
            userForm.ShowDialog();
            ApplicationController.Instance.ApplicationWindow.SetWindowFade(false);
            if (userForm.Success)
            {
                Person person = (Person)userForm.Result;
                this.TryTransition(new PageChange { Context = person, Page = ApplicationPages.PersonTaskBoardPage });
            }
        }

        private void GridClose_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void GridHome_MouseLeftButtonUp_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ApplicationController.Instance.CurrentProject != null)
            {
                ApplicationController.Instance.AdminLogin = false;
                ApplicationController.Instance.ApplicationWindow.TryTransition(new PageChange { Context = null, Page = ApplicationPages.MainPage });
            }
            else
            {
                ApplicationController.Instance.AdminLogin = false;
                ApplicationController.Instance.ApplicationWindow.TryTransition(new PageChange { Context = null, Page = ApplicationPages.RootPage });
            }
        }

        private void GridConfig_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ApplicationController.Instance.ApplicationWindow.TryTransition(new PageChange { Context = null, Page = ApplicationPages.ProjectManagementPage });
        }

        private void sKrum_Unloaded(object sender, RoutedEventArgs e)
        {
            if (ApplicationController.Instance.KinectSensor.FoundSensor())
            {
                ApplicationController.Instance.KinectSensor.StopSensor();
            }
        }
    }
}