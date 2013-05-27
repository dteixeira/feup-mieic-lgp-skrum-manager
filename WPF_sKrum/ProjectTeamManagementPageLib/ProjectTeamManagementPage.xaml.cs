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
        private DispatcherTimer countdownTimerDelayScrollLeft;
        private DispatcherTimer countdownTimerDelayScrollRight;
        private DispatcherTimer countdownTimerScrollLeft;
        private DispatcherTimer countdownTimerScrollRight;

        public ApplicationPages PageType { get; set; }
        public ApplicationController.DataModificationHandler DataChangeDelegate { get; set; }

        private ObservableCollection<UserButtonControl> teamCollection = new ObservableCollection<UserButtonControl>();
        

        public ProjectTeamManagementPage(object context)
        {
            InitializeComponent();
            this.PageType = ApplicationPages.ProjectTeamManagementPage;

            // Initialize scroll up delay timer.
            this.countdownTimerDelayScrollLeft = new DispatcherTimer();
            this.countdownTimerDelayScrollLeft.Tick += new EventHandler(ScrollActionDelayLeft);
            this.countdownTimerDelayScrollLeft.Interval = TimeSpan.FromSeconds(1);

            // Initialize scroll down delay timer.
            this.countdownTimerDelayScrollRight = new DispatcherTimer();
            this.countdownTimerDelayScrollRight.Tick += new EventHandler(ScrollActionDelayRight);
            this.countdownTimerDelayScrollRight.Interval = TimeSpan.FromSeconds(1);

            // Initialize scroll up timer.
            this.countdownTimerScrollLeft = new DispatcherTimer();
            this.countdownTimerScrollLeft.Tick += new EventHandler(ScrollActionLeft);
            this.countdownTimerScrollLeft.Interval = TimeSpan.FromSeconds(0.01);

            // Initialize scroll down timer.
            this.countdownTimerScrollRight = new DispatcherTimer();
            this.countdownTimerScrollRight.Tick += new EventHandler(ScrollActionRight);
            this.countdownTimerScrollRight.Interval = TimeSpan.FromSeconds(0.01);

            // Register for project change notifications.
            this.DataChangeDelegate = new ApplicationController.DataModificationHandler(this.DataChangeHandler);
            ApplicationController.Instance.DataChangedEvent += this.DataChangeDelegate;

            PopulateProjectTeamManagementPage();
        }

        private void PopulateProjectTeamManagementPage()
        {
            try
            {
                // Get team.
                Project project = ApplicationController.Instance.CurrentProject;

                ServiceLib.DataService.DataServiceClient connection = new ServiceLib.DataService.DataServiceClient();
                List<Person> team = connection.GetAllPeople();
                connection.Close();

                this.Contents.Children.Clear();
                int row = 3;
                int column = -1;
                for (int qwe = 0; qwe < 10; qwe++)
                {
                    foreach (Person p in team)
                    {
                        // Create person control.
                        GenericControlLib.UserButtonControl button = new GenericControlLib.UserButtonControl();
                        button.UserName = p.Name;

                        // Create proper grids.
                        ++row;
                        if (row > 2)
                        {
                            row = 0;
                            column++;
                        }
                        if (row == 0)
                        {
                            ColumnDefinition columnDef = new ColumnDefinition();
                            columnDef.Width = new GridLength(1, GridUnitType.Star);
                            this.Contents.ColumnDefinitions.Add(columnDef);
                        }

                        button.UserPhoto = p.PhotoURL;

                        // Create persons control.
                        button.Width = Double.NaN;
                        button.Height = Double.NaN;
                        button.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        button.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                        button.IsDraggable = true;
                        button.Person = p;
                        button.SetValue(Grid.ColumnProperty, column);
                        button.SetValue(Grid.RowProperty, row);
                        button.Margin = new Thickness(10, 10, 10, 10);
                        this.Contents.Children.Add(button);
                    }
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        private void ScrollActionDelayLeft(object sender, EventArgs e)
        {
            this.countdownTimerScrollLeft.Start();
            this.countdownTimerDelayScrollLeft.Stop();
        }

        private void ScrollActionDelayRight(object sender, EventArgs e)
        {
            this.countdownTimerScrollRight.Start();
            this.countdownTimerDelayScrollRight.Stop();
        }

        private void ScrollActionLeft(object sender, EventArgs e)
        {
            scrollValue -= 10.0f;
            if (scrollValue < 0.0f) scrollValue = 0.0f;
            TeamScroll.ScrollToHorizontalOffset(scrollValue);
        }

        private void ScrollActionRight(object sender, EventArgs e)
        {
            scrollValue += 10.0f;
            if (scrollValue > TeamScroll.ScrollableWidth) scrollValue = (float)TeamScroll.ScrollableWidth;
            TeamScroll.ScrollToHorizontalOffset(scrollValue);
        }

        private void ScrollLeft_Start(object sender, MouseEventArgs e)
        {
            this.countdownTimerDelayScrollLeft.Start();
            this.countdownTimerScrollLeft.Stop();
        }

        private void ScrollRight_Start(object sender, MouseEventArgs e)
        {
            this.countdownTimerDelayScrollRight.Start();
            this.countdownTimerScrollRight.Stop();
        }

        private void ScrollLeft_Cancel(object sender, MouseEventArgs e)
        {
            this.countdownTimerDelayScrollLeft.Stop();
            this.countdownTimerScrollLeft.Stop();
        }

        private void ScrollRight_Cancel(object sender, MouseEventArgs e)
        {
            this.countdownTimerDelayScrollRight.Stop();
            this.countdownTimerScrollRight.Stop();
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
            directions[PageChangeDirection.Down] = null;
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
