using CandidQVmMulti.Interfaces;
using CandidQVmMulti.View.Windows;

namespace CandidQVmMulti;

public partial class App : Application
{
    public static IServiceProvider Services;
    private readonly CustomTitleWindow titleWindow;
    public static IAlertService AlertSvc;
    public App(IServiceProvider provider, CustomTitleWindow titleWindow)
    {
        SentrySdk.CaptureMessage("Hello Sentry");
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JFaF5cXGRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXZceHVSRGdeUkN/XEpWYEg=");
        InitializeComponent();
        Services = provider;
        this.titleWindow = titleWindow;
        AlertSvc = Services.GetService<IAlertService>();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
#if WINDOWS || MACCATALYST
        titleWindow.Page = new AppShell();
#else
        titleWindow.Page = new AndriodAppShell();
#endif
        //return base.CreateWindow(activationState);
              

        return titleWindow;
    }
}