using CandidQV.Models.Items;
using CandidQV.Repositories;
using System.Collections.ObjectModel;

namespace CandidQV.Views;

public partial class AirlinesPage : ContentPage
{
    private readonly AirlineRepository _repository;
    private readonly FlightNumberRepository _flightNumberRepository;
    public ObservableCollection<FlightNumber> FlightNumbers = new();
    private int _editAirlineId;
    public bool IsRefreshing { get; set; }

    public AirlinesPage(AirlineRepository repository,FlightNumberRepository flightNumberRepository)
    {
        InitializeComponent();
        _repository = repository;
        _flightNumberRepository = flightNumberRepository;
        Task.Run(async () => listView.ItemsSource =  await repository.GetAirlines());
    }

    private async void saveBtn_Clicked(object sender, EventArgs e)
    {
        if(_editAirlineId == 0)
        {
            //Add Airline

            var id = await _repository.Create(new Airline
            {
                Name = firstNameEntryField.Text,
                IataCode = iataEntryField.Text,
            });

            foreach (var flightNumber in FlightNumbers)
            {
                if(!await _flightNumberRepository.DoesRecordExistAsync(flightNumber.Number,id))
                {
                    await _flightNumberRepository.Create(new FlightNumber(flightNumber.Number,id));
                    //flightNumberList.ItemsSource = await _flightNumberRepository.GetByAirlineId(id);
                }
            }
        }
        else
        {
            //Update Airline

            await _repository.Update(new Airline
            {
                Id = _editAirlineId,
                Name = firstNameEntryField.Text,
                IataCode = iataEntryField.Text,
            });


            foreach (var flightNumber in FlightNumbers)
            {
                if (!await _flightNumberRepository.DoesRecordExistAsync(flightNumber.Number, _editAirlineId))
                {
                    await _flightNumberRepository.Create(new FlightNumber(flightNumber.Number, _editAirlineId));
                    //flightNumberList.ItemsSource = await _flightNumberRepository.GetByAirlineId(id);
                }
            }
            _editAirlineId = 0;
        }

        firstNameEntryField.Text = string.Empty;
        iataEntryField.Text = string.Empty;
        FlightNumbers.Clear();
        listView.ItemsSource =  await _repository.GetAirlines();
    }

    private async void listView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        IsRefreshing = true;
        FlightNumbers.Clear();

        var Airline = (Airline)e.Item;

        var action = await DisplayActionSheet("Options", "Cancel", null, "Edit", "Delete");

        switch (action) 
        {
            case "Edit":
                _editAirlineId = Airline.Id;
                firstNameEntryField.Text = Airline.Name;
                iataEntryField.Text = Airline.IataCode;
                var items = await _flightNumberRepository.GetByAirlineId(_editAirlineId);
                FlightNumbers = new ObservableCollection<FlightNumber>(items);
                flightNumberList.ItemsSource = FlightNumbers;
                

                break;
            case "Delete":
                await _repository.Delete(Airline);
                listView.ItemsSource = await _repository.GetAirlines();
                break;
        }
        IsRefreshing = false;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        if(_editAirlineId == 0)
        {
            await DisplayAlert("Error", "You can only add while editing an airline","ok");
            return;
        }

        string result = await DisplayPromptAsync("Flight number", "Please add a flight number","OK","Cancel","Enter Flight Number",6,Keyboard.Numeric);
        
        if(!FlightNumbers.Any(x => x.Number == result))
        {
            FlightNumbers.Add(new FlightNumber(result));
            flightNumberList.ItemsSource = FlightNumbers;
        }
    }

    private async void refreshView_Refreshing(object sender, EventArgs e)
    {
        listView.ItemsSource = await _repository.GetAirlines();
    }
}
