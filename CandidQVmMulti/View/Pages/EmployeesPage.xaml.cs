namespace CandidQVmMulti.View.Pages;

public partial class EmployeesPage : ContentPage
{
	private EmployeesPageViewModel ViewModel => BindingContext as EmployeesPageViewModel;
	public EmployeesPage(EmployeesPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (ViewModel.PreventRefresh) return;
        await ViewModel.LoadData();
    }
}