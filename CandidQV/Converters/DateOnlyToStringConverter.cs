using System.Globalization;

namespace CandidQV.Converters;
public class DateOnlyToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateOnly dateOnly)
        {
            // You can customize the format string as needed
            return dateOnly.ToString("MMM/dd/yyyy", culture);
        }
        return value; // Or return string.Empty; or throw an exception
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException(); // Or implement if two-way binding is required
    }
}