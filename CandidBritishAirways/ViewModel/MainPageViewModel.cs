using CandidBritishAirways.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CandidBritishAirways.ViewModel;
public partial class MainPageViewModel : ObservableObject
{
    [ObservableProperty]
    private ReportModel report = new();

    public MainPageViewModel()
    {
        report = new ReportModel
        {
            Id = 1,
            FlightNumber = 7890,
            Destination = 305, // e.g., airport ID
            AircraftRegistration = 501, // e.g., aircraft ID
            Date = new DateTime(2025, 10, 28),

            StartPeriod = TimeSpan.FromHours(6),
            EndPeriod = TimeSpan.FromHours(12),
            ScheduledTimeArrival = TimeSpan.FromHours(9.5), // 09:30
            ActualTimeArrival = TimeSpan.FromHours(9.75),   // 09:45
            ScheduledTimeDeparture = TimeSpan.FromHours(10.25), // 10:15
            ActualTimeDeparture = TimeSpan.FromHours(10.42),    // 10:25
            ArrivalAtGate = TimeSpan.FromHours(9.83), // 09:50
            Airborne = TimeSpan.FromHours(10.5),      // 10:30

            ParkingGate = 7,
            FrontDoorAccessPosition = 11,
            RampAccessPosition = 9,
            BaggageMakeupPosition = 4,
            CateringPosition = 6,
            BackDoorAccessPosition = 12,

            InboundWheelchairs = 3,
            OutboundWheelchairs = 2,
            LiftChairs = 1,
            Cleaners = 4,
            Fuelers = 2,

            PodNumber = "POD-456",
            PodOffload = TimeSpan.FromHours(9.9), // 09:54
            PodOnload = TimeSpan.FromHours(10.15), // 10:09
            LeftFrontSeal = "LF-001",
            RightFrontSeal = "RF-002",
            BatterySeal = "BAT-003",
            DryIce = "1.5kg",

            LFwd = "Open",
            LFwdOverwingDoor = "Closed",
            LAftOverwingDoor = "Closed",
            LAftDoor = "Open",
            RFwdDoor = "Open",
            RFwdOverwingDoor = "Closed",
            RAftOverwingDoor = "Closed",
            RAftDoor = "Open",
            FrontBaggageHoldDoor = "Closed",
            BackBaggageHoldDoor = "Closed",
            BulkBaggageHoldDoorC5 = "Closed",
            ElectricEquipmentAccess = "Locked",
            GroundServiceCommunication = "Connected",
            GroundCommunicationAir = "Disconnected",
            AirExhaustL = "Normal",
            AirExhaustR = "Normal",

            FlightCanceled = false,
            CancelledTime = null,
            CancelledParkingGate = 0,

            Comments = "Smooth turnaround. Minor delay due to ramp congestion.",
            ReportBy = 1001 // e.g., employee ID
        };
    }


    [RelayCommand]
    private async Task SubmitReportAsync()
    {
        // 🧠 Save report to SQLite or send to API
        await Task.Delay(500); // Simulate work

        // 🎉 Feedback
        await Shell.Current.DisplayAlert("Success", "Report submitted!", "OK");
    }
}
