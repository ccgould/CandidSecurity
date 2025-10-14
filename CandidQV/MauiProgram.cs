using CandidQV.Interfaces;
using CandidQV.Repositories;
using CandidQV.Services;
using CandidQV.ViewModels;
using CandidQV.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;
using ZXing.Net.Maui.Controls;

namespace CandidQV;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitCamera()
            .ConfigureSyncfusionCore()
            .UseBarcodeReader()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Font Awesome 7 Free-Regular-400.otf", "FARegular");
                fonts.AddFont("Font Awesome 7 Free-Solid-900.otf", "FASolid");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<EmployeeRepository>();
        builder.Services.AddSingleton<FlightNumberRepository>();
        builder.Services.AddSingleton<VoucherRepository>();
        builder.Services.AddSingleton<AirlineRepository>();

        builder.Services.AddSingleton<EmployeesPage>();
        builder.Services.AddSingleton<EmployeePageViewModel>();
        builder.Services.AddSingleton<AirlinesPage>();
        builder.Services.AddSingleton<AirlinesPageViewModel>();
        builder.Services.AddSingleton<VouchersPage>();
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MainPageViewModel>();

        builder.Services.AddSingleton<IAlertService, AlertService>();
        builder.Services.AddTransientWithShellRoute<VoucherCreationPage, VoucherCreationPageViewModel>("voucherCreationPage");


        return builder.Build();
    }
}
