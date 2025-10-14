
using CandidQV.Interfaces;
using Syncfusion.Licensing;

namespace CandidQV;

public partial class App : Application
{
    public static IServiceProvider Services;
    public static IAlertService AlertSvc;

    public App(IServiceProvider provider)
    {
        SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JFaF5cXGRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXZfcXZWQmdYWE12WkBWYEg=");
        InitializeComponent();
        Services = provider;
        AlertSvc = Services.GetService<IAlertService>();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}