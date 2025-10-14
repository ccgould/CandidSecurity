using CandidQVmMulti.Models;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;

namespace CandidQVmMulti.View.Popups;

public partial class AddNewAirlineEditor : Popup
{
	public AddNewAirlineEditor()
	{
		InitializeComponent();
	}
    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        var name = NameField.Text;
        var iata = IataField.Text;

        if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(iata))
        {

            await Shell.Current.ClosePopupAsync(new Airline
            {
                Name = name,
                Iata = iata
            });
        }
        else
        {
            // Optional: show toast/snackbar for validation
        }
    }

    private async void cancelBtn_Clicked(object sender, EventArgs e)
    {
        await CloseAsync();
    }
}