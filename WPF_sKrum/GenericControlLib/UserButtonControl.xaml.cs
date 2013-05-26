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
    public partial class UserButtonControl : UserControl
    {
        private Point startpoint;
        private bool allow_drag = false;
        private bool started_drag = false;
        private UserButtonControl _adorner;
        private string AdornerLayer = "dragdropadornerLayer";
        private Canvas _adornerLayer;

        private int buttonFontSize = 20;
        private string userName = "Utilizador";
        private string userPhoto = @"http://178.175.139.37/images/members/21.jpg";

        public UserButtonControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        public int ButtonFontSize
        {
            get { return this.buttonFontSize; }
            set { this.buttonFontSize = value; }
        }

        public string UserName
        {
            get { return this.userName; }
            set { this.userName = value; }
        }

        public string UserPhoto
        {
            get { return this.userPhoto; }
            set { this.userPhoto = value; }
        }

        public bool IsDraggable
        {
            get { return this.allow_drag; }
            set { this.allow_drag = value; }
        }

        public ServiceLib.DataService.Person Person { get; set; }

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
                DataObject data = new DataObject("UserButtonControl", this);

                // Create a placeholder to drag.
                _adorner = this.Clone();
                _adorner.Opacity = 0.6;
                _adorner.IsHitTestVisible = false;
                _adorner.MaxWidth = 150;
                _adorner.MaxHeight = 250;

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

        public UserButtonControl Clone()
        {
            UserButtonControl p = new UserButtonControl();
            p.userName = this.userName;
            p.userPhoto = this.userPhoto;

            p.Width = this.Width;
            p.Height = this.Height;
            return p;
        }
    }
}