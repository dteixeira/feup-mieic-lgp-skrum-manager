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
        private static Brush PriorityMustColor = new SolidColorBrush(Color.FromRgb(0x5A, 0x5A, 0x5A));
        private static Brush PriorityShouldColor = new SolidColorBrush(Color.FromRgb(0x78, 0x78, 0x78));
        private static Brush PriorityCouldColor = new SolidColorBrush(Color.FromRgb(0xB3, 0xB3, 0xB3));
        private static Brush PriorityWouldColor = new SolidColorBrush(Color.FromRgb(0xCD, 0xCD, 0xCD));

        private string storyDescription;
        private string storyName;
        private string storyPriority;
        private int storyEstimation;
        private Brush storyPriorityColor;

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

        public int StoryEstimation
        {
            get { return this.storyEstimation; }
            set { this.storyEstimation = value; }
        }

        public Brush StoryPriorityColor
        {
            get
            {
                switch (this.storyPriority)
                {
                    case "M":
                        return StoryControl.PriorityMustColor;
                    case "S":
                        return StoryControl.PriorityShouldColor;
                    case "C":
                        return StoryControl.PriorityCouldColor;
                    case "W":
                        return StoryControl.PriorityWouldColor;
                    default:
                        return StoryControl.PriorityMustColor;
                }
            }
        }
	}
}