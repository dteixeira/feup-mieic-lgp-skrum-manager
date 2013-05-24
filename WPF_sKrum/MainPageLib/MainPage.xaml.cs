using System.Windows;
using System.Windows.Controls;
using SharedTypes;

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
            directions[PageChangeDirection.Left] = "AJUSTES DO PROJECTO";
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
                    return new PageChange { Context = null, Page = ApplicationPages.ProjectConfigurationPage };
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

        private void ButtonControl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PageChange page = new PageChange { Context = null, Page = ApplicationPages.BacklogPage };
            ApplicationController.Instance.ApplicationWindow.TryTransition(page);
        }
    }
}