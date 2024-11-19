using System.Globalization;

namespace HotelReservationSystem.Utils;

public static class DateHelper
{
    public static DateTime ParseDate(string input)
    {
        return DateTime.ParseExact(input.Trim(), "yyyyMMdd", CultureInfo.InvariantCulture);
    }

    public static string DateToString(DateTime input)
    {
        return input.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
    }
}