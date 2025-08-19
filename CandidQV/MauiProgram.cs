using CandidQV.Repositories;
using CandidQV.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace CandidQV;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkitCamera()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<EmployeeRepository>();
        builder.Services.AddSingleton<FlightNumberRepository>();
        builder.Services.AddSingleton<VoucherRepository>();
        builder.Services.AddSingleton<AirlineRepository>();

        builder.Services.AddSingleton<EmployeesPage>();
        builder.Services.AddSingleton<AirlinesPage>();
        builder.Services.AddSingleton<VouchersPage>();


        return builder.Build();
    }
}
