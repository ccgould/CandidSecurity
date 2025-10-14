namespace CandidRadioTracker.Models;
public class RadioLog
{
    public int Id { get; set; }
    public string Date { get; set; }
    public int EmployeeId { get; set; }
    public string RadioId { get; set; }
    public string OutTime { get; set; }
    public string InTime { get; set; }
}
