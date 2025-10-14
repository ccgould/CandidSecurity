using CandidQV.Views;
using CommunityToolkit.Maui.Alerts;

namespace CandidQV;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(VoucherCreationPage),typeof(VoucherCreationPage));
        Routing.RegisterRoute(nameof(BarcodeScanner),typeof(BarcodeScanner));

    }

    public static async Task DisplayToastAsync(string message)
    {
        // Toast is currently not working in MCT on Windows
        if (OperatingSystem.IsWindows())
            return;

        var toast = Toast.Make(message, textSize: 18);

        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await toast.Show(cts.Token);
    }
}
