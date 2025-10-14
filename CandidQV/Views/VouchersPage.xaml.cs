using CandidQV.Models.Items;
using CandidQV.Repositories;

namespace CandidQV.Views;

public partial class VouchersPage : ContentPage
{
    private readonly VoucherRepository _repository;
    private readonly FlightNumberRepository _flightNumberRepository;
    private readonly AirlineRepository _airlineRepository;
    private readonly EmployeeRepository _employeeRepository;
    private int _editVoucherId;

    public VouchersPage(VoucherRepository repository,
        FlightNumberRepository flightNumberRepository,
        AirlineRepository airlineRepository,
        EmployeeRepository employeeRepository)
    {
        InitializeComponent();
        _repository = repository;
        _flightNumberRepository = flightNumberRepository;
        _airlineRepository = airlineRepository;
        _employeeRepository = employeeRepository;

        employeePicker.ItemsSource = employeeRepository.GetAllAsync().Result;
        //flightNumberPicker.ItemsSource = flightNumberRepository.GetFlightNumber().Result;
        airlinePicker.ItemsSource = airlineRepository.GetAllAsync().Result;
        Task.Run(async () => listView.ItemsSource =  await repository.GetVouchersAsync());
    }

    private async void saveBtn_Clicked(object sender, EventArgs e)
    {
        if(_editVoucherId == 0)
        {
            //Add Voucher

            await _repository.CreateAsync(new Voucher
            {
                PassengerName = passengerNameEntryField.Text,
                FlightNumberId = ((FlightNumber)flightNumberPicker.SelectedItem).Id,
                AirlineId = ((Airline)airlinePicker.SelectedItem).Id,
                EmployeeID = ((Employee)employeePicker.SelectedItem).Id,
                IsUsDeparture = (bool)usRadBtn.Value,
                StartTimeString = StartTimePicker.Time.ToString(),
                EndTimeString = EndTimePicker.Time.ToString(),
            });
        }
        else
        {
            //Update Voucher

            await _repository.UpdateAsync(new Voucher
            {
                Id = _editVoucherId,
                PassengerName = passengerNameEntryField.Text,
                FlightNumberId = ((FlightNumber)flightNumberPicker.SelectedItem).Id,
                AirlineId = ((Airline)airlinePicker.SelectedItem).Id,
                EmployeeID = ((Employee)employeePicker.SelectedItem).Id,
                IsUsDeparture = (bool)usRadBtn.Value,
                StartTimeString = StartTimePicker.Time.ToString(),
                EndTimeString = EndTimePicker.Time.ToString(),
            });

            _editVoucherId = 0;
        }

        passengerNameEntryField.Text = string.Empty;
        flightNumberPicker.SelectedIndex = -1;
        employeePicker.SelectedIndex = -1;
        usRadBtn.Value = true;
        StartTimePicker.Time = DateTime.Now.TimeOfDay;
        EndTimePicker.Time = DateTime.Now.TimeOfDay;
        listView.ItemsSource = await _repository.GetVouchersAsync();
    }

    private async void listView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var Voucher = (Voucher)e.Item;
        var action = await DisplayActionSheet("Options", "Cancel", null, "Edit", "Delete");

        switch (action) 
        {
            case "Edit":
                _editVoucherId = Voucher.Id;
                passengerNameEntryField.Text = Voucher.PassengerName;
                flightNumberPicker.SelectedItem = _flightNumberRepository.GetByIdAsync(Voucher.FlightNumberId);
                airlinePicker.SelectedItem = _airlineRepository.GetById(Voucher.AirlineId);
                employeePicker.SelectedItem = _airlineRepository.GetById(Voucher.EmployeeID);
                usRadBtn.Value = Voucher.IsUsDeparture;
                StartTimePicker.Time = TimeSpan.Parse(Voucher.StartTimeString);
                EndTimePicker.Time = TimeSpan.Parse(Voucher.EndTimeString);
                break;
            case "Delete":
                await _repository.DeleteAsync(Voucher);
                listView.ItemsSource = await _repository.GetVouchersAsync();
                break;
        }
    }
}