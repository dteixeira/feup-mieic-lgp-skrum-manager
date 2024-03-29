﻿using SharedTypes;
using System.Windows.Controls;

namespace MainPageLib
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl, ITargetPage
    {
        public MainPage(object context)
        {
            InitializeComponent();
            this.PageType = ApplicationPages.MainPage;
        }

        public void SetupNavigation()
        {
            System.Collections.Generic.Dictionary<PageChangeDirection, string> directions = new System.Collections.Generic.Dictionary<PageChangeDirection, string>();
            directions[PageChangeDirection.Up] = null;
            directions[PageChangeDirection.Down] = null;
            directions[PageChangeDirection.Left] = "GESTÃO DE EQUIPA";
            directions[PageChangeDirection.Right] = "TASKBOARD";
            ApplicationController.Instance.ApplicationWindow.SetupNavigation(directions);
        }

        public ApplicationPages PageType { get; set; }

        public PageChange PageChangeTarget(PageChangeDirection direction)
        {
            switch (direction)
            {
                case PageChangeDirection.Down:
                    return null;
                case PageChangeDirection.Left:
                    return new PageChange { Context = null, Page = ApplicationPages.ProjectTeamManagementPage };
                case PageChangeDirection.Right:
                    return new PageChange { Context = null, Page = ApplicationPages.TaskBoardPage };
                case PageChangeDirection.Up:
                    return null;
                default:
                    return null;
            }
        }

        public void UnloadPage()
        {
            // Nothing to do.
        }

        public void DataChangeHandler(object sender, ServiceLib.NotificationService.NotificationType notification)
        {
            // Nothing to do.
        }

        public ApplicationController.DataModificationHandler DataChangeDelegate { get; set; }

        private void ButtonControl_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PageChange page = new PageChange { Context = null, Page = ApplicationPages.BacklogPage };
            ApplicationController.Instance.ApplicationWindow.TryTransition(page);
        }

        private void ButtonControl_MouseLeftButtonUp_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PageChange page = new PageChange { Context = null, Page = ApplicationPages.ProjectBacklogPage };
            ApplicationController.Instance.ApplicationWindow.TryTransition(page);
        }

        private void ButtonControl_MouseLeftButtonUp_2(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PageChange page = new PageChange { Context = null, Page = ApplicationPages.TaskBoardPage };
            ApplicationController.Instance.ApplicationWindow.TryTransition(page);
        }

        private void ButtonControl_MouseLeftButtonUp_3(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PageChange page = new PageChange { Context = null, Page = ApplicationPages.MeetingPage };
            ApplicationController.Instance.ApplicationWindow.TryTransition(page);
        }
    }
}