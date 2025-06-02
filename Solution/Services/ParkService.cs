using Solution.Models;

namespace Solution.Services;

public class ParkService
{
    private readonly GridCell[,] _grid;
    public int Width { get; }
    public int Height { get; }

    public ParkService(int width, int height)
    {
        Width = width;
        Height = height;
        _grid = new GridCell[height, width];

        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
            _grid[y, x] = new GridCell();
    }

    public bool PlaceRide(int x, int y, Ride ride)
    {
        
        return true;
    }

    public bool RemoveRide(int x, int y)
    {
        return true;
    }

    public GridCell[,] GetGrid() => _grid;

    public bool IsValidCoordinate(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;
}