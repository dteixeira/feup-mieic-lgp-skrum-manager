﻿using GenericControlLib;
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
            // Get current project if selected.
            Project project = ApplicationController.Instance.CurrentProject;

            // Get current sprint.
            Sprint sprint = project.Sprints.FirstOrDefault(s => s.Closed == false);

            int closedStories = 0;
            int openedStories = 0;
            int closedTasks = 0;
            int openedTasks = 0;
            int estimatedWork = 0;
            double doneWork = 0;
            

            int sprintdur = project.SprintDuration * 7;
            List<int> graphicRawData = new List<int>(sprintdur);
            for (int i = 0; i < sprintdur; i++)
            {
                graphicRawData.Add(0);
            }

            // Iterate all user stories in the sprint.
            foreach (var story in sprint.Stories)
            {
                if (story.State == StoryState.Completed)
                    closedStories++;
                else if (story.State == StoryState.Abandoned)
                    closedStories++;
                else
                    openedStories++;
                foreach(var task in story.Tasks)
                {
                    if (task.State == TaskState.Completed)
                    {
                        closedTasks++;
                        try
                        {
                            DateTime lastworkedtime = task.PersonTasks.Max(pt => pt.CreationDate);
                            TimeSpan diftime = lastworkedtime - sprint.BeginDate;
                            int key = diftime.Days;
                            graphicRawData[key] = graphicRawData[key] + task.Estimation;
                        }
                        catch (Exception){}
                    }
                    else
                        openedTasks++;
                    estimatedWork += task.Estimation;
                    doneWork += task.PersonTasks.Sum(pt=>pt.SpentTime);
                }
            }

            this.WorkExecuted.Done = (int)doneWork;
            this.WorkExecuted.Expected = estimatedWork;

            this.UserStoriesExecuted.Done = closedStories;
            this.UserStoriesExecuted.Todo = openedStories;

            this.TasksExecuted.Done = closedTasks;
            this.TasksExecuted.Todo = openedTasks;

            List<KeyValuePair<string, int>> previsiondata = new List<KeyValuePair<string, int>>();
            List<KeyValuePair<string, int>> graphicdata = new List<KeyValuePair<string, int>>();
            previsiondata.Add(new KeyValuePair<string, int>("0", estimatedWork));
            graphicdata.Add(new KeyValuePair<string, int>("0", estimatedWork));



            TimeSpan biggestDifTime = System.DateTime.Now - sprint.BeginDate;
            int biggestkey = biggestDifTime.Days;

            for (int i = 1; i < sprintdur; i++)
            {
                previsiondata.Add(new KeyValuePair<string, int>(i.ToString(), (estimatedWork/sprintdur)*(sprintdur-i)));
                if(i<= biggestkey)
                    graphicdata.Add(new KeyValuePair<string, int>(i.ToString(), (graphicdata[i - 1].Value - graphicRawData[i])));
            }

            List<List<KeyValuePair<string,int>>> data = new List<List<KeyValuePair<string,int>>> ();
            List<string> names = new List<string>();
            names.Add("Previsão");
            data.Add(previsiondata);
            names.Add("Real");
            data.Add(graphicdata);
            GraphicControl graphic = new GraphicControl(data,names);
            graphic.SetValue(Grid.RowProperty, 1);
            graphic.Margin = new Thickness(50);
            this.LeftArea.Children.Add(graphic);
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
            directions[PageChangeDirection.Down] = "TASKBOARD";
            directions[PageChangeDirection.Left] = "MENU INICIAL";
            directions[PageChangeDirection.Right] = null;
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
