using MongoDB.Driver;
using Solution.Models;
using Spectre.Console;

namespace Solution.Views;

public class InventoryViews
{
    private readonly IMongoCollection<Item> _itemCollection;

    public InventoryViews(IMongoCollection<Item> itemCollection)
    {
        _itemCollection = itemCollection;
    }

    public void DisplayInventory(List<InventoryEntry> inventoryEntries)
    {
        if (inventoryEntries == null || inventoryEntries.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]Your inventory is empty.[/]");
            // AnsiConsole.MarkupLine("\n[grey]Press any key to return to the main menu[/]");
            Console.ReadKey(true);
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[underline bold cyan]Inventory[/]")
            .ShowRowSeparators();

        table.AddColumn("[bold]Icon[/]");
        table.AddColumn("[bold]Name[/]");
        table.AddColumn("[bold]Description[/]");
        table.AddColumn("[bold]Amount[/]");

        foreach (var entry in inventoryEntries)
        {
            var item = _itemCollection.Find(i => i.Id == entry.ItemId).FirstOrDefault();

            if (item != null)
                table.AddRow(item.ItemIcon, item.ItemName, item.ItemDescription, entry.Count.ToString());
            else
                table.AddRow("[red]?[/]", $"[red]Unknown Item ID:[/] {entry.ItemId}", entry.Count.ToString());
        }

        AnsiConsole.Write(table);
        //AnsiConsole.MarkupLine("\n[grey]Press any key to return to the main menu[/]");
        Console.ReadKey(true);
    }
}