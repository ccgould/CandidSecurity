using System.Globalization;

namespace CandidQVmMulti.View.Converters
{
    internal class PrefixMultiConverter : IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values[0] is not null && values[1] is not null)
            {
                var terminal = (int)values[0];
                var airline = (string)values[1];

                string prefix = (terminal == 0 ? "(DOM)" : "(US)") ?? string.Empty;
                string name = values[1]?.ToString() ?? string.Empty;

                return $"{name} {prefix}";

            }

            return values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
