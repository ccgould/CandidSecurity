using CandidQVmMulti.Models;
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

    private void VouchersDataGrid_CellTapped(object sender, Syncfusion.Maui.DataGrid.DataGridCellTappedEventArgs e)
    {
        if(e is not null)
        {
            if(e.RowData is Voucher voucher)
            {
                voucher.IsSelected = !voucher.IsSelected;
            }
        }
    }
}