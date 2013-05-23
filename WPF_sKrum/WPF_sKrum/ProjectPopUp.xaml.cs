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
using ServiceLib.DataService;
using System.Linq;

namespace WPFApplication
{
	/// <summary>
	/// Interaction logic for ProjectPopUp.xaml
	/// </summary>
	public partial class ProjectPopUp : Window
	{
        private ApplicationController backdata;
        private float scrollValue = 0.0f;

		public ProjectPopUp()
		{
			this.InitializeComponent();
            this.backdata = ApplicationController.Instance;
        
        }

        public void fillLettters(Dictionary<string,List<Project>> dic)
        {

        }

        public void fillProjects()
        {
            Dictionary<string,List<Project>> dic = new Dictionary<string,List<Project>>();
            List<Project> projects = backdata.Projects;
            var x = (from p in projects
                    orderby p.Name ascending
                    select p).ToList<Project>();
            foreach(int letter in Enumerable.Range('A', 'Z' - 'A' + 1)) 
            {
                dic[letter.ToString()] = (from p in projects
                                         where p.Name[0] == letter
                                         select p).ToList<Project>();
            }

            foreach (String s in dic.Keys)
            {
                foreach (Project p in dic[s])
                {

                }
            }
        }
	}

}