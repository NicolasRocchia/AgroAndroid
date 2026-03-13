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
