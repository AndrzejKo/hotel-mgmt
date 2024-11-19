using System.Text.Json;

namespace HotelReservationSystem;

public class DataLoader
{
    public static List<T>? LoadData<T>(string filePath)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<T>>(json, options);
    }
}