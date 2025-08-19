using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace CandidQV.Models.Items;
public partial class Employee : ObservableObject
{
    [PrimaryKey]
    [AutoIncrement]
    [Column("id")]
    public int Id { get; set; }

    private string _firstName;

    [Column("first_name")]

    public string FirstName
    {
        get => _firstName;
        set
        {
            // SetProperty handles this automatically if used in the setter
            // but for manual notification, you'd do:
            if (SetProperty(ref _firstName, value))
            {
                // If FirstName changes, and it affects FullName,
                // you might manually raise PropertyChanged for FullName
                OnPropertyChanged(nameof(FullName));
            }
        }
    }

    private string _lastName;

    [Column("last_name")]
    public string LastName
    {
        get => _lastName;
        set
        {
            if (SetProperty(ref _lastName, value))
            {
                OnPropertyChanged(nameof(FullName));
            }
        }
    }
    private string _middleInitial;

    [Column("middle_initial")]
    public string MiddleInitial
    {
        get => _middleInitial;
        set
        {
            if (SetProperty(ref _middleInitial, value))
            {
                OnPropertyChanged(nameof(FullName));
            }
        }
    }

    public string FullName => $"{FirstName} {MiddleInitial} {LastName}";


    // Example of manual notification for a property not directly assigned in a setter
    public void UpdateData()
    {
        // Imagine some internal logic updates _firstName and _lastName
        // ...
        // After the update, manually notify for affected properties
        OnPropertyChanged(nameof(FirstName));
        OnPropertyChanged(nameof(LastName));
        OnPropertyChanged(nameof(FullName));
    }
}
