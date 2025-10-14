using CandidRadioTracker.Interfaces;

namespace CandidRadioTracker;

public partial class App : Application
{
    public static IServiceProvider Services;
    public static IAlertService AlertSvc;
    public App(IServiceProvider provider)
    {
        InitializeComponent();
        Services = provider;
        AlertSvc = Services.GetService<IAlertService>();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}