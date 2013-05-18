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
            // Database seeding code here.
            // Use ServiceHelper to easily create default items.
        }
    }
}
