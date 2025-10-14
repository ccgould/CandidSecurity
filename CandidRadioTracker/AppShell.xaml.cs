using CandidRadioTracker.Views;

namespace CandidRadioTracker;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(ScannerPage), typeof(ScannerPage));
        Routing.RegisterRoute(nameof(RadioLogEditorPage), typeof(RadioLogEditorPage));
    }
}
