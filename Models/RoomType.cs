namespace HotelReservationSystem.Models;

public class RoomType
{
    public required string Code { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<string> Amenities { get; set; } = [];
    public List<string> Features { get; set; } = [];
}
