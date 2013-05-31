using Kinect.Gestures;
using Kinect.Pointers;
using Microsoft.Kinect.Toolkit.Interaction;
using SharedTypes;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PopupFormControlLib
{
    /// <summary>
    /// Interaction logic for FormWindow.xaml
    /// </summary>
    public partial class YesNoFormWindow : Window
    {
        public bool Success { get; private set; }

        public string FormTitle
        {
            set { this.FieldNameLabel.Content = value; }
        }

        public YesNoFormWindow()
        {
            InitializeComponent();
            this.Success = false;
            if (ApplicationController.Instance.KinectSensor.FoundSensor())
            {
                ApplicationController.Instance.KinectSensor.Pointers.KinectPointerMoved += new EventHandler<KinectPointerEventArgs>(this.KinectPointerMovedHandler);
                ApplicationController.Instance.KinectSensor.Gestures.KinectGestureRecognized += new EventHandler<Kinect.Gestures.KinectGestureEventArgs>(this.KinectGestureRecognizedHandler);
            }

            // Hide cursor if needed.
            if (ApplicationController.Instance.TrackingID != -1)
            {
                Mouse.OverrideCursor = Cursors.None;
                RightOpen.Visibility = Visibility.Visible;
                RightClosed.Visibility = Visibility.Collapsed;
            }
        }

        private void Close_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void KinectPointerMovedHandler(object sender, KinectPointerEventArgs e)
        {
            // Set correct hand visible.
            if (e.RightHand.HandEventType == InteractionHandEventType.GripRelease)
            {
                RightOpen.Visibility = Visibility.Visible;
                RightClosed.Visibility = Visibility.Collapsed;
            }
            else if (e.RightHand.HandEventType == InteractionHandEventType.Grip)
            {
                RightOpen.Visibility = Visibility.Collapsed;
                RightClosed.Visibility = Visibility.Visible;
            }

            // Position right hand.
            Canvas.SetLeft(RightOpen, (e.RightHand.X * this.RenderSize.Width) - (RightOpen.RenderSize.Width / 2) - this.LayoutRoot.Margin.Left);
            Canvas.SetTop(RightOpen, (e.RightHand.Y * this.RenderSize.Height) - (RightOpen.RenderSize.Height / 2) - this.LayoutRoot.Margin.Top);
            Canvas.SetLeft(RightClosed, (e.RightHand.X * this.RenderSize.Width) - (RightClosed.RenderSize.Width / 2) - this.LayoutRoot.Margin.Left);
            Canvas.SetTop(RightClosed, (e.RightHand.Y * this.RenderSize.Height) - (RightClosed.RenderSize.Height / 2) - this.LayoutRoot.Margin.Top);
        }

        private void KinectGestureRecognizedHandler(object sender, KinectGestureEventArgs e)
        {
            switch (e.GestureType)
            {
                case KinectGestureType.WaveLeftHand:
                    RightOpen.Visibility = Visibility.Collapsed;
                    RightClosed.Visibility = Visibility.Collapsed;
                    Mouse.OverrideCursor = Cursors.Arrow;
                    break;

                case KinectGestureType.WaveRightHand:
                    if (ApplicationController.Instance.Gripping)
                    {
                        RightOpen.Visibility = Visibility.Collapsed;
                        RightClosed.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        RightOpen.Visibility = Visibility.Visible;
                        RightClosed.Visibility = Visibility.Collapsed;
                    }
                    Mouse.OverrideCursor = Cursors.None;
                    break;

                default:
                    break;
            }
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void NoButton_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Success = false;
            this.Close();
        }

        private void YesButton_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Success = true;
            this.Close();
        }
    }
}