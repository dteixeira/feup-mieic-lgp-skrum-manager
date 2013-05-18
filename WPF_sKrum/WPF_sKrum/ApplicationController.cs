using Kinect.Gestures.Circles;
using Kinect.Gestures.Swipes;
using Kinect.Gestures.Waves;
using Kinect.Sensor;
using Microsoft.Kinect;
using System.Collections.Generic;
using ServiceLib.NotificationService;
using ServiceLib.DataService;

namespace WPFApplication
{
    public class ApplicationController : INotificationServiceCallback
    {
        private static ApplicationController instance;

        public static ApplicationController Instance
        {
            get
            {
                if (ApplicationController.instance == null)
                {
                    ApplicationController.instance = new ApplicationController();
                }
                return ApplicationController.instance;
            }
        }

        /// <summary>
        /// Method signature to register to data changed events.
        /// </summary>
        /// <param name="sender">Object that triggered the event</param>
        /// <param name="type">Type of the data modification event that occured.</param>
        public delegate void DataModificationHandler(object sender, NotificationType type);

        /// <summary>
        /// Used to register for service data change notification.
        /// </summary>
        public event DataModificationHandler DataChangedEvent;

        private KinectSensorController sensor;
        private int trackingId;
        private bool grip;
        private Dictionary<ApplicationPages, ApplicationPages> pagesRight;
        private Dictionary<ApplicationPages, ApplicationPages> pagesLeft;
        private Dictionary<ApplicationPages, ApplicationPages> pagesUp;
        private Dictionary<ApplicationPages, ApplicationPages> pagesDown;
        private ApplicationPages currentPage;
        private Skeleton[] skeletons;
        private NotificationServiceClient notifications;
        private DataServiceClient data;

        private Project[] data_projects;
        private Person[] data_users;
        private Story[] userstories;
        private int cur_project;
        private int cur_user;


        public KinectSensorController KinectSensor
        {
            get { return this.sensor; }
        }

        public NotificationServiceClient Notifications
        {
            get { return this.notifications; }
        }

        public DataServiceClient Data
        {
            get { return this.data; }
        }

        public int TrackingId
        {
            get { return this.trackingId; }
            set { this.trackingId = value; }
        }

        public bool Gripping
        {
            get { return this.grip; }
            set { this.grip = value; }
        }

        public Dictionary<ApplicationPages, ApplicationPages> PagesLeft
        {
            get { return this.pagesLeft; }
        }

        public Dictionary<ApplicationPages, ApplicationPages> PagesRight
        {
            get { return this.pagesRight; }
        }

        public Dictionary<ApplicationPages, ApplicationPages> PagesUp
        {
            get { return this.pagesUp; }
        }

        public Dictionary<ApplicationPages, ApplicationPages> PagesDown
        {
            get { return this.pagesDown; }
        }

        public ApplicationPages CurrentPage
        {
            get { return this.currentPage; }
            set { this.currentPage = value; }
        }

        public Skeleton[] Skeletons
        {
            get { return this.skeletons; }
            set { this.skeletons = value; }
        }

        public Story[] UserStories
        {
            get { return this.userstories; }
            set { this.userstories = value; }
        }

        private ApplicationController()
        {
            this.sensor = new KinectSensorController(KinectSensorType.Xbox360Sensor);
            this.trackingId = -1;
            this.grip = false;
            this.pagesDown = new Dictionary<ApplicationPages, ApplicationPages>();
            this.pagesLeft = new Dictionary<ApplicationPages, ApplicationPages>();
            this.pagesUp = new Dictionary<ApplicationPages, ApplicationPages>();
            this.pagesRight = new Dictionary<ApplicationPages, ApplicationPages>();
            this.currentPage = ApplicationPages.sKrum;
            this.cur_project = 0;
            this.cur_user = 0;

            // Service clients initialisation.
            this.notifications = new NotificationServiceClient(new System.ServiceModel.InstanceContext(this));
            this.data = new DataServiceClient();

            // Register for global notifications.
            this.notifications.Subscribe(-1);


            this.data_projects = this.data.GetAllProjects();
            this.data_users = this.data.GetAllPeople();

            //this.userstories = this.data.GetAllStoriesInProject(this.data_projects[this.cur_project].ProjectID);      
            

            // sKrum page possible transitions.
            this.pagesRight.Add(ApplicationPages.sKrum, ApplicationPages.MainPage);

            // ProjectsPage page possible transitions.
            this.pagesRight.Add(ApplicationPages.ProjectsPage, ApplicationPages.MainPage);
            this.pagesUp.Add(ApplicationPages.ProjectsPage, ApplicationPages.UsersPage);
            this.pagesDown.Add(ApplicationPages.ProjectsPage, ApplicationPages.UsersPage);

            // UsersPage page possible transitions.
            this.pagesRight.Add(ApplicationPages.UsersPage, ApplicationPages.MainPage);
            this.pagesUp.Add(ApplicationPages.UsersPage, ApplicationPages.ProjectsPage);
            this.pagesDown.Add(ApplicationPages.UsersPage, ApplicationPages.ProjectsPage);

            // MainPage page possible transitions.
            this.pagesLeft.Add(ApplicationPages.MainPage, ApplicationPages.ProjectsPage);
            this.pagesRight.Add(ApplicationPages.MainPage, ApplicationPages.TaskBoardPage);

            // TaskboardPage page possible transitions.
            this.pagesLeft.Add(ApplicationPages.TaskBoardPage, ApplicationPages.MainPage);
            this.pagesUp.Add(ApplicationPages.TaskBoardPage, ApplicationPages.UserStatsPage);
            this.pagesDown.Add(ApplicationPages.TaskBoardPage, ApplicationPages.UserStatsPage);

            // StatsUser page possible transitions.
            this.pagesLeft.Add(ApplicationPages.UserStatsPage, ApplicationPages.MainPage);
            this.pagesUp.Add(ApplicationPages.UserStatsPage, ApplicationPages.TaskBoardPage);
            this.pagesDown.Add(ApplicationPages.UserStatsPage, ApplicationPages.TaskBoardPage);

            // Setup gestures if a sensor was found.
            if (sensor.FoundSensor())
            {
                sensor.Gestures.AddGesture(new KinectGestureWaveRightHand());
                sensor.Gestures.AddGesture(new KinectGestureWaveLeftHand());
                sensor.Gestures.AddGesture(new KinectGestureSwipeRightToLeft());
                sensor.Gestures.AddGesture(new KinectGestureSwipeLeftToRight());
                sensor.Gestures.AddGesture(new KinectGestureCircleRightHand());
                sensor.Gestures.AddGesture(new KinectGestureCircleLeftHand());
            }
        }

        /// <summary>
        /// Notifies all registered clients of a service data modification.
        /// </summary>
        /// <param name="notification">The type of modification to be notified</param>
        public void DataChanged(NotificationType notification)
        {
            if (DataChangedEvent != null)
            {
                System.Delegate[] delegateList = DataChangedEvent.GetInvocationList();
                foreach (DataModificationHandler handler in delegateList)
                {
                    try
                    {
                        handler(this, notification);
                    }
                    catch (System.Exception e)
                    {
                        System.Console.WriteLine(e.Message);
                        DataChangedEvent -= handler;
                    }
                }
            }
        }
    }
}