using System.Globalization;

namespace CandidQVmMulti.View.Converters;
internal class TicksToStringTimeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return TimeOnly.FromTimeSpan(new TimeSpan((long)value));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
