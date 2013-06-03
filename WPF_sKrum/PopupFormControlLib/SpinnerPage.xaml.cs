using System.Windows.Controls;

namespace PopupFormControlLib
{
    /// <summary>
    /// Interaction logic for SpinnerPage.xaml
    /// </summary>
    public partial class SpinnerPage : UserControl, IFormPage
    {
        public SpinnerPage()
        {
            this.InitializeComponent();
            this.PageValue = 0.0;
        }

        public double Min
        {
            set
            {
                this.NumericSpinner.Min = value;
                this.PageValue = value;
            }
        }

        public double Max
        {
            set { this.NumericSpinner.Max = value; }
        }

        public double Increment
        {
            set { this.NumericSpinner.Increment = value; }
        }

        public string PageName { get; set; }

        public string PageTitle { get; set; }

        public object PageValue
        {
            get { return (double)this.NumericSpinner.SpinnerValue; }
            set { this.NumericSpinner.SpinnerValue = (double)value; }
        }

        public double DefaultValue
        {
            set
            {
                this.PageValue = value;
                this.NumericSpinner.SpinnerValue = value;
            }
        }
    }
}