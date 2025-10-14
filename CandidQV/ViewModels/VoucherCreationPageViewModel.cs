using CandidQV.Extensions;
using CandidQV.Models.Items;
using CandidQV.Repositories;
using CandidQV.Views;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IATA.BarCodedBoardingPass;
using System.Collections.ObjectModel;
using System.Globalization;

namespace CandidQV.ViewModels;

[QueryProperty(nameof(VoucherId), "VoucherId")]

public partial class VoucherCreationPageViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<Airline> airlines;
    [ObservableProperty] private ObservableCollection<Employee> employees;
    [ObservableProperty] private ObservableCollection<FlightNumber> flightNumbers;
    [ObservableProperty] private TimeSpan startTime;
    [ObservableProperty] private TimeSpan endTime;
    [ObservableProperty] private FlightNumber selectedFlightNumber;



    private readonly VoucherRepository voucherRepository;
    private readonly EmployeeRepository employeeRepository;
    private readonly AirlineRepository airlineRepository;
    private readonly FlightNumberRepository flightNumberRepository;

    [ObservableProperty]
    private bool isLoadingFlights;


    [ObservableProperty]
    private TerminalType selectedTerminal = TerminalType.US;

    public IEnumerable<TerminalType> TerminalOptions => Enum.GetValues(typeof(TerminalType)).Cast<TerminalType>();

    [ObservableProperty]
    private Airline selectedAirline;

    private Employee selectedEmployee;
    public Employee SelectedEmployee
    {
        get => selectedEmployee;
        set
        {
            if (SetProperty(ref selectedEmployee, value))
            {
                CurrentVoucher.Employee = value;
                CurrentVoucher.EmployeeID = value?.Id ?? 0;
                ShowToast($"Selected officer: {value?.FullName}");
            }
        }
    }


    private int voucherId;
    public int VoucherId
    {
        get => voucherId;
        set
        {
            voucherId = value;
            _ = Init(); // Fire-and-forget with safety
        }
    }


    [ObservableProperty]
    private Voucher currentVoucher;
    private bool _isInitializing;

    public VoucherCreationPageViewModel(VoucherRepository voucherRepository, 
        EmployeeRepository employeeRepository, 
        AirlineRepository airlineRepository, 
        FlightNumberRepository flightNumberRepository)
    {
        this.voucherRepository = voucherRepository;
        this.employeeRepository = employeeRepository;
        this.airlineRepository = airlineRepository;
        this.flightNumberRepository = flightNumberRepository;

        FlightNumbers = new();
    }

    public async Task Init()
    {
        try
        {
            Airlines = new(await airlineRepository.GetAllAsync());
            Employees = new(await employeeRepository.GetAllAsync());
            var voucher = await voucherRepository.GetByIdAsync(VoucherId);
            await Load(voucher);
        }
        catch (Exception ex)
        {
            App.AlertSvc.ShowAlert("Error", ex.Message);
        }
    }

    [RelayCommand]
    private async Task Save()
    {

        CurrentVoucher.StartTime = TimeOnly.FromTimeSpan(StartTime).ToShortTimeString();
        CurrentVoucher.EndTime = TimeOnly.FromTimeSpan(EndTime).ToShortTimeString();
        CurrentVoucher.IsUsDeparture = SelectedTerminal == TerminalType.US;
        CurrentVoucher.AirlineId = SelectedAirline.Id;

        if (CurrentVoucher.Id == 0)
        {
            CurrentVoucher.DateString = DateOnly.FromDateTime(DateTime.Now).ToString("ddd, dd MMM yyyy");
            await voucherRepository.CreateAsync(CurrentVoucher);
        }
        else
        {
            //Update Airline
            await voucherRepository.UpdateAsync(CurrentVoucher);
        }
        await Shell.Current.GoToAsync("..");

    }

    [RelayCommand]
    private void TakePhoto()
    {
        //imageView.Source = string.Empty;
    }

    //public async void ApplyQueryAttributes(IDictionary<string, object> query)
    //{
    //    await Init();
    //    await LoadTaskAsync(query);
    //}

    public async Task Load(Voucher voucher)
    {

        _isInitializing = true;

        if(voucher is null)
        {
            voucher = new();
        }

        CurrentVoucher = voucher;
        
        SelectedTerminal = voucher.IsUsDeparture ? TerminalType.US : TerminalType.Domestic;
        
        SelectedEmployee = Employees.FirstOrDefault(a => a.Id == CurrentVoucher.EmployeeID);
                
        SelectedAirline = Airlines.FirstOrDefault(a => a.Id == CurrentVoucher.AirlineId);


        if (DateTime.TryParseExact(CurrentVoucher.StartTime, "h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
        {
            StartTime = dt.TimeOfDay;
        }

        if (DateTime.TryParseExact(CurrentVoucher.EndTime, "h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt1))
        {
            EndTime = dt1.TimeOfDay;
        }
    }

    private void ShowToast(string message)
    {
        ToastDuration duration = ToastDuration.Short;

        CommunityToolkit.Maui.Alerts.Toast.Make(message, duration).Show();
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await AppShell.DisplayToastAsync("Changes Not Saved");
        await Shell.Current.GoToAsync("..");
    }

    partial void OnSelectedFlightNumberChanged(FlightNumber value)
    {
        CurrentVoucher.FlightNumber = value;
        CurrentVoucher.FlightNumberId = value.Id;
    }

    internal async Task SelectedAirlineChanged(Airline selectedItem)
    {
        FlightNumbers = new(await flightNumberRepository.GetByAirlineIdAsync(selectedItem.Id));
        SelectedAirline = selectedItem;
        if(_isInitializing)
        {
            SelectedFlightNumber = FlightNumbers.FirstOrDefault(x => x.Id == CurrentVoucher.FlightNumberId);
            _isInitializing = false;
        }
    }


    [RelayCommand]
    private async Task Scan()
    {
        IataData iata = null;
        NavigationResultBroker.BarcodeResultSource = new TaskCompletionSource<string>();

        await Shell.Current.GoToAsync(nameof(BarcodeScanner));

        // Wait for result from ScannerPage
        string scannedValue = await NavigationResultBroker.BarcodeResultSource.Task;

        if (!string.IsNullOrWhiteSpace(scannedValue))
        {
            iata = IataParser.DecodeObject(scannedValue.Truncate(158));
        }

        // Use the result
        await App.AlertSvc.ShowAlertAsync("Scanned", scannedValue, "OK");

        if(iata is not null)
        {
            CurrentVoucher.PassengerName = FormatNameWithTitle(iata?.PassengerName ?? string.Empty);
            SelectedAirline = Airlines.FirstOrDefault(x => x.IataCode.Equals(iata.OperatingCarrierDesignator,StringComparison.OrdinalIgnoreCase));
            FlightNumbers = new(await flightNumberRepository.GetByAirlineIdAsync(SelectedAirline?.Id??-1));

            SelectedFlightNumber = await GetFlightNumber(iata.FlightNumber,iata.OperatingCarrierDesignator);

        }
    }

    private async Task<FlightNumber> GetFlightNumber(string flightNumber,string airline)
    {
        var result = FlightNumbers.FirstOrDefault(x => x.Number == flightNumber.TrimStart('0'));

        if (result is null) 
        {
            App.AlertSvc.ShowConfirmation("Flight Number Not Found", $"Flight number {airline}{flightNumber.TrimStart('0')} was not found in airline {airline} databank. Would you like to add?", async (value)  => 
            {
                if(value)
                {
                    var id = Airlines.FirstOrDefault(x => x.IataCode.Equals(airline, StringComparison.OrdinalIgnoreCase))?.Id ?? -1;
                    await flightNumberRepository.CreateAsync(new FlightNumber(flightNumber.TrimStart('0'), id));
                    FlightNumbers = new(await flightNumberRepository.GetByAirlineIdAsync(id));
                    SelectedFlightNumber = await GetFlightNumber(flightNumber, airline);
                }
            });

        }

        return result;
    }

    public static string FormatNameWithTitle(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        var knownTitles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "Mr", "Mrs", "Ms", "Miss", "Dr", "Prof", "Sir", "Madam", "Mx", "Rev", "Capt", "Major", "Lt", "Col"
    };

        var parts = input.Split('/');
        if (parts.Length < 2) return input;

        string lastName = parts[0].Trim();
        string title = string.Empty;
        string firstName;

        // Check if last segment is a known title
        if (parts.Length > 2 && knownTitles.Contains(parts[^1].Trim()))
        {
            title = parts[^1].Trim();
            firstName = string.Join(" ", parts.Skip(1).Take(parts.Length - 2).Select(p => p.Trim()));
        }
        else
        {
            firstName = string.Join(" ", parts.Skip(1).Select(p => p.Trim()));
        }

        return string.IsNullOrEmpty(title)
            ? $"{firstName} {lastName}"
            : $"{title} {firstName} {lastName}";
    }

}

public static class NavigationResultBroker
{
    public static TaskCompletionSource<string> BarcodeResultSource { get; set; }
}

