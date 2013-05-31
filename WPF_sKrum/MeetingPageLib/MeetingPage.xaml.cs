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

namespace MeetingPageLib
{
    /// <summary>
    /// Interaction logic for MeetingPage.xaml
    /// </summary>
    public partial class MeetingPage : UserControl, ITargetPage
    {
        private float scrollValue = 0.0f;
        private float scrollValueNotes = 0.0f;

        private DispatcherTimer countdownTimerDelayScrollUp;
        private DispatcherTimer countdownTimerDelayScrollDown;
        private DispatcherTimer countdownTimerScrollUp;
        private DispatcherTimer countdownTimerScrollDown;


        private enum BacklogSelected { Meetings, Notes };
        private BacklogSelected currentScroll;

        private int biggestNumber = 0;


        private ObservableCollection<MeetingControl> collection = new ObservableCollection<MeetingControl>();

        public ApplicationPages PageType { get; set; }
        public ApplicationController.DataModificationHandler DataChangeDelegate { get; set; }

        public MeetingPage(object context)
        {
            InitializeComponent();
            this.PageType = ApplicationPages.MeetingPage;

            // Initialize scroll up delay timer.
            this.countdownTimerDelayScrollUp = new DispatcherTimer();
            this.countdownTimerDelayScrollUp.Tick += new EventHandler(ScrollActionDelayUp);
            this.countdownTimerDelayScrollUp.Interval = TimeSpan.FromSeconds(1);

            // Initialize scroll down delay timer.
            this.countdownTimerDelayScrollDown = new DispatcherTimer();
            this.countdownTimerDelayScrollDown.Tick += new EventHandler(ScrollActionDelayDown);
            this.countdownTimerDelayScrollDown.Interval = TimeSpan.FromSeconds(1);

            // Initialize scroll up timer.
            this.countdownTimerScrollUp = new DispatcherTimer();
            this.countdownTimerScrollUp.Tick += new EventHandler(ScrollActionUp);
            this.countdownTimerScrollUp.Interval = TimeSpan.FromSeconds(0.01);

            // Initialize scroll down timer.
            this.countdownTimerScrollDown = new DispatcherTimer();
            this.countdownTimerScrollDown.Tick += new EventHandler(ScrollActionDown);
            this.countdownTimerScrollDown.Interval = TimeSpan.FromSeconds(0.01);


            // Register for project change notifications.
            this.DataChangeDelegate = new ApplicationController.DataModificationHandler(this.DataChangeHandler);
            ApplicationController.Instance.DataChangedEvent += this.DataChangeDelegate;

            PopulateMeeting();
        }

        private void PopulateMeeting()
        {
            try
            {
                // Clears meetings.
                collection.Clear();

                // Biggest Meeting Number
                biggestNumber = 0;

                // Get meetings if possible.
                List<Meeting> meetings = ApplicationController.Instance.CurrentProject.Meetings;

                // Iterate all meetings in the project.
                foreach (var meeting in meetings)
                {
                    if (meeting.Number > biggestNumber)
                        biggestNumber = meeting.Number;

                    // Create the meeting control.
                    MeetingControl meetingControl = new MeetingControl
                    {
                        MeetingDate = meeting.Date,
                        MeetingNumber = meeting.Number,
                        MeetingNotes = meeting.Notes,
                        Meeting = meeting,
                    };
                    meetingControl.MeetingHoverEvent += ShowMeeting_Hover;
                    meetingControl.Width = Double.NaN;
                    meetingControl.Height = Double.NaN;
                    meetingControl.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    meetingControl.SetValue(Grid.ColumnProperty, 0);

                    collection.Add(meetingControl);
                }
                this.Meetings.ItemsSource = collection;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }

        }

        public void ShowMeeting_Hover(object sender, MouseEventArgs e)
        {
            this.MeetingNotes.Visibility = Visibility.Visible;
            this.NotesScroll.ScrollToVerticalOffset(0);
            this.MeetingNotes.Text = ((MeetingControl)sender).MeetingNotes;
        }

        private void ScrollActionDelayUp(object sender, EventArgs e)
        {
            this.countdownTimerScrollUp.Start();
            this.countdownTimerDelayScrollUp.Stop();
        }

        private void ScrollActionDelayDown(object sender, EventArgs e)
        {
            this.countdownTimerScrollDown.Start();
            this.countdownTimerDelayScrollDown.Stop();
        }

        private void ScrollActionUp(object sender, EventArgs e)
        {
            switch (currentScroll)
            {
                case BacklogSelected.Notes:
                    scrollValueNotes -= 10.0f;
                    if (scrollValueNotes < 0.0f) scrollValueNotes = 0.0f;
                    NotesScroll.ScrollToVerticalOffset(scrollValueNotes);
                    break;

                default:
                    scrollValue -= 10.0f;
                    if (scrollValue < 0.0f) scrollValue = 0.0f;
                    MeetingScroll.ScrollToVerticalOffset(scrollValue);
                    break;
            }
        }

