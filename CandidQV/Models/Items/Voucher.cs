using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.ComponentModel;

namespace CandidQV.Models.Items;
[Table("vouchers_tbl")]
public partial class Voucher : ObservableObject
{
    private string dateString;
    private string startTimeString;
    private string endTimeString;
    private bool isSent;
    private string passengerName;

    [PrimaryKey]
    [AutoIncrement]
    [Column("id")]
    public int Id { get; set; }

    [Column("passenger_name")]
    public string PassengerName
    {
        get => passengerName;
        set
        {
            passengerName = value;
            OnPropertyChanged(nameof(PassengerName));
        }
    }

    [Column("flight_number")]
    public int FlightNumberId { get; set; }

    [Column("airline_id")]
    public int AirlineId { get; set; }

    [Ignore]
    public Airline Airline { get; set; }

    [Ignore]
    public string AirlineIataCode => Airline?.IataCode ?? "N/A";

    [Ignore]
    public FlightNumber FlightNumber { get; set; }

    [Ignore]
    public string FlightNumberString => FlightNumber?.Number ?? "N/A";


    [Column("employee_id")]
    public int EmployeeID { get; set; }

    [Column("us_departure")]
    public bool IsUsDeparture { get; set; } = true;

    [Column("start_time")]
    public string StartTimeString
    {
        get => startTimeString;
        set
        {
            startTimeString = value;
            StartTime = DateTime.Today.Add(TimeSpan.Parse(value)).ToString("hh:mm tt");
        }
    }
    public string StartTime { get; set; }

    [Column("end_time")]
    public string EndTimeString
    {
        get => endTimeString;
        set
        {
            endTimeString = value;
            EndTime = DateTime.Today.Add(TimeSpan.Parse(value)).ToString("hh:mm tt");
        }
    }
    public string EndTime { get; set; }

    [Column("date")]
    public string DateString
    {
        get => dateString;
        set
        {
            dateString = value;
            Date = DateTime.Parse(value);
        }
    }

    [Ignore]
    public DateTime Date { get; set; } = DateTime.Now;

    [Column("image_path")]
    public string ImagePath { get; set; }

    [Column("is_sent")]
    [DefaultValue(false)]
    public bool IsSent
    {
        get => isSent;
        set
        {
            isSent = value;
            OnPropertyChanged(nameof(IsSent));
        }
    }

    [Ignore]
    public Employee Employee { get; set; }
}
