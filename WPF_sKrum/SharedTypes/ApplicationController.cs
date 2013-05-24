using Kinect.Gestures.Circles;
using Kinect.Gestures.Swipes;
using Kinect.Gestures.Waves;
using Kinect.Sensor;
using Microsoft.Kinect;
using System.Collections.Generic;
using ServiceLib.NotificationService;
using ServiceLib.DataService;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;

namespace SharedTypes
{
    public class ApplicationController : INotificationServiceCallback
    {
        // Initialize singleton instance of the application controller.
        static ApplicationController()
        {
            ApplicationController.instance = new ApplicationController();
        }

        private static ApplicationController instance;

        // Provides access to the singleton instance of the controller application-wise.
        public static ApplicationController Instance
        {
            get { return ApplicationController.instance; }
            private set { ApplicationController.Instance = value; }
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

        private Project currentProject = null;
        public KinectSensorController KinectSensor { get; private set; }
        private NotificationServiceClient Notifications { get; set; }
        private DataServiceClient Data { get; set; }
        public int TrackingID { get; set; }
        public bool Gripping { get; set; }
        public ITargetPage CurrentPage { get; set; }
        public Skeleton[] Skeletons { get; set; }
        public List<Project> Projects { get; private set; }
        public List<Person> People { get; private set; }
        public ApplicationWindow ApplicationWindow { get; set; }
        public bool IgnoreNextProjectUpdate { get; set; }
        public Project CurrentProject
        {
            get { return this.currentProject; }
            set
            {
                if (this.currentProject == null || this.CurrentProject.ProjectID != value.ProjectID)
                {
                    // Unsubscribe notifications for the current project.
                    if (this.currentProject != null)
                    {
                        this.Notifications.Unsubscribe(this.currentProject.ProjectID);
                    }

                    // Subscribe to a new project.
                    this.currentProject = value;
                    if (this.currentProject != null)
                    {
                        this.Notifications.Subscribe(this.currentProject.ProjectID);
                    }
                }
                else
                {
                    this.currentProject = value;
                }
            }
        }

        private ApplicationController()
        {
            this.KinectSensor = new KinectSensorController(KinectSensorType.Xbox360Sensor);
            this.TrackingID = -1;
            this.Gripping = false;

            // Service clients initialisation.
            this.Notifications = new NotificationServiceClient(new System.ServiceModel.InstanceContext(this));
            this.Data = new DataServiceClient();

            // Register for global notifications.
            this.Notifications.Subscribe(-1);

            // Get project and person info.
            this.Projects = this.Data.GetAllProjects();
            this.People = this.Data.GetAllPeople();
            if (this.Projects.Count > 0)
            {
                this.CurrentProject = this.Projects[0];
            }

            // Setup gestures if a sensor was found.
            if (this.KinectSensor.FoundSensor())
            {
                this.KinectSensor.Gestures.AddGesture(new KinectGestureWaveRightHand());
                this.KinectSensor.Gestures.AddGesture(new KinectGestureWaveLeftHand());
                this.KinectSensor.Gestures.AddGesture(new KinectGestureSwipeRightToLeft());
                this.KinectSensor.Gestures.AddGesture(new KinectGestureSwipeLeftToRight());
                this.KinectSensor.Gestures.AddGesture(new KinectGestureCircleRightHand());
                this.KinectSensor.Gestures.AddGesture(new KinectGestureCircleLeftHand());
            }
        }

        /// <summary>
        /// Notifies all registered clients of a service data modification.
        /// </summary>
        /// <param name="notification">The type of modification to be notified</param>
        public void DataChanged(NotificationType notification)
        {
            // Update info.
            switch (notification)
            {
                case NotificationType.GlobalPersonModification:
                    this.People = this.Data.GetAllPeople();
                    break;
                case NotificationType.GlobalProjectModification:
                    this.Projects = this.Data.GetAllProjects();
                    break;
                case NotificationType.ProjectModification:
                    if (this.IgnoreNextProjectUpdate)
                    {
                        this.IgnoreNextProjectUpdate = false;
                        return;
                    }
                    if (currentProject != null)
                    {
                        Project updated = this.Data.GetProjectByID(this.currentProject.ProjectID);
                        int index = this.Projects.FindIndex(p => p.ProjectID == updated.ProjectID);
                        this.Projects[index] = updated;
                        this.CurrentProject = updated;
                    }
                    break;
            }

            // Notify all needed clients.
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