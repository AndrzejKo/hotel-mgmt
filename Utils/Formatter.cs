using HotelReservationSystem.Models;

namespace HotelReservationSystem.Utils;

public static class Formatter
{
    public static string FormatRoomAvailability(RoomAvailability availability)
    {
        string from = DateHelper.DateToString(availability.From);
        string to = DateHelper.DateToString(availability.To);

        return $"({from}-{to}, {availability.Availability})";
    }
}