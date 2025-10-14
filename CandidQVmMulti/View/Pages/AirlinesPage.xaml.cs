using CandidQVmMulti.ViewModels;

namespace CandidQVmMulti.View.Pages;

public partial class AirlinesPage : ContentPage
{
	private AirlinesPageViewModel ViewModel => BindingContext as AirlinesPageViewModel;
	public AirlinesPage(AirlinesPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
		if (ViewModel.PreventRefresh) return;
        await ViewModel.LoadAirlinesAsync();
    }
}