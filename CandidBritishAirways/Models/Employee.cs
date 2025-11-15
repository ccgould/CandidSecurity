using CommunityToolkit.Mvvm.ComponentModel;

namespace CandidBritishAirways.Models;

public partial class Employee : ObservableObject
{
    [ObservableProperty] private int id;
    [ObservableProperty] private string name;
    [ObservableProperty] private string email;
    [ObservableProperty] private string position;
    [ObservableProperty] private long addedDate;
    [ObservableProperty] private bool isActive;
}