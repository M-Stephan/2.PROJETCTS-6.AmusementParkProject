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
            ShopViews shopView = new();
            Banking banking = new();
            MooveAttraction mover = new();


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
                        .AddChoices("Display Park Grid", "View Bank", "View Inventory", "Go to the Shop", "Place a Ride", "Remove a Ride", "Exit"));

                // Perform action based on user's menu choice
                switch (choice)
                {
                    case "Display Park Grid":
                        gridPark.DisplayGrid(gridState.Grid);
                        break;
                    case "View Bank":
                        banking.ShowAmount();
                        break;
                    case "View Inventory":
                        inventory.ShowInventory();
                        break;
                    case "Go to the Shop":
                        shopView.ShopMenu(inventory, banking);
                        break;
                    case "Place a Ride":
                        mover.PlaceItem(inventory, gridState);
                        break;
                    case "Remove a Ride":
                        mover.RemoveItem(inventory, gridState);
                        break;
                    case "Exit":
                        Environment.Exit(0);
                        return;
                }
            }
        }
    }
}
