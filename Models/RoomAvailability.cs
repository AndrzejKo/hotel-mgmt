using HotelReservationSystem.Utils;

namespace HotelReservationSystem.Models
{
    public class RoomAvailability
    {
        public string HotelId { get; private set; }
        public string RoomType { get; private set; }
        public DateTime From { get; private set; }
        public DateTime To { get; private set; }
        public int Availability {get; private set;}

        public RoomAvailability(string hotelId, string roomType, DateTime from, DateTime to, int availability)
        {
            HotelId = hotelId;
            RoomType = roomType;
            From = from;
            To = to;
            Availability = availability;
        }
    }
}