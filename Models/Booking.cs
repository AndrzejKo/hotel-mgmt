using HotelReservationSystem.Utils;

namespace HotelReservationSystem.Models
{
    public class Booking
    {
        public required string HotelId { get; set; }
        public required string Arrival { get; set; }
        public required string Departure { get; set; }
        public required string RoomType { get; set; }
        public string RoomRate { get; set; } = string.Empty;

        private DateTime? _arrivalDate;
        public DateTime ArrivalDate => _arrivalDate ??= DateHelper.ParseDate(Arrival);

        private DateTime? _departureDate;
        public DateTime DepartureDate => _departureDate ??= DateHelper.ParseDate(Departure);

        public int BookedRooms { get; set; } = 1; // By default the booking is for 1 room
    }
}