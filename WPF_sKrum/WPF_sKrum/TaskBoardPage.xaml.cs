using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using TaskboardRowLib;
using TaskLib;
using UserStoryLib;

namespace WPF_sKrum
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TaskBoardPage : UserControl
    {
        private float scroll_value = 0.0f;
        private DispatcherTimer CountdownTimer;

        private ApplicationController backdata;

        public TaskBoardPage()
        {
            InitializeComponent();
            this.backdata = ApplicationController.Instance;
            this.backdata.CurrentPage = this.GetType().Name;

            this.PopulateTaskboard();

            this.CountdownTimer = new DispatcherTimer();
            this.CountdownTimer.Tick += new EventHandler(ScrollAction);
            this.CountdownTimer.Interval = TimeSpan.FromSeconds(0.01);
            this.CountdownTimer.Start();
        }

        public void PopulateTaskboard()
        {
            int number_us = 10;
            Random random = new Random();

            for (int i = 0; i < number_us; i++)
            {
                RowDefinition c = new RowDefinition();
                c.Height = GridLength.Auto;
                Taskboard.RowDefinitions.Add(c);

                UserStoryControl us = new UserStoryControl();
                us.Nome = "US" + i.ToString();
                us.Width = Double.NaN;
                us.Height = Double.NaN;
                us.SetValue(Grid.RowProperty, i);
                us.SetValue(Grid.ColumnProperty, 0);
                us.VerticalAlignment = VerticalAlignment.Top;

                Taskboard.Children.Add(us);

                //TODO meter TaskboardRowControl num controlo a parte

                TaskboardRowControl listtasks = new TaskboardRowControl();
                listtasks.Name = "TaskDropTarget";
                listtasks.Width = Double.NaN;
                listtasks.Height = Double.NaN;
                listtasks.SetValue(Grid.RowProperty, i);
                listtasks.SetValue(Grid.ColumnProperty, 1);
                listtasks.State = TasksState.TODO;

                TaskboardRowControl listtasksstate2 = new TaskboardRowControl();
                listtasksstate2.Name = "TaskDropTarget";
                listtasksstate2.Width = Double.NaN;
                listtasksstate2.Height = Double.NaN;
                listtasksstate2.SetValue(Grid.RowProperty, i);
                listtasksstate2.SetValue(Grid.ColumnProperty, 2);
                listtasksstate2.State = TasksState.DOING;

                TaskboardRowControl listtasksstate3 = new TaskboardRowControl();
                listtasksstate3.Name = "TaskDropTarget";
                listtasksstate3.Width = Double.NaN;
                listtasksstate3.Height = Double.NaN;
                listtasksstate3.SetValue(Grid.RowProperty, i);
                listtasksstate3.SetValue(Grid.ColumnProperty, 3);
                listtasksstate3.State = TasksState.DONE;

                TaskboardRowControl.tasks[i] = new Dictionary<TasksState, ObservableCollection<TaskControl>>();
                TaskboardRowControl.tasks[i][TasksState.TODO] = new ObservableCollection<TaskControl>();
                TaskboardRowControl.tasks[i][TasksState.DOING] = new ObservableCollection<TaskControl>();
                TaskboardRowControl.tasks[i][TasksState.DONE] = new ObservableCollection<TaskControl>();

                int tasks_siz = random.Next(0, 7);

                for (int i2 = 0; i2 < tasks_siz; i2++)
                {
                    TaskControl us2 = new TaskControl();
                    us2.USID = i;
                    us2.Nome = i.ToString() + "Task";
                    us2.Width = Double.NaN;
                    us2.Height = Double.NaN;
                    us2.State = TasksState.TODO;
                    TaskboardRowControl.tasks[i][TasksState.TODO].Add(us2);
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
                    TaskboardRowControl.tasks[i][TasksState.DOING].Add(us2);
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
                    TaskboardRowControl.tasks[i][TasksState.DONE].Add(us2);
                }

                Taskboard.Children.Add(listtasks);
                Taskboard.Children.Add(listtasksstate2);
                Taskboard.Children.Add(listtasksstate3);
                listtasks.ItemsSource = TaskboardRowControl.tasks[i][TasksState.TODO];
                listtasksstate2.ItemsSource = TaskboardRowControl.tasks[i][TasksState.DOING];
                listtasksstate3.ItemsSource = TaskboardRowControl.tasks[i][TasksState.DONE];
            }
        }

        private void ScrollAction(object sender, EventArgs e)
        {
            Point mouse_pos = Mouse.GetPosition(null);
            if (mouse_pos.X == 0 && mouse_pos.Y == 0) return;
            if (mouse_pos.Y < 80)
            {
                scroll_value -= 10.0f;
                if (scroll_value < 0.0f) scroll_value = 0.0f;
            }
            else if (mouse_pos.Y > this.RenderSize.Height - 80)
            {
                scroll_value += 10.0f;
                if (scroll_value > TaskboardScroll.ScrollableHeight) scroll_value = (float)TaskboardScroll.ScrollableHeight;
            }
            TaskboardScroll.ScrollToVerticalOffset(scroll_value);
        }
    }
}