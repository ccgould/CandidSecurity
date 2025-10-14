using CandidQV.Models.Items;
using CandidQV.ViewModels;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Syncfusion.Maui.Accordion;

namespace CandidQV.Views;

public partial class AirlinesPage : ContentPage
{

    public AirlinesPageViewModel ViewModel => BindingContext as AirlinesPageViewModel;


    public AirlinesPage(AirlinesPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        AirlineAccordion.Expanded += AirlineAccordion_Expanded;
    }

    private void AirlineAccordion_Expanded(object? sender, ExpandedAndCollapsedEventArgs e)
    {
        ViewModel.CurrentExpanded = e.Index;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.InitAsync();
    }

    //private async void OnDeleteRequested(object sender, EventArgs e)
    //{
    //    if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is Airline airline)
    //    {
    //        bool confirm = await DisplayAlert("Confirm Delete",
    //                                          $"Delete {airline.IataCode}?",
    //                                          "Yes", "Cancel");
    //        if (!confirm) return;

    //        await ViewModel.DeleteAirlineAsync(airline);

    //        var snackbarOptions = new SnackbarOptions
    //        {
    //            BackgroundColor = Colors.DarkRed,
    //            TextColor = Colors.White,
    //            ActionButtonTextColor = Colors.Yellow,
    //            CornerRadius = 8,
    //            Font = Microsoft.Maui.Font.Default
    //        };



    //        await Snackbar.Make("Aircraft deleted", async () =>
    //        {
    //            await ViewModel.UndoDeleteAsync();
    //        }, "Undo", TimeSpan.FromSeconds(5), snackbarOptions).Show();
    //    }

    //    if (sender is SwipeItem swipeItem2 && swipeItem2.CommandParameter is FlightNumber flightNumber)
    //    {
    //        bool confirm = await DisplayAlert("Confirm Delete",
    //                                          $"Delete {flightNumber.Number}?",
    //                                          "Yes", "Cancel");
    //        if (!confirm) return;

    //        await ViewModel.DeleteFlightNumberAsync(flightNumber);

    //        var snackbarOptions = new SnackbarOptions
    //        {
    //            BackgroundColor = Colors.DarkRed,
    //            TextColor = Colors.White,
    //            ActionButtonTextColor = Colors.Yellow,
    //            CornerRadius = 8,
    //            Font = Microsoft.Maui.Font.Default
    //        };



    //        await Snackbar.Make("Flight Number deleted", async () =>
    //        {
    //            await ViewModel.UndoFlightNumberDeleteAsync();
    //        }, "Undo", TimeSpan.FromSeconds(5), snackbarOptions).Show();
    //    }
    //}

    private async void OnEditRequested(object sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is Airline airline)
        {
            await ViewModel.OnAircraftSelected(airline);
        }
    }
}
