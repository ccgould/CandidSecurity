using CandidBritishAirways.Models;
using CandidBritishAirways.Services;
using CandidBritishAirways.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text;

namespace CandidBritishAirways.ViewModel;

public partial class ReportViewModel : ObservableObject
{
    private readonly DatabaseService _dbService;
    private readonly MySqlEmployeeService mySqlEmployeeService;
    private readonly MySqlGateService mySqlGateService;
    [ObservableProperty] private ObservableCollection<ReportModel> reports = new();
    [ObservableProperty] private string searchQuery;
    [ObservableProperty] private DateTime startDate = DateTime.Today.AddDays(-7);
    [ObservableProperty] private DateTime endDate = DateTime.Today;
    [ObservableProperty] private bool isBusy;


    public ReportViewModel(DatabaseService dbService, MySqlEmployeeService mySqlEmployeeService, MySqlGateService mySqlGateService)
    {
        _dbService = dbService;
        this.mySqlEmployeeService = mySqlEmployeeService;
        this.mySqlGateService = mySqlGateService;
        LoadReportsCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadReportsAsync()
    {
        IsBusy = true;
        var allReports = await _dbService.GetReportsWithNamesAsync();
        Reports = new ObservableCollection<ReportModel>(allReports);
        IsBusy = false;
    }

    [RelayCommand]
    private async Task SearchReportsAsync()
    {
        if (int.TryParse(SearchQuery, out int flightNumber))
        {
            var results = await _dbService.SearchByFlightNumberAsync(flightNumber);
            Reports = new ObservableCollection<ReportModel>(results);
        }
    }

    [RelayCommand]
    private async Task FilterByDateAsync()
    {
        var results = await _dbService.FilterByDateRangeAsync(StartDate, EndDate);
        Reports = new ObservableCollection<ReportModel>(results);
    }

    [RelayCommand]
    private async Task CombinedSearchAsync()
    {
        if (int.TryParse(SearchQuery, out int flightNumber))
        {
            var results = await _dbService.SearchFlightAndDateAsync(flightNumber, StartDate, EndDate);
            Reports = new ObservableCollection<ReportModel>(results);
        }
    }

    [RelayCommand]
    private async Task SendReport(ReportModel report)
    {
        var sb = new StringBuilder();
        sb.AppendLine("British Airways flight report.");
        sb.AppendLine($"Flight Number: BA{report.FlightNumber}");
        sb.AppendLine($"Gate: {await GetGateName(report.ParkingGate)}");
        sb.AppendLine($"STA: {report.ScheduledTimeArrival?.ToString(@"hhmm")}");
        sb.AppendLine($"Touch Down: {report.ActualTimeArrival?.ToString(@"hhmm")}");
        sb.AppendLine($"Arrived At Gate: {report.ArrivalAtGate?.ToString(@"hhmm")}");

        sb.AppendLine("Officers:");

        if (report.FrontDoorAccessPosition != 0)
            sb.AppendLine($"{await GetEmployeeName(report.FrontDoorAccessPosition)}");
        if (report.RampAccessPosition != 0)
            sb.AppendLine($"{await GetEmployeeName(report.RampAccessPosition)}");
        if (report.BaggageMakeupPosition != 0)
            sb.AppendLine($"{await GetEmployeeName(report.BaggageMakeupPosition)}");
        if (report.CateringPosition != 0)
            sb.AppendLine($"{await GetEmployeeName(report.CateringPosition)}");

        sb.AppendLine($"Wheelchair/s Out: {report.OutboundWheelchairs}");
        sb.AppendLine($"Wheelchair/s In: {report.InboundWheelchairs}");
        sb.AppendLine($"Lift Chairs: {report.LiftChairs}");
        sb.AppendLine($"Cleaners: {report.Cleaners}");

        sb.AppendLine($"STD: {report.ScheduledTimeDeparture?.ToString(@"hhmm")}");
        sb.AppendLine($"Departed From Gate: {report.ActualTimeDeparture?.ToString(@"hhmm")}");
        sb.AppendLine($"Airborne: {report.Airborne?.ToString(@"hhmm")}");
        sb.AppendLine($"Tail Number: {report.AircraftRegistration}");

        // Catering Info
        if (report.IsCatering)
        {
            sb.AppendLine();
            sb.AppendLine("🍽 Catering Details:");
            sb.AppendLine($"Pod Number: {report.PodNumber}");
            sb.AppendLine($"Offloaded: {report.PodOffload?.ToString(@"hhmm")}");
            sb.AppendLine($"Onloaded: {report.PodOnload?.ToString(@"hhmm")}");
            sb.AppendLine($"Left Front Seal: {report.LeftFrontSeal}");
            sb.AppendLine($"Right Front Seal: {report.RightFrontSeal}");
            sb.AppendLine($"Battery Seal: {report.BatterySeal}");
            sb.AppendLine($"Dry Ice: {report.DryIce}");
        }

        // Cancellation Info
        if (report.FlightCanceled)
        {
            sb.AppendLine();
            sb.AppendLine("❌ Flight Cancellation Details:");
            sb.AppendLine($"Cancelled Parking Gate: {await GetEmployeeName(report.CancelledParkingGate)}");
            sb.AppendLine($"Cancellation Time: {report.CancelledTime?.ToString(@"hhmm")}");

            // Seal Details
            sb.AppendLine();
            sb.AppendLine("🚪 Aircraft Seal Details:");

            sb.AppendLine("Passenger Doors:");
            sb.AppendLine($"LFwd: {report.LFwd}");
            sb.AppendLine($"LFwd Overwing: {report.LFwdOverwingDoor}");
            sb.AppendLine($"LAft Overwing: {report.LAftOverwingDoor}");
            sb.AppendLine($"LAft: {report.LAftDoor}");
            sb.AppendLine($"RFwd: {report.RFwdDoor}");
            sb.AppendLine($"RFwd Overwing: {report.RFwdOverwingDoor}");
            sb.AppendLine($"RAft Overwing: {report.RAftOverwingDoor}");
            sb.AppendLine($"RAft: {report.RAftDoor}");

            sb.AppendLine("Baggage Doors:");
            sb.AppendLine($"Front Hold: {report.FrontBaggageHoldDoor}");
            sb.AppendLine($"Back Hold: {report.BackBaggageHoldDoor}");
            sb.AppendLine($"Bulk Hold C5: {report.BulkBaggageHoldDoorC5}");

            sb.AppendLine("Packs:");
            sb.AppendLine($"Electric Equipment Access: {report.ElectricEquipmentAccess}");
            sb.AppendLine($"Ground Service Communication: {report.GroundServiceCommunication}");
            sb.AppendLine($"Ground Communication Air: {report.GroundCommunicationAir}");
            sb.AppendLine($"Air Exhaust Left: {report.AirExhaustL}");
            sb.AppendLine($"Air Exhaust Right: {report.AirExhaustR}");
        }

        sb.AppendLine();
        sb.AppendLine($"Additional Comments: {report.Comments}");


    // Ensure the operation is on the main UI thread
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Clipboard.Default.SetTextAsync(sb.ToString());
        });
    }

    public async Task<string> GetEmployeeName(int id)
    {
        var employee = await mySqlEmployeeService.GetEmployeeAsync(id);
        return employee?.Name ?? "Unknown Employee";
    }

    public async Task<string> GetGateName(int id)
    {
        var gate = await mySqlGateService.GetGetAsync(id);
        return gate?.Name ?? "Unknown Gate";
    }

    [RelayCommand]
    public async Task EditReport(ReportModel report)
    {
        var navigationParameter = new ShellNavigationQueryParameters
        {
            { "report", report }
        };
        await Shell.Current.GoToAsync(nameof(MainPage), navigationParameter);
    }
}
