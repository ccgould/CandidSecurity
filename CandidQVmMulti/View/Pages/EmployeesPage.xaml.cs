namespace CandidQVmMulti.View.Pages;

public partial class EmployeesPage : ContentPage
{
	private EmployeesPageViewModel ViewModel => BindingContext as EmployeesPageViewModel;
	public EmployeesPage(EmployeesPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
#if WINDOWS
        EmployeesDataGrid2.SearchController.AllowFiltering = true;
#endif
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (ViewModel.PreventRefresh) return;
        await ViewModel.LoadData();
    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
#if WINDOWS
        EmployeesDataGrid2.SearchController.Search(e.NewTextValue);
#endif
    }
}