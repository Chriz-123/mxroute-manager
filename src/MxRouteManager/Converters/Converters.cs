using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using MxRouteManager.Models;

namespace MxRouteManager.Converters;

/// <summary>true, wenn beide gebundenen Werte (als String) gleich sind.</summary>
public sealed class MultiEqualsConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
        => values.Length == 2 && string.Equals(values[0]?.ToString(), values[1]?.ToString(), StringComparison.Ordinal);

    public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>true -> Visible, false -> Collapsed. Parameter "invert" kehrt um.</summary>
public sealed class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool b = value is bool v && v;
        if (parameter as string == "invert") b = !b;
        return b ? Visibility.Visible : Visibility.Collapsed;
    }
    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Nicht-leerer String / nicht-null -> Visible.</summary>
public sealed class NotEmptyToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool has = value is string s ? !string.IsNullOrWhiteSpace(s) : value is not null;
        if (parameter as string == "invert") has = !has;
        return has ? Visibility.Visible : Visibility.Collapsed;
    }
    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public sealed class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && !b;
    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && !b;
}

/// <summary>Vergleicht den Wert mit dem Parameter (String) -> bool.</summary>
public sealed class StringEqualsConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => string.Equals(value?.ToString(), parameter?.ToString(), StringComparison.Ordinal);
    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        => (value is bool b && b) ? parameter?.ToString() ?? "" : Binding.DoNothing;
}

/// <summary>bool -> Farbe: true=gruen (aktiv), false=grau/rot.</summary>
public sealed class BoolToStatusBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool b = value is bool v && v;
        if (parameter as string == "invert") b = !b;
        return new SolidColorBrush(b ? Color.FromRgb(0x22, 0xC5, 0x5E) : Color.FromRgb(0xEF, 0x44, 0x44));
    }
    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Byte-Zahl -> lesbare Groesse.</summary>
public sealed class BytesToHumanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        => value is long l ? FormatBytes.Humanize(l) : (value is int i ? FormatBytes.Humanize(i) : "0 B");
    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
