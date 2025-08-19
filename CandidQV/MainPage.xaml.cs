using CandidQV.Repositories;

namespace CandidQV;

public partial class MainPage : ContentPage
{
    public MainPage(VoucherRepository repository)
    {
        InitializeComponent();
        Task.Run( async () => listView.ItemsSource = await repository.GetVouchers());
    }
}
