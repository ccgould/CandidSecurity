using CandidQVmMulti.Enumerators;
using CandidQVmMulti.Models;
using CandidQVmMulti.Services;
using CandidQVmMulti.View.Pages;
using CandidQVmMulti.View.Popups;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security.Cryptography.Xml;

namespace CandidQVmMulti.ViewModels;

public partial class VouchersPageViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Voucher> vouchers = new();

    [ObservableProperty] private ObservableCollection<Airline> airlines;

    [ObservableProperty] private Airline selectedAirline;

    [ObservableProperty]
    private string selectedText;
    [ObservableProperty]
    private bool isPanelVisible;

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
    private readonly SignatureService signatureService;

    public int VouchersCount => Vouchers?.Count ?? 0;

    public VouchersPageViewModel(
        MySqlAirlinesService airlinesService,
        MySqlVoucherService mySqlVoucherService, 
        MySqlEmployeeService mySqlEmployeeService, 
        MySqlFlightNumberService mySqlFlightNumberService,
        ExportServices exportServices,
        SignatureService signatureService)
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
        this.signatureService = signatureService;
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

            // Fetch data from DB (I/O-bound, so await is fine)
            var collection = await mySqlVoucherService.GetAllVouchersAsync();

            // Offload filtering and object creation to background thread
            var filtered = await Task.Run(() => ApplyFilters(collection));
            var vouchers = await Task.Run(() => new ObservableCollection<Voucher>(filtered));

            // Back on UI thread
            Vouchers = vouchers;

            await LoadAirlines();

            foreach (var person in Vouchers)
            {
                person.PropertyChanged += OnPersonSelectionChanged;
            }

            OnPropertyChanged(nameof(VouchersCount));

            await Toast.Make("Employees loaded successfully", duration: ToastDuration.Short).Show();
        }
        catch (Exception ex)
        {

            if (Application.Current?.MainPage is not null)
            {

                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
        finally
        {
            IsBusy = false;
            _isLoading = false;
        }
    }

    public async Task LoadAirlines()
    {
        if (airlinesLoaded) return;
        Airlines = new ObservableCollection<Airline>(await mySqlAirlinesService.GetAllAsync());
        airlinesLoaded = true;
    }

    private void OnPersonSelectionChanged(object? sender, PropertyChangedEventArgs e)
    {
        if(!string.IsNullOrWhiteSpace(e.PropertyName) && e.PropertyName.Equals("IsSelected"))
        {
            UpdateSelectionPanel();
        }
    }

    partial void OnVouchersChanged(ObservableCollection<Voucher> value)
    {
        OnPropertyChanged(nameof(VouchersCount));
        if (value != null)
            value.CollectionChanged += (s, e) => OnPropertyChanged(nameof(VouchersCount));
    }

    partial void OnSelectedAirlineChanged(Airline value)
    {
        _=RefreshData();
    }

    private void FireAndForget(Task task)
    {
        task.ContinueWith(t =>
        {
            if (t.Exception != null)
            {
                // Log exception
            }
        }, TaskContinuationOptions.OnlyOnFaulted);
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
        bool confirmDelete = await Shell.Current.DisplayAlert(
            "Confirm Delete",
            $"Are you sure you want to delete the voucher for {voucher.PassengerName}?",
            "Yes",
            "No");

        if (!confirmDelete)
            return;

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

        //// Export to memory stream
        //using var stream = new MemoryStream();
        //exportServices.ExportVouchersGroupedByAirline(filtered.ToList(), stream);
        //stream.Position = 0;

        //// Save file using FileSaver
        //var result = await FileSaver.Default.SaveAsync($"Candid Security {SelectedAirline?.Name ?? string.Empty} Vouchers {DateTime.Now.ToString("MMM_dd_yyyy")}.xlsx",stream,CancellationToken.None);

        //if (result.IsSuccessful)
        //{
        //    await Toast.Make("Exported vouchers to Excel", ToastDuration.Short).Show();
        //}
        //else
        //{
        //    await Toast.Make($"Failed to save file: {result.Exception.Message}", ToastDuration.Long).Show();
        //}

       await exportServices.ExportVouchersToPdfAsync(filtered.ToList(),2,"logo.jpg");
    }



    private CancellationTokenSource _debounceCts;
    private bool airlinesLoaded;

    partial void OnSearchTextChanged(string value)
    {
        _=RefreshData();
    }

    private async Task RefreshData()
    {
        _debounceCts?.Cancel();
        _debounceCts = new CancellationTokenSource();

        await Task.Delay(300, _debounceCts.Token)
            .ContinueWith(t =>
            {
                if (!t.IsCanceled)
                    MainThread.BeginInvokeOnMainThread(async() => await LoadData());
            });
    }

    private IEnumerable<Voucher> ApplyFilters(IEnumerable<Voucher> source)
    {
        // Precompute ticks for better performance
        long startTicks = StartDate.Ticks;
        long endTicks = EndDate.Ticks;

        return source.Where(v =>
            (string.IsNullOrWhiteSpace(SearchText) ||
             v.PassengerName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
            v.Date >= startTicks &&
            v.Date <= endTicks &&
            (SelectedAirline == null || v.AirlineID == SelectedAirline.Id));
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


    private void UpdateSelectionPanel()
    {
        int count = Vouchers.Count(p => p.IsSelected);
        IsPanelVisible = count > 0;
        SelectedText = $"{count} item(s) selected.";
    }

    [RelayCommand]
    private async Task SignSelected()
    {

        signatureService.SignatureCompletionSource = new TaskCompletionSource<int>();

        // Navigate to SignaturePage
        await Shell.Current.GoToAsync(nameof(SignaturePage));

        // Wait for the signature result
        var signature = await signatureService.SignatureCompletionSource.Task;


        // Handle cancel
        if (signature == -1)
        {
            await Toast.Make("Signature was not completed.", duration: ToastDuration.Short).Show();
        }
        else
        {
            // Apply signature to vouchers
            foreach (var voucher in Vouchers)
            {
                if (voucher.Status == VoucherStatus.Signed || !voucher.IsSelected) continue;
                voucher.IsSigned = true;
                voucher.SignatureID = signature;
                voucher.Status = VoucherStatus.Signed;
                await mySqlVoucherService.UpdateVoucherAsync(voucher);
            }
        }

        DeselectAll();
    }

    private void DeselectAll()
    {
        foreach (var voucher in Vouchers)
        {
            voucher.IsSelected = false;
        }
    }

    [RelayCommand]
    private async Task ClearFilters()
    {
        SelectedAirline = null;
        var result = GetMonthStartAndEnd(DateTime.Now);

        StartDate = result.startOfMonth;
        EndDate = result.endOfMonth;

        await LoadData();
    }
}