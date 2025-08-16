using CandidPortal.Models;
using CandidPortal.Repositories;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using Wpf.Ui.Abstractions.Controls;

namespace CandidPortal.ViewModels.Pages;
public partial class EmployeesPageViewModel : ObservableObject, INavigationAware
{
    [ObservableProperty] private ObservableCollection<Employee> employees;
    [ObservableProperty] private ObservableCollection<TrainingRecord> trainingRecords;
    [ObservableProperty] private ObservableCollection<TrainingType> trainingRecordTypes;
    private readonly EmployeeRepository _employeeRepository;
    private readonly TrainingRecordRepository _trainingRecordRepository;
    private readonly TrainingTypeRepository _trainingTypeRepository;
    [ObservableProperty] private Employee employee;

    public EmployeesPageViewModel(EmployeeRepository employeeRepository,TrainingRecordRepository trainingRecordRepository,TrainingTypeRepository trainingTypeRepository)
    {
        _employeeRepository = employeeRepository;
        _trainingRecordRepository = trainingRecordRepository;
        _trainingTypeRepository = trainingTypeRepository;
    }

    public async Task OnNavigatedToAsync()
    {
        //if (!_isInitialized)
        //    InitializeViewModel();


        Employees = await _employeeRepository.GetEmployees();

        if(Employees?.Any() ?? false)
        {
            Employee = Employees[0];
            TrainingRecords = await _trainingRecordRepository.GetTrainingRecords(Employee.ID);
            TrainingRecordTypes = await _trainingTypeRepository.GetTrainingTypes();
        }
    }

    public Task OnNavigatedFromAsync() => Task.CompletedTask;

    [RelayCommand]
    private async Task EmployeeSelectionChanged()
    {
        if (employee is null) return;
        TrainingRecords = await _trainingRecordRepository.GetTrainingRecords(employee.ID);
    }

    [RelayCommand]
    private async Task Save(TrainingRecord record)
    {
        await _trainingRecordRepository.Save(record);
    }

    [RelayCommand]
    private async Task AddNewRecord()
    {
        _trainingRecordRepository.AddNew(new TrainingRecord());
    }
}
 