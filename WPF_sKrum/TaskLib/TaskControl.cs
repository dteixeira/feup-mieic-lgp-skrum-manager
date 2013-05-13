using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

public enum TasksState { TODO, DOING, TESTING, DONE };

namespace TaskLib
{
    public class TaskControl : Button
    {
        private Point startpoint;
        private bool started_drag = false;
        private TaskControl _adorner;
        private string AdornerLayer = "dragdropadornerLayer";
        private Canvas _adornerLayer;

        #region DependencyPropertys...

        public static readonly DependencyProperty USIDProperty =
            DependencyProperty.Register("USID", typeof(int), typeof(TaskControl));

        public static readonly DependencyProperty NomeProperty =
            DependencyProperty.Register("Nome", typeof(string), typeof(TaskControl));

        public static readonly DependencyProperty ResponsavelProperty =
           DependencyProperty.Register("Responsavel", typeof(string), typeof(TaskControl));

        public static readonly DependencyProperty EstimativaProperty =
            DependencyProperty.Register("Estimativa", typeof(int), typeof(TaskControl));

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(TasksState), typeof(TaskControl));

        #endregion DependencyPropertys...

        #region Properties...

        public int USID
        {
            get { return (int)GetValue(USIDProperty); }
            set { SetValue(USIDProperty, value); }
        }

        public string Nome
        {
            get { return (string)GetValue(NomeProperty); }
            set { SetValue(NomeProperty, value); }
        }

        public string Responsavel
        {
            get { return (string)GetValue(ResponsavelProperty); }
            set { SetValue(ResponsavelProperty, value); }
        }

        public int Estimativa
        {
            get { return (int)GetValue(EstimativaProperty); }
            set { SetValue(EstimativaProperty, value); }
        }

        public TasksState State
        {
            get { return (TasksState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        #endregion Properties...

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            try
            {
                startpoint = e.GetPosition(null);
                started_drag = true;

                Visual visual = e.OriginalSource as Visual;
                Window _topWindow = (Window)FindAncestor(typeof(Window), visual);
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

                _adorner = this.Clone();
                _adorner.Opacity = 0.6;

                _adornerLayer.Visibility = Visibility.Visible;
                _adornerLayer.Children.Add(_adorner);

                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
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

            Point mousePos = GetMousePositionWin32();

            Canvas.SetLeft(_adorner, mousePos.X);
            Canvas.SetTop(_adorner, mousePos.Y);

            e.Handled = true;
        }

        static TaskControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TaskControl), new FrameworkPropertyMetadata(typeof(TaskControl)));
        }

        public TaskControl Clone()
        {
            TaskControl p = new TaskControl();
            p.Nome = this.Nome;
            p.Estimativa = this.Estimativa;
            p.USID = this.USID;
            p.Responsavel = this.Responsavel;
            p.State = this.State;
            return p;
        }

        #region Utilities

        public static FrameworkElement FindAncestor(Type ancestorType, Visual visual)
        {
            while (visual != null && !ancestorType.IsInstanceOfType(visual))
            {
                visual = (Visual)VisualTreeHelper.GetParent(visual);
            }
            return visual as FrameworkElement;
        }

        public static bool IsMovementBigEnough(Point initialMousePosition, Point currentPosition)
        {
            return (Math.Abs(currentPosition.X - initialMousePosition.X) >= SystemParameters.MinimumHorizontalDragDistance ||
                 Math.Abs(currentPosition.Y - initialMousePosition.Y) >= SystemParameters.MinimumVerticalDragDistance);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        public static Point GetMousePositionWin32()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        #endregion Utilities
    }
}