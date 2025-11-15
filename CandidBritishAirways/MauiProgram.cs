using CandidBritishAirways.Services;
using CandidBritishAirways.ViewModel;
using CandidBritishAirways.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace CandidBritishAirways;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();


        var getAssembly = Assembly.GetExecutingAssembly();
        using var stream = getAssembly.GetManifestResourceStream("CandidBritishAirways.appsettings.json");
        var config = new ConfigurationBuilder().AddJsonStream(stream)
            .Build();

        builder.Configuration.AddConfiguration(config);

        builder
            .UseMauiApp<App>()
            // Initialize the .NET MAUI Community Toolkit by adding the below line of code
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("FontAwesome6Brands.otf", "FontAwesomeBrands");
                fonts.AddFont("FontAwesome6Regular.otf", "FontAwesomeRegular");
                fonts.AddFont("FontAwesome6Solid.otf", "FontAwesomeSolid");
            });

        var services = builder.Services;

        services.AddTransient<MainPage>();
        services.AddTransient<MainPageViewModel>();
        services.AddTransient<ReportPage>();
        services.AddTransient<ReportViewModel>();

        services.AddSingleton<DatabaseService>();
        services.AddSingleton<SyncService>();
        services.AddSingleton<MySqlEmployeeService>();
        services.AddSingleton<MySqlGateService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
