using MongoDB.Driver;
using Solution.Models;
using Spectre.Console;

namespace Solution.Views;

public class GridPark
{
    private readonly IMongoCollection<Item> _itemCollection;

    public GridPark(IMongoCollection<Item> itemCollection)
    {
        _itemCollection = itemCollection ?? throw new ArgumentNullException(nameof(itemCollection));
    }

    public void DisplayGrid(string[,] grid)
    {
        var allItems = _itemCollection.Find(_ => true).ToList();
        var itemMap = allItems.ToDictionary(i => i.Id, i => i.ItemIcon);

        var table = new Table();
        table.AddColumn("Y\\X"); // Top-left header: rows = Y, columns = X

        var width = grid.GetLength(0); // X dimension
        var height = grid.GetLength(1); // Y dimension

        // Add columns header (X axis)
        for (var x = 1; x <= width; x++)
            table.AddColumn(x.ToString());

        // Add rows
        for (var y = 0; y < height; y++)
        {
            var row = new string[width + 1];
            row[0] = (y + 1).ToString(); // row label (Y axis)

            for (var x = 0; x < width; x++)
            {
                var cell = grid[x, y]; // note: [x, y] not [i,j]

                if (string.IsNullOrWhiteSpace(cell))
                    row[x + 1] = "[green]🟩[/]";
                else if (itemMap.TryGetValue(cell, out var icon))
                    row[x + 1] = icon;
                else
                    row[x + 1] = "[red]❓[/]";
            }

            table.AddRow(row);
        }

        table.Border(TableBorder.Rounded);
        table.ShowRowSeparators();
        AnsiConsole.Write(table);

        //AnsiConsole.MarkupLine("\n[grey]Press any key to return to the main menu[/]");
        Console.ReadKey(true);
    }
}