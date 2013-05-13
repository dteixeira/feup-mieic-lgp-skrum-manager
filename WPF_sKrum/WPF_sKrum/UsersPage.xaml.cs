using System.Windows.Controls;

namespace WPF_sKrum
{
    /// <summary>
    /// Interaction logic for UsersPage.xaml
    /// </summary>
    public partial class UsersPage : UserControl
    {
        private ApplicationController backdata;

        public UsersPage()
        {
            InitializeComponent();
            backdata = ApplicationController.Instance;
            backdata.currentPage = this.GetType().Name;
        }
    }
}