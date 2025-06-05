using MongoDB.Driver;
using Solution.Data;
using Solution.Interfaces;
using Solution.Models;
using Spectre.Console;

namespace Solution.Services;

public class MoveAttraction 
{
    private readonly Game _currentGame;
    private readonly IMongoCollection<Item> _itemCollection;
    private readonly MongoDbService _mongoService;

    public MoveAttraction(IMongoCollection<Item> itemCollection, MongoDbService mongoService, Game currentGame)
    {
        _itemCollection = itemCollection;
        _mongoService = mongoService;
        _currentGame = currentGame;
    }

    public void PlaceItem(InventoryService inventoryService, GridStateService grid)
    {
        try
        {
            var entries = inventoryService.Entries;

            if (entries.Count == 0)
            {
                AnsiConsole.MarkupLine("[bold red]❌ Your inventory is empty![/]\n");
                //AnsiConsole.MarkupLine("[grey]Press any key to return to the main menu...[/]");
                Console.ReadKey(true);
                return;
            }

            var choices = new List<(string DisplayName, string ItemId, string Icon)>();

            foreach (var entry in entries)
            {
                var item = _itemCollection.Find(i => i.Id == entry.ItemId).FirstOrDefault();
                if (item != null)
                    choices.Add((
                        $"[green]{item.ItemIcon}[/] [bold yellow]{item.ItemName}[/] [grey](x{entry.Count})[/]",
                        item.Id!,
                        item.ItemIcon));
            }

            var choicesWithBack = choices.Select(c => c.DisplayName).ToList();
            choicesWithBack.Add("[bold red]← Back to main menu[/]");

            var selectedChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[underline green]🎡 Choose an attraction to place[/]")
                    .PageSize(10)
                    .HighlightStyle("bold yellow")
                    .AddChoices(choicesWithBack));

            if (selectedChoice.Contains("Back to main menu"))
                return;

            var selected = choices.First(c => c.DisplayName == selectedChoice);

            AnsiConsole.MarkupLine("\n[bold green]📍 Where do you want to place this attraction?[/]");
            var userInputX = AnsiConsole.Ask<int>("[cyan]➡ Enter X coordinate (1-10):[/]") - 1;
            var userInputY = AnsiConsole.Ask<int>("[cyan]➡ Enter Y coordinate (1-10):[/]") - 1;

            if (userInputX < 0 || userInputX >= 10 || userInputY < 0 || userInputY >= 10)
            {
                AnsiConsole.MarkupLine("[bold red]❌ Coordinates out of bounds![/]");
                return;
            }

            if (grid.IsOccupied(userInputX, userInputY))
            {
                AnsiConsole.MarkupLine("[bold yellow]⚠️ There's already an attraction here![/]");
                return;
            }

            grid.PlaceAttraction(userInputX, userInputY, selected.ItemId);
            inventoryService.RemoveItem(selected.ItemId);

            _currentGame.Inventory = inventoryService.Entries;
            _currentGame.Grid = grid.ToListGrid();
            _mongoService.SaveGame(_currentGame);

            AnsiConsole.MarkupLine("[bold green]✅ Attraction placed successfully![/]");
            AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]❌ Error placing attraction: {ex.Message}[/]");
        }
    }

    public void RemoveItem(InventoryService inventoryService, GridStateService grid)
    {
        try
        {
            AnsiConsole.MarkupLine("[bold green]🗑️ Enter the coordinates of the attraction to remove.[/]\n");

            var x = AnsiConsole.Ask<int>("[cyan]➡ X (1-10):[/]") - 1;
            var y = AnsiConsole.Ask<int>("[cyan]➡ Y (1-10):[/]") - 1;

            if (x < 0 || x >= 10 || y < 0 || y >= 10)
            {
                AnsiConsole.MarkupLine("[bold red]❌ Coordinates out of bounds![/]");
                AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
                Console.ReadKey(true);
                return;
            }

            var itemId = grid.Grid[x, y];

            if (string.IsNullOrEmpty(itemId))
            {
                AnsiConsole.MarkupLine("[bold yellow]⚠️ There's no attraction here.[/]");
                AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
                Console.ReadKey(true);
                return;
            }

            var item = _itemCollection.Find(i => i.Id == itemId).FirstOrDefault();

            if (item == null)
            {
                AnsiConsole.MarkupLine("[bold red]❌ This attraction is unknown. Can't return to inventory.[/]");
                AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
                Console.ReadKey(true);
                return;
            }

            inventoryService.AddItem(item.Id!);
            grid.RemoveAttraction(x, y);

            _currentGame.Inventory = inventoryService.Entries;
            _currentGame.Grid = grid.ToListGrid();
            _mongoService.SaveGame(_currentGame);

            AnsiConsole.MarkupLine($"[bold green]✅ {item.ItemName} removed and returned to your inventory![/]");
            Console.ReadKey(true);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]❌ Error removing attraction: {ex.Message}[/]");
        }
    }
}