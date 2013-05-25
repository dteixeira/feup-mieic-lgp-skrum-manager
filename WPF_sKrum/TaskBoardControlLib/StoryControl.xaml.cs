using System;
using System.Collections.Generic;
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

namespace TaskBoardControlLib
{
	/// <summary>
	/// Interaction logic for StoryControl.xaml
	/// </summary>
	public partial class StoryControl : UserControl
	{
        private Point startpoint;
        private bool allow_drag = false;
        private bool started_drag = false;
        private StoryControl _adorner;
        private string AdornerLayer = "dragdropadornerLayer";
        private Canvas _adornerLayer;
        private TextTrimming storyTextTrimming = TextTrimming.None;


        private string storyDescription;
        private string storyName;
        private string storyPriority;
        private string storyEstimation;

		public StoryControl()
		{
			this.InitializeComponent();
            this.DataContext = this;
		}

        public string StoryDescription
        {
            get { return this.storyDescription; }
            set { this.storyDescription = value; }
        }

        public string StoryName
        {
            get { return this.storyName; }
            set { this.storyName = value; }
        }

        public string StoryPriority
        {
            get { return this.storyPriority; }
            set { this.storyPriority = value; }
        }

        public string StoryEstimation
        {
            get { return this.storyEstimation; }
            set { this.storyEstimation = value; }
        }

        public bool IsDraggable
        {
            get { return this.allow_drag; }
            set { this.allow_drag = value; }
        }

        public ServiceLib.DataService.Story Story { get; set; }

        public Brush StoryPriorityColor
        {
            get
            {
                switch (this.storyPriority)
                {
                    case "M":
                        return (Brush)this.FindResource("MustPriorityBrush");
                    case "S":
                        return (Brush)this.FindResource("ShouldPriorityBrush");
                    case "C":
                        return (Brush)this.FindResource("CouldPriorityBrush");
                    case "W":
                        return (Brush)this.FindResource("WouldPriorityBrush");
                    default:
                        return (Brush)this.FindResource("MustPriorityBrush");
                }
            }
        }

        public TextTrimming StoryTextTrimming
        {
            get { return this.storyTextTrimming; }
            set { this.storyTextTrimming = value; }
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (!allow_drag)
                return;

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
                _adorner.storyTextTrimming = TextTrimming.WordEllipsis;

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

            Point mousePos = SharedTypes.Utilities.GetMousePositionWin32();

            Canvas.SetLeft(_adorner, mousePos.X);
            Canvas.SetTop(_adorner, mousePos.Y);

            e.Handled = true;
        }

        public StoryControl Clone()
        {
            StoryControl p = new StoryControl();
            p.storyDescription = this.storyDescription;
            p.storyName = this.storyName;
            p.storyPriority = this.storyPriority;
            p.storyEstimation = this.storyEstimation;

            p.Width = this.Width;
            p.Height = this.Height;
            return p;
        }
	}
}