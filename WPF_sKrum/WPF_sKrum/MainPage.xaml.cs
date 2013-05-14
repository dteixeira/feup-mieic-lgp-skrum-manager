using Kinect.Gestures;
using System.Windows;
using System.Windows.Controls;

namespace WPFApplication
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
            this.backdata.CurrentPage = ApplicationPages.MainPage;
        }

        private void DailyScrum_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            KinectGestureEventArgs userGeneratedSignal = new KinectGestureEventArgs(KinectGestureType.UserGenerated, backdata.TrackingId);
            MainWindow.Instance.GestureRegognized(ApplicationPages.TaskBoardPage, userGeneratedSignal);
        }
    }
}