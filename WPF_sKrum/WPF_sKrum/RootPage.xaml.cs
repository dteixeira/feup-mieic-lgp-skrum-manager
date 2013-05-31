using SharedTypes;
using System.Windows.Controls;

namespace WPFApplication
{
    /// <summary>
    /// Interaction logic for RootPage.xaml
    /// </summary>
    public partial class RootPage : UserControl, ITargetPage
    {
        public ApplicationPages PageType { get; set; }

        public ApplicationController.DataModificationHandler DataChangeDelegate { get; set; }

        public RootPage(object context)
        {
            InitializeComponent();
            this.PageType = ApplicationPages.RootPage;
        }

        public PageChange PageChangeTarget(PageChangeDirection direction)
        {
            switch (direction)
            {
                case PageChangeDirection.Down:
                    return null;
                case PageChangeDirection.Left:
                    return new PageChange { Context = null, Page = ApplicationPages.ProjectManagementPage };
                case PageChangeDirection.Right:
                    return null;
                case PageChangeDirection.Up:
                    return null;
                default:
                    return null;
            }
        }

        public void SetupNavigation()
        {
            System.Collections.Generic.Dictionary<PageChangeDirection, string> directions = new System.Collections.Generic.Dictionary<PageChangeDirection, string>();
            directions[PageChangeDirection.Up] = null;
            directions[PageChangeDirection.Down] = null;
            directions[PageChangeDirection.Left] = "GESTÃO DE PROJECTOS";
            directions[PageChangeDirection.Right] = null;
            ApplicationController.Instance.ApplicationWindow.SetupNavigation(directions);
        }

        public void UnloadPage()
        {
            // Nothing to do.
        }

        public void DataChangeHandler(object sender, ServiceLib.NotificationService.NotificationType notification)
        {
            // Nothing to do.
        }
    }
}