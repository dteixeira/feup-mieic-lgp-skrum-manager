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
using System.Windows.Threading;
using GenericControlLib;
using System.Collections.ObjectModel;



namespace PopupSelectionControlLib
{
    /// <summary>
    /// Interaction logic for ProjectSelectionPage.xaml
    /// </summary>
    public partial class ProjectSelectionPage : UserControl , IFormPage
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

        public ProjectSelectionPage()
        {
            this.InitializeComponent();

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

            FillLetters();
        }

        private void FillLetters()
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
                    {button.ProjectImageSource = "Images/aloquete.png";}
                    else
                    {button.ProjectImageSource = "Images/mala.png";}

                    //TODO check
                    button.Width = 275;
                    button.Height = 100;
                    button.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    button.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    button.MouseLeftButtonDown += new MouseButtonEventHandler(projectSelected);
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


        public string PageName
        { get; set; }

        public string PageTitle
        { get; set; }

        public object PageValue
        { get; set; }
         
    }
}