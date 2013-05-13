using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TaskLib;

namespace TaskboardRowLib
{
    public class TaskboardRowControl : ListView
    {
        public static Dictionary<int, Dictionary<TasksState, ObservableCollection<TaskControl>>> tasks = new Dictionary<int, Dictionary<TasksState, ObservableCollection<TaskControl>>>();

        public TasksState State { get; set; }

        static TaskboardRowControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TaskboardRowControl), new FrameworkPropertyMetadata(typeof(TaskboardRowControl)));
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            this.Background = Brushes.WhiteSmoke;
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            this.Background = Brushes.White;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            //TODO check type of incoming object
            var dataObj = e.Data as DataObject;
            TaskControl dragged = dataObj.GetData("TaskControl") as TaskControl;
            this.Background = Brushes.White;

            if (this.State != dragged.State)
            {
                tasks[dragged.USID][dragged.State].Remove(dragged);
                TaskControl newtask = new TaskControl();
                newtask.USID = dragged.USID;
                newtask.Nome = dragged.Nome;
                newtask.Width = dragged.Width;
                newtask.Height = dragged.Height;
                newtask.State = this.State;

                //dragged.State = destiny.State;
                tasks[dragged.USID][this.State].Add(newtask);
            }
            e.Handled = true;
        }
    }
}