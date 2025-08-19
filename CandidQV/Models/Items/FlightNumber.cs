using SQLite;


namespace CandidQV.Models.Items;
public class FlightNumber
{
    public FlightNumber()
    {
        
    }

    public FlightNumber(string flightNumber)
    {
        Number = flightNumber;
    }

    public FlightNumber(string flightNumber, int airlineId)
    {
        Number = flightNumber;
        AirlineId = airlineId;
    }

    [PrimaryKey]
    [AutoIncrement]
    [Column("id")]
    public int Id { get; set; }
    [Column("airline_id")]
    public int AirlineId { get; set; }
    [Column("flight_number")]
    public string Number { get; set; }
}
