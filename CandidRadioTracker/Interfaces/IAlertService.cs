namespace CandidRadioTracker.Interfaces;
public interface IAlertService
{
    // ----- async calls (use with "await" - MUST BE ON DISPATCHER THREAD) -----
    Task ShowAlertAsync(string title, string message, string cancel = "OK");
    Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Yes", string cancel = "No");

    // ----- "Fire and forget" calls -----
    void ShowAlert(string title, string message, string cancel = "OK");
    /// <param name="callback">Action to perform afterwards.</param>
    void ShowConfirmation(string title, string message, Action<bool> callback,
                          string accept = "Yes", string cancel = "No");
    void ShowPrompt(string title, string message, Action<string> callback, string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLenght = 1, Keyboard keyboard = null, string initialValue = "");
    Task<string> PromptAsync(string title, string message,string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLenght = 1, Keyboard keyboard = null, string initialValue = "");
    Task<string> DisplayActionSheetAsync(string title, string cancel, string destruction, params string[] buttons);
    Task<string> DisplayActionSheetAsync(string title, string cancel, string destruction, FlowDirection flowDirection, params string[] buttons);
}
