using Solution.Data;
using Solution.Models;
using Solution.Services;
using Spectre.Console;

namespace Solution.Views;

public class GamePersistenceManager
{
    private readonly MongoDbService _mongoService;

    public GamePersistenceManager()
    {
        _mongoService = new MongoDbService();
    }

    public void SaveGame(Game currentGame, InventoryService inventoryService, BankingService bankingService, GridStateService gridStateService)
    {
        currentGame.Money = bankingService._money;
        currentGame.Grid = gridStateService.ToListGrid();

        currentGame.Inventory = inventoryService.Entries.Select(e => new InventoryEntry
        {
            ItemId = e.ItemId,
            Count = e.Count
        }).ToList();

        _mongoService.SaveGame(currentGame);

        AnsiConsole.WriteLine();
    }

    public void DeleteGame(Game currentGame)
    {
        AnsiConsole.Write(
            new Panel(
                    $"[bold red]WARNING![/]\nYou are about to delete [yellow]{currentGame.Name}[/]. This action is irreversible.")
                .BorderColor(Color.Red)
                .Header("[white on red] Confirm Deletion [/]")
                .Padding(1, 1));

        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Are you sure you want to delete this game?[/]")
                    .AddChoices("❌ No, keep it","🗑️Yes, delete it"));

            if (choice.StartsWith("❌"))
            {
                AnsiConsole.MarkupLine("[green]Aborted. The game was not deleted.[/]");
                return;
            }

            if (choice.StartsWith("🗑️"))
            {
                _mongoService.DeleteGame(currentGame.Name);
                AnsiConsole.MarkupLine($"[bold green]✅ Game '[yellow]{currentGame.Name}[/]' deleted successfully.[/]");
                AnsiConsole.WriteLine();
                Environment.Exit(0);
            }
        }
    }
}