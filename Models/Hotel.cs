namespace HotelReservationSystem.Models;

public class Hotel
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public List<RoomType> RoomTypes { get; set; } = [];
    public List<Room> Rooms { get; set; } = [];
}
