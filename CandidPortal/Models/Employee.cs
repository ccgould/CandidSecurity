namespace CandidPortal.Models;

public class Employee
{
    private string fullName;

    public int ID { get; set; }
    public string FirstName { get; set; }
    public string MiddleInitial { get; set; }
    public string LastName { get; set; }
    public int BadgeID { get; set; }
    public int EmployeeID { get; set; }

    public string FullName
    {
        get
        {
            return $"{LastName}, {MiddleInitial} {FirstName}";
        }
    }

    public override string? ToString()
    {
        return FullName;
    }
}