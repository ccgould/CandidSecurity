using CandidQVmMulti.View.Pages;
using CandidQVmMulti.ViewModels;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;
using static CandidQVmMulti.ViewModels.AppShellViewModel;

namespace CandidQVmMulti;

public partial class AppShell : Shell
{
    private AppShellViewModel _vm;

    public ObservableCollection<FlyoutItemModel> MenuItms { get; private set; } = new();
    public AppShell()
    {
        InitializeComponent();
        _vm = new AppShellViewModel();
        BindingContext = _vm;
        Navigating += AppShell_Navigating;
        Routing.RegisterRoute(nameof(AddVoucherPage), typeof(AddVoucherPage));
    }

    private void AppShell_Navigating(object? sender, ShellNavigatingEventArgs e)
    {
        _vm.RefreshMenu(sender, e);
    }


    private void FlyoutCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }

    private void ProgressBar_MeasureInvalidated(object sender, EventArgs e)
    {

    }
}
