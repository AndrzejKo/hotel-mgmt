using HotelReservationSystem.Models;
using HotelReservationSystem.Utils;

namespace HotelReservationSystem;

public class HotelService
{
    public static int CheckAvailability(List<Hotel> hotels, List<Booking> bookings, string hotelId, DateTime from, DateTime to, string roomType)
    {
        var hotelBookings = bookings.Where(x => x.HotelId == hotelId && x.RoomType == roomType).ToList();
        var totalRoomsOfType = hotels.FirstOrDefault(x => x.Id == hotelId)?.Rooms.Count(r => r.RoomType == roomType) ?? 0;

        // If there are no bookings for given hotel and roomType then all rooms are available
        if (!hotelBookings.Any()) return totalRoomsOfType;

        // Calculate booked intervals
        var bookedIntervals = CalculateIntervalBookings(hotelBookings);

        // Get max booking of given room type within checked interval
        var maxBooked = bookedIntervals
            .Where(i => HasOverlap(i.From, i.To, from, to))
            .DefaultIfEmpty(new IntervalBooking { BookedRooms = 0 })
            .Max(x => x.BookedRooms);

        return totalRoomsOfType - maxBooked;
    }

    public static IEnumerable<RoomAvailability> SearchRooms(List<Hotel> hotels, List<Booking> bookings, string hotelId, DateTime from, DateTime to, string roomType)
    {
        var hotelBookings = bookings.Where(x => x.HotelId == hotelId && x.RoomType == roomType).ToList();
        var totalRoomsOfType = hotels.FirstOrDefault(x => x.Id == hotelId)?.Rooms.Count(r => r.RoomType == roomType) ?? 0;

        // Add search invertal as booking with 0 booked rooms. It will cut intervals without affecting booked count.
        hotelBookings.Add(new Booking()
        {
            HotelId = hotelId,
            RoomType = roomType,
            Arrival = DateHelper.DateToString(from),
            Departure = DateHelper.DateToString(to),
            BookedRooms = 0
        });

        // Calculate booked intervals
        var bookedIntervals = CalculateIntervalBookings(hotelBookings);

        // Get room availability
        var availabilities = bookedIntervals
            .Where(i => HasOverlap(i.From, i.To, from, to))
            .Select(i => new RoomAvailability(hotelId, roomType, i.From, i.To, totalRoomsOfType - i.BookedRooms))
            .Where(i => i.Availability > 0) // Requirement: only rooms with availability
            .OrderBy(a => a.From);

        return MergeAvailabilities(availabilities);
    }

    private static IEnumerable<RoomAvailability> MergeAvailabilities(IEnumerable<RoomAvailability> availabilities)
    {
        var roomAv = availabilities.ToArray();
        if (roomAv.Length <= 1) return availabilities; // Nothing to merge

        var merged = new List<RoomAvailability>();

        RoomAvailability current = new RoomAvailability(roomAv[0].HotelId, roomAv[0].RoomType, roomAv[0].From, roomAv[0].To, roomAv[0].Availability);
        for (int i = 1; i < roomAv.Length; i++)
        {
            var a = roomAv[i];

            if (current.To == a.From && current.Availability == a.Availability)
            {
                current = new RoomAvailability(current.HotelId, current.RoomType, current.From, a.To, current.Availability);
            }
            else
            {
                merged.Add(current);
                current = new RoomAvailability(a.HotelId, a.RoomType, a.From, a.To, a.Availability);

                if (i == roomAv.Length - 1) merged.Add(current); // Last one
            }
        }

        return merged;
    }

    private static bool HasOverlap(DateTime from, DateTime to, DateTime from1, DateTime to1)
    {
        return from < to1 && from1 < to;
    }

    private static List<IntervalBooking> CalculateIntervalBookings(List<Booking> bookings)
    {
        var intervalEnds = bookings.Select(x => x.ArrivalDate).Union(bookings.Select(x => x.DepartureDate)).Distinct().Order().ToArray();
        var intervalBookings = new List<IntervalBooking>();

        for (int i = 0; i < intervalEnds.Count() - 1; i++)
        {
            var from = intervalEnds[i];
            var to = intervalEnds[i + 1];
            var booked = bookings.Where(x => HasOverlap(from, to, x.ArrivalDate, x.DepartureDate)).Sum(x => x.BookedRooms);

            intervalBookings.Add(new IntervalBooking()
            {
                From = from,
                To = to,
                BookedRooms = booked
            });
        }

        return intervalBookings;
    }

    class IntervalBooking()
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int BookedRooms { get; set; }
    }
}