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
            ViewModel.Initailized = true;
            await ViewModel.LoadData();
        }
    }

    private async void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        await ViewModel.LoadFlightNumbers();
    }


    private async void SfComboBox_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        //Add a way to stop this if editing
        await ViewModel.LoadFlightNumbers();
    }
}