using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace GenericControlLib
{
    /// <summary>
    /// Interaction logic for NotificationControl.xaml
    /// </summary>
    public partial class NotificationControl : UserControl
    {
        private Storyboard Animation { get; set; }

        public NotificationControl()
        {
            this.InitializeComponent();
            this.Animation = this.FindResource("NotificationAnimation") as Storyboard;
        }

        public string NotificationText
        {
            set { this.NotificationTextBlock.Text = value; }
        }

        public TimeSpan VisibleTimespan
        {
            set
            {
                if (this.Animation != null)
                {
                    this.Animation.Children[1].BeginTime = value;
                }
            }
        }

        public void StartAnimation()
        {
            if (this.Animation != null)
            {
                this.Animation.Begin();
            }
        }
    }
}