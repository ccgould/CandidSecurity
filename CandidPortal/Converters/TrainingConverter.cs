using CandidPortal.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Windows.Data;

namespace CandidPortal.Converters;
[ValueConversion(typeof(int), typeof(String))]
public class TrainingConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int id = (int)value;

        var task = Task.Run(async () =>
        {
            var service = App.Services.GetService<TrainingTypeRepository>();

            return await service.GetTrainingType(id);
        });

        var result = task.Result;

        if (result is null)
        {
            return "N/A";
        }

        return result.Name;
     }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}
