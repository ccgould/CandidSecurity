using CandidQV.Repositories;
using System.Globalization;

namespace CandidQV.Converters;
public class EmployeeIdToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int id)
        {
            var employeeRepos = App.Services.GetService<EmployeeRepository>();
            // You can customize the format string as needed
            var employee = Task.Run(async () => await employeeRepos.GetByIdAsync(id));

            return employee.Result?.FullName ?? "N/A";

        }
        return value; // Or return string.Empty; or throw an exception
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException(); // Or implement if two-way binding is required
    }
}