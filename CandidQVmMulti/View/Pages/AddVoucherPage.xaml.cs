using CandidQVmMulti.ViewModels;

namespace CandidQVmMulti.View.Pages;

public partial class AddVoucherPage : ContentPage
{
	private AddVoucherPageViewModel ViewModel => BindingContext as AddVoucherPageViewModel;
    public AddVoucherPage(AddVoucherPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if(DeviceInfo.Current.Platform == DevicePlatform.Android)
        {
            if (ViewModel.IsEditing) return;
            await ViewModel.LoadData();
        }
    }

    private async void SfComboBox_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        if(DeviceInfo.Current.Platform == DevicePlatform.WinUI)
        {

        }
        if (!ViewModel.Initailized) return;
        await ViewModel.LoadFlightNumbers();
    }
}