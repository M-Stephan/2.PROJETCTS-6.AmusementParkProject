using Spectre.Console;

namespace Park
{
    public class Menu
    {


        public void DisplayMenu()
        {
            GridState gridState = new();
            GridPark gridPark = new();
            Inventory inventory = new();
            inventory.LoadDefaultItems();

            // Entry point of the application - displays welcome screen and main menu
            AnsiConsole.Write(
                new FigletText("Fun Park!")
                    .Centered()
                    .Color(Color.Orange1));

            AnsiConsole.MarkupLine("[deepskyblue3_1 bold]Welcome to the Amusement Park Simulator![/]\n");
            AnsiConsole.MarkupLine("[grey]Press any key to start...[/]");
            Console.ReadKey(true);

            // Main program loop showing the user menu
            while (true)
            {
                AnsiConsole.Clear();

                // Show selection menu to the user
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[green]Amusement Park Menu[/]")
                        .AddChoices("Display Park Grid", "View Ride Inventory", "Place a Ride", "Remove a Ride", "Exit"));

                // Perform action based on user's menu choice
                switch (choice)
                {
                    case "Display Park Grid":
                        gridPark.DisplayGrid(gridState.Grid);
                        break;
                    case "View Ride Inventory":
                        inventory.ShowInventory();
                        break;
                    case "Place a Ride":
                        /*PlaceRide() */
                        Console.WriteLine("Standby");
                        break;
                    case "Remove a Ride":
                        /*RemoveRide() */
                        Console.WriteLine("Standby");
                        break;
                    case "Exit":
                        Environment.Exit(0);
                        return;
                }
            }
        }
    }
}
