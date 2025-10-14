using CandidQVmMulti.Interfaces;

namespace CandidQVmMulti;

public partial class App : Application
{
    public static IServiceProvider Services;
    public static IAlertService AlertSvc;
    public App(IServiceProvider provider)
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JFaF5cXGRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXZceHVSRGdeUkN/XEpWYEg=");
        InitializeComponent();
        Services = provider;
        AlertSvc = Services.GetService<IAlertService>();

#if WINDOWS || MACCATALYST
        MainPage = new AppShell();
#else
        MainPage = new AndriodAppShell();
#endif
    }
}