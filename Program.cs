using HotelReservationSystem.Commands;
using HotelReservationSystem.Models;
using HotelReservationSystem.Utils;

namespace HotelReservationSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: myapp --hotels <hotels.json> --bookings <bookings.json>");
                return;
            }

            string hotelsFilePath = args[1];
            string bookingsFilePath = args[3];

            List<Hotel>? hotels = DataLoader.LoadData<Hotel>(hotelsFilePath);
            if (hotels == null) Console.WriteLine("Error: Invalid hotel file");

            List<Booking>? bookings = DataLoader.LoadData<Booking>(bookingsFilePath);
            if (hotels == null) Console.WriteLine("Error: Invalid bookings file");

            while (true)
            {
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    break;
                }

                if (input.StartsWith("Availability"))
                {
                    try
                    {
                        var command = CommandParser.GetAvailabilityCommand(input);
                        if (command == null)
                        {
                            Console.WriteLine("Invalid command");
                        }
                        else
                        {
                            var availability = HotelService.CheckAvailability(hotels!, bookings!, command.HotelId, command.From, command.To, command.RoomType);
                            Console.WriteLine(availability);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                    }
                }
                else if (input.StartsWith("Search"))
                {
                    try
                    {
                        var command = CommandParser.GetSearchCommand(input);
                        if (command == null)
                        {
                            Console.WriteLine("Invalid command");
                        }
                        else
                        {
                            var today = DateTime.Now.Date;
                            var availability = HotelService.SearchRooms(hotels!, bookings!, command.HotelId, today, today.AddDays(command.Days), command.RoomType);
                            var formatted = availability.Select(x=>Formatter.FormatRoomAvailability(x));
                            Console.WriteLine(string.Join(",", formatted));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                    }
                }
                else if (string.IsNullOrWhiteSpace(input))
                {
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("Invalid command");
                }
            }
        }
    }
}