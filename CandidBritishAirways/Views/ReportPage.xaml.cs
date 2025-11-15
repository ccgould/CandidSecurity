using CandidBritishAirways.ViewModel;

namespace CandidBritishAirways.Views;

public partial class ReportPage : ContentPage
{
	private ReportViewModel viewModel => BindingContext as ReportViewModel;
	public ReportPage(ReportViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		viewModel.LoadReportsCommand.Execute(null);
    }
}