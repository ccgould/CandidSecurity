using CandidQVmMulti.Models;
using CandidQVmMulti.Services;
using CandidQVmMulti.View.Pages;
using CandidQVmMulti.View.Popups;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Spire.Xls;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.ComTypes;

namespace CandidQVmMulti.ViewModels;

public partial class VouchersPageViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Voucher> vouchers = new();

    [ObservableProperty]
    private string searchText;

    [ObservableProperty]
    private DateTime startDate;

    [ObservableProperty]
    private DateTime endDate;

    [ObservableProperty] private bool isBusy;
    private bool _isLoading;
    private readonly MySqlAirlinesService mySqlAirlinesService;
    private readonly MySqlVoucherService mySqlVoucherService;
    private readonly MySqlEmployeeService mySqlEmployeeService;
    private readonly MySqlFlightNumberService mySqlFlightNumberService;
    private readonly ExportServices exportServices;

    public int VouchersCount => Vouchers?.Count ?? 0;

    public VouchersPageViewModel(
        MySqlAirlinesService airlinesService,
        MySqlVoucherService mySqlVoucherService, 
        MySqlEmployeeService mySqlEmployeeService, 
        MySqlFlightNumberService mySqlFlightNumberService,
        ExportServices exportServices)
    {
        vouchers = new();

        var result = GetMonthStartAndEnd(DateTime.Now);

        startDate = result.startOfMonth;
        endDate = result.endOfMonth;
        this.mySqlAirlinesService = airlinesService;
        this.mySqlVoucherService = mySqlVoucherService;
        this.mySqlEmployeeService = mySqlEmployeeService;
        this.mySqlFlightNumberService = mySqlFlightNumberService;
        this.exportServices = exportServices;
    }

    public static (DateTime startOfMonth, DateTime endOfMonth) GetMonthStartAndEnd(DateTime date)
    {
        DateTime startOfMonth = new DateTime(date.Year, date.Month, day: 1);

        int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
        DateTime endOfMonth = new DateTime(date.Year, date.Month, daysInMonth);

        return (startOfMonth, endOfMonth);
    }

    public async Task LoadData()
    {
        if (_isLoading) return;
        _isLoading = true;

        try
        {
            IsBusy = true; // triggers spinner

            var collection = await mySqlVoucherService.GetAllVouchersAsync();
            var filtered = ApplyFilters(collection);
            Vouchers = new ObservableCollection<Voucher>(filtered);

            OnPropertyChanged(nameof(VouchersCount));

            await Toast.Make("Employees loaded successfully", duration: ToastDuration.Short).Show();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
            _isLoading = false;
        }
    }

    partial void OnVouchersChanged(ObservableCollection<Voucher> value)
    {
        OnPropertyChanged(nameof(VouchersCount));
        if (value != null)
            value.CollectionChanged += (s, e) => OnPropertyChanged(nameof(VouchersCount));
    }

    [RelayCommand]
    private async Task Edit(Voucher voucher)
    {
        var navigationParameters = new ShellNavigationQueryParameters
        {
            { "Vouchers", voucher}
        };

        await Shell.Current.GoToAsync(nameof(AddVoucherPage), navigationParameters);
    }

   [RelayCommand]
    private async Task Delete(Voucher voucher)
    {
        if (Vouchers.Contains(voucher))
        {
            Vouchers.Remove(voucher);
        }

        await mySqlVoucherService.DeleteVoucherAsync(voucher.Id);
     }

    [RelayCommand]
    private async Task Add()
    {
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            await OpenCreateVoucherPopup();
        }
        else
        {
            var navigationParameters = new ShellNavigationQueryParameters
        {
            { "Vouchers", new Voucher()}
        };

            await Shell.Current.GoToAsync(nameof(AddVoucherPage), navigationParameters);
        }
    }

    [RelayCommand]
    private async Task Export()
    {
        var collection = await mySqlVoucherService.GetAllVouchersAsync();
        var filtered = ApplyFilters(collection);
        exportServices.ExportVouchersGroupedByAirline(filtered.ToList(), "Vouchers.xlsx");
        // Save to file or share via Share.Default.RequestAsync
        await Toast.Make("Exported vouchers to CSV", ToastDuration.Short).Show();

    }

    private CancellationTokenSource _debounceCts;

    partial void OnSearchTextChanged(string value)
    {
        RefreshData();
    }

    private void RefreshData()
    {
        _debounceCts?.Cancel();
        _debounceCts = new CancellationTokenSource();

        Task.Delay(300, _debounceCts.Token)
            .ContinueWith(t =>
            {
                if (!t.IsCanceled)
                    MainThread.BeginInvokeOnMainThread(() => LoadData());
            });
    }

    private IEnumerable<Voucher> ApplyFilters(IEnumerable<Voucher> source)
    {
        return source
            .Where(v => string.IsNullOrWhiteSpace(SearchText) || v.PassengerName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
            .Where(v => v.Date >= StartDate.Ticks)
            .Where(v => v.Date <= EndDate.Ticks);
    }


    partial void OnStartDateChanged(DateTime value)
    {
        RefreshData();
    }

    partial void OnEndDateChanged(DateTime value)
    {
        RefreshData();
    }

    [RelayCommand]
    private async Task OpenCreateVoucherPopup()
    {
        // Make sure you have access to the current page
        var popup = new CreateVoucherPopup(new CreateVoucherPopupViewModel(mySqlAirlinesService,mySqlVoucherService,mySqlEmployeeService,mySqlFlightNumberService));
        await Shell.Current.CurrentPage.ShowPopupAsync(popup);
    }
}