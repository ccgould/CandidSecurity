using System;
using System.Globalization;
using CandidQVmMulti.Enumerators;
using Microsoft.Maui.Controls;

namespace CandidQVmMulti.View.Converters;

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            0 => Color.FromArgb("#D1FAE5"),      // Green
            1 => Color.FromArgb("#FEF3C7"),     // Yellow
            2 => Color.FromArgb("#DBEAFE"),   // Blue
            _ => Color.FromArgb("#F3F4F6")              // Default Gray
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class IntToStatusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            VoucherStatus.InProgress => "In Progress",
            VoucherStatus.Pending => "Pending",
            VoucherStatus.Signed => "Signed",
            _ => "N/A"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}