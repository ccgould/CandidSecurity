using CandidQVmMulti.ViewModels;

namespace CandidQVmMulti.View.Pages;

public partial class VouchersPage : ContentPage
{
    private VouchersPageViewModel ViewModel => BindingContext as VouchersPageViewModel;
    public VouchersPage(VouchersPageViewModel vm)
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