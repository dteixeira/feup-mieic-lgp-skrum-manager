using ServiceLib.DataService;
using ServiceLib.NotificationService;
using SharedTypes;
using System;
using System.Collections;
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

        private ObservableCollection<StoryControl> collection = new ObservableCollection<StoryControl>();
        private ObservableCollection<StoryControl> collectionSprint = new ObservableCollection<StoryControl>();

        public ApplicationPages PageType { get; set; }
        public ApplicationController.DataModificationHandler DataChangeDelegate { get; set; }

        public ProjectBacklogPage(object context)
        {
            InitializeComponent();
            this.PageType = ApplicationPages.ProjectBacklogPage;
            this.Backlog.ItemsSource = this.collection;
            this.SprintBacklog.ItemsSource = this.collectionSprint;

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
                // Clears backlog.
                collection.Clear();
                collectionSprint.Clear();

                // Get current project if selected.
                Project project = ApplicationController.Instance.CurrentProject;
                List<Story> sprintBacklog;
                Sprint sprint = project.Sprints.FirstOrDefault(sp => sp.Closed == false);
                if (sprint != null)
                {
                    sprintBacklog = sprint.Stories;
                }
                else
                {
                    sprintBacklog = new List<Story>();
                }

                // Get all open tasks not in the current sprint and order them.
                ServiceLib.DataService.DataServiceClient client = new ServiceLib.DataService.DataServiceClient();
                List<Story> backlog = new List<Story>();
                List<Story> tempStories = client.GetAllStoriesInProject(project.ProjectID);
                client.Close();
                Story firstStory = tempStories.FirstOrDefault(s => s.PreviousStory == null);
                if (firstStory != null)
                {
                    backlog.Add(firstStory);
                    tempStories.Remove(firstStory);
                    while (tempStories.Count > 0)
                    {
                        Story tempStory = tempStories.FirstOrDefault(s => s.PreviousStory == backlog.Last().StoryID);
                        if (tempStory != null)
                        {
                            backlog.Add(tempStory);
                            tempStories.Remove(tempStory);
                        }
                    }
                }
                backlog = backlog.Where(s => s.State == StoryState.InProgress && !sprintBacklog.Select(st => st.StoryID).Contains(s.StoryID)).ToList<Story>();

                // Add all story controls to the backlog.
                foreach (var story in backlog.Select((s, i) => new { Value = s, Index = i }))
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
                    this.collection.Add(storyControl);
                }

                if (sprint != null)
                {
                    // Add all story controls to the sprint backlog.
                    // Iterate all user stories in the sprint.
                    foreach (var story in sprintBacklog.Select((s, i) => new { Value = s, Index = i }))
                    {
                        // Create the story control.
                        StoryControl storyControl = new StoryControl
                        {
                            StoryDescription = story.Value.Description,
                            StoryName = "US" + story.Value.Number.ToString("D3"),
                            StoryPriority = story.Value.Priority.ToString()[0].ToString(),
                            StoryEstimation = story.Value.StorySprints.FirstOrDefault(s => s.SprintID == sprint.SprintID).Points.ToString(),
                            StoryNumber = story.Value.Number,
                            Story = story.Value,
                            IsDraggable = false
                        };
                        storyControl.Width = Double.NaN;
                        storyControl.Height = Double.NaN;
                        storyControl.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                        storyControl.SetValue(Grid.ColumnProperty, 0);
                        storyControl.SetValue(Grid.RowProperty, story.Index);
                        this.collectionSprint.Add(storyControl);
                    }
                    this.Sort();
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        private void Sort()
        {
            StorySorter sorter = new StorySorter();
            ListCollectionView sprintview = (ListCollectionView)CollectionViewSource.GetDefaultView(this.SprintBacklog.ItemsSource);
            sprintview.CustomSort = sorter;
            this.SprintBacklog.Items.Refresh();
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

        private void SprintBacklog_Drop(object sender, DragEventArgs e)
        {
            try
            {
                var dataObj = e.Data as DataObject;
                StoryControl dragged = dataObj.GetData("StoryControl") as StoryControl;

                // Check if it comes from sprint, if not ignore.
                if (this.collection.Contains(dragged))
                {
                    // Obtain story points estimation.
                    PopupFormControlLib.FormWindow form = new PopupFormControlLib.FormWindow();
                    PopupFormControlLib.CollectionSpinnerPage pointsPage = new PopupFormControlLib.CollectionSpinnerPage { PageName = "points", PageTitle = "Estimativa de Esforço", ValueCollection = new List<double> { 1, 2, 3, 5, 8, 13 } };
                    form.FormPages.Add(pointsPage);
                    ApplicationController.Instance.ApplicationWindow.SetWindowFade(true);
                    form.ShowDialog();
                    if (form.Success)
                    {
                        // Get story points.
                        int points = (int)((double)form["points"].PageValue);
                        StorySprint storySprint = new StorySprint
                        {
                            Points = points,
                            StoryID = dragged.Story.StoryID
                        };

                        // Get current sprint.
                        Sprint current = ApplicationController.Instance.CurrentProject.Sprints.FirstOrDefault(sp => sp.Closed == false);

                        // No open sprints, open new one.
                        if (current == null)
                        {
                            ApplicationController.Instance.ApplicationWindow.ShowNotificationMessage("Um novo Sprint foi criado.", new TimeSpan(0, 0, 3));
                            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.CreateSprint));
                            thread.Start(storySprint);
                            this.collection.Remove(dragged);
                            this.collectionSprint.Add(dragged);
                        }
                        // Has an open sprint, might be closed.
                        else
                        {
                            // Check if sprint should be closed, and if so close it,
                            // notify the user and open a new one.
                            if (current.BeginDate.AddDays(7 * ApplicationController.Instance.CurrentProject.SprintDuration).Date <= System.DateTime.Today)
                            {
                                ApplicationController.Instance.ApplicationWindow.ShowNotificationMessage("A data de conclusão do último Sprint foi ultrapassada. Um novo Sprint foi criado.", new TimeSpan(0, 0, 3));
                                storySprint.SprintID = current.SprintID;
                                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.CloseSprint));
                                thread.Start(storySprint);
                                this.collection.Remove(dragged);
                                this.collectionSprint.Add(dragged);
                            }
                            // Current sprint still active, simple add the story to it.
                            else
                            {
                                storySprint.SprintID = current.SprintID;
                                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.AddStory));
                                thread.Start(storySprint);
                                this.collection.Remove(dragged);
                                this.collectionSprint.Add(dragged);
                            }
                        }
                    }
                    ApplicationController.Instance.ApplicationWindow.SetWindowFade(false);
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        public void CreateSprint(object obj)
        {
            StorySprint storySprint = (StorySprint)obj;
            DataServiceClient client = new DataServiceClient();
            Project project = ApplicationController.Instance.CurrentProject;
            int number = 1;
            if (project.Sprints.Count > 0)
            {
                number = project.Sprints.Max(sp => sp.Number) + 1;
            }

            // Create a new sprint.
            Sprint sprint = new Sprint 
            {
                BeginDate = System.DateTime.Today,
                Closed = false,
                ProjectID = ApplicationController.Instance.CurrentProject.ProjectID,
                Number = number
            };
            ApplicationController.Instance.IgnoreNextProjectUpdate = true;
            sprint = client.CreateSprint(sprint);
            
            // Force refresh if sprint creation failed.
            if (sprint == null)
            {
                ApplicationController.Instance.IgnoreNextProjectUpdate = false;
                ApplicationController.Instance.DataChanged(NotificationType.ProjectModification);
            }
            else
            {
                storySprint.SprintID = sprint.SprintID;
                ApplicationController.Instance.IgnoreNextProjectUpdate = false;
                client.AddStoryInSprint(storySprint);
            }
            client.Close();
        }

        public void CloseSprint(object obj)
        {
            StorySprint storySprint = (StorySprint)obj;
            DataServiceClient client = new DataServiceClient();
            Project project = ApplicationController.Instance.CurrentProject;
            Sprint currentSprint = project.Sprints.FirstOrDefault(sp => sp.Closed == false);
            currentSprint.Closed = true;
            currentSprint.EndDate = System.DateTime.Today;
            ApplicationController.Instance.IgnoreNextProjectUpdate = true;
            client.UpdateSprint(currentSprint);
            client.Close();
            this.CreateSprint(obj);
        }

        public void AddStory(object obj)
        {
            StorySprint storySprint = (StorySprint)obj;
            DataServiceClient client = new DataServiceClient();
            client.AddStoryInSprint(storySprint);
            client.Close();
        }
    }
}
