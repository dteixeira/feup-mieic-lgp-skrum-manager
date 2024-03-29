﻿using ServiceLib.DataService;
using ServiceLib.NotificationService;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ProjectConfigurationPageLib
{
    /// <summary>
    /// Interaction logic for ProjectConfigurationPage.xaml
    /// </summary>
    public partial class ProjectConfigurationPage : UserControl, ITargetPage
    {
        public ApplicationPages PageType { get; set; }
        public ApplicationController.DataModificationHandler DataChangeDelegate { get; set; }

        public ProjectConfigurationPage(object context)
        {
            InitializeComponent();
            this.PageType = ApplicationPages.ProjectConfigurationPage;

            // Register for project change notifications.
            this.DataChangeDelegate = new ApplicationController.DataModificationHandler(this.DataChangeHandler);
            ApplicationController.Instance.DataChangedEvent += this.DataChangeDelegate;

            PopulateProjectConfiguration();
        }

        private void PopulateProjectConfiguration()
        {
            //TODO
            // Get current project if selected.
            Project project = ApplicationController.Instance.CurrentProject;
            this.DurSpinner.SpinnerValue = project.SprintDuration;
            this.LimAlertSpinner.SpinnerValue = project.AlertLimit;
            this.NumIntSpinner.SpinnerValue = project.Speed;

        }

        public PageChange PageChangeTarget(PageChangeDirection direction)
        {
            switch (direction)
            {
                case PageChangeDirection.Down:
                    return null;
                case PageChangeDirection.Left:
                    return new PageChange { Context = null, Page = ApplicationPages.ProjectManagementPage };
                case PageChangeDirection.Right:
                    return new PageChange { Context = null, Page = ApplicationPages.MainPage };
                case PageChangeDirection.Up:
                    return new PageChange { Context = null, Page = ApplicationPages.ProjectTeamManagementPage };
                default:
                    return null;
            }
        }

        public void SetupNavigation()
        {
            System.Collections.Generic.Dictionary<PageChangeDirection, string> directions = new System.Collections.Generic.Dictionary<PageChangeDirection, string>();
            directions[PageChangeDirection.Up] = "GESTÃO DE EQUIPA";
            directions[PageChangeDirection.Down] = null;
            directions[PageChangeDirection.Left] = "GESTÃO DE PROJECTOS";
            directions[PageChangeDirection.Right] = "MENU INICIAL";
            ApplicationController.Instance.ApplicationWindow.SetupNavigation(directions);
        }

        public void UnloadPage()
        {
            // Unregister for notifications.

            // Update project data using web service
            // Launch thread to update the project.
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.UpdateProjConfInService));
            thread.Start(new object[] { this.DurSpinner.SpinnerValue, this.LimAlertSpinner.SpinnerValue, this.NumIntSpinner.SpinnerValue });

            ApplicationController.Instance.DataChangedEvent -= this.DataChangeDelegate;
        }

        private void UpdateProjConfInService(object obj)
        {
            try
            {
                //Set Project with the new data
                Project project = ApplicationController.Instance.CurrentProject;
                object[] param = obj as object[];
                project.SprintDuration = Convert.ToInt32(param[0]);
                project.AlertLimit = Convert.ToInt32(param[1]);
                project.Speed = Convert.ToInt32(param[2]);
                //Update data using web service
                ServiceLib.DataService.DataServiceClient client = new ServiceLib.DataService.DataServiceClient();
                SharedTypes.ApplicationController.Instance.IgnoreNextProjectUpdate = true;
                client.UpdateProject(project);
                client.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }


        public void DataChangeHandler(object sender, NotificationType notification)
        {
            try
            {
                // Respond only to project modifications.
                if (notification == NotificationType.ProjectModification)
                {
                    // Repopulate the taskboard with the current project.
                    this.PopulateProjectConfiguration();
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
    }
}
