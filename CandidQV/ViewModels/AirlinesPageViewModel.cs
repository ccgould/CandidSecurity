using CandidQV.Models.Items;
using CandidQV.Repositories;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Syncfusion.Maui.Accordion;
using System.Collections.ObjectModel;

namespace CandidQV.ViewModels;
public partial class AirlinesPageViewModel : ObservableObject
{
    [ObservableProperty] private AirlineRepository airlineRepository;
    [ObservableProperty] private FlightNumberRepository flightNumberRepository;
    [ObservableProperty] private Airline airline;
    //[ObservableProperty] private string fullName;
    //[ObservableProperty] private string iataCode;
    [ObservableProperty] private ObservableCollection<FlightNumber> flightNumbers;
    [ObservableProperty] private ObservableCollection<Airline> airlines;
    [ObservableProperty] private ObservableCollection<AccordionItem> accordionItems;


    private int _editAirlineId;
    private Airline _lastDeletedAirline;
    private FlightNumber _lastDeletedFlightNumber;

    public int CurrentExpanded { get; internal set; }

    public AirlinesPageViewModel(AirlineRepository repository, FlightNumberRepository flightNumberRepository)
    {
        this.airlineRepository = repository;
        this.flightNumberRepository = flightNumberRepository;
        flightNumbers = new();
        airline = new();
        airlines = new();
        accordionItems = new();
    }

    public async Task InitAsync()
    {
        var airlines = await AirlineRepository.GetAllAsync();
        Airlines.Clear();
        foreach (var airline in airlines)
            Airlines.Add(airline);

        //AccordionItems.Clear();

        foreach (var airline in Airlines)
        {
            //var flightStack = new VerticalStackLayout();

            await GetFlightNumbersAsync(airline.Id);

            foreach (var flight in FlightNumbers.ToList()) // ToList to avoid collection mutation issues
            {
                airline.FlightNumbers.Add(flight);
                //    var flightRow = new HorizontalStackLayout
                //    {
                //        Spacing = 10,
                //        Padding = new Thickness(10, 5)
                //    };

                //    var flightLabel = new Label
                //    {
                //        Text = flight.Number,
                //        VerticalOptions = LayoutOptions.Center
                //    };

                //    var deleteButton = new Button
                //    {
                //        Text = "Delete",
                //        BackgroundColor = Colors.Red,
                //        TextColor = Colors.White,
                //        Padding = new Thickness(10, 2),
                //        CornerRadius = 5
                //    };

                //    deleteButton.Clicked += async (s, e) =>
                //    {
                //        FlightNumbers.Remove(flight);
                //        await InitAsync(); // Refresh UI
                //    };

                //    flightRow.Children.Add(flightLabel);
                //    flightRow.Children.Add(deleteButton);
                //    flightStack.Children.Add(flightRow);
                //}

                //var accordionItem = new AccordionItem
                //{
                //    //Header = new Label
                //    //{
                //    //    Text = airline?.Name?? string.Empty,
                //    //    FontAttributes = FontAttributes.Bold,
                //    //    FontSize = 16,
                //    //    Padding = new Thickness(10)
                //    //},
                //    Content = flight
                //};

                //AccordionItems.Add(accordionItem);
            }
        }
    }

    internal async Task GetFlightNumbersAsync(int airlineId)
    {
        var flightNumbers = await FlightNumberRepository.GetByAirlineIdAsync(airlineId);
        FlightNumbers.Clear();
        foreach (var flightNumber in flightNumbers)
            FlightNumbers.Add(flightNumber);
    }

    [RelayCommand]
    private async Task Save()
    {
        try
        {
            if (Airline.Id == 0)
            {
                //Add Airline

                var id = await AirlineRepository.CreateAsync(Airline);
                Airlines.Add(Airline);

                foreach (var flightNumber in FlightNumbers)
                {
                    if (!await FlightNumberRepository.DoesRecordExistAsync(flightNumber.Number, id))
                    {
                        await FlightNumberRepository.CreateAsync(new FlightNumber(flightNumber.Number, id));
                    }
                }
            }
            else
            {
                //Update Airline
                await AirlineRepository.UpdateAsync(Airline);
            }

            Airline = new();
            FlightNumbers.Clear();
            _editAirlineId = 0;
        }
        catch (Exception ex)
        {
            await App.AlertSvc.ShowAlertAsync("Error", ex.Message);
        }
    }

    private async Task TappedItem(Airline airline)
    {
        FlightNumbers.Clear();


        var action = await App.AlertSvc.DisplayActionSheetAsync("Options", "Cancel", null, "Edit", "Delete");

        switch (action)
        {
            case "Edit":
                _editAirlineId = airline.Id;
                Airline = airline;
                await GetFlightNumbersAsync(airline.Id);
                break;
            case "Delete":
                await AirlineRepository.Delete(airline);
                Airlines.Remove(airline);
                break;
        }
    }

