using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.Collections.ObjectModel;

namespace CandidQV.Models.Items;
[Table("airlines_tbl")]
public partial class Airline : ObservableObject
{
    private ObservableCollection<FlightNumber> flightNumbers = new();

    [PrimaryKey]
    [AutoIncrement]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("iata_code")]
    public string IataCode { get; set; }

    [Ignore]
    public ObservableCollection<FlightNumber> FlightNumbers
    {
        get => flightNumbers;
        set
        {
            flightNumbers = value;
            OnPropertyChanged();
        }
    }
}

