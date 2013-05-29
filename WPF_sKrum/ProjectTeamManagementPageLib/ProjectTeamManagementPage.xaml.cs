using GenericControlLib;
using ServiceLib.DataService;
using ServiceLib.NotificationService;
using SharedTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;

namespace ProjectTeamManagementPageLib
{
    /// <summary>
    /// Interaction logic for MeetingPage.xaml
    /// </summary>
    public partial class ProjectTeamManagementPage : UserControl, ITargetPage
    {
        private float scrollValue = 0.0f;
        private DispatcherTimer countdownTimerDelayScrollLeft;
        private DispatcherTimer countdownTimerDelayScrollRight;
        private DispatcherTimer countdownTimerScrollLeft;
        private DispatcherTimer countdownTimerScrollRight;

        public ApplicationPages PageType { get; set; }
        public ApplicationController.DataModificationHandler DataChangeDelegate { get; set; }

        private ObservableCollection<UserButtonControl> teamCollection = new ObservableCollection<UserButtonControl>();
        

        public ProjectTeamManagementPage(object context)
        {
            InitializeComponent();
            this.PageType = ApplicationPages.ProjectTeamManagementPage;

            // Initialize scroll up delay timer.
            this.countdownTimerDelayScrollLeft = new DispatcherTimer();
            this.countdownTimerDelayScrollLeft.Tick += new EventHandler(ScrollActionDelayLeft);
            this.countdownTimerDelayScrollLeft.Interval = TimeSpan.FromSeconds(1);

            // Initialize scroll down delay timer.
            this.countdownTimerDelayScrollRight = new DispatcherTimer();
            this.countdownTimerDelayScrollRight.Tick += new EventHandler(ScrollActionDelayRight);
            this.countdownTimerDelayScrollRight.Interval = TimeSpan.FromSeconds(1);

            // Initialize scroll up timer.
            this.countdownTimerScrollLeft = new DispatcherTimer();
            this.countdownTimerScrollLeft.Tick += new EventHandler(ScrollActionLeft);
            this.countdownTimerScrollLeft.Interval = TimeSpan.FromSeconds(0.01);

            // Initialize scroll down timer.
            this.countdownTimerScrollRight = new DispatcherTimer();
            this.countdownTimerScrollRight.Tick += new EventHandler(ScrollActionRight);
            this.countdownTimerScrollRight.Interval = TimeSpan.FromSeconds(0.01);

            // Register for project change notifications.
            this.DataChangeDelegate = new ApplicationController.DataModificationHandler(this.DataChangeHandler);
            ApplicationController.Instance.DataChangedEvent += this.DataChangeDelegate;

            PopulateProjectTeamManagementPage();
        }

