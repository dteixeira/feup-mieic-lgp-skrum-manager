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
using TaskBoardControlLib;

namespace TaskboardRowLib
{
    /// <summary>
    /// Interaction logic for TaskboardRowControl.xaml
    /// </summary>
    public partial class TaskboardRowControl : UserControl
    {
        /* Static members and static constructor */

        public static Dictionary<int, Dictionary<TasksState, ObservableCollection<TaskControl>>> AllTasks { get; set; }

        static TaskboardRowControl()
        {
            TaskboardRowControl.AllTasks = new Dictionary<int, Dictionary<TasksState, ObservableCollection<TaskControl>>>();
        }

        /* Instance members and methods */

        public ObservableCollection<TaskControl> Tasks { get; set; }
        public TasksState State { get; set; }

        public TaskboardRowControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        public static void CreateLine(int USID)
        {
            TaskboardRowControl.AllTasks[USID] = new Dictionary<TasksState, ObservableCollection<TaskControl>>();
            TaskboardRowControl.AllTasks[USID][TasksState.Todo] = new ObservableCollection<TaskControl>();
            TaskboardRowControl.AllTasks[USID][TasksState.Doing] = new ObservableCollection<TaskControl>();
            TaskboardRowControl.AllTasks[USID][TasksState.Done] = new ObservableCollection<TaskControl>();
        }

        protected override void OnDrop(DragEventArgs e)
        {
            try
            {
                var dataObj = e.Data as DataObject;
                TaskControl dragged = dataObj.GetData("TaskControl") as TaskControl;

                if (this.State != dragged.State)
                {
                    // Launch thread to update the project.
                    System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.UpdateTaskInService));
                    thread.Start(new object[]{ this.State, dragged.Task });

                    // Update visualization.
                    TaskboardRowControl.AllTasks[dragged.USID][dragged.State].Remove(dragged);
                    dragged.State = this.State;
                    TaskboardRowControl.AllTasks[dragged.USID][this.State].Add(dragged);
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        public void UpdateTaskInService(object info)
        {
            try
            {
                object[] param = info as object[];
                TasksState state = (TasksState)param[0];
                ServiceLib.DataService.Task task = (ServiceLib.DataService.Task)param[1];

                // Change the task state.
                switch (state)
                {
                    case TasksState.Done:
                        task.State = ServiceLib.DataService.TaskState.Completed;
                        break;
                    case TasksState.Doing:
                        task.State = ServiceLib.DataService.TaskState.InProgress;
                        break;
                    case TasksState.Todo:
                        task.State = ServiceLib.DataService.TaskState.Waiting;
                        break;
                    default:
                        // Nothing to change.
                        return;
                }

                // Update the task and force a taskboard update if
                // update fails.
                ServiceLib.DataService.DataServiceClient client = new ServiceLib.DataService.DataServiceClient();
                SharedTypes.ApplicationController.Instance.IgnoreNextProjectUpdate = true;
                task = client.UpdateTask(task);
                if (task == null)
                {
                    SharedTypes.ApplicationController.Instance.IgnoreNextProjectUpdate = false;
                    SharedTypes.ApplicationController.Instance.DataChanged(ServiceLib.NotificationService.NotificationType.ProjectModification);
                }
                client.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
    }
}
