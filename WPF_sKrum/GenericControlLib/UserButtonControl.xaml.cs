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
        private string userName = "Utilizador";
        private string userPhoto = @"http://178.175.139.37/images/members/21.jpg";

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

        public string UserName
        {
            get { return this.userName; }
            set { this.userName = value; }
        }

        public string UserPhoto
        {
            get { return this.userPhoto; }
            set { this.userPhoto = value; }
        }

        public ServiceLib.DataService.Story Story { get; set; }
    }
}