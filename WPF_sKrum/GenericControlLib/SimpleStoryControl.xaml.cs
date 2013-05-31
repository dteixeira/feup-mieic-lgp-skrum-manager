using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GenericControlLib
{
    /// <summary>
    /// Interaction logic for StoryControl.xaml
    /// </summary>
    public partial class SimpleStoryControl : UserControl
    {
        private TextTrimming storyTextTrimming = TextTrimming.None;

        private string storyDescription;
        private string storyName;
        private string storyPriority;
        private string storyEstimation;
        private int storyNumber;

        public SimpleStoryControl()
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

        public string StoryEstimation
        {
            get { return this.storyEstimation; }
            set { this.storyEstimation = value; }
        }

        public int StoryNumber
        {
            get { return this.storyNumber; }
            set { this.storyNumber = value; }
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

        public TextTrimming StoryTextTrimming
        {
            get { return this.storyTextTrimming; }
            set { this.storyTextTrimming = value; }
        }
    }
}