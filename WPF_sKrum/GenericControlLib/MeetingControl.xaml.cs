using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GenericControlLib
{
    /// <summary>
    /// Interaction logic for MeetingControl.xaml
    /// </summary>
    public partial class MeetingControl : UserControl
    {
        private Point startpoint;
        private bool started_drag = false;
        private MeetingControl _adorner;
        private string AdornerLayer = "dragdropadornerLayer";
        private Canvas _adornerLayer;

        private DateTime meetingDate;
        private int meetingNumber;
        private string meetingNotes;

        public delegate void myHoverDelegate(object obj, MouseEventArgs e);

        public event myHoverDelegate MeetingHoverEvent;

        public MeetingControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public DateTime MeetingDate
        {
            get { return this.meetingDate; }
            set { this.meetingDate = value; }
        }

        public int MeetingNumber
        {
            get { return this.meetingNumber; }
            set { this.meetingNumber = value; }
        }

        public string MeetingNotes
        {
            get { return this.meetingNotes; }
            set { this.meetingNotes = value; }
        }

        public string MeetingDateText
        {
            get { return this.meetingDate.ToShortDateString(); }
        }

        public string MeetingName
        {
            get { return "REUNIÃO " + this.MeetingNumber.ToString(); }
        }

        public ServiceLib.DataService.Meeting Meeting { get; set; }

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
                DataObject data = new DataObject("MeetingControl", this);

                // Create a placeholder to drag.
                _adorner = this.Clone();
                _adorner.Opacity = 0.6;
                _adorner.IsHitTestVisible = false;
                _adorner.Width = 250;
                _adorner.MaxHeight = 150;

                _adornerLayer.Visibility = Visibility.Visible;
                _adornerLayer.Children.Add(_adorner);

                DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
                _adornerLayer.Children.Remove(_adorner);
                _adornerLayer.Visibility = Visibility.Collapsed;
                started_drag = false;
            }
            else if (e.LeftButton == MouseButtonState.Released)
            {
                if (this.MeetingHoverEvent != null)
                    this.MeetingHoverEvent(this, e);
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

        public MeetingControl Clone()
        {
            MeetingControl p = new MeetingControl();
            p.MeetingDate = this.meetingDate;
            p.MeetingNumber = this.meetingNumber;
            p.MeetingNotes = this.meetingNotes;
            p.Meeting = this.Meeting;

            p.Width = this.Width;
            p.Height = this.Height;
            return p;
        }
    }
}