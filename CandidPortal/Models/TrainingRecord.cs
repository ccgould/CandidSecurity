namespace CandidPortal.Models;
public partial class TrainingRecord : ObservableObject
{
    [ObservableProperty] private int iD;
    [ObservableProperty] private int employeeID;
    [ObservableProperty] private int trainingType;
    [ObservableProperty] private decimal annualInternal;
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(ExpiredDate))] [NotifyPropertyChangedFor(nameof(IsExpired))] private DateOnly issuedDate;
    public DateOnly ExpiredDate => GetExpirationDate();
    public bool IsExpired => DateTime.Now.CompareTo(IssuedDate.ToDateTime(new TimeOnly(12, 0))) > 0;

    private DateOnly GetExpirationDate()
    {
        DateTime dateTime = IssuedDate.ToDateTime(new TimeOnly(10, 0)).AddYears(Convert.ToInt32(AnnualInternal));
        return DateOnly.FromDateTime(dateTime);
    }

}
