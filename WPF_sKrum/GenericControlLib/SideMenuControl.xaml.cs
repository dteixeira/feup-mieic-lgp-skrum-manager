using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GenericControlLib
{
    /// <summary>
    /// Interaction logic for SideMenu.xaml
    /// </summary>
    public partial class SideMenuControl : UserControl
    {
        public enum Options { Add, Edit, Remove, SpecialAdd, Close, Order };

        private Visibility firstVisibility = Visibility.Collapsed;
        private Visibility secondVisibility = Visibility.Collapsed;
        private Visibility thirdVisibility = Visibility.Collapsed;
        private Visibility fourthVisibility = Visibility.Collapsed;
        private Visibility fifthVisibility = Visibility.Collapsed;
        private Options firstType;
        private Options secondType;
        private Options thirdType;
        private Options fourthType;
        private Options fifthType;

        public delegate void myDropDelegate(object obj, DragEventArgs e);

        public event myDropDelegate FirstMenuDropEvent;

        public event myDropDelegate SecondMenuDropEvent;

        public event myDropDelegate ThirdMenuDropEvent;

        public event myDropDelegate FourthMenuDropEvent;

        public event myDropDelegate FifthMenuDropEvent;

        public delegate void myClickDelegate(object obj, MouseEventArgs e);

        public event myClickDelegate FirstMenuClickEvent;

        public event myClickDelegate SecondMenuClickEvent;

        public event myClickDelegate ThirdMenuClickEvent;

        public event myClickDelegate FourthMenuClickEvent;

        public event myClickDelegate FifthMenuClickEvent;

        public SideMenuControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.FirstIconBack.AllowDrop = true;
            this.SecondIconBack.AllowDrop = true;
            this.ThirdIconBack.AllowDrop = true;
            this.FourthIconBack.AllowDrop = true;
            this.FifthIconBack.AllowDrop = true;
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

        public Visibility FifthVisibility
        {
            get { return this.fifthVisibility; }
            set { this.fifthVisibility = value; }
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

        public Options FifthType
        {
            get { return this.fifthType; }
            set { this.fifthType = value; }
        }

        public string FirstIcon
        {
            get { return this.convertTypetoIcon(firstType); }
        }

        public string SecondIcon
        {
            get { return this.convertTypetoIcon(secondType); }
        }

        public string ThirdIcon
        {
            get { return this.convertTypetoIcon(thirdType); }
        }

        public string FourthIcon
        {
            get { return this.convertTypetoIcon(fourthType); }
        }

        public string FifthIcon
        {
            get { return this.convertTypetoIcon(fifthType); }
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
                case Options.SpecialAdd:
                    return "Images/add2.png";
                case Options.Close:
                    return "Images/check.png";
                case Options.Order:
                    return "Images/order.png";
                default:
                    return "Images/none.png";
            }
        }

        private void FirstIconBack_Drop(object sender, DragEventArgs e)
        {
            if (this.FirstMenuDropEvent != null)
                this.FirstMenuDropEvent(sender, e);
        }

        private void SecondIconBack_Drop(object sender, DragEventArgs e)
        {
            if (this.SecondMenuDropEvent != null)
                this.SecondMenuDropEvent(sender, e);
        }

        private void ThirdIconBack_Drop(object sender, DragEventArgs e)
        {
            if (this.ThirdMenuDropEvent != null)
                this.ThirdMenuDropEvent(sender, e);
        }

        private void FourthIconBack_Drop(object sender, DragEventArgs e)
        {
            if (this.FourthMenuDropEvent != null)
                this.FourthMenuDropEvent(sender, e);
        }

        private void FifthIconBack_Drop(object sender, DragEventArgs e)
        {
            if (this.FifthMenuDropEvent != null)
                this.FifthMenuDropEvent(sender, e);
        }

        private void FirstIconBack_Click(object sender, MouseEventArgs e)
        {
            if (this.FirstMenuClickEvent != null)
                this.FirstMenuClickEvent(sender, e);
        }

        private void SecondIconBack_Click(object sender, MouseEventArgs e)
        {
            if (this.SecondMenuClickEvent != null)
                this.SecondMenuClickEvent(sender, e);
        }

        private void ThirdIconBack_Click(object sender, MouseEventArgs e)
        {
            if (this.ThirdMenuClickEvent != null)
                this.ThirdMenuClickEvent(sender, e);
        }

        private void FourthIconBack_Click(object sender, MouseEventArgs e)
        {
            if (this.FourthMenuClickEvent != null)
                this.FourthMenuClickEvent(sender, e);
        }

        private void FifthIconBack_Click(object sender, MouseEventArgs e)
        {
            if (this.FifthMenuClickEvent != null)
                this.FifthMenuClickEvent(sender, e);
        }
    }
}