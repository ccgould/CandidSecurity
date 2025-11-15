using CandidBritishAirways.ViewModel;

namespace CandidBritishAirways;

public partial class MainPage : ContentPage
{
    private MainPageViewModel viewModel => BindingContext as MainPageViewModel;
    public MainPage(MainPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.LoadData();
    }
}
