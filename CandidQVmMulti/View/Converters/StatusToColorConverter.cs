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
            VoucherStatus.InProgress => Color.FromArgb("#B22222"),      // Firebrick
            VoucherStatus.Pending => Color.FromArgb("#FEF3C7"),     // Yellow
            VoucherStatus.Signed => Color.FromArgb("#D1FAE5"),   // Green
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