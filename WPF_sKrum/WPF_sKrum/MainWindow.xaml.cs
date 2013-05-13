using Kinect.Gestures;
using Kinect.Pointers;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit.Interaction;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WpfPageTransitions;

namespace WPF_sKrum
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow instance;

        public static MainWindow Instance
        {
            get { return MainWindow.instance; }
        }

        private ApplicationController backdata;
        public string namespace_pages = "WPF_sKrum.";

        public static int pagesInfoSemaphore = 0;
        private DispatcherTimer pagesStatTimer;
        private DispatcherTimer pagesStatDissapearTimer;

        public string Transition
        {
            get { return this.pageTransitionControl.TransitionType.ToString(); }
            set { Dispatcher.Invoke(new Action(() => { this.pageTransitionControl.TransitionType = (PageTransitionType)Enum.Parse(typeof(PageTransitionType), value, true); })); }
        }

        public MainWindow()
        {
            MainWindow.instance = this;

            InitializeComponent();
            this.backdata = ApplicationController.Instance;

            if (this.backdata.KinectSensor.FoundSensor())
            {
                this.backdata.KinectSensor.Gestures.KinectGestureRecognized += new EventHandler<KinectGestureEventArgs>(this.GestureRegognized);

                // Setup pointer controller.
                this.backdata.KinectSensor.Pointers.KinectPointerMoved += new EventHandler<KinectPointerEventArgs>(this.PointerMoved);

                // Register skeleton frame ready callback;
                this.backdata.KinectSensor.Sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(this.runtime_SkeletonFrameReady);

                // Start the sensor.
                this.backdata.KinectSensor.StartSensor();
            }

            this.pagesStatTimer = new DispatcherTimer();
            this.pagesStatTimer.Tick += new EventHandler(PagesInfoAppearAction);
            this.pagesStatTimer.Interval = TimeSpan.FromSeconds(1);

            this.pagesStatDissapearTimer = new DispatcherTimer();
            this.pagesStatDissapearTimer.Tick += new EventHandler(PagesInfoDisappearAction);
            this.pagesStatDissapearTimer.Interval = TimeSpan.FromSeconds(0.1);
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
                this.Transition = "Fade";
                var UsernewPage = Activator.CreateInstance(Type.GetType(this.namespace_pages + sender.ToString()));
                this.pageTransitionControl.ShowPage((UserControl)UsernewPage);
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
                this.UpperBar.Background = Brushes.Blue;
            }

            // Disengage previous skeleton.
            else if (e.GestureType == KinectGestureType.WaveLeftHand && backdata.TrackingId == e.TrackingId)
            {
                this.backdata.TrackingId = -1;
                this.backdata.KinectSensor.StopTrackingSkeleton();
                this.RightOpen.Visibility = Visibility.Collapsed;
                this.RightClosed.Visibility = Visibility.Collapsed;
                this.UpperBar.Background = Brushes.Gray;
            }

            else if (backdata.TrackingId != -1)
            {
                // Process event modifications.
                switch (e.GestureType)
                {
                    case KinectGestureType.SwipeRightToLeft:
                        if (backdata.SwipeOrdersRightToLeft.ContainsKey(backdata.currentPage))
                        {
                            this.Window_Every.Background = Brushes.Transparent;
                            this.Logo.Visibility = Visibility.Collapsed;
                            this.Transition = "SlideRight";
                            var newPage = Activator.CreateInstance(Type.GetType(this.namespace_pages + backdata.SwipeOrdersRightToLeft[backdata.currentPage]));
                            this.pageTransitionControl.ShowPage((UserControl)newPage);
                        }
                        break;

                    case KinectGestureType.SwipeLeftToRight:
                        if (backdata.SwipeOrdersLeftToRight.ContainsKey(backdata.currentPage))
                        {
                            this.Window_Every.Background = Brushes.Transparent;
                            this.Logo.Visibility = Visibility.Collapsed;
                            this.Transition = "SlideLeft";
                            var newPage = Activator.CreateInstance(Type.GetType(this.namespace_pages + backdata.SwipeOrdersLeftToRight[backdata.currentPage]));
                            this.pageTransitionControl.ShowPage((UserControl)newPage);
                        }

                        break;

                    case KinectGestureType.CircleLeftHand:
                        if (backdata.SwipeOrdersDowntoUp.ContainsKey(backdata.currentPage))
                        {
                            this.Window_Every.Background = Brushes.Transparent;
                            this.Logo.Visibility = Visibility.Collapsed;
                            this.Transition = "SlideUp";
                            var newPage = Activator.CreateInstance(Type.GetType(this.namespace_pages + backdata.SwipeOrdersDowntoUp[backdata.currentPage]));
                            this.pageTransitionControl.ShowPage((UserControl)newPage);
                        }

                        break;

                    case KinectGestureType.CircleRightHand:
                        if (backdata.SwipeOrdersUptoDown.ContainsKey(backdata.currentPage))
                        {
                            this.Window_Every.Background = Brushes.Transparent;
                            this.Logo.Visibility = Visibility.Collapsed;
                            this.Transition = "SlideDown";
                            var newPage = Activator.CreateInstance(Type.GetType(this.namespace_pages + backdata.SwipeOrdersUptoDown[backdata.currentPage]));
                            this.pageTransitionControl.ShowPage((UserControl)newPage);
                        }

                        break;

                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Callback for SkeletonFrameReady event. Draws the tracked skeleton
        /// and updates the gesture recognition
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
                    backdata.skeletons = new Skeleton[SFrame.SkeletonArrayLength];
                    SFrame.CopySkeletonDataTo(backdata.skeletons);
                    receivedData = true;
                }
            }

            if (receivedData)
            {
                // No skeleton engaged, draw all.
                if (backdata.TrackingId != -1)
                {
                    var skeleton = backdata.skeletons.FirstOrDefault(s => s.TrackingId == backdata.TrackingId);
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

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

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

        private void PagesInfoAppearAction(object sender, EventArgs e)
        {
            this.pagesStatTimer.Stop();
            this.pagesStatDissapearTimer.Stop();
            if (this.backdata.SwipeOrdersDowntoUp.ContainsKey(this.backdata.currentPage))
            {
                this.DownPage.Visibility = Visibility.Visible;
            }
            if (this.backdata.SwipeOrdersUptoDown.ContainsKey(this.backdata.currentPage))
            {
                this.UpPage.Visibility = Visibility.Visible;
            }
            if (this.backdata.SwipeOrdersRightToLeft.ContainsKey(this.backdata.currentPage))
            {
                this.RightPage.Visibility = Visibility.Visible;
            }
            if (this.backdata.SwipeOrdersLeftToRight.ContainsKey(this.backdata.currentPage))
            {
                this.LeftPage.Visibility = Visibility.Visible;
            }
        }

        private void PagesInfoDisappearAction(object sender, EventArgs e)
        {
            this.pagesStatTimer.Stop();
            this.pagesStatDissapearTimer.Stop();

            this.UpPage.Visibility = Visibility.Collapsed;
            this.DownPage.Visibility = Visibility.Collapsed;
            this.LeftPage.Visibility = Visibility.Collapsed;
            this.RightPage.Visibility = Visibility.Collapsed;
        }

        private void HoverPagesStat_MouseLeave(object sender, MouseEventArgs e)
        {
            pagesInfoSemaphore--;
            if (pagesInfoSemaphore != 0) return;
            else
            {
                //Start/Continue disappearing and stop the action of apearing
                this.pagesStatTimer.Stop();
                this.pagesStatDissapearTimer.Start();
            }
        }

        private void HoverPagesStat_MouseEnter(object sender, MouseEventArgs e)
        {
            pagesInfoSemaphore++;
            if (pagesInfoSemaphore != 1) return;
            else
            {
                //Start/Continue appearing and stop the action of dissapearing
                this.pagesStatTimer.Start();
                this.pagesStatDissapearTimer.Stop();
            }
        }

        private void HoverPagesStatLeft_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.LeftPage.Visibility == Visibility.Visible && backdata.SwipeOrdersLeftToRight.ContainsKey(backdata.currentPage))
            {
                KinectGestureEventArgs userGeneratedSignal = new KinectGestureEventArgs(KinectGestureType.UserGenerated, backdata.TrackingId);
                MainWindow.Instance.GestureRegognized(backdata.SwipeOrdersLeftToRight[backdata.currentPage], userGeneratedSignal);
                this.PagesInfoDisappearAction(null, null);
            }
        }

        private void HoverPagesStatRight_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.RightPage.Visibility == Visibility.Visible && backdata.SwipeOrdersRightToLeft.ContainsKey(backdata.currentPage))
            {
                KinectGestureEventArgs userGeneratedSignal = new KinectGestureEventArgs(KinectGestureType.UserGenerated, backdata.TrackingId);
                MainWindow.Instance.GestureRegognized(backdata.SwipeOrdersRightToLeft[backdata.currentPage], userGeneratedSignal);
                this.PagesInfoDisappearAction(null, null);
            }
        }

        private void HoverPagesStatUp_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.UpPage.Visibility == Visibility.Visible && backdata.SwipeOrdersUptoDown.ContainsKey(backdata.currentPage))
            {
                KinectGestureEventArgs userGeneratedSignal = new KinectGestureEventArgs(KinectGestureType.UserGenerated, backdata.TrackingId);
                MainWindow.Instance.GestureRegognized(backdata.SwipeOrdersUptoDown[backdata.currentPage], userGeneratedSignal);
                this.PagesInfoDisappearAction(null, null);
            }
        }

        private void HoverPagesStatDown_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.DownPage.Visibility == Visibility.Visible && backdata.SwipeOrdersDowntoUp.ContainsKey(backdata.currentPage))
            {
                KinectGestureEventArgs userGeneratedSignal = new KinectGestureEventArgs(KinectGestureType.UserGenerated, backdata.TrackingId);
                MainWindow.Instance.GestureRegognized(backdata.SwipeOrdersDowntoUp[backdata.currentPage], userGeneratedSignal);
                this.PagesInfoDisappearAction(null, null);
            }
        }
    }
}