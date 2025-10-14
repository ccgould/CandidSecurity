using CandidQVmMulti.Models;
using CandidQVmMulti.Services;
using CandidQVmMulti.View.Pages;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    private readonly MySqlVoucherService service;

    public int VouchersCount => Vouchers?.Count ?? 0;

    public VouchersPageViewModel(MySqlVoucherService service)
    {
        this.service = service;
        vouchers = new();

        var result = GetMonthStartAndEnd(DateTime.Now);

        startDate = result.startOfMonth;
        endDate = result.endOfMonth;

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

            var collection = await service.GetAllVouchersAsync();
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

        await service.DeleteVoucherAsync(voucher.Id);
     }

    [RelayCommand]
    private async Task Add()
    {
        var navigationParameters = new ShellNavigationQueryParameters
        {
            { "Vouchers", new Voucher()}
        };

        await Shell.Current.GoToAsync(nameof(AddVoucherPage),navigationParameters);
    }

    [RelayCommand]
    private async Task Export()
    {
        var csv = string.Join(Environment.NewLine, Vouchers.Select(v => $"{v.Id},{v.PassengerName},{v.Date:yyyy-MM-dd}"));
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

}