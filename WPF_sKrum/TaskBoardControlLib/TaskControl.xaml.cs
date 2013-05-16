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
	/// Interaction logic for TaskControl.xaml
	/// </summary>
	public partial class TaskControl : UserControl
	{
        private string taskDescription;

		public TaskControl()
		{
			this.InitializeComponent();
            this.DataContext = this;
		}

        public string TaskDescription
        {
            get { return this.taskDescription; }
            set { this.taskDescription = value; }
        }
	}
}