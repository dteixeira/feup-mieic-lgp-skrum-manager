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
using TaskBoardControlLib;

namespace TaskboardRowLib
{
    /// <summary>
    /// Interaction logic for TaskboardRowControl.xaml
    /// </summary>
    public partial class TaskboardRowControl : UserControl
    {
        /* Static members and static constructor */

        public static Dictionary<int, Dictionary<TasksState, ObservableCollection<TaskControl>>> AllTasks { get; set; }
        public static UserControl CurrentTaskBoard { get; set; }
        private delegate void TaskBoardUpdate(ServiceLib.DataService.Task task); 

        static TaskboardRowControl()
        {
            TaskboardRowControl.AllTasks = new Dictionary<int, Dictionary<TasksState, ObservableCollection<TaskControl>>>();
        }

        /* Instance members and methods */

        public ObservableCollection<TaskControl> Tasks { get; set; }
        public TasksState State { get; set; }

        public TaskboardRowControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        public static void CreateLine(int USID)
        {
            TaskboardRowControl.AllTasks[USID] = new Dictionary<TasksState, ObservableCollection<TaskControl>>();
            TaskboardRowControl.AllTasks[USID][TasksState.Todo] = new ObservableCollection<TaskControl>();
            TaskboardRowControl.AllTasks[USID][TasksState.Doing] = new ObservableCollection<TaskControl>();
            TaskboardRowControl.AllTasks[USID][TasksState.Done] = new ObservableCollection<TaskControl>();
        }

        protected override void OnDrop(DragEventArgs e)
        {
            try
            {
                var dataObj = e.Data as DataObject;
                TaskControl taskControl = dataObj.GetData("TaskControl") as TaskControl;
                StoryControl storyControl = dataObj.GetData("StoryControl") as StoryControl;

                // Handle task drop.
                if (taskControl != null)
                {
                    // Can't mode a task to todo again.
                    if (this.State == TasksState.Todo)
                    {
                        return;
                    }
                    if (this.State != taskControl.State)
                    {
                        // If no user is attributed to the task
                        ServiceLib.DataService.Person person = null;
                        if (this.State != TasksState.Todo && taskControl.Task.PersonTasks.Count == 0)
                        {
                            // Select a user.
                            SharedTypes.ApplicationController.Instance.ApplicationWindow.SetWindowFade(true);
                            PopupSelectionControlLib.SelectionWindow userForm = new PopupSelectionControlLib.SelectionWindow();
                            PopupSelectionControlLib.UserSelectionPage userPage = new PopupSelectionControlLib.UserSelectionPage(true);
                            userPage.PageTitle = "Escolha o Responsável";
                            userForm.FormPage = userPage;
                            userForm.ShowDialog();
                            if (userForm.Success)
                            {
                                person = (ServiceLib.DataService.Person)userForm.Result;
                                SharedTypes.ApplicationController.Instance.ApplicationWindow.SetWindowFade(false);
                            }
                            else
                            {
                                SharedTypes.ApplicationController.Instance.ApplicationWindow.SetWindowFade(false);
                                return;
                            }
                        }

                        // Launch thread to update the project.
                        System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.UpdateTaskInService));
                        thread.Start(new object[] { this.State, taskControl.Task, person });

                        // Update visualization.
                        TaskboardRowControl.AllTasks[taskControl.USID][taskControl.State].Remove(taskControl);
                        taskControl.State = this.State;
                        TaskboardRowControl.AllTasks[taskControl.USID][this.State].Add(taskControl);
                    }
                }
                // Handle story drop.
                else if (storyControl != null)
                {
                    PopupFormControlLib.FormWindow form = new PopupFormControlLib.FormWindow();
                    PopupFormControlLib.TextAreaPage descriptionPage = new PopupFormControlLib.TextAreaPage { PageName = "description", PageTitle = "Descrição" };
                    PopupFormControlLib.SpinnerPage estimationPage = new PopupFormControlLib.SpinnerPage { PageName = "estimation", PageTitle = "Estimativa de Esforço", Min = 1, Max = 999, Increment = 1 };
                    form.FormPages.Add(descriptionPage);
                    form.FormPages.Add(estimationPage);
                    SharedTypes.ApplicationController.Instance.ApplicationWindow.SetWindowFade(true);
                    form.ShowDialog();

                    // Create a new task.
                    if (form.Success)
                    {
                        string description = (string)form["description"].PageValue;
                        int estimation = (int)((double)form["estimation"].PageValue);
                        if (description != null && description != "")
                        {
                            ServiceLib.DataService.Task task = new ServiceLib.DataService.Task
                            {
                                CreationDate = System.DateTime.Now,
                                Description = description,
                                Estimation = estimation,
                                State = ServiceLib.DataService.TaskState.Waiting,
                                StoryID = storyControl.Story.StoryID
                            };

                            // Start a new thread to interact with the service.
                            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(this.CreateTask));
                            thread.Start(task);
                        }
                    }
                    SharedTypes.ApplicationController.Instance.ApplicationWindow.SetWindowFade(false);
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        public void CreateTask(object obj)
        {
            ServiceLib.DataService.Task task = (ServiceLib.DataService.Task)obj;
            ServiceLib.DataService.DataServiceClient client = new ServiceLib.DataService.DataServiceClient();
            SharedTypes.ApplicationController.Instance.IgnoreNextProjectUpdate = true;
            task = client.CreateTask(task);
            client.Close();
            if (Dispatcher.Thread.Equals(System.Threading.Thread.CurrentThread))
            {
                CreateTaskInTaskboard(task);
            }
            else
            {
                Dispatcher.BeginInvoke(new TaskBoardUpdate(CreateTaskInTaskboard), new object[] { task });
            }
        }

