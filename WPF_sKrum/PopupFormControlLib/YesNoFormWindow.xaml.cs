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
                if (ApplicationController.Instance.UserHandedness == KinectGestureUserHandedness.RightHanded)
                {
                    RightOpen.Visibility = Visibility.Visible;
                    RightClosed.Visibility = Visibility.Collapsed;
                }
                else
                {
                    LeftOpen.Visibility = Visibility.Visible;
                    LeftClosed.Visibility = Visibility.Collapsed;
                }
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
                if (ApplicationController.Instance.UserHandedness == KinectGestureUserHandedness.RightHanded)
                {
                    RightOpen.Visibility = Visibility.Visible;
                    RightClosed.Visibility = Visibility.Collapsed;
                }
                else
                {
                    LeftOpen.Visibility = Visibility.Visible;
                    LeftClosed.Visibility = Visibility.Collapsed;
                }

            }
            else if (e.RightHand.HandEventType == InteractionHandEventType.Grip)
            {
                if (ApplicationController.Instance.UserHandedness == KinectGestureUserHandedness.RightHanded)
                {
                    RightOpen.Visibility = Visibility.Collapsed;
                    RightClosed.Visibility = Visibility.Visible;
                }
                else
                {
                    LeftOpen.Visibility = Visibility.Collapsed;
                    LeftClosed.Visibility = Visibility.Visible;
                }
            }

            // Position hand.
            if (ApplicationController.Instance.UserHandedness == KinectGestureUserHandedness.RightHanded)
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

                // Normalize pointer coordinates.
                double normalizedX = Math.Max(0, Math.Min(e.RightHand.X, 1.0));
                double normalizedY = Math.Max(0, Math.Min(e.RightHand.Y, 1.0));

                Canvas.SetLeft(RightOpen, (normalizedX * this.RenderSize.Width) - (RightOpen.RenderSize.Width / 2) - this.LayoutRoot.Margin.Left);
                Canvas.SetTop(RightOpen, (normalizedY * this.RenderSize.Height) - (RightOpen.RenderSize.Height / 2) - this.LayoutRoot.Margin.Top);

                Canvas.SetLeft(RightClosed, (normalizedX * this.RenderSize.Width) - (RightClosed.RenderSize.Width / 2) - this.LayoutRoot.Margin.Left);
                Canvas.SetTop(RightClosed, (normalizedY * this.RenderSize.Height) - (RightClosed.RenderSize.Height / 2) - this.LayoutRoot.Margin.Top);
            }
            else
            {
                // Set correct hand visible.
                if (e.LeftHand.HandEventType == InteractionHandEventType.GripRelease)
                {
                    LeftOpen.Visibility = Visibility.Visible;
                    LeftClosed.Visibility = Visibility.Collapsed;
                }
                else if (e.LeftHand.HandEventType == InteractionHandEventType.Grip)
                {
                    LeftOpen.Visibility = Visibility.Collapsed;
                    LeftClosed.Visibility = Visibility.Visible;
                }

                // Normalize pointer coordinates.
                double normalizedX = Math.Max(0, Math.Min(e.LeftHand.X, 1.0));
                double normalizedY = Math.Max(0, Math.Min(e.LeftHand.Y, 1.0));

                Canvas.SetLeft(LeftOpen, (normalizedX * this.RenderSize.Width) - (LeftOpen.RenderSize.Width / 2) - this.LayoutRoot.Margin.Left);
                Canvas.SetTop(LeftOpen, (normalizedY * this.RenderSize.Height) - (LeftOpen.RenderSize.Height / 2) - this.LayoutRoot.Margin.Top);

                Canvas.SetLeft(LeftClosed, (normalizedX * this.RenderSize.Width) - (LeftClosed.RenderSize.Width / 2) - this.LayoutRoot.Margin.Left);
                Canvas.SetTop(LeftClosed, (normalizedY * this.RenderSize.Height) - (LeftClosed.RenderSize.Height / 2) - this.LayoutRoot.Margin.Top);
            }
        }

        private void KinectGestureRecognizedHandler(object sender, KinectGestureEventArgs e)
        {
            if (ApplicationController.Instance.UserHandedness == KinectGestureUserHandedness.RightHanded)
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
            else
            {
                switch (e.GestureType)
                {
                    case KinectGestureType.WaveLeftHand:
                        if (ApplicationController.Instance.Gripping)
                        {
                            LeftOpen.Visibility = Visibility.Collapsed;
                            LeftClosed.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            LeftOpen.Visibility = Visibility.Visible;
                            LeftClosed.Visibility = Visibility.Collapsed;
                        }
                        Mouse.OverrideCursor = Cursors.None;
                        break;
                    case KinectGestureType.WaveRightHand:
                        LeftOpen.Visibility = Visibility.Collapsed;
                        LeftClosed.Visibility = Visibility.Collapsed;
                        Mouse.OverrideCursor = Cursors.Arrow;
                        break;
                    default:
                        break;
                }
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