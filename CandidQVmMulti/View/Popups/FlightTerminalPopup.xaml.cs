using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;

namespace CandidQVmMulti.View.Popups;

public partial class FlightTerminalPopup : Popup
{
    public FlightTerminalPopup()
    {
        InitializeComponent();
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        var flightNumber = FlightNumberEntry.Text;
        var selectedTerminal = TerminalPicker.SelectedIndex;

        if (!string.IsNullOrWhiteSpace(flightNumber) && selectedTerminal != -1)
        {

            await Shell.Current.ClosePopupAsync(new FlightTerminalResult
            {
                FlightNumber = flightNumber,
                Terminal = selectedTerminal + 1
            });
        }
        else
        {
            // Optional: show toast/snackbar for validation
        }
    }
}

public class FlightTerminalResult
{
    public string FlightNumber { get; set; }
    public int Terminal { get; set; }
}