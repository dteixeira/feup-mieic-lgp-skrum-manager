using ServiceLib.DataService;
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
using System.Windows.Threading;

namespace ProjectManagementPageLib
{
    /// <summary>
    /// Interaction logic for ProjectConfigurationPage.xaml
    /// </summary>
    public partial class ProjectManagementPage : UserControl, ITargetPage
    {
        private enum ScrollSelected { Letters, Content };
        private ScrollSelected currentScroll;
        private float scrollValueContent = 0.0f;
        private float scrollValueLetters = 0.0f;

        private Dictionary<string, List<Project>> dic;

        //Timers for the content/letter scroll
        private DispatcherTimer countdownTimerDelayScrollLeft;
        private DispatcherTimer countdownTimerDelayScrollRight;
        private DispatcherTimer countdownTimerScrollLeft;
        private DispatcherTimer countdownTimerScrollRight;

        public ApplicationPages PageType { get; set; }
        public ApplicationController.DataModificationHandler DataChangeDelegate { get; set; }

        public ProjectManagementPage(object context)
        {
            InitializeComponent();
            this.PageType = ApplicationPages.ProjectManagementPage;

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

            PopulateProjectManagementPage();
        }



        private void PopulateProjectManagementPage()
        {
            try
            {
                //Build the dictionary with all the projects in the database
                dic = new Dictionary<string, List<Project>>();
                List<Project> projects = ApplicationController.Instance.Projects;

                var x = (from p in projects
                         orderby p.Name ascending
                         select p).ToList<Project>();
                projects = x;

                foreach (int letter in Enumerable.Range('A', 'Z' - 'A' + 1))
                {
                    dic[Convert.ToChar(letter).ToString()] = (from p in projects
                                                              where p.Name[0].ToString().ToUpper().Equals(Convert.ToChar(letter).ToString())
                                                              select p).ToList<Project>();
                }

                //Fill the scroller with  the letters
                foreach (string s in dic.Keys)
                {
                    GenericControlLib.LetterControl letra = new GenericControlLib.LetterControl();
                    letra.LetterText = s;
                    letra.Width = Double.NaN;
                    letra.Height = Double.NaN;
                    letra.MouseLeftButtonDown += new MouseButtonEventHandler(letterSelected);
                    Letters.Children.Add(letra);
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        private void letterSelected(object sender, EventArgs e)
        {
            //TODO
            //Limpar formatação das outras letras
            //Mudar formatação da letra seleccionada
            GenericControlLib.LetterControl letra = (GenericControlLib.LetterControl)sender;
            FillProjects(dic[letra.LetterText]);

        }

        /// <summary>
        ///     Fills the content placeholder with the project usercontrols
        /// </summary>
        /// <param name="projects">List of projects with name started by the desired letter</param>
        private void FillProjects(List<Project> projects)
        {
            try
            {
                this.Contents.Children.Clear();
                foreach (Project p in projects)
                {
                    GenericControlLib.ProjectButtonControl button = new GenericControlLib.ProjectButtonControl();
                    button.ProjectName = p.Name;
                    if (p.Password != "")
                    { button.ProjectImageSource = "Images/aloquete.png"; }
                    else
                    { button.ProjectImageSource = "Images/mala.png"; }

                    //TODO check
                    button.Width = 300;
                    button.Height = 100;
                    button.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    button.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    button.IsDraggable = true;
                    button.Project = p;
                    this.Contents.Children.Add(button);
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        private void projectSelected(object sender, EventArgs e)
        {
            GenericControlLib.ProjectButtonControl project = (GenericControlLib.ProjectButtonControl)sender;
            MessageBox.Show(project.ProjectName);
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
            switch (currentScroll)
            {
                case ScrollSelected.Letters:
                    scrollValueLetters -= 10.0f;
                    if (scrollValueLetters < 0.0f) scrollValueLetters = 0.0f;
                    LetterScroll.ScrollToHorizontalOffset(scrollValueLetters);
                    break;
                default:
                    scrollValueContent -= 10.0f;
                    if (scrollValueContent < 0.0f) scrollValueContent = 0.0f;
                    ContentScroll.ScrollToHorizontalOffset(scrollValueContent);
                    break;
            }
        }

        private void ScrollActionRight(object sender, EventArgs e)
        {

            switch (currentScroll)
            {
                case ScrollSelected.Letters:
                    scrollValueLetters += 10.0f;
                    if (scrollValueLetters > LetterScroll.ScrollableWidth) scrollValueLetters = (float)LetterScroll.ScrollableWidth;
                    LetterScroll.ScrollToHorizontalOffset(scrollValueLetters);
                    break;
                default:
                    scrollValueContent += 10.0f;
                    if (scrollValueContent > ContentScroll.ScrollableWidth) scrollValueContent = (float)ContentScroll.ScrollableWidth;
                    ContentScroll.ScrollToHorizontalOffset(scrollValueContent);
                    break;
            }

        }

        private void ScrollLeft_Start(object sender, MouseEventArgs e)
        {
            currentScroll = ScrollSelected.Letters;
            this.countdownTimerDelayScrollLeft.Start();
            this.countdownTimerScrollLeft.Stop();
        }

        private void ScrollRight_Start(object sender, MouseEventArgs e)
        {
            currentScroll = ScrollSelected.Letters;
            this.countdownTimerDelayScrollRight.Start();
            this.countdownTimerScrollRight.Stop();
        }

        private void ContentScrollLeft_Start(object sender, MouseEventArgs e)
        {
            currentScroll = ScrollSelected.Content;
            this.countdownTimerDelayScrollLeft.Start();
            this.countdownTimerScrollLeft.Stop();
        }

        private void ContentScrollRight_Start(object sender, MouseEventArgs e)
        {
            currentScroll = ScrollSelected.Content;
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
                    return null;
                case PageChangeDirection.Right:
                    return new PageChange { Context = null, Page = ApplicationPages.ProjectConfigurationPage };
                case PageChangeDirection.Up:
                    return new PageChange { Context = null, Page = ApplicationPages.PeopleManagementPage };
                default:
                    return null;
            }
        }

        public void SetupNavigation()
        {
            System.Collections.Generic.Dictionary<PageChangeDirection, string> directions = new System.Collections.Generic.Dictionary<PageChangeDirection, string>();
            directions[PageChangeDirection.Up] = "GESTÃO DE UTILIZADORES";
            directions[PageChangeDirection.Down] = null;
            directions[PageChangeDirection.Left] = null;
            directions[PageChangeDirection.Right] = "AJUSTES DE PROJECTO";
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
                    this.PopulateProjectManagementPage();
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
    }
}
