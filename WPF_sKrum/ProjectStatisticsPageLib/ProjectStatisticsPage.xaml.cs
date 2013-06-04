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
    public partial class ProjectStatisticsPage : UserControl, ITargetPage
    {
        public ApplicationPages PageType { get; set; }
        public ApplicationController.DataModificationHandler DataChangeDelegate { get; set; }

        public ProjectStatisticsPage(object context)
        {
            InitializeComponent();
            this.PageType = ApplicationPages.ProjectStatisticsPage;

            // Register for project change notifications.
            this.DataChangeDelegate = new ApplicationController.DataModificationHandler(this.DataChangeHandler);
            ApplicationController.Instance.DataChangedEvent += this.DataChangeDelegate;

            PopulateProjectStatisticsPage();
        }

        private void PopulateProjectStatisticsPage()
        {
            try
            {
                // Get current project if selected.
                Project project = ApplicationController.Instance.CurrentProject;

                // Get current sprint.
                Sprint sprint = project.Sprints.FirstOrDefault(s => s.Closed == false);

                int sprintdur = project.SprintDuration * 7;
                List<int> graphicRawData = new List<int>(sprintdur);
                for (int i = 0; i < sprintdur; i++)
                {
                    graphicRawData.Add(0);
                }

                // Total work statistics.
                this.WorkExecuted.Done = (
                    from s in sprint.Stories
                    from t in s.Tasks
                    from pt in t.PersonTasks
                    select pt.SpentTime).Sum();
                this.WorkExecuted.Expected = (
                    from s in sprint.Stories
                    from t in s.Tasks
                    select t.Estimation).Sum();

                // User story statitics.
                this.UserStoriesExecuted.Done = sprint.Stories.Count(s => s.State == StoryState.Completed || s.State == StoryState.Abandoned);
                this.UserStoriesExecuted.Total = sprint.Stories.Count();

                // Task statistics.
                this.TasksExecuted.Done = (
                    from s in sprint.Stories
                    from t in s.Tasks
                    where t.State == TaskState.Completed
                    select t).Count();
                this.TasksExecuted.Total = (
                    from s in sprint.Stories
                    from t in s.Tasks
                    select t).Count();

                // Create the burndown chart.
                List<KeyValuePair<string, double>> previsiondata = new List<KeyValuePair<string, double>>();
                List<KeyValuePair<string, double>> graphicdata = new List<KeyValuePair<string, double>>();
                previsiondata.Add(new KeyValuePair<string, double>("0", this.WorkExecuted.Expected));
                graphicdata.Add(new KeyValuePair<string, double>("0", this.WorkExecuted.Expected));

                double expectedDayWork = this.WorkExecuted.Expected / sprintdur;
                for (int i = 1; i <= sprintdur; ++i)
                {
                    DateTime day = sprint.BeginDate.Date.AddDays(i - 1);
                    previsiondata.Add(new KeyValuePair<string, double>(i.ToString(), this.WorkExecuted.Expected - i * expectedDayWork));
                    if (System.DateTime.Today >= day.Date)
                    {
                        double workInDay = (
                            from s in sprint.Stories
                            from t in s.Tasks
                            from pt in t.PersonTasks
                            where pt.CreationDate.Date <= day.Date
                            select pt.SpentTime > t.Estimation ? t.Estimation : pt.SpentTime).Sum();
                        graphicdata.Add(new KeyValuePair<string, double>(i.ToString(), this.WorkExecuted.Expected - workInDay < 0 ? 0 : this.WorkExecuted.Expected - workInDay));
                    }
                }
                List<List<KeyValuePair<string, double>>> data = new List<List<KeyValuePair<string, double>>>();
                data.Add(previsiondata);
                data.Add(graphicdata);

                if (graphicdata[graphicdata.Count - 1].Value > previsiondata[graphicdata.Count - 1].Value)
                {
                    this.Global_status.ButtonText = "ATRASADO";
                }

                GraphicControl graphic = new GraphicControl(data);
                graphic.SetValue(Grid.RowProperty, 1);
                graphic.Margin = new Thickness(0, 0, 0, 0);
                this.LeftArea.Children.Add(graphic);
            }
            catch (Exception)
            {
            }
        }

        public PageChange PageChangeTarget(PageChangeDirection direction)
        {
            switch (direction)
            {
                case PageChangeDirection.Down:
                    return new PageChange { Context = null, Page = ApplicationPages.TaskBoardPage };
                case PageChangeDirection.Left:
                    return new PageChange { Context = null, Page = ApplicationPages.MainPage };
                case PageChangeDirection.Right:
                    if (ApplicationController.Instance.Team.Count > 0)
                    {
                        return new PageChange { Context = ApplicationController.Instance.Team[0], Page = ApplicationPages.PersonTaskBoardPage };
                    }
                    else
                    {
                        return null;
                    }
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
            directions[PageChangeDirection.Down] = "TASKBOARD";
            directions[PageChangeDirection.Left] = "MENU INICIAL"; 
            if (ApplicationController.Instance.Team.Count > 0)
            {
                directions[PageChangeDirection.Right] = "TASKBOARD PESSOAL";
            }
            else
            {
                directions[PageChangeDirection.Right] = null;
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
                    this.PopulateProjectStatisticsPage();
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

    }
}
