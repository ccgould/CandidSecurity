using CandidQVmMulti.Services;
using CandidQVmMulti.ViewModels;

namespace CandidQVmMulti.View.Pages;

public partial class MainPage : ContentPage
{
    private MainPageViewModel ViewModel => BindingContext as MainPageViewModel;

    public MainPage(MainPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.LoadData();

    }
}
