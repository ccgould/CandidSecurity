using CandidQVmMulti.Services;
using CandidQVmMulti.View.Pages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CandidQVmMulti.ViewModels;

public partial class MainPageViewModel : ObservableObject
{
    private readonly MySqlEmployeeService employeeService;
    private readonly MySqlAirlinesService airlinesService;
    private readonly MySqlVoucherService voucherService;
    [ObservableProperty] private int employeeCount;
    [ObservableProperty] private int airlineCount;
    [ObservableProperty] private int vouchersCount;
    [ObservableProperty] private int todaysAssistanceCount;

    public MainPageViewModel(MySqlEmployeeService employeeService, MySqlAirlinesService airlinesService, MySqlVoucherService voucherService)
    {
        this.employeeService = employeeService;
        this.airlinesService = airlinesService;
        this.voucherService = voucherService;
    }

    internal async Task LoadData()
    {
        try
        {
            EmployeeCount = await employeeService.GetCountAsync();
            AirlineCount = await airlinesService.GetCountAsync();
            VouchersCount = await voucherService.GetCountAsync();
            TodaysAssistanceCount = await voucherService.GetTodayVoucherCountAsync();
        }
        catch (Exception ex)
        {
            await App.AlertSvc.ShowAlertAsync("Error", ex.Message);
        }

    }

    [RelayCommand]
    private async Task Navigate(string page)
    {
        await Shell.Current.GoToAsync($"{page}");
    }
}
