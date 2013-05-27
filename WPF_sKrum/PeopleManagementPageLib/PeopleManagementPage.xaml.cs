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

namespace PeopleManagementPageLib
{
    /// <summary>
    /// Interaction logic for PeopleManagementPage.xaml
    /// </summary>
    public partial class PeopleManagementPage : UserControl, ITargetPage
    {
        private enum ScrollSelected { Letters, Content };
        private ScrollSelected currentScroll;
        private float scrollValueContent = 0.0f;
        private float scrollValueLetters = 0.0f;
        private string currentLetter;

        private Dictionary<string, List<Person>> dic;

        //Timers for the content/letter scroll
        private DispatcherTimer countdownTimerDelayScrollLeft;
        private DispatcherTimer countdownTimerDelayScrollRight;
        private DispatcherTimer countdownTimerScrollLeft;
        private DispatcherTimer countdownTimerScrollRight;

        public ApplicationPages PageType { get; set; }
        public ApplicationController.DataModificationHandler DataChangeDelegate { get; set; }

        public PeopleManagementPage(object context)
        {
            InitializeComponent();
            this.PageType = ApplicationPages.MeetingPage;

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

            PopulatePeopleManagement();
        }

        private void PopulatePeopleManagement()
        {
            try
            {
                //Build the dictionary with all the persons in the database
                dic = new Dictionary<string, List<Person>>();
                List<Person> persons = ApplicationController.Instance.People;

                var x = (from p in persons
                         orderby p.Name ascending
                         select p).ToList<Person>();
                persons = x;

                foreach (int letter in Enumerable.Range('A', 'Z' - 'A' + 1))
                {
                    dic[Convert.ToChar(letter).ToString()] = (from p in persons
                                                              where p.Name[0].ToString().ToUpper().Equals(Convert.ToChar(letter).ToString())
                                                              select p).ToList<Person>();
                }

                //Fill the scroller with  the letters
                GenericControlLib.LetterControl letterA = null;
                foreach (string s in dic.Keys)
                {
                    GenericControlLib.LetterControl letra = new GenericControlLib.LetterControl();
                    letra.LetterText = s;
                    letra.Width = 120;
                    letra.Height = Double.NaN;
                    letra.LetterSize = 80;
                    letra.MouseLeftButtonDown += new MouseButtonEventHandler(letterSelected);
                    Letters.Children.Add(letra);

                    // Save letter 'A'.
                    if (s == "A")
                    {
                        letterA = letra;
                        this.currentLetter = "A";
                    }
                }

                // Select first letter.
                this.letterSelected(letterA, null);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        private void letterSelected(object sender, EventArgs e)
        {
            // Handle visual selection
            GenericControlLib.LetterControl letter = (GenericControlLib.LetterControl)sender;
            foreach (var child in this.Letters.Children)
            {
                ((GenericControlLib.LetterControl)child).BackgroundRectangleStyle = "RectangleStyle1";
                ((GenericControlLib.LetterControl)child).LetterStyle = "TextBlockStyle";
            }
            letter.BackgroundRectangleStyle = "RectangleSelectedStyle";
            letter.LetterStyle = "TextBlockSelectedStyle";

            // Fill with people.
            this.currentLetter = letter.LetterText;
            FillPersons(dic[letter.LetterText]);

        }

        /// <summary>
        ///     Fills the content placeholder with the user usercontrols
        /// </summary>
        /// <param name="persons">List of persons with name started by the desired letter</param>
        private void FillPersons(List<Person> persons)
        {
            try
            {
                this.Contents.Children.Clear();
                int row = 3;
                int column = -1;
                foreach (Person p in persons)
                {
                    // Create person control.
                    GenericControlLib.UserButtonControl button = new GenericControlLib.UserButtonControl();
                    button.UserName = p.Name;

                    // Create proper grids.
                    ++row;
                    if (row > 2)
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
                    //button.Margin = new Thickness(10, 10, 10, 10);
                    this.Contents.Children.Add(button);
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        private void userSelected(object sender, EventArgs e)
        {
            GenericControlLib.UserButtonControl person = (GenericControlLib.UserButtonControl)sender;
            MessageBox.Show(person.UserName);
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
                    return new PageChange { Context = null, Page = ApplicationPages.ProjectManagementPage };
                case PageChangeDirection.Left:
                    return null;
                case PageChangeDirection.Right:
                    return new PageChange { Context = null, Page = ApplicationPages.ProjectConfigurationPage };
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
            directions[PageChangeDirection.Down] = "GESTÃO DE PROJECTOS";
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
                // Respond only to user modifications.
                if (notification == NotificationType.GlobalPersonModification)
                {
                    // Repopulate with the changes.
                    this.PopulatePeopleManagement();
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
    }
}
