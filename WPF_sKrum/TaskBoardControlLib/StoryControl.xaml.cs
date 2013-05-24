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
        private string storyDescription;
        private string storyName;
        private string storyPriority;
        private int storyEstimation;

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
	}
}