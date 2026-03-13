using System.Globalization;

namespace AgroConnect.Mobile.Helpers;

public static class Constants
{
#if DEBUG
    // Dispositivo físico: usar IP local del PC en la misma red
    // Emulador Android: cambiar a https://10.0.2.2:7250/api/
    public const string ApiBaseUrl = "https://192.168.0.219:7250/api/";
#else
    public const string ApiBaseUrl = "https://agroconnect.digital/api/";
#endif

    // ── SecureStorage keys ──
    public const string TokenKey = "auth_token";
    public const string TokenExpiresKey = "auth_token_expires";
    public const string LoginDataKey = "auth_login_data";

    // ── Defaults ──
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
    // ── Estados de receta (consistentes con web) ──
    public static Color GetStatusColor(string? status) => status?.ToUpperInvariant() switch
    {
        "ABIERTA" => Color.FromArgb("#FFC107"),
        "PENDIENTE" => Color.FromArgb("#2196F3"),
        "APROBADA" => Color.FromArgb("#4CAF50"),
        "RECHAZADA" => Color.FromArgb("#F44336"),
        "OBSERVADA" => Color.FromArgb("#FF9800"),
        "REDIRIGIDA" => Color.FromArgb("#9C27B0"),
        "CERRADA" => Color.FromArgb("#9E9E9E"),
        "ANULADA" => Color.FromArgb("#F44336"),
        _ => Color.FromArgb("#FFC107")
    };

    // ── Estados de ejecución (alineados con API) ──
    public static Color GetExecutionStatusColor(string? status) => status?.ToUpperInvariant() switch
    {
        "PENDIENTE" => Color.FromArgb("#FFC107"),
        "ACEPTADA" => Color.FromArgb("#2196F3"),
        "EN_CAMINO" => Color.FromArgb("#03A9F4"),
        "EN_CURSO" => Color.FromArgb("#4CAF50"),
        "PAUSADA" => Color.FromArgb("#FF9800"),
        "COMPLETADA" => Color.FromArgb("#2E7D32"),
        "COMPLETADA_ADMIN" => Color.FromArgb("#2E7D32"),
        "CANCELADA" => Color.FromArgb("#F44336"),
        _ => Color.FromArgb("#9E9E9E")
    };

    public static string GetExecutionStatusLabel(string? status) => status?.ToUpperInvariant() switch
    {
        "PENDIENTE" => "Pendiente",
        "ACEPTADA" => "Aceptada",
        "EN_CAMINO" => "En camino",
        "EN_CURSO" => "En curso",
        "PAUSADA" => "Pausada",
        "COMPLETADA" => "Completada",
        "COMPLETADA_ADMIN" => "Completada (admin)",
        "CANCELADA" => "Cancelada",
        _ => status ?? "—"
    };

    // ── Clase toxicológica ──
    public static Color GetToxColor(string? toxClass) => toxClass?.ToUpperInvariant() switch
    {
        "IA" => Color.FromArgb("#9C27B0"),
        "IB" => Color.FromArgb("#F44336"),
        "II" => Color.FromArgb("#FF9800"),
        "III" => Color.FromArgb("#2196F3"),
        "IV" => Color.FromArgb("#4CAF50"),
        _ => Color.FromArgb("#9E9E9E")
    };

    // ── Riesgo territorial ──
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
        "REDIRIGIDA" => "Redirigida",
        "CERRADA" => "Cerrada",
        "ANULADA" => "Anulada",
        _ => status ?? "—"
    };
}