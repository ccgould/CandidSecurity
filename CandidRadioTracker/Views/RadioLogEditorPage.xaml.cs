using CandidRadioTracker.ViewModels;

namespace CandidRadioTracker.Views;

public partial class RadioLogEditorPage : ContentPage
{
	public RadioLogEditorPage(RadioLogEditorViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}