using CandidQVmMulti.Services;
using CandidQVmMulti.View.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Networking;
namespace CandidQVmMulti.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private readonly MySqlEmployeeService employeeService;
    private readonly MySqlAirlinesService airlinesService;
    private readonly MySqlVoucherService voucherService;
    private readonly System.Timers.Timer _timer;
    [ObservableProperty] private int employeeCount;
    [ObservableProperty] private int airlineCount;
    [ObservableProperty] private int vouchersCount;
    [ObservableProperty] private int todaysAssistanceCount;
    [ObservableProperty] private int unsignedVouchersCount;
    private NetworkAccess connectivity => Connectivity.Current.NetworkAccess;


    public MainPageViewModel(MySqlEmployeeService employeeService, MySqlAirlinesService airlinesService, MySqlVoucherService voucherService)
    {
        this.employeeService = employeeService;
        this.airlinesService = airlinesService;
        this.voucherService = voucherService;


        _timer = new System.Timers.Timer(30000); // refresh every 30 seconds
        _timer.Elapsed += async (s, e) => await LoadData();
        _timer.AutoReset = true;
        _timer.Enabled = true;

    }

    internal async Task LoadData()
    {
        try
        {
            if (connectivity == NetworkAccess.Internet)
            {
                EmployeeCount = await employeeService.GetCountAsync();
                AirlineCount = await airlinesService.GetCountAsync();
                VouchersCount = await voucherService.GetCountAsync();
                TodaysAssistanceCount = await voucherService.GetTodaysAssistanceCountAsync();
                UnsignedVouchersCount = await voucherService.GetUnsignedVouchersCountAsync();
            }
        }
        catch (Exception ex)
        {
            await App.AlertSvc.ShowAlertAsync("Error", ex.Message);
        }

    }

    [RelayCommand]
    private async Task Navigate(string page)
    {
        await Shell.Current.GoToAsync($"//{page}");
    }
}
