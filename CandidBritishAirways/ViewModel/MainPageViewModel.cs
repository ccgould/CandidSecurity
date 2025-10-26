using CandidBritishAirways.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidBritishAirways.ViewModel;
public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty]
    private ReportModel report = new();

    [RelayCommand]
    private async Task SubmitReportAsync()
    {
        // 🧠 Save report to SQLite or send to API
        await Task.Delay(500); // Simulate work

        // 🎉 Feedback
        await Shell.Current.DisplayAlert("Success", "Report submitted!", "OK");
    }
}
