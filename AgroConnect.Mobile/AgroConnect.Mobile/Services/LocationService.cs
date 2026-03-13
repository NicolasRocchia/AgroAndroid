using System.Globalization;
using System.Net.Http.Json;
using AgroConnect.Mobile.Models;
using AgroConnect.Mobile.Services.Interfaces;

namespace AgroConnect.Mobile.Services;

public class LocationService : ILocationService
{
    public async Task<(double Latitude, double Longitude)?> GetCurrentLocationAsync()
    {
        try
        {
            var location = await Geolocation.Default.GetLocationAsync(
                new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10)));
            return location is null ? null : (location.Latitude, location.Longitude);
        }
        catch { return null; }
    }

    public async Task<WeatherData?> GetWeatherAsync(double lat, double lng)
    {
        try
        {
            var latStr = lat.ToString("F4", CultureInfo.InvariantCulture);
            var lngStr = lng.ToString("F4", CultureInfo.InvariantCulture);
            var url = $"https://api.open-meteo.com/v1/forecast?latitude={latStr}&longitude={lngStr}&current_weather=true&windspeed_unit=kmh";

            using var http = new HttpClient();
            var response = await http.GetFromJsonAsync<OpenMeteoResponse>(url);
            if (response?.CurrentWeather is null) return null;

            var w = response.CurrentWeather;
            return new WeatherData(w.Temperature, 0, w.Windspeed, DegreesToDirection(w.Winddirection));
        }
        catch { return null; }
    }

    private static string DegreesToDirection(double deg) => deg switch
    {
        >= 337.5 or < 22.5 => "N", >= 22.5 and < 67.5 => "NE",
        >= 67.5 and < 112.5 => "E", >= 112.5 and < 157.5 => "SE",
        >= 157.5 and < 202.5 => "S", >= 202.5 and < 247.5 => "SO",
        >= 247.5 and < 292.5 => "O", _ => "NO"
    };

    private record OpenMeteoResponse(CurrentWeather? CurrentWeather);
    private record CurrentWeather(double Temperature, double Windspeed, double Winddirection);
}
