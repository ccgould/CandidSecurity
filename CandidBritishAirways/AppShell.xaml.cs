using CandidBritishAirways.Views;

namespace CandidBritishAirways;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(ReportPage), typeof(ReportPage));
        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));

    }
}