        private void PopulateProjectTeamManagementPage()
        {
            try
            {
                // Get team.
                Project project = ApplicationController.Instance.CurrentProject;

                ServiceLib.DataService.DataServiceClient client = new DataServiceClient();
                List<Person> team = client.GetAllPeopleInProject(ApplicationController.Instance.CurrentProject.ProjectID);
                client.Close();

                // Remove existing scrum master control.
                bool found = false;
                this.Contents.Children.Clear();
                foreach (UIElement child in this.ScrumMasterArea.Children)
                {
                    if (child is UserButtonVerticalControl)
                    {
                        this.ScrumMasterArea.Children.Remove(child);
                        break;
                    }
                }

                int row = 4;
                int column = -1;
                foreach (Person p in team)
                {
                    foreach (Role r in p.Roles)
                    {
                        if (r.RoleDescription == RoleDescription.ScrumMaster && r.ProjectID == ApplicationController.Instance.CurrentProject.ProjectID)
                        {
                            UserButtonVerticalControl scrumMasterControl = new UserButtonVerticalControl();
                            scrumMasterControl.UserName = p.Name;
                            scrumMasterControl.UserPhoto = p.PhotoURL;
                            scrumMasterControl.Width = Double.NaN;
                            scrumMasterControl.Height = double.NaN;
                            scrumMasterControl.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                            scrumMasterControl.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                            scrumMasterControl.SetValue(Grid.RowProperty, 1);
                            scrumMasterControl.IsHitTestVisible = false;
                            this.ScrumMasterArea.Children.Add(scrumMasterControl);
                            found = true;
                            break;
                        }
                    }

                    // Create person control.
                    GenericControlLib.UserButtonControl button = new GenericControlLib.UserButtonControl();
                    button.UserName = p.Name;

                    // Create proper grids.
                    ++row;
                    if (row > 3)
                    {
                        row = 0;
                        column++;
                    }
                    if (row == 0)
                    {
                        ColumnDefinition columnDef = new ColumnDefinition();
                        columnDef.Width = new GridLength(1, GridUnitType.Star);
                        this.Contents.ColumnDefinitions.Add(columnDef);
                    }
                    button.UserPhoto = p.PhotoURL;

                    // Create persons control.
                    button.Width = Double.NaN;
                    button.Height = Double.NaN;
                    button.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    button.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    button.IsDraggable = true;
                    button.Person = p;
                    button.SetValue(Grid.ColumnProperty, column);
                    button.SetValue(Grid.RowProperty, row);
                    button.Margin = new Thickness(10, 10, 10, 10);
                    this.Contents.Children.Add(button);
                }

                // Add default scrum master.
                if (!found)
                {
                    UserButtonVerticalControl scrumMasterControl = new UserButtonVerticalControl();
                    scrumMasterControl.UserName = "Utilizador";
                    scrumMasterControl.Width = Double.NaN;
                    scrumMasterControl.Height = double.NaN;
                    scrumMasterControl.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                    scrumMasterControl.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    scrumMasterControl.SetValue(Grid.RowProperty, 1);
                    scrumMasterControl.IsHitTestVisible = false;
                    this.ScrumMasterArea.Children.Add(scrumMasterControl);
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        private void ScrollActionDelayLeft(object sender, EventArgs e)
        {
            this.countdownTimerScrollLeft.Start();
            this.countdownTimerDelayScrollLeft.Stop();
        }

        private void ScrollActionDelayRight(object sender, EventArgs e)
        {
            this.countdownTimerScrollRight.Start();
            this.countdownTimerDelayScrollRight.Stop();
        }

        private void ScrollActionLeft(object sender, EventArgs e)
        {
            scrollValue -= 10.0f;
            if (scrollValue < 0.0f) scrollValue = 0.0f;
            TeamScroll.ScrollToHorizontalOffset(scrollValue);
        }

        private void ScrollActionRight(object sender, EventArgs e)
        {
            scrollValue += 10.0f;
            if (scrollValue > TeamScroll.ScrollableWidth) scrollValue = (float)TeamScroll.ScrollableWidth;
            TeamScroll.ScrollToHorizontalOffset(scrollValue);
        }

        private void ScrollLeft_Start(object sender, MouseEventArgs e)
        {
            this.countdownTimerDelayScrollLeft.Start();
            this.countdownTimerScrollLeft.Stop();
        }

        private void ScrollRight_Start(object sender, MouseEventArgs e)
        {
            this.countdownTimerDelayScrollRight.Start();
            this.countdownTimerScrollRight.Stop();
        }

        private void ScrollLeft_Cancel(object sender, MouseEventArgs e)
        {
            this.countdownTimerDelayScrollLeft.Stop();
            this.countdownTimerScrollLeft.Stop();
        }

        private void ScrollRight_Cancel(object sender, MouseEventArgs e)
        {
            this.countdownTimerDelayScrollRight.Stop();
            this.countdownTimerScrollRight.Stop();
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
                    return null;
                default:
                    return null;
            }
        }

        public void SetupNavigation()
        {
            System.Collections.Generic.Dictionary<PageChangeDirection, string> directions = new System.Collections.Generic.Dictionary<PageChangeDirection, string>();
            directions[PageChangeDirection.Up] = null;
            directions[PageChangeDirection.Down] = null;
            directions[PageChangeDirection.Left] = "GESTÃO DE PROJECTOS";
            directions[PageChangeDirection.Right] = "MENU INICIAL";
            ApplicationController.Instance.ApplicationWindow.SetupNavigation(directions);
        }

        public void UnloadPage()
        {
            // Unregister for notifications.
            ApplicationController.Instance.DataChangedEvent -= this.DataChangeDelegate;
        }


        public void DataChangeHandler(object sender, NotificationType notification)
        {
            try
            {
                // Respond only to project modifications.
                if (notification == NotificationType.ProjectModification)
                {
                    // Repopulate the taskboard with the current project.
                    this.PopulateProjectTeamManagementPage();
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        private void Rectangle_Drop(object sender, DragEventArgs e)
        {
            // Confirm deletion.
            var dataObj = e.Data as DataObject;
            GenericControlLib.UserButtonControl userControl = dataObj.GetData("UserButtonControl") as GenericControlLib.UserButtonControl;

            // Define assigned time.
            ApplicationController.Instance.ApplicationWindow.SetWindowFade(true);
            PopupFormControlLib.FormWindow workForm = new PopupFormControlLib.FormWindow();
            PopupFormControlLib.SpinnerPage workPage = new PopupFormControlLib.SpinnerPage { PageName = "work", PageTitle = "Tempo Disponível", Min = 0.1, Max = 1, Increment = 0.1 };
            workForm.FormPages.Add(workPage);
            workForm.ShowDialog();
            if (workForm.Success)
            {
                Role role = new Role
                {
                    AssignedTime = (double)workForm["work"].PageValue,
                    PersonID = userControl.Person.PersonID,
                    ProjectID = ApplicationController.Instance.CurrentProject.ProjectID,
                    RoleDescription = RoleDescription.ScrumMaster
                };
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ChangeScrumMaster));
                thread.Start(role);
            }
            ApplicationController.Instance.ApplicationWindow.SetWindowFade(false);
        }

        public void ChangeScrumMaster(object obj)
        {
            Role role = (Role)obj;
            DataServiceClient client = new DataServiceClient();
            var people = client.GetAllPeopleInProject(ApplicationController.Instance.CurrentProject.ProjectID);

            // Remove current scrum master if any.
            bool found = false;
            foreach (Person p in people)
            {
                foreach (Role r in p.Roles)
                {
                    if (r.ProjectID == ApplicationController.Instance.CurrentProject.ProjectID && r.RoleDescription == RoleDescription.ScrumMaster)
                    {
                        ApplicationController.Instance.IgnoreNextProjectUpdate = true;
                        client.DeleteRole(r.RoleID);
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    break;
                }
            }

            // Add new scrum master
            client.CreateRole(role);
            client.Close();
        }

        public void AddPerson_Click(object sender, MouseEventArgs e)
        {
            // Select a user.
            ApplicationController.Instance.ApplicationWindow.SetWindowFade(true);
            PopupSelectionControlLib.SelectionWindow userForm = new PopupSelectionControlLib.SelectionWindow();
            PopupSelectionControlLib.UserSelectionPage userPage = new PopupSelectionControlLib.UserSelectionPage();
            userPage.PageTitle = "Escolha uma Pessoa";
            userForm.FormPage = userPage;
            userForm.ShowDialog();
            if (userForm.Success)
            {
                ServiceLib.DataService.Person person = (ServiceLib.DataService.Person)userForm.Result;
                foreach (Role r in person.Roles)
                {
                    // Return if the role already exists.
                    if (r.RoleDescription == RoleDescription.TeamMember && r.ProjectID == ApplicationController.Instance.CurrentProject.ProjectID)
                    {
                        return;
                    }
                }

                // Define assigned time.
                PopupFormControlLib.FormWindow workForm = new PopupFormControlLib.FormWindow();
                PopupFormControlLib.SpinnerPage workPage = new PopupFormControlLib.SpinnerPage { PageName = "work", PageTitle = "Tempo Disponível", Min = 0.1, Max = 1, Increment = 0.1 };
                workForm.FormPages.Add(workPage);
                workForm.ShowDialog();
                if (workForm.Success)
                {
                    Role role = new Role
                    {
                        AssignedTime = (double)workForm["work"].PageValue,
                        PersonID = person.PersonID,
                        ProjectID = ApplicationController.Instance.CurrentProject.ProjectID,
                        RoleDescription = RoleDescription.TeamMember
                    };
                    System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(AddPerson));
                    thread.Start(role);
                }
            }
            ApplicationController.Instance.ApplicationWindow.SetWindowFade(false);
        }

        public void AddPerson(object obj)
        {
            Role role = (Role)obj;
            DataServiceClient client = new DataServiceClient();
            client.CreateRole(role);
            client.Close();
        }

        public void DeletePerson_Drop(object sender, DragEventArgs e)
        {
            // Confirm deletion.
            var dataObj = e.Data as DataObject;
            GenericControlLib.UserButtonControl userControl = dataObj.GetData("UserButtonControl") as GenericControlLib.UserButtonControl;
            ApplicationController.Instance.ApplicationWindow.SetWindowFade(true);
            PopupFormControlLib.YesNoFormWindow form = new PopupFormControlLib.YesNoFormWindow();
            form.FormTitle = "Apagar Membro da Equipa?";
            form.ShowDialog();
            if (form.Success)
            {
                // Remove role in this project.
                foreach (Role role in userControl.Person.Roles)
                {
                    if (role.ProjectID == ApplicationController.Instance.CurrentProject.ProjectID)
                    {
                        System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(DeletePerson));
                        thread.Start(role);
                    }
                }
            }
            ApplicationController.Instance.ApplicationWindow.SetWindowFade(false);
        }

        public void DeletePerson(object obj)
        {
            Role role = (Role)obj;
            DataServiceClient client = new DataServiceClient();
            client.DeleteRole(role.RoleID);
            client.Close();
        }
    }
}
