using System.Text.RegularExpressions;

namespace HotelReservationSystem.Commands;

public static class CommandParser
{
    public static RoomAvailabilityCommand? GetAvailabilityCommand(string input)
    {
        var parameters = GetParams(input);
        if (parameters.Count != 3) return null;

        return new RoomAvailabilityCommand(parameters[0], parameters[1], parameters[2]);
    }

    public static SearchCommand? GetSearchCommand(string input)
    {
        var parameters = GetParams(input);
        if (parameters.Count != 3) return null;

        return new SearchCommand(parameters[0], parameters[1], parameters[2]);
    }

    static List<string> GetParams(string input)
    {
        var parameters = new List<string>();

        // Example: Availability(H1, 20240901-20240903, DBL)   
        string pattern = @"(\w+)\(([^,]+),\s*([^,]+),\s*([^)]+)\)";

        Match match = Regex.Match(input, pattern);
        if (match.Success)
        {
            for (int i = 2; i < match.Groups.Count; i++)
            {
                parameters.Add(match.Groups[i].Value.Trim());
            }
        }

        return parameters;
    }
}