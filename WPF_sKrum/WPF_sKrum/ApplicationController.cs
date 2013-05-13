using Kinect.Gestures.Circles;
using Kinect.Gestures.Swipes;
using Kinect.Gestures.Waves;
using Kinect.Sensor;
using Microsoft.Kinect;
using System.Collections.Generic;

namespace WPF_sKrum
{
    public class ApplicationController
    {
        private static ApplicationController instance;

        public static ApplicationController Instance
        {
            get
            {
                if (ApplicationController.instance == null)
                    ApplicationController.instance = new ApplicationController();
                return ApplicationController.instance;
            }
        }

        private KinectSensorController sensor;
        private int tracking;
        private bool grip;
        private Dictionary<string, string> orders_right;
        private Dictionary<string, string> orders_left;
        private Dictionary<string, string> orders_up;
        private Dictionary<string, string> orders_down;
        private string cpage;
        private Skeleton[] skeletons_array;

        public KinectSensorController KinectSensor
        {
            get { return this.sensor; }
            set { this.sensor = value; }
        }

        public int TrackingId
        {
            get { return this.tracking; }
            set { this.tracking = value; }
        }

        public bool Gripping
        {
            get { return this.grip; }
            set { this.grip = value; }
        }

        public Dictionary<string, string> SwipeOrdersLeftToRight
        {
            get { return this.orders_left; }
            set { this.orders_left = value; }
        }

        public Dictionary<string, string> SwipeOrdersRightToLeft
        {
            get { return this.orders_right; }
            set { this.orders_right = value; }
        }

        public Dictionary<string, string> SwipeOrdersUptoDown
        {
            get { return this.orders_up; }
            set { this.orders_up = value; }
        }

        public Dictionary<string, string> SwipeOrdersDowntoUp
        {
            get { return this.orders_down; }
            set { this.orders_down = value; }
        }

        public string currentPage
        {
            get { return this.cpage; }
            set { this.cpage = value; }
        }

        public Skeleton[] skeletons
        {
            get { return this.skeletons_array; }
            set { this.skeletons_array = value; }
        }

        private ApplicationController()
        {
            this.sensor = new KinectSensorController(KinectSensorType.Xbox360Sensor);
            tracking = -1;
            grip = false;
            orders_down = new Dictionary<string, string>();
            orders_left = new Dictionary<string, string>();
            orders_up = new Dictionary<string, string>();
            orders_right = new Dictionary<string, string>();
            cpage = "sKrum";

            orders_right.Add("sKrum", "MainPage");

            orders_right.Add("ProjectsPage", "MainPage");
            orders_up.Add("ProjectsPage", "UsersPage");
            orders_down.Add("ProjectsPage", "UsersPage");

            orders_right.Add("UsersPage", "MainPage");
            orders_up.Add("UsersPage", "ProjectsPage");
            orders_down.Add("UsersPage", "ProjectsPage");

            orders_left.Add("MainPage", "ProjectsPage");
            orders_right.Add("MainPage", "TaskBoardPage");

            orders_left.Add("TaskBoardPage", "MainPage");
            orders_up.Add("TaskBoardPage", "StatsUser");
            orders_down.Add("TaskBoardPage", "StatsUser");

            orders_left.Add("StatsUser", "MainPage");
            orders_up.Add("StatsUser", "TaskBoardPage");
            orders_down.Add("StatsUser", "TaskBoardPage");

            if (!sensor.FoundSensor())
            {
                //this.Close();
                return;
            }

            // Setup gestures.
            sensor.Gestures.AddGesture(new KinectGestureWaveRightHand());
            sensor.Gestures.AddGesture(new KinectGestureWaveLeftHand());
            sensor.Gestures.AddGesture(new KinectGestureSwipeRightToLeft());
            sensor.Gestures.AddGesture(new KinectGestureSwipeLeftToRight());
            sensor.Gestures.AddGesture(new KinectGestureCircleRightHand());
            sensor.Gestures.AddGesture(new KinectGestureCircleLeftHand());
        }
    }
}