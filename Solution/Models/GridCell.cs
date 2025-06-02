namespace Solution.Models;

public class GridCell
{
    public Ride? Ride { get; set; }
    public bool IsOccupied => Ride != null;
}