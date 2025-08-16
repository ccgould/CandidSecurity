using System.Globalization;
using System.Windows.Data;

namespace CandidPortal.Converters;
public class DateOnlyToDateTimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateOnly dateOnlyValue)
        {
            // Convert DateOnly to DateTime (time component will be midnight)
            return new DateTime(dateOnlyValue.Year, dateOnlyValue.Month, dateOnlyValue.Day);
        }
        return null; // Or DependencyProperty.UnsetValue for binding failures
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime dateTimeValue)
        {
            // Convert DateTime to DateOnly (time component will be ignored)
            return new DateOnly(dateTimeValue.Year, dateTimeValue.Month, dateTimeValue.Day);
        }
        return null; // Or DependencyProperty.UnsetValue for binding failures
    }
}
