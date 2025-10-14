using CandidRadioTracker.Views;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CandidRadioTracker.ViewModels;
public partial class RadioLogEditorViewModel: ObservableObject
{
    private readonly FireBaseServices fireBaseServices;
    public static TaskCompletionSource<string> BarcodeResultSource = new();
    [ObservableProperty] private string barcode;
    [ObservableProperty] private TimeSpan inTme;
    [ObservableProperty] private TimeSpan outTime;

    public RadioLogEditorViewModel(FireBaseServices fireBaseServices)
    {
        this.fireBaseServices = fireBaseServices;
    }

    [RelayCommand]
    private async Task ScanBarcode()
    {

        await Shell.Current.GoToAsync(nameof(ScannerPage));
        Barcode = await BarcodeResultSource.Task;
    }


    public async Task SaveRadioLog(string value)
    {
        //Select the guard
        string result = await App.AlertSvc.PromptAsync("Select Guard", "Please select a guard that the radio has been assigned to", "OK", "Cancel", "Enter Flight Number", 6, Keyboard.Numeric);

        if (string.IsNullOrWhiteSpace(result))
        {
            await App.AlertSvc.ShowAlertAsync("No flight Number", "Field was empty please type a flight number");
            return;
        }

        await fireBaseServices.SaveRadioLog(value, 0, DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromTimeSpan(DateTime.Now.TimeOfDay));

        var snackbarOptions = new SnackbarOptions
        {
            BackgroundColor = Colors.DarkRed,
            TextColor = Colors.White,
            ActionButtonTextColor = Colors.Yellow,
            CornerRadius = 8,
            Font = Microsoft.Maui.Font.Default
        };

        await Snackbar.Make("Log Saved", async () =>
        {
            //await UndoFlightNumberDeleteAsync();
        }, "Undo", TimeSpan.FromSeconds(5), snackbarOptions).Show();

    }
}
