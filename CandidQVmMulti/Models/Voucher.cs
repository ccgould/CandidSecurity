namespace CandidQVmMulti.Models;
public class Voucher
{
    public int Id { get; set; }
    public long Date { get; set; }
    public string PassengerName { get; set; }
    public int EmployeeID { get; set; }
    public string Employee { get; set; }
    public string FullFlightNumber => $"({Iata}) {Flight}";
    public int AirlineID { get; set; }
    public string Airline { get; set; }
    public int FlightID { get; set; }
    public string Flight { get; set; }
    public string Iata { get; set; }
    public long StartTime { get; set; }
    public long EndTime { get; set; }
    public int Status { get; set; }
}