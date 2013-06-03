using GenericControlLib;
using ServiceLib.DataService;
using ServiceLib.NotificationService;
using SharedTypes;
using System;
using System.Collections.Generic;
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

namespace ProjectStatisticsPageLib
{
    /// <summary>
    /// Interaction logic for ProjectStatisticsPage.xaml
    /// </summary>
    public partial class PersonalStatisticsPage : UserControl, ITargetPage
    {
        public ApplicationPages PageType { get; set; }
        private Person CurrentPerson { get; set; }
        public ApplicationController.DataModificationHandler DataChangeDelegate { get; set; }

        public PersonalStatisticsPage(object context)
        {
            InitializeComponent();
            this.PageType = ApplicationPages.PersonStatisticsPage;
            this.CurrentPerson = (Person)context;
            UserButtonControl user = new UserButtonControl
            {
                UserName = this.CurrentPerson.Name,
                UserPhoto = this.CurrentPerson.PhotoURL
            };
            user.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            user.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            this.PersonGrid.Children.Add(user);

            // Register for project change notifications.
            this.DataChangeDelegate = new ApplicationController.DataModificationHandler(this.DataChangeHandler);
            ApplicationController.Instance.DataChangedEvent += this.DataChangeDelegate;

            PopulatePersonalStatisticsPage();
        }

        private void PopulatePersonalStatisticsPage()
        {
            // Get current project if selected.
            Project project = ApplicationController.Instance.CurrentProject;

            // Get current sprint.
            Sprint sprint = project.Sprints.FirstOrDefault(s => s.Closed == false);

            List<int> graphicRawData = new List<int>(project.SprintDuration * 7);
            List<int> storyIDs = sprint.Stories.Select(st => st.StoryID).ToList<int>();
            List<Task> tasks = this.CurrentPerson.Tasks.Where(t => storyIDs.Contains(t.StoryID)).ToList<Task>();
            List<KeyValuePair<string, double>> graphicdata = new List<KeyValuePair<string, double>>();
            double estimatedWork = tasks.Sum(t => t.Estimation);
            double totalWork = 0;
            for (int i = 1; i <= project.SprintDuration * 7; ++i)
            {
                DateTime day = sprint.BeginDate.Date.AddDays(i - 1);
                double workDone = (
                    from t in tasks
                    from pt in t.PersonTasks
                    where pt.CreationDate.Date == day.Date
                    select pt.SpentTime).Sum();
                totalWork += workDone;
                graphicdata.Add(new KeyValuePair<string, double>(i.ToString(), workDone));
            }

            // Completed tasks statistics.
            this.TasksExecuted.Total = tasks.Count();
            this.TasksExecuted.Done = tasks.Count(t => t.State == TaskState.Completed);

            // Completed stories statistics.
            var involvedStories = sprint.Stories.Where(s => tasks.Select(t => t.StoryID).Contains(s.StoryID));
            this.UserStoriesExecuted.Total = involvedStories.Count();
            this.UserStoriesExecuted.Done = involvedStories.Count(s => s.State == StoryState.Completed || s.State == StoryState.Abandoned);

            // Worked statistics.
            this.WorkExecuted.Done = totalWork;
            this.WorkExecuted.Expected = estimatedWork;

            GraphicColumnControl graphic = new GraphicColumnControl(graphicdata);
            graphic.SetValue(Grid.RowProperty, 1);
            graphic.Margin = new Thickness(0, 0, 0, 0);
            graphic.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            graphic.Height = double.NaN;
            this.LeftArea.Children.Add(graphic);
        }

        public PageChange PageChangeTarget(PageChangeDirection direction)
        {
            int index = ApplicationController.Instance.Team.FindIndex(p => p.PersonID == this.CurrentPerson.PersonID);

            // User was deleted, return to project main page.
            if (index < 0)
            {
                return new PageChange { Context = null, Page = ApplicationPages.MainPage };
            }
            else
            {


                switch (direction)
                {
                    case PageChangeDirection.Down:
                        return new PageChange { Context = this.CurrentPerson, Page = ApplicationPages.PersonTaskBoardPage};
                    case PageChangeDirection.Left:
                        // First person of the list.
                        if (index == 0)
                        {
                            return new PageChange { Context = null, Page = ApplicationPages.TaskBoardPage };
                        }
                        else
                        {
                            return new PageChange { Context = ApplicationController.Instance.Team[index - 1], Page = ApplicationPages.PersonTaskBoardPage };
                        }
                    case PageChangeDirection.Right:
                        // Last person of the list.
                        if (index == ApplicationController.Instance.Team.Count - 1)
                        {
                            return new PageChange { Context = null, Page = ApplicationPages.ProjectStatisticsPage };
                        }
                        else
                        {
                            return new PageChange { Context = ApplicationController.Instance.Team[index + 1], Page = ApplicationPages.PersonTaskBoardPage };
                        }
                    case PageChangeDirection.Up:
                        return null;
                    default:
                        return null;
                }
            }
        }

        public void SetupNavigation()
        {
            int index = ApplicationController.Instance.Team.FindIndex(p => p.PersonID == this.CurrentPerson.PersonID);
            System.Collections.Generic.Dictionary<PageChangeDirection, string> directions = new System.Collections.Generic.Dictionary<PageChangeDirection, string>();
            directions[PageChangeDirection.Up] = null;
            directions[PageChangeDirection.Down] = "TASKBOARD PESSOAL";

            // Variable left destination.
            if (index == 0)
            {
                directions[PageChangeDirection.Left] = "TASKBOARD";
            }
            else
            {
                directions[PageChangeDirection.Left] = "TASKBOARD PESSOAL";
            }

            // Variable right destination.
            if (index == ApplicationController.Instance.Team.Count - 1)
            {
                directions[PageChangeDirection.Right] = "ESTATÍSTICAS";
            }
            else
            {
                directions[PageChangeDirection.Right] = "TASKBOARD PESSOAL";
            }
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
                    this.PopulatePersonalStatisticsPage();
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

    }
}