        public void CreateTaskInTaskboard(ServiceLib.DataService.Task task)
        {
            TaskControl taskControl = new TaskControl
            {
                Task = task,
                TaskDescription = task.Description,
                TaskEstimationWork = string.Format("{0:0.#} / {1}", 0, task.Estimation),
                USID = task.StoryID,
                State = TasksState.Todo
            };
            taskControl.Width = Double.NaN;
            taskControl.Height = Double.NaN;
            AllTasks[taskControl.Task.StoryID][TasksState.Todo].Add(taskControl);
        }

        public void UpdateTaskInService(object info)
        {
            try
            {
                object[] param = info as object[];
                TasksState state = (TasksState)param[0];
                ServiceLib.DataService.Task task = (ServiceLib.DataService.Task)param[1];
                ServiceLib.DataService.Person person = (ServiceLib.DataService.Person)param[2];

                // Change the task state.
                switch (state)
                {
                    case TasksState.Done:
                        task.State = ServiceLib.DataService.TaskState.Completed;
                        break;
                    case TasksState.Doing:
                        task.State = ServiceLib.DataService.TaskState.InProgress;
                        break;
                    case TasksState.Todo:
                        task.State = ServiceLib.DataService.TaskState.Waiting;
                        break;
                    default:
                        // Nothing to change.
                        return;
                }

                // Update the task and force a taskboard update if
                // update fails.
                ServiceLib.DataService.DataServiceClient client = new ServiceLib.DataService.DataServiceClient();
                SharedTypes.ApplicationController.Instance.IgnoreNextProjectUpdate = true;
                task = client.UpdateTask(task);
                if (task == null)
                {
                    SharedTypes.ApplicationController.Instance.IgnoreNextProjectUpdate = false;
                    SharedTypes.ApplicationController.Instance.DataChanged(ServiceLib.NotificationService.NotificationType.ProjectModification);
                }
                if (person != null)
                {
                    ServiceLib.DataService.PersonTask personTask = new ServiceLib.DataService.PersonTask
                    {
                        CreationDate = System.DateTime.Now,
                        PersonID = person.PersonID,
                        SpentTime = 0,
                        TaskID = task.TaskID
                    };
                    client.AddWorkInTask(personTask);
                }
                client.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }
    }
}
