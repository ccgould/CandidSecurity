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

    public VouchersPage(VoucherRepository repository,FlightNumberRepository flightNumberRepository,AirlineRepository airlineRepository,EmployeeRepository employeeRepository)
    {
        InitializeComponent();
        _repository = repository;
        _flightNumberRepository = flightNumberRepository;
        _airlineRepository = airlineRepository;
        _employeeRepository = employeeRepository;

        employeePicker.ItemsSource = employeeRepository.GetEmployees().Result;
        flightNumberPicker.ItemsSource = flightNumberRepository.GetFlightNumber().Result;
        airlinePicker.ItemsSource = airlineRepository.GetAirlines().Result;
        Task.Run(async () => listView.ItemsSource =  await repository.GetVouchers());
    }

    private async void saveBtn_Clicked(object sender, EventArgs e)
    {
        if(_editVoucherId == 0)
        {
            //Add Voucher

            await _repository.Create(new Voucher
            {
                PassengerName = passengerNameEntryField.Text,
                FlightNumberId = ((FlightNumber)flightNumberPicker.SelectedItem).Id,
                AirlineId = ((Airline)airlinePicker.SelectedItem).Id,
                EmployeeID = ((Employee)employeePicker.SelectedItem).Id,
                IsUsDeparture = (bool)usRadBtn.Value,
                StartTime = TimeOnly.FromTimeSpan(StartTimePicker.Time),
                EndTime = TimeOnly.FromTimeSpan(EndTimePicker.Time),
            });

        }
        else
        {
            //Update Voucher

            await _repository.Update(new Voucher
            {
                Id = _editVoucherId,
                PassengerName = passengerNameEntryField.Text,
                FlightNumberId = ((FlightNumber)flightNumberPicker.SelectedItem).Id,
                AirlineId = ((Airline)airlinePicker.SelectedItem).Id,
                EmployeeID = ((Employee)employeePicker.SelectedItem).Id,
                IsUsDeparture = (bool)usRadBtn.Value,
                StartTime = TimeOnly.FromTimeSpan(StartTimePicker.Time),
                EndTime = TimeOnly.FromTimeSpan(EndTimePicker.Time),
            });

            _editVoucherId = 0;
        }

        passengerNameEntryField.Text = string.Empty;
        flightNumberPicker.SelectedIndex = -1;
        employeePicker.SelectedIndex = -1;
        usRadBtn.Value = true;
        StartTimePicker.Time = DateTime.Now.TimeOfDay;
        EndTimePicker.Time = DateTime.Now.TimeOfDay;
        listView.ItemsSource = await _repository.GetVouchers();
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
                flightNumberPicker.SelectedItem = _flightNumberRepository.GetById(Voucher.FlightNumberId);
                airlinePicker.SelectedItem = _airlineRepository.GetById(Voucher.AirlineId);
                employeePicker.SelectedItem = _airlineRepository.GetById(Voucher.EmployeeID);
                usRadBtn.Value = Voucher.IsUsDeparture;
                StartTimePicker.Time = Voucher.StartTime.ToTimeSpan();
                EndTimePicker.Time = Voucher.EndTime.ToTimeSpan();
                break;
            case "Delete":
                await _repository.Delete(Voucher);
                listView.ItemsSource = await _repository.GetVouchers();
                break;

        }

    }
}
