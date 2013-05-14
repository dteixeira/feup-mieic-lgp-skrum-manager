using System.Windows.Controls;

namespace WPFApplication
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
            this.backdata = ApplicationController.Instance;
            this.backdata.CurrentPage = ApplicationPages.ProjectsPage;
        }
    }
}