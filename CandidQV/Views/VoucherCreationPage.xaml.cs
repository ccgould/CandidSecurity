using CandidQV.Models.Items;
using CandidQV.ViewModels;

namespace CandidQV.Views;

public partial class VoucherCreationPage : ContentPage
{
    public VoucherCreationPageViewModel ViewModel => BindingContext as VoucherCreationPageViewModel;


    public VoucherCreationPage(VoucherCreationPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }

    protected async override void OnAppearing()
    {
        try
        {
            base.OnAppearing();
            await ViewModel.Init();
        }
        catch (Exception ex)
        {
            App.AlertSvc.ShowAlert("Error", ex.Message);
        }
    }

    private async void airlinePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
      await  ViewModel.SelectedAirlineChanged((Airline)airlinePicker.SelectedItem);
    }
}