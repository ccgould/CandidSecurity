using System.Globalization;

namespace CandidQV.Converters;

class SentStatusTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool flag)
            return flag ? "SENT" : "PENDING";

        return "PENDING"; // fallback if value isn't a bool
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
