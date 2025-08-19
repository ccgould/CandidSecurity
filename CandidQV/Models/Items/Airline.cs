using SQLite;

namespace CandidQV.Models.Items;
[Table("Airlines")]
public class Airline
{
    [PrimaryKey]
    [AutoIncrement]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("iata_code")]
    public string IataCode { get; set; }
}

