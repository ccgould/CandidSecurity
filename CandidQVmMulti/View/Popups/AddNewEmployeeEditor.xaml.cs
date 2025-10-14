using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;

namespace CandidQVmMulti.View.Popups;

public partial class AddNewEmployeeEditor : Popup
{
	public AddNewEmployeeEditor()
	{
		InitializeComponent();
	}
    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        var name = NameField.Text;
        var position = PositionField.Text;

        if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(position))
        {

            await Shell.Current.ClosePopupAsync(new EmployeeResult
            {
                Name = name,
                Position = position
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