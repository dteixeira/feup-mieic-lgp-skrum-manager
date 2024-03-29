﻿using SharedTypes;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace GenericControlLib
{
    /// <summary>
    /// Interaction logic for NavigationControl.xaml
    /// </summary>
    public partial class NavigationControl : UserControl
    {
        private DispatcherTimer pagesStatTimer;

        public delegate void NavigationHandler(PageChangeDirection direction);

        public event NavigationHandler NavigationEvent;

        public string UpBarText
        {
            get { return this.UpText.Text; }
            set { this.UpText.Text = value; }
        }

        public string DownBarText
        {
            get { return this.DownText.Text; }
            set { this.DownText.Text = value; }
        }

        public string LeftBarText
        {
            get { return this.LeftText.Text; }
            set { this.LeftText.Text = value; }
        }

        public string RightBarText
        {
            get { return this.RightText.Text; }
            set { this.RightText.Text = value; }
        }

        public NavigationControl()
        {
            this.InitializeComponent();
            this.pagesStatTimer = new DispatcherTimer();
            this.pagesStatTimer.Tick += new EventHandler(PagesInfoAppearAction);
            this.pagesStatTimer.Interval = TimeSpan.FromSeconds(1);
            this.UpBarText = null;
        }

        private void NotifyNavigationEvent(PageChangeDirection direction)
        {
            if (this.NavigationEvent != null)
            {
                System.Delegate[] delegateList = NavigationEvent.GetInvocationList();
                foreach (NavigationHandler handler in delegateList)
                {
                    try
                    {
                        handler(direction);
                    }
                    catch (System.Exception e)
                    {
                        System.Console.WriteLine(e.Message);
                        NavigationEvent -= handler;
                    }
                }
            }
        }

        private void ActionArea_MouseEnter(object sender, MouseEventArgs e)
        {
            this.pagesStatTimer.Start();
        }

        private void ActionArea_MouseLeave(object sender, MouseEventArgs e)
        {
            this.pagesStatTimer.Stop();
            this.CenterStopAction.Visibility = System.Windows.Visibility.Visible;
        }

        private void ActionStop_MouseEnter(object sender, MouseEventArgs e)
        {
            this.pagesStatTimer.Stop();
            this.HideEverything();
        }

        private void HideEverything()
        {
            // Hide everything.
            this.CenterStopAction.Visibility = System.Windows.Visibility.Hidden;
            this.UpLeftCorner.Visibility = System.Windows.Visibility.Hidden;
            this.UpRightCorner.Visibility = System.Windows.Visibility.Hidden;
            this.DownLeftCorner.Visibility = System.Windows.Visibility.Hidden;
            this.DownRightCorner.Visibility = System.Windows.Visibility.Hidden;
            this.UpBar.Visibility = System.Windows.Visibility.Hidden;
            this.DownBar.Visibility = System.Windows.Visibility.Hidden;
            this.LeftBar.Visibility = System.Windows.Visibility.Hidden;
            this.RightBar.Visibility = System.Windows.Visibility.Hidden;
            this.UpBar.SetValue(Panel.ZIndexProperty, 1);
            this.DownBar.SetValue(Panel.ZIndexProperty, 1);
            this.LeftBar.SetValue(Panel.ZIndexProperty, 1);
            this.RightBar.SetValue(Panel.ZIndexProperty, 1);
        }

        private void PagesInfoAppearAction(object sender, EventArgs e)
        {
            // Set center rectangle hittable.
            this.CenterStopAction.Visibility = System.Windows.Visibility.Visible;

            // Show needed bars.
            if (this.UpBarText != "")
            {
                this.UpLeftCorner.Visibility = System.Windows.Visibility.Visible;
                this.UpRightCorner.Visibility = System.Windows.Visibility.Visible;
                this.UpBar.Visibility = System.Windows.Visibility.Visible;
                this.UpBar.SetValue(Panel.ZIndexProperty, 20);
            }
            if (this.DownBarText != "")
            {
                this.DownLeftCorner.Visibility = System.Windows.Visibility.Visible;
                this.DownRightCorner.Visibility = System.Windows.Visibility.Visible;
                this.DownBar.Visibility = System.Windows.Visibility.Visible;
                this.DownBar.SetValue(Panel.ZIndexProperty, 20);
            }
            if (this.LeftBarText != "")
            {
                this.UpLeftCorner.Visibility = System.Windows.Visibility.Visible;
                this.DownLeftCorner.Visibility = System.Windows.Visibility.Visible;
                this.LeftBar.Visibility = System.Windows.Visibility.Visible;
                this.LeftBar.SetValue(Panel.ZIndexProperty, 20);
            }
            if (this.RightBarText != "")
            {
                this.UpRightCorner.Visibility = System.Windows.Visibility.Visible;
                this.DownRightCorner.Visibility = System.Windows.Visibility.Visible;
                this.RightBar.Visibility = System.Windows.Visibility.Visible;
                this.RightBar.SetValue(Panel.ZIndexProperty, 20);
            }
        }

        private void UpBar_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.pagesStatTimer.Stop();
            this.HideEverything();
            this.NotifyNavigationEvent(PageChangeDirection.Up);
        }

        private void DownBar_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.pagesStatTimer.Stop();
            this.HideEverything();
            this.NotifyNavigationEvent(PageChangeDirection.Down);
        }

        private void LeftBar_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.pagesStatTimer.Stop();
            this.HideEverything();
            this.NotifyNavigationEvent(PageChangeDirection.Left);
        }

        private void RightBar_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.pagesStatTimer.Stop();
            this.HideEverything();
            this.NotifyNavigationEvent(PageChangeDirection.Right);
        }
    }
}