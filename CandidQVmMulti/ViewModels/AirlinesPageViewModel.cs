using CandidQVmMulti.Models;
using CandidQVmMulti.Services;
using CandidQVmMulti.View.Popups;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CandidQVmMulti.ViewModels;

public partial class AirlinesPageViewModel : ObservableObject
{
    [ObservableProperty]
   
    private string searchText;

    [ObservableProperty]
    private ObservableCollection<Airline> airlines;
    private bool _isLoading;
    [ObservableProperty] private bool isBusy;
    private Airline deletedFlightAirline;
    private FlightNumber deletedFlight;
    private readonly MySqlAirlinesService service;
    [ObservableProperty] private ObservableCollection<Airline> filteredAirlines;

    public bool PreventRefresh { get; private set; }

    public int AirlinesCount => Airlines?.Count ?? 0;

    public MySqlFlightNumberService MySqlFlightNumberService { get; }

    public AirlinesPageViewModel(MySqlAirlinesService service, MySqlFlightNumberService mySqlFlightNumberService)
    {
        this.service = service;
        MySqlFlightNumberService = mySqlFlightNumberService;
        airlines = new();
        filteredAirlines = new();
    }


    internal async Task LoadAirlinesAsync()
    {
        if (_isLoading) return;
        _isLoading = true;

        try
        {
            IsBusy = true; // triggers spinner
            var employees = await service.GetAllAsync();
            Airlines.Clear();
            foreach (var emp in employees)
                Airlines.Add(emp);

            OnPropertyChanged(nameof(AirlinesCount));

            FilterAirlines();


            await Toast.Make("Airlines loaded successfully", duration: ToastDuration.Short).Show();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
            _isLoading = false;
        }

        var collection = await service.GetAllAsync();

        if (collection is not null)
        {
            Airlines = new ObservableCollection<Airline>(collection);
        }
    }

    [RelayCommand]
    private async Task AddAirline()
    {
        PreventRefresh = true;
        var popup = new AddNewAirlineEditor();
        IPopupResult<Airline> result = await Shell.Current.CurrentPage.ShowPopupAsync<Airline>(popup);

        if (!result.WasDismissedByTappingOutsideOfPopup)
        {
            if (result.Result == null)
                return;

            result.Result.AddDate = DateTime.Now.Ticks;

            await service.AddAsync(result.Result);

            // Show toast after undo
            var toast = Toast.Make($"Airline {result.Result.Name} was added to the database!", ToastDuration.Short, 14);
            await toast.Show();

            Airlines.Add(result.Result);

            FilterAirlines();

            OnPropertyChanged(nameof(AirlinesCount));
            PreventRefresh = false;
        }
    }

    [RelayCommand]
    private async Task EditAirline(Airline airline)
    {
        PreventRefresh = true;
        var popup = new EditAirlineEditor(airline);
        IPopupResult<Airline> result = await Shell.Current.CurrentPage.ShowPopupAsync<Airline>(popup);

        if (!result.WasDismissedByTappingOutsideOfPopup)
        {
            if (result.Result == null)
                return;

            airline.Name = result.Result.Name;
            airline.Iata = result.Result.Iata;


            await service.UpdateAsync(airline);

            // Show toast after undo
            var toast = Toast.Make($"Airline {airline.Name} was updated in the database!", ToastDuration.Short, 14);
            await toast.Show();
            PreventRefresh = false;
        }
    }

    [RelayCommand]
    private async Task DeleteAirline(Airline airline)
    {
        if (airline != null)
            Airlines.Remove(airline);
        OnPropertyChanged(nameof(AirlinesCount));

        await service.DeleteAsync(airline.Id);
    }

    [RelayCommand]
    private async Task AddFlight(Airline airline)
    {
        PreventRefresh = true;
        if (airline != null)
        {
            var popup = new FlightTerminalPopup();
            IPopupResult<FlightTerminalResult> result = await Shell.Current.CurrentPage.ShowPopupAsync<FlightTerminalResult>(popup);

            if (!result.WasDismissedByTappingOutsideOfPopup)
            {
                if(result.Result == null) 
                    return;

                //Use result.FlightNumber and result.Terminal
                var flightNumber = airline.AddFlightNumber(airline.Id,result.Result.Terminal, result.Result.FlightNumber, DateTime.Now);
                await MySqlFlightNumberService.AddFlightNumberAsync(flightNumber);

                // Show toast after undo
                var toast = Toast.Make($"Flight {airline.Iata}{result.Result.FlightNumber} added to {airline.Name}!", ToastDuration.Short, 14);
                await toast.Show();
            }
        }
        PreventRefresh = false;
    }

    [RelayCommand]
    private async Task DeleteFlight(FlightNumber flight)
    {
        if (flight != null)
        {
            foreach (var airline in Airlines)
            {
                if (airline.FlightNumbers.Contains(flight))
                {
                    airline.FlightNumbers.Remove(flight);
                    deletedFlightAirline = airline;
                    deletedFlight = flight;
                    await MySqlFlightNumberService.DeleteFlightNumberAsync(flight.Id);
                    break;
                }
            }
        }

        await ShowUndoSnackbarAsync(() =>
        {
            deletedFlightAirline.FlightNumbers.Add(deletedFlight); // Restore it
            _ = MySqlFlightNumberService.AddFlightNumberAsync(deletedFlight); // Re-add to DB
        });

    }

    public async Task ShowUndoSnackbarAsync(Action undoAction)
    {
        var snackbar = Snackbar.Make(
            "Flight deleted",
            async () =>
            {
                undoAction?.Invoke();

                // Show toast after undo
                var toast = Toast.Make("Flight restored", ToastDuration.Short, 14);
                await toast.Show();
            },
            "Undo",
            TimeSpan.FromSeconds(5));

        await snackbar.Show();
    }

    private void FilterAirlines()
    {
        FilteredAirlines.Clear();

        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? Airlines
            : Airlines.Where(v =>
                (v.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (v.Iata?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false));

        foreach (var voucher in filtered)
            FilteredAirlines.Add(voucher);
    }

    [RelayCommand]
    private void PerformSearch()
    {
        FilterAirlines();
    }
}
