using System.Windows.Controls;

namespace WPF_sKrum
{
    /// <summary>
    /// Interaction logic for StatsUser.xaml
    /// </summary>
    public partial class StatsUser : UserControl
    {
        private ApplicationController backdata;

        public StatsUser()
        {
            InitializeComponent();
            backdata = ApplicationController.Instance;
            backdata.CurrentPage = this.GetType().Name;
        }
    }
}