using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for SideMenu.xaml
    /// </summary>
    public partial class SideMenuControl : UserControl
    {
        public enum Options {Add, Edit, Remove,  };

        private Visibility firstVisibility = Visibility.Collapsed;
        private Visibility secondVisibility = Visibility.Collapsed;
        private Visibility thirdVisibility = Visibility.Collapsed;
        private Visibility fourthVisibility = Visibility.Collapsed;
        private Options firstType;
        private Options secondType;
        private Options thirdType;
        private Options fourthType;


        public SideMenuControl()
		{
			this.InitializeComponent();
            this.DataContext = this;
		}

        public Visibility FirstVisibility
        {
            get { return this.firstVisibility; }
            set { this.firstVisibility = value; }
        }

        public Visibility SecondVisibility
        {
            get { return this.secondVisibility; }
            set { this.secondVisibility = value; }
        }

        public Visibility ThirdVisibility
        {
            get { return this.thirdVisibility; }
            set { this.thirdVisibility = value; }
        }

        public Visibility FourthVisibility
        {
            get { return this.fourthVisibility; }
            set { this.fourthVisibility = value; }
        }

        public Options FirstType
        {
            get { return this.firstType; }
            set { this.firstType = value; }
        }

        public Options SecondType
        {
            get { return this.secondType; }
            set { this.secondType = value; }
        }

        public Options ThirdType
        {
            get { return this.thirdType; }
            set { this.thirdType = value; }
        }

        public Options FourthType
        {
            get { return this.fourthType; }
            set { this.fourthType = value; }
        }

        public string FirstIcon
        {
            get{ return this.convertTypetoIcon(firstType); }
        }
        
        public string SecondIcon
        {
            get{ return this.convertTypetoIcon(secondType); }
        }

        public string ThirdIcon
        {
            get{ return this.convertTypetoIcon(thirdType); }
        }

        public string FourthIcon
        {
            get{ return this.convertTypetoIcon(fourthType); }
        }

        private string convertTypetoIcon(Options type)
        {
            switch (type)
            {
                case Options.Add:
                    return "Images/add.png";
                case Options.Edit:
                    return "Images/edit.png";
                case Options.Remove:
                    return "Images/remove.png";
                default:
                    return "Images/none.png";
            }
        }

    }
}
