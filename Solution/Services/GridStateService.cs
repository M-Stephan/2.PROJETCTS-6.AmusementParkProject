using MongoDB.Driver;
using Solution.Models;

namespace Solution.Services;

public class GridStateService
{
    private readonly IMongoCollection<Item> _itemCollection;

    public GridStateService(IMongoCollection<Item> itemCollection)
    {
        _itemCollection = itemCollection ?? throw new ArgumentNullException(nameof(itemCollection));
        Grid = new string[10, 10];
        InitializeGrid();
    }

    public string[,] Grid { get; }

    private void InitializeGrid()
    {
        for (var i = 0; i < 10; i++)
        for (var j = 0; j < 10; j++)
            Grid[i, j] = null; // Empty space is now null
    }

    public List<List<string?>> ToListGrid()
    {
        var listGrid = new List<List<string?>>();
        for (var i = 0; i < 10; i++)
        {
            var row = new List<string?>();
            for (var j = 0; j < 10; j++)
                row.Add(Grid[i, j]);
            listGrid.Add(row);
        }

        return listGrid;
    }

    public void LoadFromListGrid(List<List<string?>> listGrid)
    {
        for (var i = 0; i < 10; i++)
        for (var j = 0; j < 10; j++)
            Grid[i, j] = listGrid[i][j]; // Allow nulls
    }

    public void PlaceAttraction(int x, int y, string itemId)
    {
        if (x < 0 || x >= 10 || y < 0 || y >= 10)
            throw new ArgumentOutOfRangeException("Coordinates are out of range.");

        var item = _itemCollection.Find(i => i.Id == itemId).FirstOrDefault();
        if (item == null)
            throw new ArgumentException($"Item with ID {itemId} not found.");

        Grid[x, y] = itemId;
    }

    public void RemoveAttraction(int x, int y)
    {
        if (x < 0 || x >= 10 || y < 0 || y >= 10)
            throw new ArgumentOutOfRangeException("Coordinates are out of range.");

        Grid[x, y] = null;
    }

    public bool IsOccupied(int x, int y)
    {
        return Grid[x, y] != null;
    }
}