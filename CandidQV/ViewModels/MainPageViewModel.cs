using CandidQV.Models.Items;
using CandidQV.Repositories;
using CandidQV.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;


namespace CandidQV.ViewModels;
public partial class MainPageViewModel : ObservableObject
{
    private readonly VoucherRepository repository;
    [ObservableProperty] private ObservableCollection<object> selectedVouchers;
    [ObservableProperty] private ObservableCollection<Voucher> filteredVouchers;
    [ObservableProperty] private bool isRefreshing;
    [ObservableProperty] private string searchBarText;
    [ObservableProperty] DateTime selectedDate = DateTime.Today;
    private CancellationTokenSource _debounceCts;
    private Voucher _lastDeletedVoucher;

    public MainPageViewModel(VoucherRepository repository)
    {
        this.repository = repository;
        selectedVouchers = new();
    }

    [RelayCommand]
    private async Task Add()
    {
        await Shell.Current.GoToAsync(nameof(VoucherCreationPage));
        Refresh();
    }


    [RelayCommand]
    private async Task Export()
    {
        try
        {
            if (!SelectedVouchers?.Any() ?? false)
            {
                await AppShell.DisplayToastAsync("No vouchers selected");
                return;
            }
            IsRefreshing = true;
            await repository.ExportSelectedAsync(SelectedVouchers);
            SelectedVouchers.Clear();
            IsRefreshing = false;

        }
        catch (Exception ex)
        {
            App.AlertSvc.ShowAlert("Error", ex.Message);
        }
    }

    partial void OnSearchBarTextChanged(string value) => DebounceFilter();
    partial void OnSelectedDateChanged(DateTime value) => DebounceFilter();

    private void DebounceFilter()
    {
        _debounceCts?.Cancel();
        _debounceCts = new CancellationTokenSource();
        var token = _debounceCts.Token;

        Task.Delay(300, token).ContinueWith(async t =>
        {
            if (!t.IsCanceled)
            {
                await FilterResults();
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private async Task FilterResults()
    {
        var results = await repository.GetFilteredVouchersAsync(SearchBarText, SelectedDate);  
        FilteredVouchers = new ObservableCollection<Voucher>(results);
    }

    internal async Task DeleteVoucherAsync(Voucher voucher)
    {
        // Need to make sure delete flights
        if (voucher == null) return;
        _lastDeletedVoucher = voucher;
        // Optionally delete from DB
        await repository.DeleteAsync(voucher);
        FilteredVouchers.Remove(voucher);
    }

    internal async Task OnVoucherSelected(Voucher voucher)
    {
        await Shell.Current.GoToAsync(nameof(VoucherCreationPage), new Dictionary<string, object>
        {
            { "VoucherId", voucher.Id }
        });

    }

    internal async Task UndoDeleteAsync()
    {
        if (_lastDeletedVoucher == null) return;
        await repository.CreateAsync(_lastDeletedVoucher);
        FilteredVouchers.Add(_lastDeletedVoucher);
        _lastDeletedVoucher = null;
    }

    internal void Refresh()
    {
        try
        {
            DebounceFilter();
        }
        catch (Exception ex)
        {

            App.AlertSvc.ShowAlert("Error", ex.Message);
        }
        finally
        {
            IsRefreshing = false;
        }
    }
}
