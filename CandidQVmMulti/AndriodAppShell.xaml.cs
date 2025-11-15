using CandidQVmMulti.View.Pages;

namespace CandidQVmMulti;

public partial class AndriodAppShell : Shell
{
    public AndriodAppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(AddVoucherPage), typeof(AddVoucherPage));
        Routing.RegisterRoute(nameof(AirlinesPage), typeof(AirlinesPage));
        Routing.RegisterRoute(nameof(EmployeesPage), typeof(EmployeesPage));
        Routing.RegisterRoute(nameof(SignaturePage), typeof(SignaturePage));
    }
}
