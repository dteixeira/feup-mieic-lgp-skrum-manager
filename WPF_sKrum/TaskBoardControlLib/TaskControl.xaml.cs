using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

public enum TasksState { Todo, Doing, Testing, Done };

namespace TaskBoardControlLib
{
    /// <summary>
    /// Interaction logic for TaskControl.xaml
    /// </summary>
    public partial class TaskControl : UserControl
    {
        private Point startpoint;
        private bool started_drag = false;
        private TaskControl _adorner;
        private string AdornerLayer = "dragdropadornerLayer";
        private Canvas _adornerLayer;
        private TextTrimming taskTextTrimming = TextTrimming.None;
        private string taskDescription;
        private TasksState state;
        private int usId;

        public delegate void StartDragHandler(TaskControl taskControl);

        public delegate void StopDragHandler(TaskControl taskControl);

        public event StartDragHandler StartDragEvent;

        public event StopDragHandler StopDragEvent;

        public TaskControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        public string TaskDescription
        {
            get { return this.taskDescription; }
            set { this.taskDescription = value; }
        }

        public TasksState State
        {
            get { return this.state; }
            set { this.state = value; }
        }

        public int USID
        {
            get { return this.usId; }
            set { this.usId = value; }
        }

        public TextTrimming TaskTextTrimming
        {
            get { return this.taskTextTrimming; }
            set { this.taskTextTrimming = value; }
        }

        public string TaskEstimationWork { get; set; }

        public ServiceLib.DataService.Task Task { get; set; }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            try
            {
                startpoint = e.GetPosition(null);
                started_drag = true;

                Visual visual = e.OriginalSource as Visual;
                Window _topWindow = (Window)SharedTypes.Utilities.FindAncestor(typeof(Window), visual);
                _adornerLayer = (Canvas)LogicalTreeHelper.FindLogicalNode(_topWindow, AdornerLayer);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception in DragDropHelper: " + exc.InnerException.ToString());
            }
            e.Handled = true;
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = startpoint - mousePos;

            if (started_drag && e.LeftButton == MouseButtonState.Pressed &&
                    (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                startpoint = mousePos;
                DataObject data = new DataObject("TaskControl", this);

                // Create a placeholder to drag.
                _adorner = this.Clone();
                _adorner.Opacity = 0.6;
                _adorner.IsHitTestVisible = false;
                _adorner.MaxWidth = 300;
                _adorner.MaxHeight = 150;
                _adorner.taskTextTrimming = TextTrimming.WordEllipsis;

                _adornerLayer.Visibility = Visibility.Visible;
                _adornerLayer.Children.Add(_adorner);

                // Drag started callbacks.
                if (this.StartDragEvent != null)
                {
                    this.StartDragEvent(this);
                }
                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);

                // Drag stopped event.
                if (this.StopDragEvent != null)
                {
                    this.StopDragEvent(this);
                }

                _adornerLayer.Children.Remove(_adorner);
                _adornerLayer.Visibility = Visibility.Collapsed;
                started_drag = false;
            }
            else if (e.LeftButton == MouseButtonState.Released)
            {
                started_drag = false;
            }
            e.Handled = true;
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            started_drag = false;
            e.Handled = true;
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);

            Point mousePos = SharedTypes.Utilities.GetMousePositionWin32();

            Canvas.SetLeft(_adorner, mousePos.X);
            Canvas.SetTop(_adorner, mousePos.Y);

            e.Handled = true;
        }

        public TaskControl Clone()
        {
            TaskControl p = new TaskControl();
            p.TaskDescription = this.TaskDescription;
            p.Width = this.Width;
            p.Height = this.Height;
            return p;
        }
    }
}