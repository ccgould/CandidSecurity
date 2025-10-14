using System.Globalization;

namespace CandidQV.Converters;

public class SentStatusConverter : IValueConverter
{
    public Color TrueColor { get; set; } = Colors.Green;
    public Color FalseColor { get; set; } = Colors.Orange;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool flag)
            return flag ? TrueColor : FalseColor;

        return FalseColor; // fallback if value isn't a bool
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
