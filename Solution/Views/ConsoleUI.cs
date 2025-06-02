using Solution.Services;
using Spectre.Console;

namespace Solution.Views;

public static class ConsoleUI
{
    private static readonly ParkService _parkService = new(5, 5);
    private static readonly RideService _rideService = new();

    public static void Run()
    {
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
                case "Display Park Grid": DisplayGrid(); break;
                case "View Ride Inventory": DisplayInventory(); break;
                case "Place a Ride": PlaceRide(); break;
                case "Remove a Ride": RemoveRide(); break;
                case "Exit": return;
            }
        }
    }

    /// <summary>
    ///     Displays the current amusement park grid with rides and empty plots.
    /// </summary>
    private static void DisplayGrid()
    {
        var grid = _parkService.GetGrid(); // Get current park layout (2D array of cells)

        // Create a visual table for the grid
        var table = new Table()
            .Title("[bold green]Y/X[/]")
            .Border(TableBorder.Square)
            .BorderColor(Color.Grey);

        // Add X-axis column headers (1 to Width)
        table.AddColumn(new TableColumn("[grey]Y/X[/]"));
        for (var x = 0; x < _parkService.Width; x++)
            table.AddColumn(new TableColumn($"[white]{x + 1}[/]").Centered());

        // Populate each row of the table with Y-axis headers and ride/grid emojis
        for (var y = 0; y < _parkService.Height; y++)
        {
            var row = new List<Markup> { new($"[white]{y + 1}[/]") };
            for (var x = 0; x < _parkService.Width; x++)
            {
                var cell = grid[y, x];
                var symbol = cell.IsOccupied
                    ? $"[bold]{cell.Ride!.Emoji}[/]" // Show ride emoji
                    : "[green]🟩[/]"; // Show grass for empty plot

                row.Add(new Markup(symbol));
            }

            table.AddRow(row);
        }

        // Display the grid and wait for user input
        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText("Amusement Park").Color(Color.Green));
        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine("\n[grey]Press any key to return to the menu...[/]");
        Console.ReadKey(true);
    }

    /// <summary>
    ///     Displays the list of available rides that can be placed in the park.
    /// </summary>
    private static void DisplayInventory()
    {
        var rides = _rideService.GetAvailableRides(); // Get all rides in the inventory

        // Create a table to display each ride with its type and emoji
        var table = new Table()
            .Title("[bold steelblue1]Ride Inventory[/]")
            .AddColumn("[yellow]Ride[/]")
            .AddColumn("[lightgreen]Type[/]")
            .AddColumn("[aqua]Emoji[/]")
            .Border(TableBorder.Square);

        foreach (var ride in rides)
            table.AddRow(ride.Name, ride.Type.ToString(), ride.Emoji);

        // Show the inventory table and wait for key press
        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine("[grey]Press any key to return...[/]");
        Console.ReadKey(true);
    }

    private static void PlaceRide()
    {
        AnsiConsole.Clear();

        // Choisir un manège
        var rides = _rideService.GetAvailableRides();
        var selectedRide = AnsiConsole.Prompt(
            new SelectionPrompt<Ride>()
                .Title("[green]Select a ride to place:[/]")
                .UseConverter(r => $"{r.Name} ({r.Type})")
                .AddChoices(rides));

        // Choisir les coordonnées
        int x = AnsiConsole.Ask<int>("[yellow]Enter the X coordinate (1 to 5):[/]") - 1;
        int y = AnsiConsole.Ask<int>("[yellow]Enter the Y coordinate (1 to 5):[/]") - 1;

        if (!_parkService.IsValidCoordinate(x, y))
        {
            AnsiConsole.MarkupLine("[red]Invalid coordinates![/]");
            AnsiConsole.MarkupLine("Press any key to return...");
            Console.ReadKey(true);
            return;
        }

        var grid = _parkService.GetGrid();
        var cell = grid[y, x];

        if (cell.IsOccupied)
        {
            AnsiConsole.MarkupLine("[red]This cell is already occupied by another ride![/]");
        }
        else
        {
            cell.Ride = selectedRide;
            AnsiConsole.MarkupLine($"[green]Ride {selectedRide.Name} placed successfully at ({x + 1},{y + 1})![/]");
        }

        AnsiConsole.MarkupLine("Press any key to return...");
        Console.ReadKey(true);
    }

    private static void RemoveRide()
    {
        AnsiConsole.Clear();

        int x = AnsiConsole.Ask<int>("[yellow]Enter the X coordinate of the ride to remove (1 to 5):[/]") - 1;
        int y = AnsiConsole.Ask<int>("[yellow]Enter the Y coordinate of the ride to remove (1 to 5):[/]") - 1;

        if (!_parkService.IsValidCoordinate(x, y))
        {
            AnsiConsole.MarkupLine("[red]Invalid coordinates![/]");
            AnsiConsole.MarkupLine("Press any key to return...");
            Console.ReadKey(true);
            return;
        }

        var grid = _parkService.GetGrid();
        var cell = grid[y, x];

        if (!cell.IsOccupied)
        {
            AnsiConsole.MarkupLine("[red]No ride found at this location![/]");
        }
        else
        {
            var removedRide = cell.Ride;
            cell.Ride = null;
            AnsiConsole.MarkupLine($"[green]Ride {removedRide?.Name} removed from ({x + 1},{y + 1})![/]");
        }

        AnsiConsole.MarkupLine("Press any key to return...");
        Console.ReadKey(true);
    }
}