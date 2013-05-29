using GenericControlLib;
using ServiceLib.DataService;
using ServiceLib.NotificationService;
using SharedTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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
                collection.Clear();
                collectionSprint.Clear();

                // Get current project if selected.
                Project project = ApplicationController.Instance.CurrentProject;
                List<Sprint> sprints = project.Sprints;

                // Order stories.
                ServiceLib.DataService.DataServiceClient client = new ServiceLib.DataService.DataServiceClient();
                List<Story> stories = new List<Story>();
                List<Story> tempStories = client.GetAllStoriesInProject(project.ProjectID);
                client.Close();
                Story firstStory = tempStories.FirstOrDefault(s => s.PreviousStory == null);
                if (firstStory != null)
                {
                    stories.Add(firstStory);
                    tempStories.Remove(firstStory);
                    while (tempStories.Count > 0)
                    {
                        Story tempStory = tempStories.FirstOrDefault(s => s.PreviousStory == stories.Last().StoryID);
                        if (tempStory != null)
                        {
                            stories.Add(tempStory);
                            tempStories.Remove(tempStory);
                        }
                    }
                }
                stories = stories.Where(s => s.State == StoryState.InProgress).ToList<Story>();

                // Iterate all user stories in the project.
                foreach (var story in stories.Select((s, i) => new { Value = s, Index = i }))
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

                // Iterate all sprints in the project.
                foreach (var sprint in sprints.Select((s, i) => new { Value = s, Index = i }))
                {
                    // Create the sprint control.
                    int total = sprint.Value.Stories.Count;
                    int done = sprint.Value.Stories.Count(st => st.State == StoryState.Completed);
                    string ratio = string.Format("{0} / {1}", done, total);
                    SprintControl sprintControl = new SprintControl()
                    {
                        SprintBeginDate = sprint.Value.BeginDate,
                        SprintEndDate = sprint.Value.EndDate,
                        SprintNumber = sprint.Value.Number,
                        StoryRatio = ratio
                    };

                    sprintControl.Width = Double.NaN;
                    sprintControl.Height = Double.NaN;
                    sprintControl.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    collectionSprint.Add(sprintControl);
                }
                this.Sprints.ItemsSource = collectionSprint;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
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

        public void AddStory_Click(object sender, MouseEventArgs e)
        {
            PopupFormControlLib.FormWindow form = new PopupFormControlLib.FormWindow();
            PopupFormControlLib.TextAreaPage descriptionPage = new PopupFormControlLib.TextAreaPage { PageName = "description", PageTitle = "Descrição" };
            PopupFormControlLib.StoryPriorityPage priorityPage = new PopupFormControlLib.StoryPriorityPage { PageName = "priority", PageTitle = "Prioridade" };
            form.FormPages.Add(descriptionPage);
            form.FormPages.Add(priorityPage);
            ApplicationController.Instance.ApplicationWindow.SetWindowFade(true);
            form.ShowDialog();
            if (form.Success)
            {
                string description = (string)form["description"].PageValue;
                StoryPriority priority = (StoryPriority)form["priority"].PageValue;
                if (description != null && description != "")
                {
                    Story story = new Story
                    {
                        CreationDate = System.DateTime.Now,
                        Description = description,
                        Priority = priority,
                        State = StoryState.InProgress,
                        ProjectID = ApplicationController.Instance.CurrentProject.ProjectID
                    };
                    System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(AddStory));
                    thread.Start(story);
                }
            }
            ApplicationController.Instance.ApplicationWindow.SetWindowFade(false);
        }

        public void AddStory(object obj)
        {
            DataServiceClient client = new DataServiceClient();
            client.CreateStory((Story)obj);
            client.Close();
        }

        public void EditStory_Drop(object sender, DragEventArgs e)
        {
            var dataObj = e.Data as DataObject;
            TaskBoardControlLib.StoryControl storyControl = dataObj.GetData("StoryControl") as TaskBoardControlLib.StoryControl;
            if (storyControl != null)
            {
                PopupFormControlLib.FormWindow form = new PopupFormControlLib.FormWindow();
                PopupFormControlLib.TextAreaPage descriptionPage = new PopupFormControlLib.TextAreaPage { PageName = "description", PageTitle = "Descrição", DefaultValue = storyControl.Story.Description };
                PopupFormControlLib.StoryPriorityPage priorityPage = new PopupFormControlLib.StoryPriorityPage { PageName = "priority", PageTitle = "Prioridade", DefaultValue = storyControl.Story.Priority };
                form.FormPages.Add(descriptionPage);
                form.FormPages.Add(priorityPage);
                ApplicationController.Instance.ApplicationWindow.SetWindowFade(true);
                form.ShowDialog();
                if (form.Success)
                {
                    string description = (string)form["description"].PageValue;
                    StoryPriority priority = (StoryPriority)form["priority"].PageValue;
                    if (description != null && description != "")
                    {
                        Story story = storyControl.Story;
                        story.Description = description;
                        story.Priority = priority;
                        System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(EditStory));
                        thread.Start(story);
                    }
                }
                ApplicationController.Instance.ApplicationWindow.SetWindowFade(false);
            }
        }

        public void EditStory(object obj)
        {
            DataServiceClient client = new DataServiceClient();
            client.UpdateStory((Story)obj);
            client.Close();
        }

        public void DeleteStory_Drop(object sender, DragEventArgs e)
        {
            var dataObj = e.Data as DataObject;
            TaskBoardControlLib.StoryControl storyControl = dataObj.GetData("StoryControl") as TaskBoardControlLib.StoryControl;
            if (storyControl != null)
            {
                PopupFormControlLib.YesNoFormWindow form = new PopupFormControlLib.YesNoFormWindow();
                form.FormTitle = "Apagar a Story?";
                ApplicationController.Instance.ApplicationWindow.SetWindowFade(true);
                form.ShowDialog();
                if (form.Success)
                {
                    System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(DeleteStory));
                    thread.Start(storyControl.Story);
                }
                ApplicationController.Instance.ApplicationWindow.SetWindowFade(false);
            }
        }

        public void DeleteStory(object obj)
        {
            Story story = (Story)obj;
            DataServiceClient client = new DataServiceClient();
            client.DeleteStory(story.StoryID);
            client.Close();
        }

        public void CloseStory_Drop(object sender, DragEventArgs e)
        {
            var dataObj = e.Data as DataObject;
            TaskBoardControlLib.StoryControl storyControl = dataObj.GetData("StoryControl") as TaskBoardControlLib.StoryControl;
            if (storyControl != null)
            {
                PopupFormControlLib.YesNoFormWindow form = new PopupFormControlLib.YesNoFormWindow();
                form.FormTitle = "Fechar a Story?";
                ApplicationController.Instance.ApplicationWindow.SetWindowFade(true);
                form.ShowDialog();
                if (form.Success)
                {
                    Story story = storyControl.Story;
                    story.State = StoryState.Completed;
                    System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(CloseStory));
                    thread.Start(storyControl.Story);
                }
                ApplicationController.Instance.ApplicationWindow.SetWindowFade(false);
            }
        }

        public void CloseStory(object obj)
        {
            Story story = (Story)obj;
            DataServiceClient client = new DataServiceClient();
            client.DeleteStory(story.StoryID);
            client.Close();
        }
    }
}