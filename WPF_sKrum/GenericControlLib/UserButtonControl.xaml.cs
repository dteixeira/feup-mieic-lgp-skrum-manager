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
    /// Interaction logic for ProjectButton.xaml
    /// </summary>
    public partial class UserButtonControl : UserControl
    {
        private int buttonFontSize = 20;
        private string buttonText = "Projecto";
        private string imageSource = @"Images\utilizador.png";

        public UserButtonControl()
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

        public string ImageSource
        {
            get { return this.imageSource; }
            set { this.imageSource = value; }
        }
    }
}