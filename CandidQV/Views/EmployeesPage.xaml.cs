using CandidQV.Models.Items;
using CandidQV.ViewModels;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace CandidQV.Views;

public partial class EmployeesPage : ContentPage
{
    public EmployeePageViewModel ViewModel => BindingContext as EmployeePageViewModel;

    public EmployeesPage(EmployeePageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.InitAsync();
    }

    private async void OnDeleteRequested(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is Employee employee)
        {
            bool confirm = await DisplayAlert("Confirm Delete",
                                              $"Delete {employee.FullName}?",
                                              "Yes", "Cancel");
            if (!confirm) return;

            await ViewModel.DeleteEmployeeAsync(employee);

            var snackBarOptions = new SnackbarOptions
            {
                BackgroundColor = Colors.DarkRed,
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.Yellow,
                CornerRadius = 8,
                Font = Microsoft.Maui.Font.Default
            };

            await Snackbar.Make("Employee deleted", async () =>
            {
                await ViewModel.UndoDeleteAsync();
            }, "Undo", TimeSpan.FromSeconds(5), snackBarOptions).Show();
        }
    }

    private void OnEditRequested(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is Employee employee)
        {
            ViewModel.OnEmployeeSelected(employee);
        }
    }
}
