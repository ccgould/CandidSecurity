using CandidQVmMulti.Models;
using CandidQVmMulti.Services;
using CandidQVmMulti.View.Popups;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;

public partial class EmployeesPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string searchText;
    [ObservableProperty] private bool isBusy;

    [ObservableProperty]
    private ObservableCollection<Employee> employees;
    private List<Employee> allEmployees = new();

    private bool _isLoading;
    private readonly MySqlEmployeeService service;

    public int EmployeesCount => Employees?.Count ?? 0;

    public bool PreventRefresh { get; internal set; }

    public EmployeesPageViewModel(MySqlEmployeeService service)
    {
        this.service = service;
        employees = new();
    }

    public async Task LoadData()
    {
        if (_isLoading) return;
        _isLoading = true;

        try
        {
            IsBusy = true; // triggers spinner
            var employees = await service.GetAllEmployeesAsync();
            allEmployees = employees.ToList();
            Employees = new ObservableCollection<Employee>(allEmployees);

            OnPropertyChanged(nameof(EmployeesCount));

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

        var collection = await service.GetAllEmployeesAsync();

        if(collection is not null)
        {
            Employees = new ObservableCollection<Employee>(collection);
        }
    }

    private CancellationTokenSource _debounceCts;

    partial void OnSearchTextChanged(string value)
    {
        _debounceCts?.Cancel();
        _debounceCts = new CancellationTokenSource();

        Task.Delay(300, _debounceCts.Token)
            .ContinueWith(async t =>
            {
                if (!t.IsCanceled)
                {
                    await FilterEmployeesAsync(value);
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
    }


    private Task FilterEmployeesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            Employees = new ObservableCollection<Employee>(allEmployees);
        }
        else
        {
            var filtered = allEmployees
                .Where(e =>
                    (e.Name?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.Position?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();

            Employees = new ObservableCollection<Employee>(filtered);
        }

        OnPropertyChanged(nameof(EmployeesCount));
        return Task.CompletedTask;
    }


    [RelayCommand]
    private async Task AddEmployee()
    {
        PreventRefresh = true;
        var popup = new AddNewEmployeeEditor();
        IPopupResult<EmployeeResult> result = await Shell.Current.CurrentPage.ShowPopupAsync<EmployeeResult>(popup);

        if (!result.WasDismissedByTappingOutsideOfPopup)
        {
            if (result.Result == null)
                return;

            var employee = new Employee
            {
                Name = result.Result.Name,
                Position = result.Result.Position,
                AddedDate = DateTime.Now.Ticks,
                IsActive = true
            };

            // Show toast after undo
            var toast = Toast.Make($"Employee {employee.Name} was added to the database!", ToastDuration.Short, 14);
            await toast.Show();

            Employees.Add(employee);
            OnPropertyChanged(nameof(EmployeesCount));
            await service.AddEmployeeAsync(employee);
        }
        PreventRefresh = false;
    }

    [RelayCommand]
    private async Task EditEmployee(Employee employee)
    {
        PreventRefresh = true;
        if (employee != null)
        {
            var popup = new EmployeeEditor(employee);
            IPopupResult<EmployeeResult> result = await Shell.Current.CurrentPage.ShowPopupAsync<EmployeeResult>(popup);

            if (!result.WasDismissedByTappingOutsideOfPopup)
            {
                if (result.Result == null)
                    return;

                employee.Name = result.Result.Name;
                employee.Position = result.Result.Position;             
                                
                
                await service.UpdateEmployeeAsync(employee);

                // Show toast after undo
                var toast = Toast.Make($"Employee {employee.Name} was updated in the database!", ToastDuration.Short, 14);
                await toast.Show();
            }
        }
        PreventRefresh = false;
    }

    [RelayCommand]
    private async Task DeleteEmployee(Employee employee)
    {
        // Dummy delete logic
        if (employee != null)
            Employees.Remove(employee);
        OnPropertyChanged(nameof(EmployeesCount));
        await service.DeleteEmployeeAsync(employee.Id);

    }
}
