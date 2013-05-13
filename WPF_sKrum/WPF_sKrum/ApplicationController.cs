using Kinect.Gestures.Circles;
using Kinect.Gestures.Swipes;
using Kinect.Gestures.Waves;
using Kinect.Sensor;
using Microsoft.Kinect;
using System.Collections.Generic;

namespace WPF_sKrum
{
    public class ApplicationController : NotificationService.INotificationServiceCallback
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
        public delegate void DataModificationHandler(object sender, NotificationService.NotificationType type);

        public event DataModificationHandler DataChangedEvent; 

        private KinectSensorController sensor;
        private int trackingId;
        private bool grip;
        private Dictionary<string, string> ordersRight;
        private Dictionary<string, string> ordersLeft;
        private Dictionary<string, string> ordersUp;
        private Dictionary<string, string> ordersDown;
        private string currentPage;
        private Skeleton[] skeletons;
        private NotificationService.NotificationServiceClient notifications;
        private ProjectService.ProjectServiceClient projects;
        private UserService.UserServiceClient users;

        public KinectSensorController KinectSensor
        {
            get { return this.sensor; }
            set { this.sensor = value; }
        }

        public NotificationService.NotificationServiceClient Notifications
        {
            get { return this.notifications; }
        }

        public ProjectService.ProjectServiceClient Projects
        {
            get { return this.projects; }
        }

        public UserService.UserServiceClient Users
        {
            get { return this.users; }
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

        public Dictionary<string, string> SwipeOrdersLeftToRight
        {
            get { return this.ordersLeft; }
            set { this.ordersLeft = value; }
        }

        public Dictionary<string, string> SwipeOrdersRightToLeft
        {
            get { return this.ordersRight; }
            set { this.ordersRight = value; }
        }

        public Dictionary<string, string> SwipeOrdersUptoDown
        {
            get { return this.ordersUp; }
            set { this.ordersUp = value; }
        }

        public Dictionary<string, string> SwipeOrdersDowntoUp
        {
            get { return this.ordersDown; }
            set { this.ordersDown = value; }
        }

        public string CurrentPage
        {
            get { return this.currentPage; }
            set { this.currentPage = value; }
        }

        public Skeleton[] Skeletons
        {
            get { return this.skeletons; }
            set { this.skeletons = value; }
        }

        private ApplicationController()
        {
            this.sensor = new KinectSensorController(KinectSensorType.Xbox360Sensor);
            trackingId = -1;
            grip = false;
            ordersDown = new Dictionary<string, string>();
            ordersLeft = new Dictionary<string, string>();
            ordersUp = new Dictionary<string, string>();
            ordersRight = new Dictionary<string, string>();
            currentPage = "sKrum";

            // Service clients initialisation.
            this.notifications = new NotificationService.NotificationServiceClient(new System.ServiceModel.InstanceContext(this));
            this.users = new UserService.UserServiceClient();
            this.projects = new ProjectService.ProjectServiceClient();

            // sKrum page possible transitions.
            ordersRight.Add("sKrum", "MainPage");

            // ProjectsPage page possible transitions.
            ordersRight.Add("ProjectsPage", "MainPage");
            ordersUp.Add("ProjectsPage", "UsersPage");
            ordersDown.Add("ProjectsPage", "UsersPage");

            // UsersPage page possible transitions.
            ordersRight.Add("UsersPage", "MainPage");
            ordersUp.Add("UsersPage", "ProjectsPage");
            ordersDown.Add("UsersPage", "ProjectsPage");

            // MainPage page possible transitions.
            ordersLeft.Add("MainPage", "ProjectsPage");
            ordersRight.Add("MainPage", "TaskBoardPage");

            // TaskboardPage page possible transitions.
            ordersLeft.Add("TaskBoardPage", "MainPage");
            ordersUp.Add("TaskBoardPage", "StatsUser");
            ordersDown.Add("TaskBoardPage", "StatsUser");

            // StatsUser page possible transitions.
            ordersLeft.Add("StatsUser", "MainPage");
            ordersUp.Add("StatsUser", "TaskBoardPage");
            ordersDown.Add("StatsUser", "TaskBoardPage");

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

        public void DataChanged(NotificationService.NotificationType notification)
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