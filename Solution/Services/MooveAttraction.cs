using Spectre.Console;

namespace Park
{
    public class MooveAttraction
    {
        public void PlaceItem(Inventory inventory, GridState grid)
        {
            if (inventory.Items.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Your inventory is empty![/]\n");
                AnsiConsole.MarkupLine("\n[grey]Press any key to return at the main menu[/]");
                Console.ReadKey(true);
                return;
            }

            var itemName = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("[green]Choose an attraction to place[/]")
                .AddChoices(inventory.Items.Select(i => i._itemName)));

            var itemToPlace = inventory.Items.FirstOrDefault(i => i._itemName == itemName);

            if (itemToPlace != null)
            {
                AnsiConsole.MarkupLine("[green]Where you want to place this attraction ?[/]\n");
                AnsiConsole.MarkupLine("[green]X = ? [/]");
                var userInputX = int.Parse(Console.ReadLine()!);
                AnsiConsole.MarkupLine("[green]Y = ? [/]");
                var userInputY = int.Parse(Console.ReadLine()!);
                grid.PlaceAttraction(userInputX - 1, userInputY - 1, itemToPlace._itemIcon);
                inventory.RemoveItem(itemToPlace);
                AnsiConsole.MarkupLine("[green]Attraction placed successfully![/]");
            }
        }

        public void RemoveItem(Inventory inventory, GridState grid)
        {
            AnsiConsole.MarkupLine("[green]Enter the coordinates of the attraction to remove.[/]\n");

            int x = AnsiConsole.Ask<int>("[green]X (1-10):[/]") - 1;
            int y = AnsiConsole.Ask<int>("[green]Y (1-10):[/]") - 1;

            if (x < 0 || x >= 10 || y < 0 || y >= 10)
            {
                AnsiConsole.MarkupLine("[red]Coordinates out of bounds![/]");
                return;
            }

            if (!grid.IsOccupied(x, y))
            {
                AnsiConsole.MarkupLine("[yellow]There's no attraction here.[/]");
                return;
            }

            string icon = grid.Grid[x, y];

            var knownItems = new List<Item>
            {
                new Item("Ferris wheel", "[gold1]🎡[/]", 1, 25060),
                new Item("Roller Coaster", "[red]🎢[/]", 1, 15170),
                new Item("Carousel", "[orchid1]🎠[/]", 1, 7550),
                new Item("Food Stand", "[yellow1]🌭[/]", 1, 3250),
                new Item("Ticket Booth", "[blue]🎫[/]", 1, 7500),
                new Item("Bumper Cars", "[orange1]🚗[/]", 1, 12320),
                new Item("Water Slide", "[deepskyblue1]🌊[/]", 1, 13480),
                new Item("Swing Ride", "[violet]🎑[/]", 1, 12865),
            };  

            
            var item = knownItems.FirstOrDefault(i => i._itemIcon == icon);

            if (item == null)
            {
                AnsiConsole.MarkupLine("[red]This attraction is unknown, can't return to inventory.[/]");
                return;
            }

            
            inventory.AddItem(new Item(item._itemName, item._itemIcon, 1, item._itemCost));

            
            grid.RemoveAttraction(x, y);

            AnsiConsole.MarkupLine($"[green]{item._itemName} removed from the park and returned to your inventory![/]");
        }

    }
}