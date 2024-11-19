using System.Reflection;
using HotelReservationSystem.Models;
using HotelReservationSystem.Utils;
using Xunit;

namespace HotelReservationSystem.Tests;

public class HotelServiceTests
{
    Hotel california = new Hotel()
    {
        Id = "H1",
        Name = "California",
        RoomTypes = new List<RoomType>{
            new RoomType(){Code = "SGL"},
            new RoomType(){Code = "DBL"}
        },
        Rooms = new List<Room>(){
            new Room(){RoomId = "101", RoomType = "SGL"},
            new Room(){RoomId = "102", RoomType = "SGL"},
            new Room(){RoomId = "201", RoomType = "DBL"},
            new Room(){RoomId = "202", RoomType = "DBL"}
        }
    };

    Hotel fawltyTowers = new Hotel()
    {
        Id = "H2",
        Name = "Fawlty Towers",
        RoomTypes = new List<RoomType>{
            new RoomType(){Code = "SGL"},
            new RoomType(){Code = "DBL"}
        },
        Rooms = new List<Room>(){
            new Room(){RoomId = "101", RoomType = "SGL"},
            new Room(){RoomId = "102", RoomType = "SGL"},
            new Room(){RoomId = "201", RoomType = "DBL"},
            new Room(){RoomId = "202", RoomType = "DBL"}
        }
    };

    [Fact]
    public void SearchRooms_ComplexBookings()
    {
        /*
        Date:      01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28
                                ●  ●  ●  ○        ●  ○                 ●  ○  ●  ○  ●  ○
                                         ●  ●  ●  ●  ●  ●  ●  ●  ○        ●  ○  ●  ○                
                                                  ●  ●  ○
        Available:  2  2  2  2  1  1  1  1  1  1 -1  0  1  1  1  2  2  1  1  1  1  1  2  2  2  2  2  2 
        */
        // Arrange
        var hotels = new List<Hotel> { california };
        var bookings = new List<Booking>{
            // Row 1
            new Booking(){HotelId = "H1", RoomType = "SGL", Arrival = "20240905", Departure="20240908"},
            new Booking(){HotelId = "H1", RoomType = "SGL", Arrival = "20240911", Departure="20240912"},
            new Booking(){HotelId = "H1", RoomType = "SGL", Arrival = "20240918", Departure="20240919"},
            new Booking(){HotelId = "H1", RoomType = "SGL", Arrival = "20240920", Departure="20240921"},
            new Booking(){HotelId = "H1", RoomType = "SGL", Arrival = "20240922", Departure="20240923"},
            // Row 2
            new Booking(){HotelId = "H1", RoomType = "SGL", Arrival = "20240908", Departure="20240916"},
            new Booking(){HotelId = "H1", RoomType = "SGL", Arrival = "20240919", Departure="20240920"},
            new Booking(){HotelId = "H1", RoomType = "SGL", Arrival = "20240921", Departure="20240922"},
            // Row3
            new Booking(){HotelId = "H1", RoomType = "SGL", Arrival = "20240911", Departure="20240913"},
        };

        // Act
        var ra = HotelService.SearchRooms(hotels, bookings, "H1", DateHelper.ParseDate("20240101"), DateHelper.ParseDate("20241231"), "SGL").ToArray();
        var ra2 = HotelService.SearchRooms(hotels, bookings, "H1", DateHelper.ParseDate("20241001"), DateHelper.ParseDate("20251231"), "SGL").ToArray();
        var ra3 = HotelService.SearchRooms(hotels, bookings, "H1", DateHelper.ParseDate("20231001"), DateHelper.ParseDate("20231231"), "SGL").ToArray();

        // Assert
        Assert.Equal(6, ra.Length);
        Assert.True(ra[0].From == DateHelper.ParseDate("20240101") && ra[0].To == DateHelper.ParseDate("20240905") && ra[0].Availability == 2);
        Assert.True(ra[1].From == DateHelper.ParseDate("20240905") && ra[1].To == DateHelper.ParseDate("20240911") && ra[1].Availability == 1);
        Assert.True(ra[2].From == DateHelper.ParseDate("20240913") && ra[2].To == DateHelper.ParseDate("20240916") && ra[2].Availability == 1);
        Assert.True(ra[3].From == DateHelper.ParseDate("20240916") && ra[3].To == DateHelper.ParseDate("20240918") && ra[3].Availability == 2);
        Assert.True(ra[4].From == DateHelper.ParseDate("20240918") && ra[4].To == DateHelper.ParseDate("20240923") && ra[4].Availability == 1);
        Assert.True(ra[5].From == DateHelper.ParseDate("20240923") && ra[5].To == DateHelper.ParseDate("20241231") && ra[5].Availability == 2);

        Assert.Single(ra2);
        Assert.True(ra2[0].From == DateHelper.ParseDate("20241001") && ra2[0].To == DateHelper.ParseDate("20251231") && ra2[0].Availability == 2);

        Assert.Single(ra3);
        Assert.True(ra3[0].From == DateHelper.ParseDate("20231001") && ra3[0].To == DateHelper.ParseDate("20231231") && ra3[0].Availability == 2);
    }

