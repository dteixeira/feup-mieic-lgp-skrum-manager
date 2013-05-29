using GenericControlLib;
using ServiceLib.DataService;
using ServiceLib.NotificationService;
using SharedTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace BacklogPageLib
{
    /// <summary>
    /// Interaction logic for BacklogPage.xaml
    /// </summary>
    public partial class BacklogPage : UserControl, ITargetPage
    {
        private float scrollValue = 0.0f;
        private float scrollValueSprints = 0.0f;
        
        private DispatcherTimer countdownTimerDelayScrollUp;
        private DispatcherTimer countdownTimerDelayScrollDown;
        private DispatcherTimer countdownTimerScrollUp;
        private DispatcherTimer countdownTimerScrollDown;
        private enum BacklogSelected { Backlog, Sprint };
        private BacklogSelected currentBacklog;

        private ObservableCollection<StoryControl> collection = new ObservableCollection<StoryControl>();
        private ObservableCollection<SprintControl> collectionSprint = new ObservableCollection<SprintControl>();

        
        public ApplicationPages PageType { get; set; }
        public ApplicationController.DataModificationHandler DataChangeDelegate { get; set; }

        public BacklogPage(object context)
        {
            InitializeComponent();
            this.PageType = ApplicationPages.BacklogPage;

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

            PopulateBacklog();
        }

        private void PopulateBacklog()
        {
            try
            {
                // Clears backlog.
                this.Backlog.Items.Clear();
                collection.Clear();

                // Get current project if selected.
                Project project = ApplicationController.Instance.CurrentProject;

                ServiceLib.DataService.DataServiceClient connection = new ServiceLib.DataService.DataServiceClient();
                List<Story> stories = connection.GetAllStoriesInProject(project.ProjectID);
                List<Sprint> sprints = connection.GetAllSprintsInProject(project.ProjectID);
                connection.Close();

                // Iterate all user stories in the project.
                foreach (var story in  stories.Select((s, i) => new { Value = s, Index = i }))
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
                    
                    collection.Add(storyControl);
                }
                this.Backlog.ItemsSource = collection;

                for(int qwe=0;qwe<10;qwe++)
                {
                // Iterate all sprints in the project.
                foreach (var sprint in sprints.Select((s, i) => new { Value = s, Index = i }))
                {
                    // Create the sprint control.
                    SprintControl sprintControl = new SprintControl()
                    {
                        SprintBeginDate = sprint.Value.BeginDate,
                        SprintEndDate = sprint.Value.EndDate,
                        SprintNumber = sprint.Value.Number
                    };

                    sprintControl.Width = Double.NaN;
                    sprintControl.Height = Double.NaN;
                    sprintControl.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    
                    collectionSprint.Add(sprintControl);
                }
                }
                this.Sprints.ItemsSource = collectionSprint;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
            this.Sort();
        }

        private void Sort()
        {
            ListCollectionView view = (ListCollectionView)CollectionViewSource.GetDefaultView(this.Backlog.ItemsSource);
            StorySorter sorter = new StorySorter();
            view.CustomSort = sorter;
            this.Backlog.Items.Refresh();
        }

        public class StorySorter : IComparer
        {
            public int Compare(object x, object y)
            {
                if (((StoryControl)x).StoryNumber > ((StoryControl)y).StoryNumber)
                    return 1;
                if (((StoryControl)x).StoryNumber < ((StoryControl)y).StoryNumber)
                    return -1;
                else
                    return 0;
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
                case BacklogSelected.Sprint:
                    scrollValueSprints -= 10.0f;
                    if (scrollValueSprints < 0.0f) scrollValueSprints = 0.0f;
                    SprintsScroll.ScrollToVerticalOffset(scrollValueSprints);
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
                case BacklogSelected.Sprint:
                    scrollValueSprints += 10.0f;
                    if (scrollValueSprints > SprintsScroll.ScrollableHeight) scrollValueSprints = (float)SprintsScroll.ScrollableHeight;
                    SprintsScroll.ScrollToVerticalOffset(scrollValueSprints);
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

        private void SprintScrollUp_Start(object sender, MouseEventArgs e)
        {
            currentBacklog = BacklogSelected.Sprint;
            this.countdownTimerDelayScrollUp.Start();
            this.countdownTimerScrollUp.Stop();
        }

        private void SprintScrollDown_Start(object sender, MouseEventArgs e)
        {
            currentBacklog = BacklogSelected.Sprint;
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
                    return new PageChange { Context = null, Page = ApplicationPages.ProjectBacklogPage };
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
            directions[PageChangeDirection.Right] = "SPRINT BACKLOG";
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
                    this.PopulateBacklog();
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
    }
}
