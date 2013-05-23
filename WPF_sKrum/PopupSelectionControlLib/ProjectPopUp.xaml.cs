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
using System.Linq;
using SharedTypes;
using ServiceLib.DataService;

namespace PopupSelectionControlLib
{
	/// <summary>
	/// Interaction logic for ProjectPopUp.xaml
	/// </summary>
	public partial class ProjectPopUp : Window
	{
		public ProjectPopUp(object context)
		{
			this.InitializeComponent();
        }

        public void FillLettters(Dictionary<string,List<Project>> dic)
        {
            // TODO Finish this.
        }

        public void FillProjects()
        {
            Dictionary<string,List<Project>> dic = new Dictionary<string,List<Project>>();
            List<Project> projects = ApplicationController.Instance.Projects;
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
                    // TODO Finish this.
                }
            }
        }
    }

}