    [Fact]
    public void SearchRooms_NoRelevantBookings_ReturnsRoomTypeCount()
    {
        // Arrange
        var hotels = new List<Hotel>{
            california, fawltyTowers
        };
        var bookings = new List<Booking>{
            new Booking(){HotelId = "H2", RoomType = "SGL", Arrival = "20240901", Departure="20240902"}, // Different hotel
            new Booking(){HotelId = "H1", RoomType = "DBL", Arrival = "20240901", Departure="20240902"} // Different room type
        };

        // Act
        var ra = HotelService.SearchRooms(hotels, bookings, "H1", DateHelper.ParseDate("20240901"), DateHelper.ParseDate("20240902"), "SGL").ToArray();

        // Assert
        Assert.Single(ra);
        Assert.True(ra[0].From == DateHelper.ParseDate("20240901") && ra[0].To == DateHelper.ParseDate("20240902") && ra[0].Availability == 2);
    }

    [Fact]
    public void SearchRooms_NoBookings_ReturnsRoomTypeCount()
    {
        // Arrange
        var hotels = new List<Hotel>{
            california, fawltyTowers
        };
        var bookings = new List<Booking>();

        // Act
        var ra = HotelService.SearchRooms(hotels, bookings, "H1", DateHelper.ParseDate("20240901"), DateHelper.ParseDate("20240902"), "SGL").ToArray();

        // Assert
        Assert.Single(ra);
        Assert.True(ra[0].From == DateHelper.ParseDate("20240901") && ra[0].To == DateHelper.ParseDate("20240902") && ra[0].Availability == 2);
    }

    [Fact]
    public void SearchRooms_ZeroRooms_ReturnsZero()
    {
        // Arrange
        var hotels = new List<Hotel> { fawltyTowers };
        var bookings = new List<Booking>();

        // Act
        var ra = HotelService.SearchRooms(hotels, bookings, "H1", DateHelper.ParseDate("20240901"), DateHelper.ParseDate("20240902"), "SGL").ToArray();

        // Assert
        Assert.Empty(ra);
    }

    [Fact]
    public void SearchRooms_ZeroHotels_ZeroRooms_ReturnsZero()
    {
        // Arrange
        var hotels = new List<Hotel>();
        var bookings = new List<Booking>();

        // Act
        var ra = HotelService.SearchRooms(hotels, bookings, "H1", DateHelper.ParseDate("20240901"), DateHelper.ParseDate("20240902"), "SGL").ToArray();

        // Assert
        Assert.Empty(ra);
    }


