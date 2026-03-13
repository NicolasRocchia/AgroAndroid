using System.Globalization;
using AgroConnect.Mobile.Helpers;

namespace AgroConnect.Mobile.Converters;

public class StatusToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type t, object? p, CultureInfo c) => StatusHelper.GetStatusColor(value as string);
    public object? ConvertBack(object? value, Type t, object? p, CultureInfo c) => throw new NotImplementedException();
}

public class ToxClassToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type t, object? p, CultureInfo c) => StatusHelper.GetToxColor(value as string);
    public object? ConvertBack(object? value, Type t, object? p, CultureInfo c) => throw new NotImplementedException();
}

public class RiskToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type t, object? p, CultureInfo c) => StatusHelper.GetRiskColor(value as string);
    public object? ConvertBack(object? value, Type t, object? p, CultureInfo c) => throw new NotImplementedException();
}

public class DateToArgentinaConverter : IValueConverter
{
    public object? Convert(object? value, Type t, object? p, CultureInfo c) => value is DateTime dt ? dt.ToArgentinaString() : "—";
    public object? ConvertBack(object? value, Type t, object? p, CultureInfo c) => throw new NotImplementedException();
}

public class InvertBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type t, object? p, CultureInfo c) => value is bool b ? !b : value;
    public object? ConvertBack(object? value, Type t, object? p, CultureInfo c) => value is bool b ? !b : value;
}

/// <summary>Devuelve true si el string no es null ni vacío (para IsVisible en labels de error)</summary>
public class StringNotNullOrEmptyConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c)
        => value is string s && !string.IsNullOrEmpty(s);
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c)
        => throw new NotImplementedException();
}

public class ExecutionStatusToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type t, object? p, CultureInfo c)
        => StatusHelper.GetExecutionStatusColor(value as string);
    public object? ConvertBack(object? value, Type t, object? p, CultureInfo c)
        => throw new NotImplementedException();
}

/// <summary>int > 0 → true (para IsVisible de secciones con contadores)</summary>
public class IntToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c)
        => value is int n && n > 0;
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c)
        => throw new NotImplementedException();
}

/// <summary>bool → "✅" / "❌" para mostrar estado de checklist items</summary>
public class BoolToCheckConverter : IValueConverter
{
    public object Convert(object? value, Type t, object? p, CultureInfo c)
        => value is true ? "✅" : "❌";
    public object ConvertBack(object? value, Type t, object? p, CultureInfo c)
        => throw new NotImplementedException();
}

/// <summary>Parsea el JSON de meteo de la API y devuelve texto legible</summary>
public class WeatherJsonConverter : IValueConverter
{
    public object? Convert(object? value, Type t, object? p, CultureInfo c)
    {
        if (value is not string json || string.IsNullOrWhiteSpace(json))
            return "—";

        try
        {
            var doc = System.Text.Json.JsonDocument.Parse(json);
            var root = doc.RootElement;

            var parts = new List<string>();

            if (root.TryGetProperty("temperature_c", out var temp))
                parts.Add($"🌡️ {temp.GetDouble():F1}°C");

            if (root.TryGetProperty("humidity_pct", out var hum))
                parts.Add($"💧 {hum.GetDouble():F0}%");

            if (root.TryGetProperty("wind_speed_kmh", out var wind))
            {
                var windStr = $"💨 {wind.GetDouble():F1} km/h";
                if (root.TryGetProperty("wind_direction_deg", out var dir))
                    windStr += $" ({DegreesToDirection(dir.GetDouble())})";
                parts.Add(windStr);
            }

            if (root.TryGetProperty("wind_gusts_kmh", out var gusts) && gusts.GetDouble() > 0)
                parts.Add($"🌬️ Ráfagas {gusts.GetDouble():F0} km/h");

            if (root.TryGetProperty("precipitation_mm", out var precip) && precip.GetDouble() > 0)
                parts.Add($"🌧️ {precip.GetDouble():F1} mm");

            return parts.Count > 0 ? string.Join("  ·  ", parts) : "Sin datos";
        }
        catch
        {
            return "Sin datos";
        }
    }

    public object? ConvertBack(object? value, Type t, object? p, CultureInfo c)
        => throw new NotImplementedException();

    private static string DegreesToDirection(double deg) => deg switch
    {
        >= 337.5 or < 22.5 => "N", >= 22.5 and < 67.5 => "NE",
        >= 67.5 and < 112.5 => "E", >= 112.5 and < 157.5 => "SE",
        >= 157.5 and < 202.5 => "S", >= 202.5 and < 247.5 => "SO",
        >= 247.5 and < 292.5 => "O", _ => "NO"
    };
}
