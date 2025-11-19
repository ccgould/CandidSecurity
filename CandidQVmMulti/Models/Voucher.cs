using CandidQVmMulti.Enumerators;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Security.Cryptography.Xml;

namespace CandidQVmMulti.Models;
public partial class Voucher : ObservableObject
{
    public int Id { get; set; }
    [ObservableProperty] private bool isSelected;
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
    public VoucherStatus Status { get; set; }
    public bool IsSigned { get; set; }
    public int SignatureID { get; set; }
    public string Signature { get; set; }
    public string Terminal => GetTerminal();
    public int TerminalID { get; set; }

    private string GetTerminal()
    {
        var terminals = new string[] {"Domestic", "US", "Both"};
        return terminals[TerminalID];
    }
}