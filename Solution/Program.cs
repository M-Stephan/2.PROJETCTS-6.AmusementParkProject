using Solution.Data;
using Solution.Data.Seeders;
using Solution.Services;
using Solution.Views;

namespace Solution
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var mongoService = new MongoDbService();
            var itemSeeder = new ItemSeeder(mongoService);

            itemSeeder.SeedItems();

            Menu menu = new();
            menu.DisplayMenu();
        }
    }
}
