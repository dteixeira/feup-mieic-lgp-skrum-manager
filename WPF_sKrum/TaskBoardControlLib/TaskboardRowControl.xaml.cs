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
            //TODO check type of incoming object
            var dataObj = e.Data as DataObject;
            TaskControl dragged = dataObj.GetData("TaskControl") as TaskControl;
            //this.Background = Brushes.White;

            if (this.State != dragged.State)
            {
                TaskboardRowControl.AllTasks[dragged.USID][dragged.State].Remove(dragged);
                dragged.State = this.State;

                //dragged.State = destiny.State;
                TaskboardRowControl.AllTasks[dragged.USID][this.State].Add(dragged);
            }
            e.Handled = true;
        }
    }
}
