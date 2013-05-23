﻿using Kinect.Gestures;
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
            this.SetupNavigation();
        }

        private void SetupNavigation()
        {
            MainWindow.Instance.Navigation.UpBarText = null;
            MainWindow.Instance.Navigation.DownBarText = null;
            MainWindow.Instance.Navigation.LeftBarText = "GESTÃO DE PROJECTOS";
            MainWindow.Instance.Navigation.RightBarText = "TASKBOARD";
        }

        private void DailyScrum_PreviewMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            KinectGestureEventArgs userGeneratedSignal = new KinectGestureEventArgs(KinectGestureType.UserGenerated, backdata.TrackingId);
            MainWindow.Instance.GestureRegognized(ApplicationPages.TaskBoardPage, userGeneratedSignal);
        }

        private void Metting_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            // Create a poput window. TODO REMOVE
            PopupFormControlLib.FormWindow popup = new PopupFormControlLib.FormWindow();
            MainWindow.Instance.WindowBlur(true);
            popup.ShowDialog();
            MainWindow.Instance.WindowBlur(false);
        }
    }
}