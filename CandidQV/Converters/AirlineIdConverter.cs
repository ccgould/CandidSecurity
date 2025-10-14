namespace CandidQV.Converters;

using System;
using System.Globalization;
using Microsoft.Maui.Controls;

public class AirlineIdConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string iata || string.IsNullOrWhiteSpace(iata))
            return "default_airline.png"; // fallback icon

        // Normalize and map IATA to icon filename
        iata = iata.ToLowerInvariant();

        return $"{iata.ToLower()}.png";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
