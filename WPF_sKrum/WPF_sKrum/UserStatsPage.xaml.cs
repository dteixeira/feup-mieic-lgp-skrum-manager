using System.Windows.Controls;

namespace WPFApplication
{
    /// <summary>
    /// Interaction logic for StatsUser.xaml
    /// </summary>
    public partial class UserStatsPage : UserControl
    {
        private ApplicationController backdata;

        public UserStatsPage()
        {
            InitializeComponent();
            this.backdata = ApplicationController.Instance;
            this.backdata.CurrentPage = ApplicationPages.UserStatsPage;
        }
    }
}