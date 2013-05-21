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