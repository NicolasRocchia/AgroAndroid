using System.Globalization;

namespace AgroConnect.Mobile.Helpers;

public static class Constants
{
#if DEBUG
    public const string ApiBaseUrl = "https://10.0.2.2:7001/api";
#else
    public const string ApiBaseUrl = "https://agroconnect.digital/api";
#endif

    public const string TokenKey = "auth_token";
    public const string RefreshTokenKey = "auth_refresh_token";
    public const string UserInfoKey = "user_info";
    public static readonly TimeSpan HttpTimeout = TimeSpan.FromSeconds(30);
    public const double DefaultLatitude = -32.69;
    public const double DefaultLongitude = -62.10;
    public const int DefaultPageSize = 20;
}

public static class DateHelper
{
    private static readonly TimeZoneInfo ArgentinaTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("America/Argentina/Buenos_Aires");

    public static DateTime ToArgentina(this DateTime utcDate)
    {
        if (utcDate.Kind == DateTimeKind.Local)
            utcDate = utcDate.ToUniversalTime();
        return TimeZoneInfo.ConvertTimeFromUtc(utcDate, ArgentinaTimeZone);
    }

    public static string ToArgentinaString(this DateTime utcDate, string format = "dd/MM/yyyy")
        => utcDate.ToArgentina().ToString(format, CultureInfo.GetCultureInfo("es-AR"));

    public static string ToArgentinaDateTimeString(this DateTime utcDate)
        => utcDate.ToArgentina().ToString("dd/MM/yyyy HH:mm", CultureInfo.GetCultureInfo("es-AR"));

    public static string TimeAgo(this DateTime utcDate)
    {
        var diff = DateTime.UtcNow - utcDate;
        return diff.TotalMinutes switch
        {
            < 1 => "ahora",
            < 60 => $"hace {(int)diff.TotalMinutes} min",
            < 1440 => $"hace {(int)diff.TotalHours}h",
            < 10080 => $"hace {(int)diff.TotalDays}d",
            _ => utcDate.ToArgentinaString()
        };
    }
}

public static class StatusHelper
{
    public static Color GetStatusColor(string? status) => status?.ToUpperInvariant() switch
    {
        "ABIERTA" => Color.FromArgb("#FFC107"),
        "PENDIENTE" => Color.FromArgb("#2196F3"),
        "APROBADA" => Color.FromArgb("#4CAF50"),
        "RECHAZADA" => Color.FromArgb("#F44336"),
        "OBSERVADA" => Color.FromArgb("#FF9800"),
        "CERRADA" => Color.FromArgb("#9E9E9E"),
        "ANULADA" => Color.FromArgb("#F44336"),
        _ => Color.FromArgb("#FFC107")
    };

    public static Color GetToxColor(string? toxClass) => toxClass?.ToUpperInvariant() switch
    {
        "IA" => Color.FromArgb("#9C27B0"),
        "IB" => Color.FromArgb("#F44336"),
        "II" => Color.FromArgb("#FF9800"),
        "III" => Color.FromArgb("#2196F3"),
        "IV" => Color.FromArgb("#4CAF50"),
        _ => Color.FromArgb("#9E9E9E")
    };

    public static Color GetRiskColor(string? risk) => risk?.ToUpperInvariant() switch
    {
        "ALTO" => Color.FromArgb("#F44336"),
        "MEDIO" => Color.FromArgb("#FF9800"),
        "BAJO" => Color.FromArgb("#4CAF50"),
        _ => Color.FromArgb("#9E9E9E")
    };

    public static string GetStatusLabel(string? status) => status?.ToUpperInvariant() switch
    {
        "ABIERTA" => "Abierta",
        "PENDIENTE" => "Pendiente",
        "APROBADA" => "Aprobada",
        "RECHAZADA" => "Rechazada",
        "OBSERVADA" => "Observada",
        "CERRADA" => "Cerrada",
        "ANULADA" => "Anulada",
        _ => status ?? "—"
    };
}
