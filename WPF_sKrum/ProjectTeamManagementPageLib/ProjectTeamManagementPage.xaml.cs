using GenericControlLib;
using ServiceLib.DataService;
using ServiceLib.NotificationService;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ProjectTeamManagementPageLib
{
    /// <summary>
    /// Interaction logic for MeetingPage.xaml
    /// </summary>
    public partial class ProjectTeamManagementPage : UserControl, ITargetPage
    {
        private float scrollValue = 0.0f;
        private DispatcherTimer countdownTimerDelayScrollUp;
        private DispatcherTimer countdownTimerDelayScrollDown;
        private DispatcherTimer countdownTimerScrollUp;
        private DispatcherTimer countdownTimerScrollDown;

        public ApplicationPages PageType { get; set; }
        public ApplicationController.DataModificationHandler DataChangeDelegate { get; set; }

        private ObservableCollection<UserButtonControl> teamCollection = new ObservableCollection<UserButtonControl>();
        

        public ProjectTeamManagementPage(object context)
        {
            InitializeComponent();
            this.PageType = ApplicationPages.ProjectTeamManagementPage;

            // Initialize scroll up delay timer.
            this.countdownTimerDelayScrollUp = new DispatcherTimer();
            this.countdownTimerDelayScrollUp.Tick += new EventHandler(ScrollActionDelayUp);
            this.countdownTimerDelayScrollUp.Interval = TimeSpan.FromSeconds(1);

            // Initialize scroll down delay timer.
            this.countdownTimerDelayScrollDown = new DispatcherTimer();
            this.countdownTimerDelayScrollDown.Tick += new EventHandler(ScrollActionDelayDown);
            this.countdownTimerDelayScrollDown.Interval = TimeSpan.FromSeconds(1);

            // Initialize scroll up timer.
            this.countdownTimerScrollUp = new DispatcherTimer();
            this.countdownTimerScrollUp.Tick += new EventHandler(ScrollActionUp);
            this.countdownTimerScrollUp.Interval = TimeSpan.FromSeconds(0.01);

            // Initialize scroll down timer.
            this.countdownTimerScrollDown = new DispatcherTimer();
            this.countdownTimerScrollDown.Tick += new EventHandler(ScrollActionDown);
            this.countdownTimerScrollDown.Interval = TimeSpan.FromSeconds(0.01);

            // Register for project change notifications.
            this.DataChangeDelegate = new ApplicationController.DataModificationHandler(this.DataChangeHandler);
            ApplicationController.Instance.DataChangedEvent += this.DataChangeDelegate;

            PopulateProjectTeamManagementPage();
        }

        private void PopulateProjectTeamManagementPage()
        {
            try
            {
                // Clears team.
                this.Team.Items.Clear();
                teamCollection.Clear();

                // Get team.
                Project project = ApplicationController.Instance.CurrentProject;

                ServiceLib.DataService.DataServiceClient connection = new ServiceLib.DataService.DataServiceClient();
                List<Person> team = connection.GetAllPeople();
                connection.Close();

                for(int qweqwe=0;qweqwe<20;qweqwe++)
                {
                // Iterate all person on team
                foreach (var person in team.Select((s, i) => new { Value = s, Index = i }))
                {
                    // Create the person.
                    UserButtonControl personControl = new UserButtonControl
                    {
                        
                    };
                    personControl.Width = Double.NaN;
                    personControl.Height = Double.NaN;
                    personControl.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    personControl.IsDraggable = true;

                    teamCollection.Add(personControl);
                }
                }
                this.Team.ItemsSource = teamCollection;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }

        }

        private void ScrollActionDelayUp(object sender, EventArgs e)
        {
            this.countdownTimerScrollUp.Start();
            this.countdownTimerDelayScrollUp.Stop();
        }

        private void ScrollActionDelayDown(object sender, EventArgs e)
        {
            this.countdownTimerScrollDown.Start();
            this.countdownTimerDelayScrollDown.Stop();
        }

        private void ScrollActionUp(object sender, EventArgs e)
        {
            scrollValue -= 10.0f;
            if (scrollValue < 0.0f) scrollValue = 0.0f;
            TeamScroll.ScrollToVerticalOffset(scrollValue);
        }

        private void ScrollActionDown(object sender, EventArgs e)
        {
            scrollValue += 10.0f;
            if (scrollValue > TeamScroll.ScrollableHeight) scrollValue = (float)TeamScroll.ScrollableHeight;
            TeamScroll.ScrollToVerticalOffset(scrollValue);
        }

        private void ScrollUp_Start(object sender, MouseEventArgs e)
        {
            this.countdownTimerDelayScrollUp.Start();
            this.countdownTimerScrollUp.Stop();
        }

        private void ScrollDown_Start(object sender, MouseEventArgs e)
        {
            this.countdownTimerDelayScrollDown.Start();
            this.countdownTimerScrollDown.Stop();
        }

        private void ScrollUp_Cancel(object sender, MouseEventArgs e)
        {
            this.countdownTimerDelayScrollUp.Stop();
            this.countdownTimerScrollUp.Stop();
        }

        private void ScrollDown_Cancel(object sender, MouseEventArgs e)
        {
            this.countdownTimerDelayScrollDown.Stop();
            this.countdownTimerScrollDown.Stop();
        }

        public PageChange PageChangeTarget(PageChangeDirection direction)
        {
            switch (direction)
            {
                case PageChangeDirection.Down:
                    return new PageChange { Context = null, Page = ApplicationPages.ProjectConfigurationPage };
                case PageChangeDirection.Left:
                    return new PageChange { Context = null, Page = ApplicationPages.ProjectManagementPage };
                case PageChangeDirection.Right:
                    return new PageChange { Context = null, Page = ApplicationPages.MainPage };
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
            directions[PageChangeDirection.Down] = "AJUSTES DE PROJECTO";
            directions[PageChangeDirection.Left] = "GESTÃO DE PROJECTOS";
            directions[PageChangeDirection.Right] = "MENU INICIAL";
            ApplicationController.Instance.ApplicationWindow.SetupNavigation(directions);
        }

        public void UnloadPage()
        {
            // Unregister for notifications.
            ApplicationController.Instance.DataChangedEvent -= this.DataChangeDelegate;
        }


        public void DataChangeHandler(object sender, NotificationType notification)
        {
            try
            {
                // Respond only to project modifications.
                if (notification == NotificationType.ProjectModification)
                {
                    // Repopulate the taskboard with the current project.
                    this.PopulateProjectTeamManagementPage();
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        private void Rectangle_Drop(object sender, DragEventArgs e)
        {
            //TODO make new ScrumMaster
            System.Console.WriteLine("asd");
        }

    }
}
