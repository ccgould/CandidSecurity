using CandidQVmMulti.Models;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;

namespace CandidQVmMulti.View.Popups;

public partial class EmployeeEditor : Popup
{
	public EmployeeEditor( Employee employee)
	{
		InitializeComponent();
        NameField.Text = employee.Name;
        PositionField.Text = employee.Position;
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

public class EmployeeResult
{
    public string Name { get; set; }
    public string Position { get; set; }
}