        private void ScrollActionDown(object sender, EventArgs e)
        {
            switch (currentScroll)
            {
                case BacklogSelected.Notes:
                    scrollValueNotes += 10.0f;
                    if (scrollValueNotes > NotesScroll.ScrollableHeight) scrollValueNotes = (float)NotesScroll.ScrollableHeight;
                    NotesScroll.ScrollToVerticalOffset(scrollValueNotes);
                    break;

                default:
                    scrollValue += 10.0f;
                    if (scrollValue > MeetingScroll.ScrollableHeight) scrollValue = (float)MeetingScroll.ScrollableHeight;
                    MeetingScroll.ScrollToVerticalOffset(scrollValue);
                    break;
            }
        }

        private void ScrollUp_Start(object sender, MouseEventArgs e)
        {
            currentScroll = BacklogSelected.Meetings;
            this.countdownTimerDelayScrollUp.Start();
            this.countdownTimerScrollUp.Stop();
        }

        private void ScrollDown_Start(object sender, MouseEventArgs e)
        {
            currentScroll = BacklogSelected.Meetings;
            this.countdownTimerDelayScrollDown.Start();
            this.countdownTimerScrollDown.Stop();
        }

        private void ScrollUp_Cancel(object sender, MouseEventArgs e)
        {
            this.countdownTimerDelayScrollUp.Stop();
            this.countdownTimerScrollUp.Stop();
        }

        private void ScrollDown_Cancel(object sender, MouseEventArgs e)
        {
            this.countdownTimerDelayScrollDown.Stop();
            this.countdownTimerScrollDown.Stop();
        }

        private void NotesScrollUp_Start(object sender, MouseEventArgs e)
        {
            currentScroll = BacklogSelected.Notes;
            this.countdownTimerDelayScrollUp.Start();
            this.countdownTimerScrollUp.Stop();
        }

        private void NotesScrollDown_Start(object sender, MouseEventArgs e)
        {
            currentScroll = BacklogSelected.Notes;
            this.countdownTimerDelayScrollDown.Start();
            this.countdownTimerScrollDown.Stop();
        }

        public PageChange PageChangeTarget(PageChangeDirection direction)
        {
            switch (direction)
            {
                case PageChangeDirection.Down:
                    return null;
                case PageChangeDirection.Left:
                    return new PageChange { Context = null, Page = ApplicationPages.MainPage };
                case PageChangeDirection.Right:
                    return null;
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
            directions[PageChangeDirection.Left] = "MENU INICIAL";
            directions[PageChangeDirection.Right] = null;
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
                    this.PopulateMeeting();
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        private void SideMenuControl_AddClickEvent(object obj, MouseEventArgs e)
        {
            PopupFormControlLib.FormWindow form = new PopupFormControlLib.FormWindow();
            PopupFormControlLib.TextAreaPage notesPage = new PopupFormControlLib.TextAreaPage { PageName = "notes", PageTitle = "Notas" };
            form.FormPages.Add(notesPage);
            ApplicationController.Instance.ApplicationWindow.SetWindowFade(true);
            form.ShowDialog();
            if (form.Success)
            {
                string notes = (string)form["notes"].PageValue;
                Meeting meeting = new Meeting
                {
                    Notes = notes,
                    Date = System.DateTime.Now,
                    ProjectID = ApplicationController.Instance.CurrentProject.ProjectID,
                    Number = biggestNumber + 1,
                };
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(AddMeeting));
                thread.Start(meeting);
            }
            ApplicationController.Instance.ApplicationWindow.SetWindowFade(false);

        }

        public void AddMeeting(object obj)
        {
            DataServiceClient client = new DataServiceClient();
            client.CreateMeeting((Meeting)obj);
            client.Close();
        }

        private void SideMenuControl_RemoveDropEvent(object obj, DragEventArgs e)
        {
            var dataObj = e.Data as DataObject;
            MeetingControl meetingControl = dataObj.GetData("MeetingControl") as MeetingControl;
            if (meetingControl != null)
            {
                PopupFormControlLib.YesNoFormWindow form = new PopupFormControlLib.YesNoFormWindow();
                form.FormTitle = "Apagar a Meeting?";
                ApplicationController.Instance.ApplicationWindow.SetWindowFade(true);
                form.ShowDialog();
                if (form.Success)
                {
                    System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(DeleteMeeting));
                    thread.Start(meetingControl.Meeting);
                }
                ApplicationController.Instance.ApplicationWindow.SetWindowFade(false);
            }
        }

        public void DeleteMeeting(object obj)
        {
            Meeting meeting = (Meeting)obj;
            DataServiceClient client = new DataServiceClient();
            client.DeleteMeeting(meeting.MeetingID);
            client.Close();
        }

    }
}
