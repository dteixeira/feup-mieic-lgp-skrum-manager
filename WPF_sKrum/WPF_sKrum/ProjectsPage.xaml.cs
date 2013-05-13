using System.Windows.Controls;

namespace WPF_sKrum
{
    /// <summary>
    /// Interaction logic for ProjectsPage.xaml
    /// </summary>
    public partial class ProjectsPage : UserControl
    {
        private ApplicationController backdata;

        public ProjectsPage()
        {
            InitializeComponent();
            backdata = ApplicationController.Instance;
            backdata.CurrentPage = this.GetType().Name;
        }
    }
}