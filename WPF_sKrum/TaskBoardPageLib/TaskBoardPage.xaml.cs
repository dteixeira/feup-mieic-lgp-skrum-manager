using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using TaskboardRowLib;
using TaskBoardControlLib;
using System.Linq;
using SharedTypes;
using ServiceLib.DataService;
using ServiceLib.NotificationService;

namespace TaskBoardPageLib
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TaskBoardPage : UserControl, ITargetPage
    {
        private float scrollValue = 0.0f;
        private DispatcherTimer countdownTimerDelayScrollUp;
        private DispatcherTimer countdownTimerDelayScrollDown;
        private DispatcherTimer countdownTimerScrollUp;
        private DispatcherTimer countdownTimerScrollDown;

        public ApplicationPages PageType { get; set; }
        public ApplicationController.DataModificationHandler DataChangeDelegate { get; set; }

        public TaskBoardPage(object context)
        {
            InitializeComponent();
            this.PageType = ApplicationPages.TaskBoardPage;

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

            // Populate taskboard.
            this.PopulateTaskboard();

            // Register for project change notifications.
            this.DataChangeDelegate = new ApplicationController.DataModificationHandler(this.DataChangeHandler);
            ApplicationController.Instance.DataChangedEvent += this.DataChangeDelegate;
        }
        

        private void PopulateTaskboard()
        {
            try
            {
                // Clears taskboard.
                this.Taskboard.RowDefinitions.Clear();
                this.Taskboard.Children.Clear();

                // Get current project if selected.
                Project project = ApplicationController.Instance.CurrentProject;

                // Get current sprint.
                Sprint sprint = project.Sprints.FirstOrDefault(s => s.Closed == false);
                
                // Iterate all user stories in the sprint.
                foreach (var story in sprint.Stories.Select((s, i) => new { Value = s, Index = i }))
                {
                    // Create a new grid line to old the story information.
                    RowDefinition rowdef = new RowDefinition();
                    rowdef.Height = GridLength.Auto;
                    Taskboard.RowDefinitions.Add(rowdef);

                    // Create the new line control.
                    TaskBoardRowDesignControl TaskboardLine = new TaskBoardRowDesignControl();
                    TaskboardLine.SetValue(Grid.RowProperty, story.Index);
                    TaskboardLine.SetValue(Grid.ColumnProperty, 0);
                    this.Taskboard.Children.Add(TaskboardLine);

                    // Create the story control.
                    StoryControl storyControl = new StoryControl
                    {
                        StoryDescription = story.Value.Description,
                        StoryName = "US" + story.Value.Number.ToString("D3"),
                        StoryPriority = story.Value.Priority.ToString()[0].ToString(),
                        StoryEstimation = story.Value.StorySprints.FirstOrDefault(s => s.SprintID == sprint.SprintID).Points,
                        Story = story.Value
                    };
                    storyControl.Width = Double.NaN;
                    storyControl.Height = Double.NaN;
                    storyControl.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    storyControl.SetValue(Grid.ColumnProperty, 1);
                    TaskboardLine.ControlGrid.Children.Add(storyControl);

                    // Create a row control to todo tasks.
                    TaskboardRowControl.CreateLine(story.Value.StoryID);
                    TaskboardRowControl todo = new TaskboardRowControl { Width = Double.NaN, Height = Double.NaN };
                    todo.SetValue(Grid.ColumnProperty, 3);
                    todo.Tasks = TaskboardRowControl.AllTasks[story.Value.StoryID][TasksState.Todo];

                    // Create a row control to doing tasks.
                    TaskboardRowControl doing = new TaskboardRowControl { Width = Double.NaN, Height = Double.NaN, State = TasksState.Doing };
                    doing.SetValue(Grid.ColumnProperty, 5);
                    doing.Tasks = TaskboardRowControl.AllTasks[story.Value.StoryID][TasksState.Doing];

                    // Create a row control to done tasks.
                    TaskboardRowControl done = new TaskboardRowControl { Width = Double.NaN, Height = Double.NaN, State = TasksState.Done };
                    done.SetValue(Grid.ColumnProperty, 7);
                    done.Tasks = TaskboardRowControl.AllTasks[story.Value.StoryID][TasksState.Done];

                    // Add task controls to grid line.
                    TaskboardLine.ControlGrid.Children.Add(todo);
                    TaskboardLine.ControlGrid.Children.Add(doing);
                    TaskboardLine.ControlGrid.Children.Add(done);

                    // For each task add it to the respective state lists.
                    foreach (Task task in story.Value.Tasks)
                    {
                        TaskControl taskControl = new TaskControl { USID = task.StoryID, TaskDescription = task.Description, Task = task };
                        taskControl.Width = Double.NaN;
                        taskControl.Height = Double.NaN;

                        // Calculate total work in this task.
                        double totalWork = 0.0;
                        foreach (PersonTask personTask in task.PersonTasks)
                        {
                            totalWork += personTask.SpentTime;
                        }
                        taskControl.TaskEstimationWork = string.Format("{0:0.#} / {1}", totalWork, task.Estimation);

                        switch (task.State)
                        {
                            case TaskState.Waiting:
                                taskControl.State = TasksState.Todo;
                                TaskboardRowControl.AllTasks[task.StoryID][TasksState.Todo].Add(taskControl);
                                break;
                            case TaskState.InProgress:
                                taskControl.State = TasksState.Doing;
                                TaskboardRowControl.AllTasks[task.StoryID][TasksState.Doing].Add(taskControl);
                                break;
                            case TaskState.Completed:
                                taskControl.State = TasksState.Done;
                                TaskboardRowControl.AllTasks[task.StoryID][TasksState.Done].Add(taskControl);
                                break;
                            default:
                                break;
                        }
                    }
                }
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
            TaskboardScroll.ScrollToVerticalOffset(scrollValue);
        }

        private void ScrollActionDown(object sender, EventArgs e)
        {
            scrollValue += 10.0f;
            if (scrollValue > TaskboardScroll.ScrollableHeight) scrollValue = (float)TaskboardScroll.ScrollableHeight;
            TaskboardScroll.ScrollToVerticalOffset(scrollValue);
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
                    return null;
                case PageChangeDirection.Left:
                    return new PageChange { Context = null, Page = ApplicationPages.MainPage };
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
            directions[PageChangeDirection.Up] = "ESTATÍSTICAS";
            directions[PageChangeDirection.Down] = null;
            directions[PageChangeDirection.Left] = "MENU INICIAL";
            directions[PageChangeDirection.Right] = "TASKBOARD PERSONALIZADA";
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
                    this.PopulateTaskboard();
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
    }
}