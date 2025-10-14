using CandidQVmMulti.Models;
using CandidQVmMulti.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CandidQVmMulti.ViewModels;
public partial class AddVoucherPageViewModel : ObservableObject,IQueryAttributable
{
    private readonly MySqlAirlinesService airlinesService;
    private readonly MySqlVoucherService mySqlVoucherService;
    private readonly MySqlEmployeeService mySqlEmployeeService;
    private readonly MySqlFlightNumberService mySqlFlightNumberService;

    [ObservableProperty] private ObservableCollection<Airline> airlines;
    [ObservableProperty] private ObservableCollection<Employee> employees;
    [ObservableProperty] private ObservableCollection<FlightNumber> flightNumbers;

    [ObservableProperty] private Airline selectedAirline;
    [ObservableProperty] private Employee selectedEmployee;
    [ObservableProperty] private FlightNumber selectedFlightNumber;

    [ObservableProperty] private string passengerName;
    [ObservableProperty] private int employeesCount;
    [ObservableProperty] private int airlinesCount;
    [ObservableProperty] private int flightNumbersCount;
    [ObservableProperty] private int selectedTerminal;

    [ObservableProperty] private TimeSpan startTime;
    [ObservableProperty] private TimeSpan endTime;
    [ObservableProperty] private bool stillInProgress;
    private bool _dirty;

    public AddVoucherPageViewModel(MySqlAirlinesService airlinesService, MySqlVoucherService mySqlVoucherService, MySqlEmployeeService mySqlEmployeeService, MySqlFlightNumberService mySqlFlightNumberService)
    {
        this.airlinesService = airlinesService;
        this.mySqlVoucherService = mySqlVoucherService;
        this.mySqlEmployeeService = mySqlEmployeeService;
        this.mySqlFlightNumberService = mySqlFlightNumberService;
        startTime = DateTime.Now.TimeOfDay;
        endTime = DateTime.Now.TimeOfDay;
    }

    internal async Task LoadData()
    {
        Airlines = new ObservableCollection<Airline>(await airlinesService.GetAllAsync());
        Employees = new ObservableCollection<Employee>(await mySqlEmployeeService.GetAllEmployeesAsync());

        EmployeesCount = Employees.Count();
        AirlinesCount = Airlines.Count();
    }

    internal async Task LoadFlightNumbers()
    {
        try
        {
            if (SelectedAirline is null) return;
            FlightNumbers = new ObservableCollection<FlightNumber>(await mySqlFlightNumberService.GetFlightNumbersByAirlineAndTerminalAsync(SelectedAirline.Id, 2));
            FlightNumbersCount = FlightNumbers.Count();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task CreateVoucher()
    {
        if(string.IsNullOrWhiteSpace(PassengerName) ||
            SelectedAirline == null ||
            SelectedEmployee == null ||
            SelectedFlightNumber == null)
        {
            // Show some error
            return;
        }

        if(_saveVoucher)
        {
            await mySqlVoucherService.AddVoucherAsync(new Voucher()
            {
                PassengerName = PassengerName,
                Date = DateTime.Now.Ticks,
                AirlineID = SelectedAirline.Id,
                FlightID = SelectedFlightNumber.Id,
                EmployeeID = SelectedEmployee.Id,
                StartTime = StartTime.Ticks,
                EndTime = EndTime.Ticks,
                Status = StillInProgress ? 0 : 1
            });
        }
        else
        {
            Voucher.PassengerName = PassengerName;
            Voucher.AirlineID = SelectedAirline.Id;
            Voucher.FlightID = SelectedFlightNumber.Id;
            Voucher.EmployeeID = SelectedEmployee.Id;
            Voucher.StartTime = StartTime.Ticks;
            Voucher.EndTime = EndTime.Ticks;
            Voucher.Status = StillInProgress ? 0 : 1;
            await mySqlVoucherService.UpdateVoucherAsync(Voucher);
        }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        if(_dirty)
        {
            bool confirmCancel = await Application.Current.MainPage.DisplayAlert(
                 "Discard Changes?",
                 "You've made changes to this voucher. Are you sure you want to cancel and lose your edits?",
                 "Yes, Cancel",
                 "Keep Editing");

            if (confirmCancel)
            {
                await Back();
            }
        }
    }

    [RelayCommand]
    private async Task Back()
    {
        await Shell.Current.GoToAsync("..");
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if(e.PropertyName == nameof(PassengerName) ||
           e.PropertyName == nameof(SelectedEmployee) ||
           e.PropertyName == nameof(SelectedAirline) ||
           e.PropertyName == nameof(SelectedFlightNumber))
        {
            _dirty = true;
        }
    }

    [ObservableProperty] private Voucher voucher;
    private bool _saveVoucher;

    internal bool Initailized { get; set; }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Initailized = false;

        if (query.TryGetValue("Vouchers", out object data))
        {
            await LoadData();

            if(data is not null)
            {

                Voucher = data as Voucher;
                PassengerName = Voucher.PassengerName;
                SelectedAirline = Airlines?.FirstOrDefault(a => a.Id == Voucher.AirlineID);
                SelectedEmployee = Employees?.FirstOrDefault(e => e.Id == Voucher.EmployeeID);
                FlightNumbers = SelectedAirline?.FlightNumbers;
                SelectedFlightNumber = FlightNumbers?.FirstOrDefault(f => f.Id == Voucher.FlightID);
                StartTime = new TimeSpan(Voucher.StartTime);
                EndTime = new TimeSpan(Voucher.EndTime);

                if(Voucher.Id == 0)
                {
                    _saveVoucher = true;
                }
            }
        }

        Initailized = true;
    }
}
