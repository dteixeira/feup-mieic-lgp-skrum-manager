using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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