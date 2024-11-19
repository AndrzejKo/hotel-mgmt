using HotelReservationSystem.Utils;

namespace HotelReservationSystem.Commands;

public class RoomAvailabilityCommand
{
    public string HotelId { get; private set; }
    public string Interval { get; private set; }
    public string RoomType { get; private set; }
    public DateTime From { get; private set; }
    public DateTime To { get; private set; }

    public RoomAvailabilityCommand(string hotelId, string interval, string roomType)
    {
        HotelId = hotelId;
        Interval = interval;
        RoomType = roomType;

        var intervalParts = interval.Split("-");
        if (intervalParts.Count() == 0) throw new ArgumentException($"Invalid interval {interval}");

        From = DateHelper.ParseDate(intervalParts[0]);
        if (intervalParts.Count() == 2)
        {
            To = DateHelper.ParseDate(intervalParts[1]);

            if ((To - From).Days <= 0)
            {
                throw new ArgumentException($"Invalid interval {interval}");
            }
        }
        else
        {
            To = From.AddDays(1);
        }
    }
}