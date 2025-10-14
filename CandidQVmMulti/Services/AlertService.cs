using CandidQVmMulti.Interfaces;

namespace CandidQVmMulti.Services;
internal class AlertService : IAlertService
{
    // ----- async calls (use with "await" - MUST BE ON DISPATCHER THREAD) -----

    public Task ShowAlertAsync(string title, string message, string cancel = "OK")
    {
        return Application.Current!.MainPage!.DisplayAlert(title, message, cancel);
    }

    public Task<bool> ShowConfirmationAsync(string title, string message, string accept = "Yes", string cancel = "No")
    {
        return Application.Current!.MainPage!.DisplayAlert(title, message, accept, cancel);
    }

    public Task<string> DisplayActionSheetAsync(string title,string cancel,string destruction, params string[] buttons)
    {
        return Application.Current!.MainPage!.DisplayActionSheet(title, cancel,destruction,buttons);
    }

    public Task<string> DisplayActionSheetAsync(string title, string cancel, string destruction, FlowDirection flowDirection, params string[] buttons)
    {
        return Application.Current!.MainPage!.DisplayActionSheet(title, cancel, destruction,flowDirection,buttons);
    }

    public Task<string> PromptAsync(string title, string message,string accept = "OK",string cancel = "Cancel",string placeholder = null, int maxLenght = 1,Keyboard keyboard = null,string initialValue = "")
    {
        return Application.Current.MainPage.DisplayPromptAsync(title, message,accept,cancel,placeholder,maxLenght,keyboard,initialValue);
    }


    // ----- "Fire and forget" calls -----

    /// <summary>
    /// "Fire and forget". Method returns BEFORE showing alert.
    /// </summary>
    public void ShowAlert(string title, string message, string cancel = "OK")
    {
        Application.Current!.MainPage!.Dispatcher.Dispatch(async () =>
            await ShowAlertAsync(title, message, cancel)
        );
    }

    /// <summary>
    /// "Fire and forget". Method returns BEFORE showing alert.
    /// </summary>
    /// <param name="callback">Action to perform afterwards.</param>
    public void ShowConfirmation(string title, string message, Action<bool> callback,
                                 string accept = "Yes", string cancel = "No")
    {
        Application.Current!.MainPage!.Dispatcher.Dispatch(async () =>
        {
            bool answer = await ShowConfirmationAsync(title, message, accept, cancel);
            callback(answer);
        });
    }

    /// <summary>
    /// "Fire and forget". Method returns BEFORE showing alert.
    /// </summary>
    /// <param name="callback">Action to perform afterwards.</param>
    public void ShowPrompt(string title, string message,  Action<string> callback,string accept = "OK", string cancel = "Cancel", string placeholder = null, int maxLenght = 1, Keyboard keyboard = null, string initialValue = "")
    {
        Application.Current!.MainPage!.Dispatcher.Dispatch(async () =>
        {
            string answer = await PromptAsync(title, message, accept, cancel, placeholder, maxLenght, keyboard, initialValue);
            callback(answer);
        });
    }
}