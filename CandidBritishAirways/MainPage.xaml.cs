using CandidBritishAirways.ViewModel;

namespace CandidBritishAirways;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
