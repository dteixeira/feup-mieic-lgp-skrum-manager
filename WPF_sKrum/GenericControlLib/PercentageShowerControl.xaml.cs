using System.Windows.Controls;

namespace GenericControlLib
{
    /// <summary>
    /// Interaction logic for PercentageShowerControl.xaml
    /// </summary>
    public partial class PercentageShowerControl : UserControl
    {
        private int done;
        private int total;

        public int Done
        {
            get { return this.done; }
            set { this.done = value; }
        }

        public int Total
        {
            get { return this.total; }
            set { this.total = value; }
        }

        public double Percentage
        {
            get
            {
                if(total > 0)
                    return (((double)this.done) / this.total);
                else
                    return 0;
            }
        }

        public string StatsText
        {
            get
            {
                return string.Format("{0:0.#} / {1:0.#}", this.done, this.total);
            }
        }

        public PercentageShowerControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Bar.Width = this.WholeArea.RenderSize.Width * Percentage;
            this.StatShower.Text = this.StatsText;
        }
    }
}