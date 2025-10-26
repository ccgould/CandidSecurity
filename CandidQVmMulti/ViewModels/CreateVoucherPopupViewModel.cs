using CandidQVmMulti.Models;
using CandidQVmMulti.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;

namespace CandidQVmMulti.ViewModels;

public partial class CreateVoucherPopupViewModel : ObservableObject
{
    [ObservableProperty]
    private DateTime date = DateTime.Today;

    [ObservableProperty]
    private TimeSpan startTime;

    [ObservableProperty]
    private TimeSpan endTime;

    [ObservableProperty]
    private string passengerName;

    [ObservableProperty]
    private ObservableCollection<Employee> guards;

    [ObservableProperty]
    private Employee selectedGuard;

    [ObservableProperty]
    private ObservableCollection<Airline> airlines;

    [ObservableProperty]
    private Airline selectedAirline;

    [ObservableProperty]
    private ObservableCollection<FlightNumber> flightNumbers;

    [ObservableProperty]
    private FlightNumber selectedFlightNumber;

    private bool _saveVoucher;
    private readonly MySqlAirlinesService airlinesService;
    private readonly MySqlVoucherService mySqlVoucherService;
    private readonly MySqlEmployeeService mySqlEmployeeService;
    private readonly MySqlFlightNumberService mySqlFlightNumberService;
    [ObservableProperty] private Voucher voucher;
    [ObservableProperty] private bool stillInProgress;

    public CreateVoucherPopupViewModel(
        MySqlAirlinesService airlinesService, 
        MySqlVoucherService mySqlVoucherService,
        MySqlEmployeeService mySqlEmployeeService, 
        MySqlFlightNumberService mySqlFlightNumberService,
        Voucher voucher = null)
    {
        this.airlinesService = airlinesService;
        this.mySqlVoucherService = mySqlVoucherService;
        this.mySqlEmployeeService = mySqlEmployeeService;
        this.mySqlFlightNumberService = mySqlFlightNumberService;
        if (voucher is null)
        {
            this.voucher = new();
            _saveVoucher = true;
        }
        else
        {
            this.voucher = voucher;
        }
        startTime = DateTime.Now.TimeOfDay;
        endTime = DateTime.Now.TimeOfDay;
    }


    internal async Task LoadFlightNumbers()
    {
        try
        {
            if (SelectedAirline is null) return;
            FlightNumbers = new ObservableCollection<FlightNumber>(await mySqlFlightNumberService.GetFlightNumbersByAirlineAndTerminalAsync(SelectedAirline.Id, 2));
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    internal async Task LoadData()
    {
        Airlines = new ObservableCollection<Airline>(await airlinesService.GetAllAsync());
        Guards = new ObservableCollection<Employee>(await mySqlEmployeeService.GetAllEmployeesAsync());
    }

       internal async Task CreateVoucher()
        {
        if (string.IsNullOrWhiteSpace(PassengerName) ||
            SelectedAirline == null ||
            SelectedGuard == null ||
            SelectedFlightNumber == null)
        {
            // Show some error
            return;
        }

        if (_saveVoucher)
        {
            await mySqlVoucherService.AddVoucherAsync(new Voucher()
            {
                PassengerName = PassengerName,
                Date = DateTime.Now.Ticks,
                AirlineID = SelectedAirline.Id,
                FlightID = SelectedFlightNumber.Id,
                EmployeeID = SelectedGuard.Id,
                StartTime = StartTime.Ticks,
                EndTime = EndTime.Ticks,
                Status = StillInProgress ? 0 : 1
            });
        }
        else
        {
            var voucher = new Voucher();
            voucher.PassengerName = PassengerName;
            voucher.AirlineID = SelectedAirline.Id;
            voucher.FlightID = SelectedFlightNumber.Id;
            voucher.EmployeeID = SelectedGuard.Id;
            voucher.StartTime = StartTime.Ticks;
            voucher.EndTime = EndTime.Ticks;
            voucher.Status = StillInProgress ? 0 : 1;
            await mySqlVoucherService.UpdateVoucherAsync(voucher);
            ///
            /// 
            ////Voucher.PassengerName = PassengerName;
            ////Voucher.AirlineID = SelectedAirline.Id;
            ////Voucher.FlightID = SelectedFlightNumber.Id;
            ////Voucher.EmployeeID = SelectedEmployee.Id;
            ////Voucher.StartTime = StartTime.Ticks;
            ////Voucher.EndTime = EndTime.Ticks;
            ////Voucher.Status = StillInProgress ? 0 : 1;
            ////await mySqlVoucherService.UpdateVoucherAsync(Voucher);
        }
    }

    [RelayCommand]
    private void Close()
    {
        // Add logic to close popup
    }
}