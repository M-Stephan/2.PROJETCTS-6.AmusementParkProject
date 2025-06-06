using Park;
using Solution.Models;
using Solution.Services;
using Spectre.Console;

namespace Solution.Views;

/// <summary>
/// Handles persistence operations for saving and deleting game data and associated auctions.
/// </summary>
public class GamePersistenceManager
{
    private readonly MongoDbService _mongoService;

    public GamePersistenceManager()
    {
        _mongoService = new MongoDbService();
    }

    /// <summary>
    /// Saves the current state of the game, including inventory, money, and grid layout.
    /// </summary>
    public void SaveGame(Game currentGame, InventoryService inventory, BankingService banking, GridStateService gridState)
    {
        currentGame.Money = banking._money;
        currentGame.Grid = gridState.ToListGrid();

        currentGame.Inventory = inventory.Entries.Select(e => new InventoryEntry
        {
            ItemId = e.ItemId,
            Count = e.Count
        }).ToList();

        _mongoService.SaveGame(currentGame);
        AnsiConsole.WriteLine();
    }

    /// <summary>
    /// Prompts the user to confirm and deletes the game and its related auction listings.
    /// </summary>
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
                    .AddChoices("❌ No, keep it", "🗑️Yes, delete it"));

            if (choice.StartsWith("❌"))
            {
                AnsiConsole.MarkupLine("[green]Aborted. The game was not deleted.[/]");
                return;
            }

            if (choice.StartsWith("🗑️"))
            {
                _mongoService.DeleteUnsoldAuctionItemsBySellerId(currentGame.Id);
                _mongoService.DeleteGame(currentGame.Name);

                AnsiConsole.MarkupLine($"[bold green]✅ Game '[yellow]{currentGame.Name}[/]' and its unsold auctions were deleted successfully.[/]");
                AnsiConsole.WriteLine();
                Environment.Exit(0);
            }
        }
    }
}
