namespace Solution.Models;

public class Ride
{
    public string Name { get; set; }
    public RideType Type { get; set; }
    public int Popularity { get; set; } = 0;
    public string Emoji => Type switch
    {
        RideType.RollerCoaster => "🎢",
        RideType.Carousel => "🎠",
        _ => "❓"
    };
}
