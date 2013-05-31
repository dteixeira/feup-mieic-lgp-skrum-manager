using System.Windows.Controls;

namespace GenericControlLib
{
    /// <summary>
    /// Interaction logic for PercentageShowerControl.xaml
    /// </summary>
    public partial class PercentageDualShowerControl : UserControl
    {
        private int done;
        private int expected;

        public int Done
        {
            get { return this.done; }
            set { this.done = value; }
        }

        public int Expected
        {
            get { return this.expected; }
            set { this.expected = value; }
        }

        public double Percentage
        {
            get
            {
                if (done > expected)
                    return 1;
                else
                    return (((double)done) / expected);
            }
        }

        public double Percentage2
        {
            get
            {
                if (done < expected)
                    return 1;
                else
                    return (((double)expected) / done);
            }
        }

        public string StatsText
        {
            get
            {
                return (done.ToString());
            }
        }

        public string StatsText2
        {
            get
            {
                return (expected.ToString());
            }
        }

        public PercentageDualShowerControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Bar.Width = this.WholeArea.RenderSize.Width * Percentage2;
            this.Bar2.Width = this.WholeArea.RenderSize.Width * Percentage;
            this.StatShower.Text = this.StatsText;
            this.StatShower2.Text = this.StatsText2;
        }
    }
}