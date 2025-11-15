using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CandidBritishAirways.Models;

[Table("Reports")]
public partial class ReportModel : ObservableObject
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [ObservableProperty] private int flightNumber;
    [ObservableProperty] private int destination;
    [ObservableProperty] private string aircraftRegistration;
    [ObservableProperty] private DateTime date;

    // Timing
    [ObservableProperty] private TimeSpan? startPeriod;
    [ObservableProperty] private TimeSpan? endPeriod;
    [ObservableProperty] private TimeSpan? scheduledTimeArrival;
    [ObservableProperty] private TimeSpan? actualTimeArrival;
    [ObservableProperty] private TimeSpan? scheduledTimeDeparture;
    [ObservableProperty] private TimeSpan? actualTimeDeparture;
    [ObservableProperty] private TimeSpan? arrivalAtGate;
    [ObservableProperty] private TimeSpan? airborne;

    // Positions
    [ObservableProperty] private int parkingGate;
    [ObservableProperty] private int frontDoorAccessPosition;
    [ObservableProperty] private int rampAccessPosition;
    [ObservableProperty] private int baggageMakeupPosition;
    [ObservableProperty] private int cateringPosition;
    [ObservableProperty] private int backDoorAccessPosition;

    // Services & Personnel
    [ObservableProperty] private int inboundWheelchairs;
    [ObservableProperty] private int outboundWheelchairs;
    [ObservableProperty] private int liftChairs;
    [ObservableProperty] private int cleaners;
    [ObservableProperty] private int fuelers;

    // Pod & Seals
    [ObservableProperty] private bool isCatering;
    [ObservableProperty] private string podNumber;
    [ObservableProperty] private TimeSpan? podOffload;
    [ObservableProperty] private TimeSpan? podOnload;
    [ObservableProperty] private string leftFrontSeal;
    [ObservableProperty] private string rightFrontSeal;
    [ObservableProperty] private string batterySeal;
    [ObservableProperty] private string dryIce;

    // Aircraft Access Points
    [ObservableProperty] private string lFwd;
    [ObservableProperty] private string lFwdOverwingDoor;
    [ObservableProperty] private string lAftOverwingDoor;
    [ObservableProperty] private string lAftDoor;
    [ObservableProperty] private string rFwdDoor;
    [ObservableProperty] private string rFwdOverwingDoor;
    [ObservableProperty] private string rAftOverwingDoor;
    [ObservableProperty] private string rAftDoor;
    [ObservableProperty] private string frontBaggageHoldDoor;
    [ObservableProperty] private string backBaggageHoldDoor;
    [ObservableProperty] private string bulkBaggageHoldDoorC5;
    [ObservableProperty] private string electricEquipmentAccess;
    [ObservableProperty] private string groundServiceCommunication;
    [ObservableProperty] private string groundCommunicationAir;
    [ObservableProperty] private string airExhaustL;
    [ObservableProperty] private string airExhaustR;

    // Cancellation
    [ObservableProperty] private bool flightCanceled;
    [ObservableProperty] private TimeSpan? cancelledTime;
    [ObservableProperty] private int cancelledParkingGate;

    // Misc
    [ObservableProperty] private string comments;
    [ObservableProperty] private int reportBy;
    [ObservableProperty] private string reportByName;
    [ObservableProperty] private DateTime? sentDate = null;
    [ObservableProperty] private bool isSent;
}