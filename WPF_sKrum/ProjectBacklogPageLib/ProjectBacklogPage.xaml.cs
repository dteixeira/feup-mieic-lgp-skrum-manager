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
using TaskBoardControlLib;

namespace ProjectBacklogPageLib
{
    /// <summary>
    /// Interaction logic for ProjectBacklogPage.xaml
    /// </summary>
    public partial class ProjectBacklogPage : UserControl, ITargetPage
    {
        private float scrollValue = 0.0f;
        private float scrollValueSprintBacklog = 0.0f;

        private DispatcherTimer countdownTimerDelayScrollUp;
        private DispatcherTimer countdownTimerDelayScrollDown;
        private DispatcherTimer countdownTimerScrollUp;
        private DispatcherTimer countdownTimerScrollDown;
        private enum BacklogSelected {Backlog, SprintBacklog};
        private BacklogSelected currentBacklog;

        public ApplicationPages PageType { get; set; }
        public ApplicationController.DataModificationHandler DataChangeDelegate { get; set; }

        public ProjectBacklogPage(object context)
        {
            InitializeComponent();
            this.PageType = ApplicationPages.ProjectBacklogPage;

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

            PopulateProjectBacklog();
        }

        private void PopulateProjectBacklog()
        {
            try
            {
                // Clears taskboard.
                this.Backlog.Items.Clear();

                // Get current project if selected.
                Project project = ApplicationController.Instance.CurrentProject;

                ServiceLib.DataService.DataServiceClient connection = new ServiceLib.DataService.DataServiceClient();
                List<Story> storiesBacklog = connection.GetAllStoriesWithoutSprintInProject(project.ProjectID);
                List<Story> storiesSprintBacklog = connection.GetAllStoriesInSprint(ApplicationController.Instance.CurrentProject.Sprints[0].SprintID);
                connection.Close();

                ObservableCollection<StoryControl> collection = new ObservableCollection<StoryControl>();
                // Iterate all user stories in the sprint.
                foreach (var story in storiesBacklog.Select((s, i) => new { Value = s, Index = i }))
                {
                    // Create the story control.
                    StoryControl storyControl = new StoryControl
                    {
                        StoryDescription = story.Value.Description,
                        StoryName = "US" + story.Value.Number.ToString("D3"),
                        StoryPriority = story.Value.Priority.ToString()[0].ToString(),
                        StoryEstimation = "",
                        StoryNumber = story.Value.Number,
                        Story = story.Value,
                        IsDraggable = true
                    };
                    storyControl.Width = Double.NaN;
                    storyControl.Height = Double.NaN;
                    storyControl.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    storyControl.SetValue(Grid.ColumnProperty, 0);
                    storyControl.SetValue(Grid.RowProperty, story.Index);

                    collection.Add(storyControl);
                }
                
                ObservableCollection<StoryControl> collectionSprint = new ObservableCollection<StoryControl>();
                foreach (var story in storiesSprintBacklog.Select((s, i) => new { Value = s, Index = i }))
                {
                    // Create the story control.
                    StoryControl storyControl = new StoryControl
                    {
                        StoryDescription = story.Value.Description,
                        StoryName = "US" + story.Value.Number.ToString("D3"),
                        StoryPriority = story.Value.Priority.ToString()[0].ToString(),
                        StoryEstimation = "",
                        StoryNumber = story.Value.Number,
                        Story = story.Value,
                        IsDraggable = true
                    };
                    storyControl.Width = Double.NaN;
                    storyControl.Height = Double.NaN;
                    storyControl.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    storyControl.SetValue(Grid.ColumnProperty, 0);
                    storyControl.SetValue(Grid.RowProperty, story.Index);

                    collectionSprint.Add(storyControl);
                }
                this.SprintBacklog.ItemsSource = collectionSprint;
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
            switch (currentBacklog)
            {
                case BacklogSelected.SprintBacklog:
                    scrollValueSprintBacklog -= 10.0f;
                    if (scrollValueSprintBacklog < 0.0f) scrollValueSprintBacklog = 0.0f;
                    SprintBacklogScroll.ScrollToVerticalOffset(scrollValueSprintBacklog);
                    break;
                default:
                    scrollValue -= 10.0f;
                    if (scrollValue < 0.0f) scrollValue = 0.0f;
                    BacklogScroll.ScrollToVerticalOffset(scrollValue);
                    break;
            }
        }

        private void ScrollActionDown(object sender, EventArgs e)
        {
            switch (currentBacklog)
            {
                case BacklogSelected.SprintBacklog:
                    scrollValueSprintBacklog += 10.0f;
                    if (scrollValueSprintBacklog > SprintBacklogScroll.ScrollableHeight) scrollValueSprintBacklog = (float)SprintBacklogScroll.ScrollableHeight;
                    SprintBacklogScroll.ScrollToVerticalOffset(scrollValueSprintBacklog);
                    break;
                default:
                    scrollValue += 10.0f;
                    if (scrollValue > BacklogScroll.ScrollableHeight) scrollValue = (float)BacklogScroll.ScrollableHeight;
                    BacklogScroll.ScrollToVerticalOffset(scrollValue);
                    break;
            }
           
        }

        private void ScrollUp_Start(object sender, MouseEventArgs e)
        {
            currentBacklog = BacklogSelected.Backlog;
            this.countdownTimerDelayScrollUp.Start();
            this.countdownTimerScrollUp.Stop();
        }

        private void ScrollDown_Start(object sender, MouseEventArgs e)
        {
            currentBacklog = BacklogSelected.Backlog;
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

        private void SprintBacklogScrollUp_Start(object sender, MouseEventArgs e)
        {
            currentBacklog = BacklogSelected.SprintBacklog;
            this.countdownTimerDelayScrollUp.Start();
            this.countdownTimerScrollUp.Stop();
        }

        private void SprintBacklogScrollDown_Start(object sender, MouseEventArgs e)
        {
            currentBacklog = BacklogSelected.SprintBacklog;
            this.countdownTimerDelayScrollDown.Start();
            this.countdownTimerScrollDown.Stop();
        }

        public PageChange PageChangeTarget(PageChangeDirection direction)
        {
            switch (direction)
            {
                case PageChangeDirection.Down:
                    return null;
                case PageChangeDirection.Left:
                    return new PageChange { Context = null, Page = ApplicationPages.MainPage };
                case PageChangeDirection.Right:
                    return new PageChange { Context = null, Page = ApplicationPages.TaskBoardPage };
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
            directions[PageChangeDirection.Left] = "MENU INICIAL";
            directions[PageChangeDirection.Right] = "TASKBOARD";
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
                    this.PopulateProjectBacklog();
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
    }
}
