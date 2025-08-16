using CandidPortal.Repositories;
using CandidPortal.ViewModels.Pages;
using Wpf.Ui.Abstractions.Controls;

namespace CandidPortal.Views.Pages;

/// <summary>
/// Interaction logic for EmployeesPage.xaml
/// </summary>
public partial class EmployeesPage : INavigableView<EmployeesPageViewModel>
{
    public EmployeesPage(EmployeesPageViewModel vm)
    {
        InitializeComponent();
        ViewModel = vm;
        DataContext = this;
    }

    public EmployeesPageViewModel ViewModel { get; }

    private async void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
    }

    private void employeeCmb_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {

    }
}