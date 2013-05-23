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
        private double min = 1;
        private double max = 99999;
        private double increment = 1;

		public SpinnerPage()
		{
			this.InitializeComponent();
            this.PageValue = 0.0;
		}

        public double Min
        {
            get { return this.min; }
            set
            {
                this.min = value;
                this.PageValue = this.min;
                this.TextValue.Text = string.Format("{0:0.#}", this.min);
            }
        }

        public double Max
        {
            get { return this.max; }
            set { this.max = value; }
        }

        public double Increment
        {
            get { return this.increment; }
            set { this.increment = value; }
        }

        public string PageName { get; set; }

        public string PageTitle { get; set; }

        public object PageValue { get; set; }

        private void TextValue_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                // Check for backspace.
                TextChange change = e.Changes.FirstOrDefault();
                if (change != null && change.RemovedLength > 0)
                {
                    return;
                }

                double temp = double.Parse(this.TextValue.Text);
                if (temp <= this.Max && temp >= this.Min)
                {
                    this.PageValue = temp;
                }
                else
                {
                    this.TextValue.Text = string.Format("{0:0.#}", this.PageValue);
                    e.Handled = true;
                }
            }
            catch (System.Exception)
            {
                this.TextValue.Text = string.Format("{0:0.#}", this.PageValue);
                e.Handled = true;
            }
        }
	}
}