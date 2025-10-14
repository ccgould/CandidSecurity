using CandidRadioTracker.Models;
using CandidRadioTracker.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CandidRadioTracker.ViewModels;
public partial class MainPageViewModel : ObservableObject
{
    private readonly FireBaseServices fireBaseServices;
    [ObservableProperty] private ObservableCollection<RadioLog> radioLogs;
    public MainPageViewModel(FireBaseServices fireBaseServices)
    {
        this.fireBaseServices = fireBaseServices;
    }

    [RelayCommand]
    public async Task AddRadioLog()
    {
        await Shell.Current.GoToAsync(nameof(RadioLogEditorPage));
    }
}
