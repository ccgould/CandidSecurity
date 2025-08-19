using CandidQV.Models.Items;
using CandidQV.Repositories;

namespace CandidQV.Views;

public partial class EmployeesPage : ContentPage
{
    private readonly EmployeeRepository _repository;
    private int _editEployeeId;

    public EmployeesPage(EmployeeRepository repository)
    {
        InitializeComponent();
        _repository = repository;
        Task.Run(async () => listView.ItemsSource =  await repository.GetEmployees());
    }

    private async void saveBtn_Clicked(object sender, EventArgs e)
    {
        if(_editEployeeId == 0)
        {
            //Add Employee

            await _repository.Create(new Employee
            {
                FirstName = firstNameEntryField.Text,
                LastName = lastNameEntryField.Text,
                MiddleInitial = middleInitialEntryField.Text,
            });
        }
        else
        {
            //Update Employee

            await _repository.Update(new Employee
            {
                Id = _editEployeeId,
                FirstName = firstNameEntryField.Text,
                LastName = lastNameEntryField.Text,
                MiddleInitial = middleInitialEntryField.Text,
            });

            _editEployeeId = 0;
        }

        firstNameEntryField.Text = string.Empty;
        middleInitialEntryField.Text = string.Empty;
        lastNameEntryField.Text = string.Empty;
        listView.ItemsSource = await _repository.GetEmployees();
    }

    private async void listView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        var employee = (Employee)e.Item;
        var action = await DisplayActionSheet("Options", "Cancel", null, "Edit", "Delete");

        switch (action) 
        {
            case "Edit":
                _editEployeeId = employee.Id;
                firstNameEntryField.Text = employee.FirstName;
                lastNameEntryField.Text = employee.LastName;
                middleInitialEntryField.Text = employee.MiddleInitial;
                break;
            case "Delete":
                await _repository.Delete(employee);
                listView.ItemsSource = await _repository.GetEmployees();
                break;

        }

    }
}
