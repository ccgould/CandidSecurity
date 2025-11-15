using System.Globalization;

namespace CandidBritishAirways.Converters
{
    public class BoolToSubmitText : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            

            if (value is not null && value is bool b) 
            { 
                return b ? "Save Report" : "Submit Report";
            }

            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