    [Fact]
    public void CheckAvailability_ComplexBookings()
    {
        /*
        Date:      01 02 03 04 05 06 07 08 09 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27 28
        10-20:                                 ●  ●  ●  ●  ●  ●  ●  ●  ●  ●  ○                          //Base interval
        10-20:                                 ●  ●  ●  ●  ●  ●  ●  ●  ●  ●  ○                          //The same interval
        14-16:                                             ●  ●  ○                                      //Inside
        05-14:                  ●  ●  ●  ●  ●  ●  ●  ●  ●  ○                                            //Overlapping from left
        16-25:                                                   ●  ●  ●  ●  ●  ●  ●  ●  ●  ○           //Overlapping from right
        01-10:      ●  ●  ●  ●  ●  ●  ●  ●  ●  ○                                                        //Outside from left
        20-28:                                                               ●  ●  ●  ●  ●  ●  ●  ●  ○  //Outside from right
        22-25:                                                                     ●  ●  ●  ○           //Outside from right
        Booked:     1  1  1  1  2  2  2  2  2  3  3  3  3  3  3  3  3  3  3  2  2  3  3  3  1  1  1  0 
        */
        // Arrange
        var hotels = new List<Hotel> { california };
        var bookings = new List<Booking>{
            new Booking(){HotelId = "H1", RoomType = "SGL", Arrival = "20240910", Departure="20240920"},//Base interval
            new Booking(){HotelId = "H1", RoomType = "SGL", Arrival = "20240910", Departure="20240920"},//The same interval
            new Booking(){HotelId = "H1", RoomType = "SGL", Arrival = "20240914", Departure="20240916"},//Inside
            new Booking(){HotelId = "H1", RoomType = "SGL", Arrival = "20240905", Departure="20240914"},//Overlapping from left
            new Booking(){HotelId = "H1", RoomType = "SGL", Arrival = "20240916", Departure="20240925"},//Overlapping from right
            new Booking(){HotelId = "H1", RoomType = "SGL", Arrival = "20240901", Departure="20240910"},//Outside from left
            new Booking(){HotelId = "H1", RoomType = "SGL", Arrival = "20240920", Departure="20240928"},//Outside from right
            new Booking(){HotelId = "H1", RoomType = "SGL", Arrival = "20240922", Departure="20240925"}//Outside from right
        };

        // Act
        var outsideLeft = HotelService.CheckAvailability(hotels, bookings, "H1", DateHelper.ParseDate("20240801"), DateHelper.ParseDate("20240802"), "SGL");
        var outsideRight = HotelService.CheckAvailability(hotels, bookings, "H1", DateHelper.ParseDate("20240101"), DateHelper.ParseDate("20240112"), "SGL");
        var covering = HotelService.CheckAvailability(hotels, bookings, "H1", DateHelper.ParseDate("20240701"), DateHelper.ParseDate("20241102"), "SGL");
        var inside = HotelService.CheckAvailability(hotels, bookings, "H1", DateHelper.ParseDate("20240902"), DateHelper.ParseDate("20240905"), "SGL");
        var inside2 = HotelService.CheckAvailability(hotels, bookings, "H1", DateHelper.ParseDate("20240926"), DateHelper.ParseDate("20240928"), "SGL");
        var inside3 = HotelService.CheckAvailability(hotels, bookings, "H1", DateHelper.ParseDate("20240906"), DateHelper.ParseDate("20240907"), "SGL");
        var overlapLeft = HotelService.CheckAvailability(hotels, bookings, "H1", DateHelper.ParseDate("20240826"), DateHelper.ParseDate("20240903"), "SGL");
        var overlapRight = HotelService.CheckAvailability(hotels, bookings, "H1", DateHelper.ParseDate("20240927"), DateHelper.ParseDate("20240929"), "SGL");

        // Assert
        Assert.Equal(2, outsideLeft);
        Assert.Equal(2, outsideRight);
        Assert.Equal(-1, covering);
        Assert.Equal(1, inside);
        Assert.Equal(1, inside2);
        Assert.Equal(0, inside3);
        Assert.Equal(1, overlapLeft);
        Assert.Equal(1, overlapRight);
    }

