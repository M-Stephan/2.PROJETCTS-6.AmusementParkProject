using Solution.Models;

namespace Solution.Services;

public class RideService
{
    public List<Ride> GetAvailableRides() => new List<Ride>
    {
        new Ride() { Name = "Mega Coaster", Type = RideType.RollerCoaster },
        new Ride { Name = "Twirl Carousel", Type = RideType.Carousel }
    };
}