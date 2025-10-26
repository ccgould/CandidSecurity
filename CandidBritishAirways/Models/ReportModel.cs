using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidBritishAirways.Models;
public class ReportModel
{
    #region Flight Info
    public int Id { get; set; }
    public int FlightNumber { get; set; }
    public int Destination { get; set; }
    public int AircraftRegistration { get; set; }
    public DateTime Date { get; set; }
    #endregion

    #region Timing
    public DateTime StartPeriod { get; set; }
    public DateTime EndPeriod { get; set; }
    public DateTime ScheduledTimeArrival { get; set; }
    public DateTime ActualTimeArrival { get; set; }
    public DateTime ScheduledTimeDeparture { get; set; }
    public DateTime ActualTimeDeparture { get; set; }
    public DateTime ArrivalAtGate { get; set; }
    public DateTime Airborne { get; set; }
    #endregion

    #region Positions
    public int ParkingGate { get; set; }
    public int FrontDoorAccessPosition { get; set; }
    public int RampAccessPosition { get; set; }
    public int BaggageMakeupPosition { get; set; }
    public int CateringPosition { get; set; }
    public int BackDoorAccessPosition { get; set; }
    #endregion

    #region Services & Personnel
    public int InboundWheelchairs { get; set; }
    public int OutboundWheelchairs { get; set; }
    public int LiftChairs { get; set; }
    public int Cleaners { get; set; }
    public int Fuelers { get; set; }
    #endregion

    #region Pod & Seals
    public string PodNumber { get; set; }
    public DateTime PodOffload { get; set; }
    public DateTime PodOnload { get; set; }
    public string LeftFrontSeal { get; set; }
    public string RightFrontSeal { get; set; }
    public string BatterySeal { get; set; }
    public string DryIce { get; set; }
    #endregion

    #region Aircraft Access Points
    public string LFwd { get; set; }
    public string LFwdOverwingDoor { get; set; }
    public string LAftOverwingDoor { get; set; }
    public string LAftDoor { get; set; }
    public string RFwdDoor { get; set; }
    public string RFwdOverwingDoor { get; set; }
    public string RAftOverwingDoor { get; set; }
    public string RAftDoor { get; set; }
    public string FrontBaggageHoldDoor { get; set; }
    public string BackBaggageHoldDoor { get; set; }
    public string BulkBaggageHoldDoorC5 { get; set; }
    public string ElectricEquipmentAccess { get; set; }
    public string GroundServiceCommunication { get; set; }
    public string GroundCommunicationAir { get; set; }
    public string AirExhaustL { get; set; }
    public string AirExhaustR { get; set; }
    #endregion

    #region Cancellation
    public bool FlightCanceled { get; set; }
    public DateTime CancelledTime { get; set; }
    public int CancelledParkingGate { get; set; }
    #endregion

    #region Misc
    public string Comments { get; set; }
    public int ReportBy { get; set; }
    #endregion


    //public enum DoorStatus
    //{
    //    Unknown = 0,
    //    Open = 1,
    //    Closed = 2,
    //    Locked = 3
    //}

    //public enum SealStatus
    //{
    //    NotPresent = 0,
    //    Present = 1,
    //    Broken = 2
    //}

    //public int LFwdValue { get; set; }

    //public DoorStatus LFwd
    //{
    //    get => (DoorStatus)LFwdValue;
    //    set => LFwdValue = (int)value;
    //}
}
