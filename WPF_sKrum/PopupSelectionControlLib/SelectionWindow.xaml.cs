﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PageTransitions;
using SharedTypes;
using Kinect.Pointers;
using Microsoft.Kinect.Toolkit.Interaction;
using Kinect.Gestures;

namespace PopupSelectionControlLib
{
    /// <summary>
    /// Interaction logic for SelectionWindow.xaml
    /// </summary>
    public partial class SelectionWindow : Window
    {
        public bool Success { get; private set; }
        public IFormPage FormPage { get; set; }
        
        public SelectionWindow()
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
            }
        }

        private void Close_MouseLeftButtonDown(object sender, RoutedEventArgs e)
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
            this.FormContent.Children.Add((UserControl) FormPage);
            this.FieldNameLabel.Content = FormPage.PageTitle;
        }
    }
}
