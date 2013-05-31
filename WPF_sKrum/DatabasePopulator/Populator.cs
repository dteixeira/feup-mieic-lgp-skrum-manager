using ServiceLib.DataService;

namespace DatabasePopulator
{
    public class Populator
    {
        private DataServiceClient data;

        public Populator()
        {
            this.data = new DataServiceClient();
        }

        public DataServiceClient Data
        {
            get { return this.data; }
        }

        public static void Main(string[] args)
        {
            Populator populator = new Populator();
            Project project = populator.Data.GetProjectByName("sKrum");
            Person person = populator.Data.GetPersonByEmail("amir@email.com");
            Role role = new Role
            {
                AssignedTime = 0.5,
                PersonID = person.PersonID,
                ProjectID = project.ProjectID,
                RoleDescription = RoleDescription.TeamMember
            };
            populator.Data.CreateRole(role);

            /*Story story = populator.Data.GetStoryByID(1);
            StorySprint storySprint = story.StorySprints[0];
            storySprint.Points = 1;
            populator.Data.AddStoryInSprint(storySprint);

            /*Populator populator = new Populator();
            Project project = populator.Data.GetProjectByName("sKrum");
            Sprint sprint = project.Sprints[0];
            foreach (Story story in sprint.Stories)
            {
                int tasks = new System.Random().Next(10) + 1;
                for (int i = 0; i < tasks; ++i)
                {
                    string description = "Look, just because I don't be givin' no man a foot massage don't make it right for Marsellus to throw Antwone into a glass motherfuckin' house, fuckin' up the way the nigger talks. Motherfucker do that shit to me, he better paralyze my ass, 'cause I'll kill the motherfucker, know what I'm sayin'?";
                    Task task = new Task
                    {
                        CreationDate = System.DateTime.Now,
                        Description = description.Substring(0, new System.Random().Next(description.Length + 1)),
                        Estimation = new System.Random().Next(10) + 1,
                        State = (new TaskState[]{ TaskState.Completed, TaskState.InProgress, TaskState.Waiting })[new System.Random().Next(3)],
                        StoryID = story.StoryID
                    };
                    populator.Data.CreateTask(task);
                }
            }*/

            /*for (int i = 0; i < 10; ++i)
            {
                string description = "Bacon ipsum dolor sit amet non jowl aliqua, chicken boudin prosciutto short ribs pork in pariatur meatball pork loin irure. Ex ad leberkas meatball frankfurter, tail bresaola ribeye drumstick do ut. Excepteur dolore consequat quis turducken tenderloin aliqua capicola laborum ut in pancetta bacon shoulder pork.";
                Story story = new Story
                {
                    CreationDate = System.DateTime.Now,
                    Description = description.Substring(0, new System.Random().Next(description.Length + 1)),
                    Priority = (new StoryPriority[] { StoryPriority.Could, StoryPriority.Must, StoryPriority.Should, StoryPriority.Wont })[new System.Random().Next(4)],
                    ProjectID = project.ProjectID,
                    State = StoryState.InProgress
                };
                story = populator.Data.CreateStory(story);
                StorySprint storySprint = new StorySprint
                {
                    Points = (new int[] { 1, 2, 3, 5, 8, 13, 21 })[new System.Random().Next(7)],
                    SprintID = sprint.SprintID,
                    StoryID = story.StoryID
                };
                populator.Data.AddStoryInSprint(storySprint);
            }*/
        }
    }
}