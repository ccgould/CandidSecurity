using CandidQV.Models.Items;
using CandidQV.Repositories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CandidQV.ViewModels;
public partial class EmployeePageViewModel : ObservableObject
{
    private readonly EmployeeRepository employeeRepository;
    [ObservableProperty] private ObservableCollection<Employee> employees;
    [ObservableProperty] private Employee employee;
    private Employee _lastDeletedEmployee;

    public EmployeePageViewModel(EmployeeRepository employeeRepository)
    {
        this.employeeRepository = employeeRepository;
        employees = new();
        employee = new();
    }

    internal async Task DeleteEmployeeAsync(Employee employee)
    {
        // Need to make sure delete flights
        if (employee == null) return;
        _lastDeletedEmployee = employee;
        Employees.Remove(employee);
        // Optionally delete from DB
        await employeeRepository.DeleteAsync(employee);
        Employees.Remove(employee);
    }

    public async Task InitAsync()
    {
        var airlines = await employeeRepository.GetAllAsync();
        Employees.Clear();
        foreach (var airline in airlines)
            Employees.Add(airline);
    }

    internal void OnEmployeeSelected(Employee employee)
    {
        if (employee == null) return;
        Employee = employee;
    }

    internal async Task UndoDeleteAsync()
    {
        if (_lastDeletedEmployee == null) return;
        await employeeRepository.CreateAsync(_lastDeletedEmployee);
        Employees.Add(_lastDeletedEmployee);
        _lastDeletedEmployee = null;
    }

    [RelayCommand]
    private async Task Save()
    {
        if (Employee.Id == 0)
        {
            //Add Employee
            await employeeRepository.CreateAsync(Employee);
            Employees.Add(Employee);

        }
        else
        {
            //Update Employee
            await employeeRepository.UpdateAsync(Employee);
        }

        Employee = new();
    }
}
