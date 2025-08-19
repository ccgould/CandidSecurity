using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace CandidQV.Models.Items;
public partial class Voucher : ObservableObject
{
    [PrimaryKey]
    [AutoIncrement]
    [Column("id")]
    public int Id { get; set; }

    [Column("passenger_name")]
    public string PassengerName { get; set; }

    [Column("flight_number")]
    public int FlightNumberId { get; set; }

    [Column("airline_id")]
    public int AirlineId { get; set; }

    [Column("employee_id")]
    public int EmployeeID { get; set; }

    [Column("us_departure")]
    public bool IsUsDeparture { get; set; }

    [Column("start_time")]
    public TimeSpan StartTime { get; set; }

    [Column("end_time")]
    public TimeSpan EndTime { get; set; }

    [Column("date")]
    public DateOnly Date { get; set; }
}