    [Fact]
    public void CheckAvailability_Overbooking_ReturnsNegative()
    {
        // Arrange
        var hotels = new List<Hotel> { fawltyTowers, california };
        var bookings = new List<Booking>{
            new Booking(){HotelId = "H1", RoomType = "SGL", Arrival = "20240901", Departure="20240902"},
            new Booking(){HotelId = "H1", RoomType = "SGL", Arrival = "20240801", Departure="20240905"},
            new Booking(){HotelId = "H1", RoomType = "SGL", Arrival = "20240701", Departure="20250902"},
            new Booking(){HotelId = "H2", RoomType = "SGL", Arrival = "20240901", Departure="20240902"} // Different hotel
        };

        // Act
        var availability = HotelService.CheckAvailability(hotels, bookings, "H1", DateHelper.ParseDate("20240901"), DateHelper.ParseDate("20240902"), "SGL");

        // Assert
        Assert.Equal(-1, availability);
    }

    [Fact]
    public void CheckAvailability_SameInterval_ReturnsSingle()
    {
        // Arrange
        var hotels = new List<Hotel> { fawltyTowers, california };
        var bookings = new List<Booking>{
            new Booking(){HotelId = "H1", RoomType = "SGL", Arrival = "20240901", Departure="20240902"},
            new Booking(){HotelId = "H2", RoomType = "SGL", Arrival = "20240901", Departure="20240902"} // Different hotel
        };

        // Act
        var availability = HotelService.CheckAvailability(hotels, bookings, "H1", DateHelper.ParseDate("20240901"), DateHelper.ParseDate("20240902"), "SGL");

        // Assert
        Assert.Equal(1, availability);
    }

    [Fact]
    public void CheckAvailability_NoRelevantBookings_ReturnsRoomTypeCount()
    {
        // Arrange
        var hotels = new List<Hotel>{
            california, fawltyTowers
        };
        var bookings = new List<Booking>{
            new Booking(){HotelId = "H2", RoomType = "SGL", Arrival = "20240901", Departure="20240902"}, // Different hotel
            new Booking(){HotelId = "H1", RoomType = "DBL", Arrival = "20240901", Departure="20240902"} // Different room type
        };

        // Act
        var availability = HotelService.CheckAvailability(hotels, bookings, "H1", DateHelper.ParseDate("20240901"), DateHelper.ParseDate("20240902"), "SGL");

        // Assert
        Assert.Equal(2, availability);
    }

    [Fact]
    public void CheckAvailability_NoBookings_ReturnsRoomTypeCount()
    {
        // Arrange
        var hotels = new List<Hotel>{
            california, fawltyTowers
        };
        var bookings = new List<Booking>();

        // Act
        var availability = HotelService.CheckAvailability(hotels, bookings, "H1", DateHelper.ParseDate("20240901"), DateHelper.ParseDate("20240902"), "SGL");

        // Assert
        Assert.Equal(2, availability);
    }

    [Fact]
    public void CheckAvailability_ZeroRooms_ReturnsZero()
    {
        // Arrange
        var hotels = new List<Hotel> { fawltyTowers };
        var bookings = new List<Booking>();

        // Act
        var availability = HotelService.CheckAvailability(hotels, bookings, "H1", DateHelper.ParseDate("20240901"), DateHelper.ParseDate("20240902"), "SGL");

        // Assert
        Assert.Equal(0, availability);
    }

    [Fact]
    public void CheckAvailability_ZeroHotels_ZeroRooms_ReturnsZero()
    {
        // Arrange
        var hotels = new List<Hotel>();
        var bookings = new List<Booking>();

        // Act
        var availability = HotelService.CheckAvailability(hotels, bookings, "H1", DateHelper.ParseDate("20240901"), DateHelper.ParseDate("20240902"), "SGL");

        // Assert
        Assert.Equal(0, availability);
    }
}