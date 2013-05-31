using System.Windows.Controls;

namespace TaskBoardControlLib
{
    /// <summary>
    /// Interaction logic for TaskBoardRowDesignControl.xaml
    /// </summary>
    public partial class TaskBoardRowDesignControl : UserControl
    {
        public TaskBoardRowDesignControl()
        {
            this.InitializeComponent();
        }

        public Grid ControlGrid
        {
            get { return this.LayoutRoot; }
        }
    }
}