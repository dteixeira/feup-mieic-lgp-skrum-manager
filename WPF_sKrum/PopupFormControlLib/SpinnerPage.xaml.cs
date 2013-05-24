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
using System.Linq;

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
            this.NumericSpinner.SpinnerChangedEvent += new GenericControlLib.NumericSpinnerControl.SpinnerChangedHandler(this.SpinnerChangeHandler);
		}

        public double Min
        {
            set { 
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

        public object PageValue { get; set; }

        public void SpinnerChangeHandler(double value)
        {
            this.PageValue = value;
        }
	}
}