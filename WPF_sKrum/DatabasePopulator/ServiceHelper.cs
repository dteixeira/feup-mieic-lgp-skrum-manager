using ServiceLib.DataService;
using System.ComponentModel;

namespace DatabasePopulator
{
    public class ServiceHelper
    {
        public static void DumpObject(object dump)
        {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(dump))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(dump);
                System.Console.WriteLine("{0}={1}", name, value);
            }
        }

        public static Person CreateDefaultPerson(Populator populator)
        {
            Person person = new Person()
            {
                Email = "default@email.domain",
                JobDescription = "I'm a default person entity.",
                Name = "Default Person",
                Password = null,
                PhotoURL = "http://default.com"
            };
            return populator.Data.CreatePerson(person);
        }

        public static Project CreateDefaultProject(Populator populator)
        {
            Project project = new Project()
            {
                AlertLimit = 1,
                Name = "Default Project",
                Password = null,
                Speed = 1,
                SprintDuration = 1
            };
            return populator.Data.CreateProject(project);
        }

        public static Role CreateDefaultRole(Populator populator, Project project, Person person)
        {
            Role role = new Role
            {
                AssignedTime = 1.0,
                Password = null,
                PersonID = person.PersonID,
                ProjectID = project.ProjectID,
                RoleDescription = RoleDescription.TeamMember
            };
            return populator.Data.CreateRole(role);
        }

        public static Story CreateDefaultStory(Populator populator, Project project)
        {
            Story story = new Story
            {
                CreationDate = System.DateTime.Now,
                Description = "Default Story",
                PreviousStory = null,
                ProjectID = project.ProjectID,
                State = StoryState.InProgress,
            };
            return populator.Data.CreateStory(story);
        }

        public static Task CreateDefaultTask(Populator populator, Story story)
        {
            Task task = new Task
            {
                CreationDate = System.DateTime.Now,
                Description = "Default Task",
                Estimation = 1,
                State = TaskState.Waiting,
                StoryID = story.StoryID
            };
            return populator.Data.CreateTask(task);
        }

        public static Sprint CreateDefaultSprint(Populator tester, Project project)
        {
            Sprint sprint = new Sprint
            {
                BeginDate = System.DateTime.Now,
                Closed = false,
                EndDate = null,
                Number = 1,
                ProjectID = project.ProjectID
            };
            return tester.Data.CreateSprint(sprint);
        }

        public static Meeting CreateDefaultMeeting(Populator populator, Project project)
        {
            Meeting meeting = new Meeting
            {
                Date = System.DateTime.Now,
                Notes = "I am a note.",
                Number = 1,
                ProjectID = project.ProjectID
            };
            return populator.Data.CreateMeeting(meeting);
        }
    }
}