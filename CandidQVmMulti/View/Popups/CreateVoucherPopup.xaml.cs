using CandidQVmMulti.ViewModels;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using System.Runtime.CompilerServices;

namespace CandidQVmMulti.View.Popups;

public partial class CreateVoucherPopup : Popup
{
    private CreateVoucherPopupViewModel viewModel => BindingContext as CreateVoucherPopupViewModel; 

	public CreateVoucherPopup(CreateVoucherPopupViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
        
    private async void closedBtn_Clicked(object sender, EventArgs e)
    {
        await viewModel.CreateVoucher();
    }

    private async void SfComboBox_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        await viewModel.LoadFlightNumbers();
    }

    private async void Popup_Opened(object sender, EventArgs e)
    {
       await viewModel.LoadData();
    }
}