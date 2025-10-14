using CandidQV.Models.Items;
using CandidQV.Repositories;
using CandidQV.ViewModels;
using CandidQV.Views;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.Threading.Tasks;

namespace CandidQV;

public partial class MainPage : ContentPage
{

    public MainPageViewModel ViewModel => BindingContext as MainPageViewModel;

    public MainPage(MainPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        refreshView.Refreshing += OnRefresh;
    }

    private void OnRefresh(object? sender, EventArgs e)
    {
        ViewModel.Refresh();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.Refresh();
    }

    private async void OnDeleteRequested(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is Voucher voucher)
        {
            bool confirm = await DisplayAlert("Confirm Delete",
                                              $"Delete {voucher.PassengerName}?",
                                              "Yes", "Cancel");
            if (!confirm) return;

            await ViewModel.DeleteVoucherAsync(voucher);

            var snackBarOptions = new SnackbarOptions
            {
                BackgroundColor = Colors.DarkRed,
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.Yellow,
                CornerRadius = 8,
                Font = Microsoft.Maui.Font.Default
            };

            await Snackbar.Make("Voucher deleted", async () =>
            {
                await ViewModel.UndoDeleteAsync();
            }, "Undo", TimeSpan.FromSeconds(5), snackBarOptions).Show();
        }
    }

    private async void OnEditRequested(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is Voucher voucher)
        {
            await ViewModel.OnVoucherSelected(voucher);
        }
    }
}
