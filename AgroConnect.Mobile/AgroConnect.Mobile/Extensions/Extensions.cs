using System.Globalization;

namespace AgroConnect.Mobile.Extensions;

public static class StringExtensions
{
    public static double ToInvariantDouble(this string value) => double.Parse(value, CultureInfo.InvariantCulture);
    public static string ToInvariantString(this double value, string format = "F6") => value.ToString(format, CultureInfo.InvariantCulture);
}

public static class CollectionExtensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source) => source is null || !source.Any();
}
