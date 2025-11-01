using CandidQVmMulti.Interfaces;
using CandidQVmMulti.Services;
using CandidQVmMulti.View.Pages;
using CandidQVmMulti.ViewModels;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;
using System.Reflection;

namespace CandidQVmMulti;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        var getAssembly = Assembly.GetExecutingAssembly();
        using var stream = getAssembly.GetManifestResourceStream("CandidQVmMulti.appsettings.json");
        var config = new ConfigurationBuilder().AddJsonStream(stream)
            .Build();

        builder.Configuration.AddConfiguration(config);



        builder
            .UseMauiApp<App>()
            // Add this section anywhere on the builder:
            .UseSentry(options => {
                // The DSN is the only required setting.
                options.Dsn = "https://fcb198d8c07d54dddd45f7f8cecc8aac@o4510272878870528.ingest.us.sentry.io/4510272880967680";

                // Use debug mode if you want to see what the SDK is doing.
                // Debug messages are written to stdout with Console.Writeline,
                // and are viewable in your IDE's debug console or with 'adb logcat', etc.
                // This option is not recommended when deploying your application.
                options.Debug = true;

                // Other Sentry options can be set here.
            })
            .ConfigureSyncfusionCore()
            .UseMauiCommunityToolkit(options =>
            {
                options.SetShouldEnableSnackbarOnWindows(true);
            })
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<MainPageViewModel>();
        builder.Services.AddTransient<AddVoucherPage>();
        builder.Services.AddTransient<AddVoucherPageViewModel>();
        builder.Services.AddTransient<VouchersPage>();
        builder.Services.AddTransient<VouchersPageViewModel>();
        builder.Services.AddTransient<EmployeesPage>();
        builder.Services.AddTransient<EmployeesPageViewModel>();
        builder.Services.AddTransient<AirlinesPage>();
        builder.Services.AddTransient<AirlinesPageViewModel>();
        builder.Services.AddSingleton<MySqlEmployeeService>();
        builder.Services.AddSingleton<MySqlAirlinesService>();
        builder.Services.AddSingleton<MySqlVoucherService>();
        builder.Services.AddSingleton<MySqlFlightNumberService>();
        builder.Services.AddSingleton<IAlertService, AlertService>();
        builder.Services.AddTransient<CreateVoucherPopupViewModel>();
        builder.Services.AddSingleton<ExportServices>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
