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
using TaskLib;
using UserStoryLib;

namespace WPFApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TaskBoardPage : UserControl
    {
        private float scrollValue = 0.0f;
        private DispatcherTimer CountdownTimerDelayScrollUp;
        private DispatcherTimer CountdownTimerDelayScrollDown;
        private DispatcherTimer CountdownTimerScrollUp;
        private DispatcherTimer CountdownTimerScrollDown;

        private ApplicationController backdata;

        public TaskBoardPage()
        {
            InitializeComponent();
            this.PopulateTaskboard();
            this.backdata = ApplicationController.Instance;
            this.backdata.CurrentPage = ApplicationPages.TaskBoardPage;

            this.CountdownTimerDelayScrollUp = new DispatcherTimer();
            this.CountdownTimerDelayScrollUp.Tick += new EventHandler(ScrollActionDelayUp);
            this.CountdownTimerDelayScrollUp.Interval = TimeSpan.FromSeconds(1);

            this.CountdownTimerDelayScrollDown = new DispatcherTimer();
            this.CountdownTimerDelayScrollDown.Tick += new EventHandler(ScrollActionDelayDown);
            this.CountdownTimerDelayScrollDown.Interval = TimeSpan.FromSeconds(1);

            this.CountdownTimerScrollUp = new DispatcherTimer();
            this.CountdownTimerScrollUp.Tick += new EventHandler(ScrollActionUp);
            this.CountdownTimerScrollUp.Interval = TimeSpan.FromSeconds(0.01);

            this.CountdownTimerScrollDown = new DispatcherTimer();
            this.CountdownTimerScrollDown.Tick += new EventHandler(ScrollActionDown);
            this.CountdownTimerScrollDown.Interval = TimeSpan.FromSeconds(0.01);
        }

        

        private void PopulateTaskboard()
        {
            int number_us = 10;
            Random random = new Random();
            bool line_change = true;

            Brush BackFirstGrayColor = new SolidColorBrush(Color.FromRgb(0xF7, 0xF7, 0xF7));
            Brush BackSecondGrayColor = new SolidColorBrush(Color.FromRgb(0xCD, 0xCD, 0xCD));




            for (int i = 0; i < number_us; i++)
            {
                //create line that holds UserStory info
                RowDefinition rowdef = new RowDefinition();
                rowdef.Height = GridLength.Auto;
                Taskboard.RowDefinitions.Add(rowdef);

                Grid TaskboardLine = new Grid();
                TaskboardLine.SetValue(Grid.RowProperty, i);
                TaskboardLine.SetValue(Grid.ColumnProperty, 0);

                ColumnDefinition coldef = new ColumnDefinition();
                coldef.Width = new GridLength(2, GridUnitType.Star);
                TaskboardLine.ColumnDefinitions.Add(coldef);
                coldef = new ColumnDefinition();
                coldef.Width = new GridLength(3, GridUnitType.Star);
                TaskboardLine.ColumnDefinitions.Add(coldef);
                coldef = new ColumnDefinition();
                coldef.Width = new GridLength(3, GridUnitType.Star);
                TaskboardLine.ColumnDefinitions.Add(coldef);
                coldef = new ColumnDefinition();
                coldef.Width = new GridLength(3, GridUnitType.Star);
                TaskboardLine.ColumnDefinitions.Add(coldef);
                
                line_change = !line_change;
                if (line_change)
                    TaskboardLine.Background = BackFirstGrayColor;
                else
                    TaskboardLine.Background = BackSecondGrayColor;
                

                UserStoryControl us = new UserStoryControl();
                us.Nome = "US" + i.ToString();
                us.Descricao = "asdadad adasdasd sdasd as dasd sdasda ad asds asdasd asdas dasdasd asdasd asdas dasdasdasda sda sdas dasd asdasd asd asd asd asd asd asd asd asd asd asd asda sdas d asdasd asd as dasd asd asd asd as dasd asd asdas dasd asd asdas dasd sad asd asd asas  asd asdasdsadas dasd asd asdasda sdasd asd asd asdasd asdasdasd asdas dasdsdasd asdasd asda s dasdas dasdasd asd a";
                us.Width = Double.NaN;
                us.Height = Double.NaN;
                us.SetValue(Grid.RowProperty, 0);
                us.SetValue(Grid.ColumnProperty, 0);
                us.VerticalAlignment = VerticalAlignment.Top;

                TaskboardLine.Children.Add(us);

                TaskboardRowControl listtasks = new TaskboardRowControl();
                listtasks.Name = "TaskDropTarget";
                listtasks.Width = Double.NaN;
                listtasks.Height = Double.NaN;
                listtasks.SetValue(Grid.RowProperty, 0);
                listtasks.SetValue(Grid.ColumnProperty, 1);
                listtasks.State = TasksState.TODO;

                TaskboardRowControl listtasksstate2 = new TaskboardRowControl();
                listtasksstate2.Name = "TaskDropTarget";
                listtasksstate2.Width = Double.NaN;
                listtasksstate2.Height = Double.NaN;
                listtasksstate2.SetValue(Grid.RowProperty, 0);
                listtasksstate2.SetValue(Grid.ColumnProperty, 2);
                listtasksstate2.State = TasksState.DOING;

                TaskboardRowControl listtasksstate3 = new TaskboardRowControl();
                listtasksstate3.Name = "TaskDropTarget";
                listtasksstate3.Width = Double.NaN;
                listtasksstate3.Height = Double.NaN;
                listtasksstate3.SetValue(Grid.RowProperty, 0);
                listtasksstate3.SetValue(Grid.ColumnProperty, 3);
                listtasksstate3.State = TasksState.DONE;

                TaskboardRowControl.CreateLine(i);

                int tasks_siz = random.Next(0, 7);

                for (int i2 = 0; i2 < tasks_siz; i2++)
                {
                    TaskControl us2 = new TaskControl();
                    us2.USID = i;
                    us2.Nome = i.ToString() + "Task";
                    us2.Width = Double.NaN;
                    us2.Height = Double.NaN;
                    us2.State = TasksState.TODO;
                    TaskboardRowControl.all_static_tasks[i][TasksState.TODO].Add(us2);
                }

                tasks_siz = random.Next(0, 7);
                for (int i2 = 0; i2 < tasks_siz; i2++)
                {
                    TaskControl us2 = new TaskControl();
                    us2.USID = i;
                    us2.Nome = i.ToString() + "Task" + i2.ToString();
                    us2.Width = Double.NaN;
                    us2.Height = Double.NaN;
                    us2.State = TasksState.DOING;
                    TaskboardRowControl.all_static_tasks[i][TasksState.DOING].Add(us2);
                }

                tasks_siz = random.Next(0, 7);
                for (int i2 = 0; i2 < tasks_siz; i2++)
                {
                    TaskControl us2 = new TaskControl();
                    us2.USID = i;
                    us2.Nome = i.ToString() + "Task" + i2.ToString();
                    us2.Width = Double.NaN;
                    us2.Height = Double.NaN;
                    us2.State = TasksState.DONE;
                    TaskboardRowControl.all_static_tasks[i][TasksState.DONE].Add(us2);
                }

                TaskboardLine.Children.Add(listtasks);
                TaskboardLine.Children.Add(listtasksstate2);
                TaskboardLine.Children.Add(listtasksstate3);
                listtasks.Tasks = TaskboardRowControl.all_static_tasks[i][TasksState.TODO];
                listtasksstate2.Tasks = TaskboardRowControl.all_static_tasks[i][TasksState.DOING];
                listtasksstate3.Tasks = TaskboardRowControl.all_static_tasks[i][TasksState.DONE];

                Taskboard.Children.Add(TaskboardLine);
            }
        }

        private void ScrollActionDelayUp(object sender, EventArgs e)
        {
            this.CountdownTimerScrollUp.Start();
            this.CountdownTimerDelayScrollUp.Stop();
        }
        private void ScrollActionDelayDown(object sender, EventArgs e)
        {
            this.CountdownTimerScrollDown.Start();
            this.CountdownTimerDelayScrollDown.Stop();
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
            this.CountdownTimerDelayScrollUp.Start();
            this.CountdownTimerScrollUp.Stop();
        }
        private void ScrollDown_Start(object sender, MouseEventArgs e)
        {
            this.CountdownTimerDelayScrollDown.Start();
            this.CountdownTimerScrollDown.Stop();
        }
        private void ScrollUp_Cancel(object sender, MouseEventArgs e)
        {
            this.CountdownTimerDelayScrollUp.Stop();
            this.CountdownTimerScrollUp.Stop();
        }
        private void ScrollDown_Cancel(object sender, MouseEventArgs e)
        {
            this.CountdownTimerDelayScrollDown.Stop();
            this.CountdownTimerScrollDown.Stop();
        }
        
        
            
    }
}