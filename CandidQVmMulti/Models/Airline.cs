using CandidQVmMulti.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CandidQVmMulti.Models;

public partial class Airline : ObservableObject
{
    [ObservableProperty] private int id;
    [ObservableProperty] private string name;
    [ObservableProperty] private string iata;
    [ObservableProperty] private long addDate;
    [ObservableProperty] private int terminal;
    [ObservableProperty] private ObservableCollection<FlightNumber> flightNumbers = new();
    [ObservableProperty] private int airlineCount;

    public Airline()
    {
        airlineCount = FlightNumbers.Count;
        FlightNumbers.CollectionChanged += FlightNumbers_CollectionChanged;
    }

    private void FlightNumbers_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        AirlineCount = FlightNumbers.Count;
    }

    internal FlightNumber AddFlightNumber(int airlineId, int terminal, string flightNumber, DateTime dateTime)
    {
        var result = new FlightNumber { AirlineId=airlineId, TerminalId=terminal, Number = flightNumber, AddedDate = dateTime.Ticks };
        FlightNumbers.Add(result);
        return result;
    }

}
