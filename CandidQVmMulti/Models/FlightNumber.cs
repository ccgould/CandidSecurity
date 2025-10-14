namespace CandidQVmMulti.Models;

public class FlightNumber
{
    public int Id { get; set; }
    public int AirlineId { get; set; }
    public int TerminalId { get; set; }
    public string Number { get; set; }
    public long AddedDate { get; set; }
}