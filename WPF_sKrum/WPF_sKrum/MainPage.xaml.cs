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
            backdata = ApplicationController.Instance;
            backdata.currentPage = this.GetType().Name;
        }

        private void DailyScrum_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            KinectGestureEventArgs userGeneratedSignal = new KinectGestureEventArgs(KinectGestureType.UserGenerated, backdata.TrackingId);
            MainWindow.Instance.GestureRegognized("TaskBoardPage", userGeneratedSignal);
        }
    }
}