    [RelayCommand]
    private async Task AddFlightNumber(Airline airline)
    {
        //if (_editAirlineId == 0)
        //{
        //    await App.AlertSvc.ShowAlertAsync("Error", "You can only add while editing an airline");
        //    return;
        //}

        string result = await App.AlertSvc.PromptAsync("Flight number", "Please add a flight number", "OK", "Cancel", "Enter Flight Number", 6, Keyboard.Numeric);

        if (string.IsNullOrWhiteSpace(result))
        {
            App.AlertSvc.ShowAlert("No flight Number", "Field was empty please type a flight number");
            return;
        }

        if(await FlightNumberRepository.DoesRecordExistAsync(result, airline.Id))
        {
            App.AlertSvc.ShowAlert("Flight Number Exist", $"Flight number {result} already exist");
            return;
        }

        if (!FlightNumbers.Any(x => x.Number == result))
        {
            await FlightNumberRepository.CreateAsync(new FlightNumber(result, airline.Id));
            airline.FlightNumbers.Add(new FlightNumber(result));
        }

    }

    public async Task OnAircraftSelected(Airline airline)
    {
        if (airline == null) return;
        Airline = airline;
        _editAirlineId = airline.Id;
        await GetFlightNumbersAsync(airline.Id);
        // Navigate, show details, or prompt user
    }

    [RelayCommand]
    public async Task DeleteAirline(Airline airline)
    {
        var result = await App.AlertSvc.ShowConfirmationAsync("Delete Airline", $"Are you sure you would like to delete {airline.Name}?");

        if (result)
        {
            // Need to make sure delete flights
            if (airline == null) return;
            _lastDeletedAirline = airline;
            // Optionally delete from DB
            await AirlineRepository.Delete(airline);
            Airlines.Remove(airline);
        }
    }

    

    [RelayCommand]
    private async Task DeleteFlightNumber(FlightNumber flightNumber)
    {

       var result = await App.AlertSvc.ShowConfirmationAsync("Confirm Delete",
                                              $"Delete {flightNumber.Number}?",
                                              "Yes", "Cancel");
        if (result) 
        {
            // Need to make sure delete flights
            if (flightNumber == null) return;
            _lastDeletedFlightNumber = flightNumber;
            Airlines[CurrentExpanded].FlightNumbers.Remove(flightNumber);
            // Optionally delete from DB
            await FlightNumberRepository.Delete(flightNumber);
        }

        var snackbarOptions = new SnackbarOptions
        {
            BackgroundColor = Colors.DarkRed,
            TextColor = Colors.White,
            ActionButtonTextColor = Colors.Yellow,
            CornerRadius = 8,
            Font = Microsoft.Maui.Font.Default
        };



        await Snackbar.Make("Flight Number deleted", async () =>
        {
            await UndoFlightNumberDeleteAsync();
        }, "Undo", TimeSpan.FromSeconds(5), snackbarOptions).Show();

    }

    public async Task UndoDeleteAsync()
    {
        if (_lastDeletedAirline == null) return;
        await AirlineRepository.CreateAsync(_lastDeletedAirline);
        Airlines.Add(_lastDeletedAirline);
        _lastDeletedAirline = null;
    }

    public async Task UndoFlightNumberDeleteAsync()
    {
        if (_lastDeletedFlightNumber == null) return;
        await FlightNumberRepository.CreateAsync(_lastDeletedFlightNumber);
        Airlines[CurrentExpanded].FlightNumbers.Add(_lastDeletedFlightNumber);
        _lastDeletedFlightNumber = null;
    }

    [RelayCommand]
    private async Task Add()
    {
        try
        {
            string result = await App.AlertSvc.PromptAsync("Airline", "Please type the airline name.", "OK", "Cancel", "Enter Name", 20);

            if (string.IsNullOrWhiteSpace(result))
            {
                App.AlertSvc.ShowAlert("No airline name", "Field was empty please type a airline name");
                return;
            }

            if (await AirlineRepository.DoesRecordExistAsync(result))
            {
                App.AlertSvc.ShowAlert("Airline Name", $"Airline {result} already exist");
                return;
            }


            string result1 = await App.AlertSvc.PromptAsync("Iata", $"Please provide the Iata for {result}.", "OK", "Cancel", "Enter Iata", 20);

            if (string.IsNullOrWhiteSpace(result1))
            {
                App.AlertSvc.ShowAlert("No iata", "Field was empty please type a iata code");
                return;
            }

            if (await AirlineRepository.DoesIataRecordExistAsync(result1))
            {
                App.AlertSvc.ShowAlert("Iata Code", $"Iata code {result1} already exist");
                return;
            }
            var airline = new Airline
            {
                IataCode = result1,
                Name = result
            };

            await AirlineRepository.CreateAsync(airline);

            Airlines.Add(airline);
        }
        catch (Exception ex)
        {
            App.AlertSvc.ShowAlert("Error", ex.Message);
        }
    }
}
