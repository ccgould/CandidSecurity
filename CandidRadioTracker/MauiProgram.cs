using CandidRadioTracker.Interfaces;
using CandidRadioTracker.Services;
using CandidRadioTracker.ViewModels;
using CandidRadioTracker.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using ZXing.Net.Maui.Controls;

namespace CandidRadioTracker;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiCommunityToolkit()
            .UseMauiApp<App>()
            .ConfigureSyncfusionToolkit()
            .UseBarcodeReader()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<IAlertService, AlertService>();
        builder.Services.AddSingleton<FireBaseServices>();
        builder.Services.AddSingleton<MainPageViewModel>();
        builder.Services.AddTransient<ScannerPage>();
        builder.Services.AddTransient<ScannerPageViewModel>();
        builder.Services.AddTransient<RadioLogEditorPage>();
        builder.Services.AddTransient<RadioLogEditorViewModel>();
        return builder.Build(); //
    }
}

