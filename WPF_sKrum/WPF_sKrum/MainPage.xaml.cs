using Kinect.Gestures;
using System.Windows;
using System.Windows.Controls;

namespace WPF_sKrum
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl
    {
        private ApplicationController backdata;

        public MainPage()
        {
            InitializeComponent();
            this.backdata = ApplicationController.Instance;
            this.backdata.CurrentPage = this.GetType().Name;

            // Subscribe project to notify.
            this.backdata.Notifications.Subscribe(-1);
            this.backdata.DataChangedEvent += backdata_DataChangedEvent;
        }

        private void backdata_DataChangedEvent(object sender, NotificationService.NotificationType type)
        {
            // TODO Notification handling goes here.
        }

        private void DailyScrum_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            KinectGestureEventArgs userGeneratedSignal = new KinectGestureEventArgs(KinectGestureType.UserGenerated, backdata.TrackingId);
            MainWindow.Instance.GestureRegognized("TaskBoardPage", userGeneratedSignal);
        }
    }
}