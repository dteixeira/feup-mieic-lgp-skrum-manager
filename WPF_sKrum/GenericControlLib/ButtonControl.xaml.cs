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

namespace GenericControlLib
{
	/// <summary>
	/// Interaction logic for ButtonControl.xaml
	/// </summary>
	public partial class ButtonControl : UserControl
	{
        private int buttonFontSize = 50;
        private string buttonText = "BUTTON";

		public ButtonControl()
		{
			this.InitializeComponent();
            this.DataContext = this;
		}

        public int ButtonFontSize
        {
            get { return this.buttonFontSize; }
            set { this.buttonFontSize = value; }
        }

        public string ButtonText
        {
            get { return this.buttonText; }
            set { this.buttonText = value; }
        }
	}
}