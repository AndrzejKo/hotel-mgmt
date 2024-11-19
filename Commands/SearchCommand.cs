namespace HotelReservationSystem.Commands;

public class SearchCommand
{
    public string HotelId { get; private set; }
    public int Days { get; private set; }
    public string RoomType { get; private set; }

    public SearchCommand(string hotelId, string days, string roomType)
    {
        HotelId = hotelId;
        RoomType = roomType;

        int d;
        if (!int.TryParse(days, out d) || d < 1)
            throw new ArgumentException($"Invalid days parameter: {days}");

        Days = d;
    }
}