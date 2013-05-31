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

namespace GenericControlLib
{
	/// <summary>
	/// Interaction logic for ProjectButton.xaml
	/// </summary>
	public partial class ProjectButtonControl : UserControl
    {
        private Point startpoint;
        private bool allow_drag = false;
        private bool started_drag = false;
        private ProjectButtonControl _adorner;
        private string AdornerLayer = "dragdropadornerLayer";
        private Canvas _adornerLayer;

        private int buttonFontSize = 30;
        private string projectName = "Projecto";
        private string projectimageSource = @"Images\mala.png";

		public ProjectButtonControl()
		{
			this.InitializeComponent();
            this.DataContext = this;
		}

        public int ButtonFontSize
        {
            get { return this.buttonFontSize; }
            set { this.buttonFontSize = value; }
        }

        public string ProjectName
        {
            get { return this.projectName; }
            set { this.projectName = value; }
        }

        public string ProjectImageSource
        {
            get { return this.projectimageSource; }
            set { this.projectimageSource = value; }
        }

        public bool IsDraggable
        {
            get { return this.allow_drag; }
            set { this.allow_drag = value; }
        }

        public ServiceLib.DataService.Project Project { get; set; }

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
                DataObject data = new DataObject("ProjectButtonControl", this);

                // Create a placeholder to drag.
                _adorner = this.Clone();
                _adorner.Opacity = 0.6;
                _adorner.IsHitTestVisible = false;
                _adorner.MaxWidth = 300;
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
                started_drag = false;
            }
            e.Handled = true;
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            started_drag = false;
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);

            Point mousePos = SharedTypes.Utilities.GetMousePositionWin32();

            Canvas.SetLeft(_adorner, mousePos.X);
            Canvas.SetTop(_adorner, mousePos.Y);

            e.Handled = true;
        }

        public ProjectButtonControl Clone()
        {
            ProjectButtonControl p = new ProjectButtonControl();
            p.projectName = this.projectName;
            p.projectimageSource = this.projectimageSource;

            p.Width = this.Width;
            p.Height = this.Height;
            return p;
        }

	}
}