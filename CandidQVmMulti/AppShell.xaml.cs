using CandidQVmMulti.View.Pages;

namespace CandidQVmMulti;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(AddVoucherPage), typeof(AddVoucherPage));
    }
}
