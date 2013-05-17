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
using TaskLib;

namespace TaskboardRowLib
{
    /// <summary>
    /// Interaction logic for TaskboardRowControl.xaml
    /// </summary>
    public partial class TaskboardRowControl : UserControl
    {
        private ObservableCollection<TaskControl> tasks;
        // first element is UserStory.ID , then the tasks state give the observable collection
        public static Dictionary<int, Dictionary<TasksState, ObservableCollection<TaskControl>>> all_static_tasks = new Dictionary<int, Dictionary<TasksState, ObservableCollection<TaskControl>>>();



        public TaskboardRowControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        public static void CreateLine(int USID)
        {
            TaskboardRowControl.all_static_tasks[USID] = new Dictionary<TasksState, ObservableCollection<TaskControl>>();
            TaskboardRowControl.all_static_tasks[USID][TasksState.TODO] = new ObservableCollection<TaskControl>();
            TaskboardRowControl.all_static_tasks[USID][TasksState.DOING] = new ObservableCollection<TaskControl>();
            TaskboardRowControl.all_static_tasks[USID][TasksState.DONE] = new ObservableCollection<TaskControl>();
        }

        public ObservableCollection<TaskControl> Tasks
        {
            get { return this.tasks; }
            set { this.tasks = value; }
        }

        public TasksState State { get; set; }

        protected override void OnDragEnter(DragEventArgs e)
        {
            //this.Background = Brushes.WhiteSmoke;
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            //this.Background = Brushes.White;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            //TODO check type of incoming object
            var dataObj = e.Data as DataObject;
            TaskControl dragged = dataObj.GetData("TaskControl") as TaskControl;
            //this.Background = Brushes.White;

            if (this.State != dragged.State)
            {
                all_static_tasks[dragged.USID][dragged.State].Remove(dragged);
                dragged.State = this.State;

                //dragged.State = destiny.State;
                all_static_tasks[dragged.USID][this.State].Add(dragged);
            }
            e.Handled = true;
        }
    }
}
