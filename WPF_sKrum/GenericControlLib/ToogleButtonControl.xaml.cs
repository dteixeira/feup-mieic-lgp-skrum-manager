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
	public partial class ToogleButtonControl : UserControl
	{
        private bool selected = false;
        private int buttonFontSize = 50;
        private string buttonText = "TOOGLEBUTTON";

        public ToogleButtonControl()
		{
			this.InitializeComponent();
            this.DataContext = this;
            this.UpdateText();
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

        public bool Selected
        {
            get { return this.selected; }
            set
            {
                this.selected = value;
                if (this.selected)
                {
                    this.OuterBorder.Background = new SolidColorBrush(Color.FromRgb(0x95, 0x95, 0x95));
                    this.TextValue.Foreground = new SolidColorBrush(Color.FromRgb(0x24, 0x24, 0x24));
                }
                else
                {
                    this.OuterBorder.Background = new SolidColorBrush(Color.FromRgb(0x24, 0x24, 0x24));
                    this.TextValue.Foreground = new SolidColorBrush(Color.FromRgb(0xE2, 0xE2, 0xE2));
                }
                //Force change on text
                this.UpdateText();
            }
        }

        private void ToogleButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            this.OuterBorder.Background = new SolidColorBrush(Color.FromRgb(0x64, 0x64, 0x64));
            this.TextValue.Foreground = new SolidColorBrush(Color.FromRgb(0xE2, 0xE2, 0xE2));
        }

        private void ToogleButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.selected)
            {
                this.OuterBorder.Background = new SolidColorBrush(Color.FromRgb(0x95, 0x95, 0x95));
                this.TextValue.Foreground = new SolidColorBrush(Color.FromRgb(0x24, 0x24, 0x24));
            }
            else
            {
                this.OuterBorder.Background = new SolidColorBrush(Color.FromRgb(0x24, 0x24, 0x24));
                this.TextValue.Foreground = new SolidColorBrush(Color.FromRgb(0xE2, 0xE2, 0xE2));
            }
        }

        private void ToogleButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selected = !selected;
            //Force change on text
            this.UpdateText();
        }

        private void UpdateText()
        {
            if (selected)
                this.StateValue.Text = "Activado";
            else
                this.StateValue.Text = "Desactivado";
        }
	}
}