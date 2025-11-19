using CandidBritishAirways.Models;
using CandidBritishAirways.Services;
using CandidBritishAirways.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CandidBritishAirways.ViewModel;

public partial class MainPageViewModel : ObservableObject, IQueryAttributable
{
    private readonly DatabaseService databaseService;
    private readonly MySqlEmployeeService mySqlEmployeeService;
    private readonly MySqlGateService mySqlGateService;


    [ObservableProperty] private ReportModel report = new();
    [ObservableProperty] private ObservableCollection<Employee> employees = new();
    [ObservableProperty] private ObservableCollection<Gate> gates = new();

    [ObservableProperty] private Employee frontDoorAccessPosition;
    [ObservableProperty] private Employee rampAccessPosition;
    [ObservableProperty] private Employee baggageMakeupPosition;
    [ObservableProperty] private Employee cateringPosition;
    [ObservableProperty] private Employee backDoorAccessPosition;
    [ObservableProperty] private Employee reportBy;
    [ObservableProperty] private Gate parkingGate;
    [ObservableProperty] private Gate cancellationParkingGate;
    [ObservableProperty] private int destination;
    [ObservableProperty] private bool isEditing;



    public MainPageViewModel(DatabaseService databaseService, MySqlEmployeeService mySqlEmployeeService, MySqlGateService mySqlGateService)
    {
        this.databaseService = databaseService;
        this.mySqlEmployeeService = mySqlEmployeeService;
        this.mySqlGateService = mySqlGateService;
        report.Date = DateTime.Now;
    }

    public async Task LoadData()
    {
        // 🧠 Load employees from MySQL
        var employeeList = await mySqlEmployeeService.GetAllEmployeesAsync();
        Employees = new ObservableCollection<Employee>(employeeList);

        var gatesList = await mySqlGateService.GetAllGatesAsync();
        Gates = new ObservableCollection<Gate>(gatesList);
    }

    [RelayCommand]
    private async Task SubmitReportAsync()
    {
        // 🧠 Save report to SQLite or send to API

        await databaseService.SaveReportAsync(Report);
                
        // 🎉 Feedback
        await Shell.Current.DisplayAlertAsync("Success", "Report submitted!", "OK");

        if(IsEditing)
        {
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            Clear();
        }

            IsEditing = false;
    }

    [RelayCommand]
    private void Clear()
    {
        Report = new();
        Report.Date = DateTime.Now;
        FrontDoorAccessPosition = null;
        RampAccessPosition = null;
        BaggageMakeupPosition = null;
        CateringPosition = null;
        BackDoorAccessPosition = null;
        ReportBy = null;
        ParkingGate = null;
        CancellationParkingGate = null;
        Destination = -1;
    }

    [RelayCommand]
    private async Task DeleteReport()
    {
        await databaseService.DeleteReportAsync(Report);
        await Shell.Current.GoToAsync($"///{nameof(ReportPage)}");
    }

    partial void OnFrontDoorAccessPositionChanged(Employee value)
    {
        Report.FrontDoorAccessPosition = value.Id;
    }

    partial void OnRampAccessPositionChanged(Employee value)
    {
        Report.RampAccessPosition = value.Id;
    }

    partial void OnBaggageMakeupPositionChanged(Employee value)
    {
        Report.BaggageMakeupPosition = value.Id;
    }

    partial void OnCateringPositionChanged(Employee value)
    {
        Report.CateringPosition = value.Id;
    }

    partial void OnBackDoorAccessPositionChanged(Employee value)
    {
        Report.BackDoorAccessPosition = value.Id;
    }

    partial void OnReportByChanged(Employee value)
    {
        Report.ReportBy = value.Id;
    }

    partial void OnParkingGateChanged(Gate value)
    {
        Report.ParkingGate = value.Id;
    }

    partial void OnCancellationParkingGateChanged(Gate value)
    {
        Report.CancelledParkingGate = value.Id;
    }


    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        await LoadData();

        if (query.ContainsKey("report"))
        {
            IsEditing = true;
            Report = query["report"] as ReportModel;
            FrontDoorAccessPosition = Employees.FirstOrDefault(x => x.Id == report.FrontDoorAccessPosition);
            RampAccessPosition = Employees.FirstOrDefault(x => x.Id == report.RampAccessPosition);
            BaggageMakeupPosition = Employees.FirstOrDefault(x => x.Id == report.BaggageMakeupPosition);
            CateringPosition = Employees.FirstOrDefault(x => x.Id == report.CateringPosition);
            BackDoorAccessPosition = Employees.FirstOrDefault(x => x.Id == report.BackDoorAccessPosition);
            ReportBy = Employees.FirstOrDefault(x => x.Id == report.ReportBy);
            ParkingGate = Gates.FirstOrDefault(x => x.Id == report.ParkingGate);
            CancellationParkingGate = Gates.FirstOrDefault(x => x.Id == report.CancelledParkingGate);
        }
    }
}
