using CandidBritishAirways.Models;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CandidBritishAirways.Services;

public class RemoteDatabaseService
{
    private readonly string _connectionString;

    public RemoteDatabaseService()
    {
        _connectionString = "Server=sql5.freesqldatabase.com\";Database=your-db;User=your-user;Password=your-password;";
    }

    public async Task<List<ReportModel>> GetReportsAsync()
    {
        var reports = new List<ReportModel>();

        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        string query = "SELECT * FROM Reports";
        using var cmd = new MySqlCommand(query, connection);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var report = new ReportModel
            {
                Id = reader.GetInt32("Id"),
                FlightNumber = reader.GetInt32("FlightNumber"),
                Destination = reader.GetInt32("Destination"),
                AircraftRegistration = reader.GetString("AircraftRegistration"),
                Date = reader.GetDateTime("Date"),

                // Timing
                StartPeriod = reader.IsDBNull("StartPeriod") ? null : reader.GetTimeSpan("StartPeriod"),
                EndPeriod = reader.IsDBNull("EndPeriod") ? null : reader.GetTimeSpan("EndPeriod"),
                ScheduledTimeArrival = reader.IsDBNull("ScheduledTimeArrival") ? null : reader.GetTimeSpan("ScheduledTimeArrival"),
                ActualTimeArrival = reader.IsDBNull("ActualTimeArrival") ? null : reader.GetTimeSpan("ActualTimeArrival"),
                ScheduledTimeDeparture = reader.IsDBNull("ScheduledTimeDeparture") ? null : reader.GetTimeSpan("ScheduledTimeDeparture"),
                ActualTimeDeparture = reader.IsDBNull("ActualTimeDeparture") ? null : reader.GetTimeSpan("ActualTimeDeparture"),
                ArrivalAtGate = reader.IsDBNull("ArrivalAtGate") ? null : reader.GetTimeSpan("ArrivalAtGate"),
                Airborne = reader.IsDBNull("Airborne") ? null : reader.GetTimeSpan("Airborne"),

                // Positions
                ParkingGate = reader.GetInt32("ParkingGate"),
                FrontDoorAccessPosition = reader.GetInt32("FrontDoorAccessPosition"),
                RampAccessPosition = reader.GetInt32("RampAccessPosition"),
                BaggageMakeupPosition = reader.GetInt32("BaggageMakeupPosition"),
                CateringPosition = reader.GetInt32("CateringPosition"),
                BackDoorAccessPosition = reader.GetInt32("BackDoorAccessPosition"),

                // Services & Personnel
                InboundWheelchairs = reader.GetInt32("InboundWheelchairs"),
                OutboundWheelchairs = reader.GetInt32("OutboundWheelchairs"),
                LiftChairs = reader.GetInt32("LiftChairs"),
                Cleaners = reader.GetInt32("Cleaners"),
                Fuelers = reader.GetInt32("Fuelers"),

                // Pod & Seals
                IsCatering = reader.GetBoolean("IsCatering"),
                PodNumber = reader.GetString("PodNumber"),
                PodOffload = reader.IsDBNull("PodOffload") ? null : reader.GetTimeSpan("PodOffload"),
                PodOnload = reader.IsDBNull("PodOnload") ? null : reader.GetTimeSpan("PodOnload"),
                LeftFrontSeal = reader.GetString("LeftFrontSeal"),
                RightFrontSeal = reader.GetString("RightFrontSeal"),
                BatterySeal = reader.GetString("BatterySeal"),
                DryIce = reader.GetString("DryIce"),

                // Aircraft Access Points
                LFwd = reader.GetString("LFwd"),
                LFwdOverwingDoor = reader.GetString("LFwdOverwingDoor"),
                LAftOverwingDoor = reader.GetString("LAftOverwingDoor"),
                LAftDoor = reader.GetString("LAftDoor"),
                RFwdDoor = reader.GetString("RFwdDoor"),
                RFwdOverwingDoor = reader.GetString("RFwdOverwingDoor"),
                RAftOverwingDoor = reader.GetString("RAftOverwingDoor"),
                RAftDoor = reader.GetString("RAftDoor"),
                FrontBaggageHoldDoor = reader.GetString("FrontBaggageHoldDoor"),
                BackBaggageHoldDoor = reader.GetString("BackBaggageHoldDoor"),
                BulkBaggageHoldDoorC5 = reader.GetString("BulkBaggageHoldDoorC5"),
                ElectricEquipmentAccess = reader.GetString("ElectricEquipmentAccess"),
                GroundServiceCommunication = reader.GetString("GroundServiceCommunication"),
                GroundCommunicationAir = reader.GetString("GroundCommunicationAir"),
                AirExhaustL = reader.GetString("AirExhaustL"),
                AirExhaustR = reader.GetString("AirExhaustR"),

                // Cancellation
                FlightCanceled = reader.GetBoolean("FlightCanceled"),
                CancelledTime = reader.IsDBNull("CancelledTime") ? null : reader.GetTimeSpan("CancelledTime"),
                CancelledParkingGate = reader.GetInt32("CancelledParkingGate"),

                // Misc
                Comments = reader.GetString("Comments"),
                ReportBy = reader.GetInt32("ReportBy")
            };

            reports.Add(report);
        }

        return reports;
    }

    public async Task<bool> SaveReportAsync(ReportModel report)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();

        string query = @"INSERT INTO Reports 
        (FlightNumber, Destination, AircraftRegistration, Date, StartPeriod, EndPeriod, ScheduledTimeArrival, ActualTimeArrival, ScheduledTimeDeparture, ActualTimeDeparture, ArrivalAtGate, Airborne, ParkingGate, FrontDoorAccessPosition, RampAccessPosition, BaggageMakeupPosition, CateringPosition, BackDoorAccessPosition, InboundWheelchairs, OutboundWheelchairs, LiftChairs, Cleaners, Fuelers, IsCatering, PodNumber, PodOffload, PodOnload, LeftFrontSeal, RightFrontSeal, BatterySeal, DryIce, LFwd, LFwdOverwingDoor, LAftOverwingDoor, LAftDoor, RFwdDoor, RFwdOverwingDoor, RAftOverwingDoor, RAftDoor, FrontBaggageHoldDoor, BackBaggageHoldDoor, BulkBaggageHoldDoorC5, ElectricEquipmentAccess, GroundServiceCommunication, GroundCommunicationAir, AirExhaustL, AirExhaustR, FlightCanceled, CancelledTime, CancelledParkingGate, Comments, ReportBy)
        VALUES (@FlightNumber, @Destination, @AircraftRegistration, @Date, @StartPeriod, @EndPeriod, @ScheduledTimeArrival, @ActualTimeArrival, @ScheduledTimeDeparture, @ActualTimeDeparture, @ArrivalAtGate, @Airborne, @ParkingGate, @FrontDoorAccessPosition, @RampAccessPosition, @BaggageMakeupPosition, @CateringPosition, @BackDoorAccessPosition, @InboundWheelchairs, @OutboundWheelchairs, @LiftChairs, @Cleaners, @Fuelers, @IsCatering, @PodNumber, @PodOffload, @PodOnload, @LeftFrontSeal, @RightFrontSeal, @BatterySeal, @DryIce, @LFwd, @LFwdOverwingDoor, @LAftOverwingDoor, @LAftDoor, @RFwdDoor, @RFwdOverwingDoor, @RAftOverwingDoor, @RAftDoor, @FrontBaggageHoldDoor, @BackBaggageHoldDoor, @BulkBaggageHoldDoorC5, @ElectricEquipmentAccess, @GroundServiceCommunication, @GroundCommunicationAir, @AirExhaustL, @AirExhaustR, @FlightCanceled, @CancelledTime, @CancelledParkingGate, @Comments, @ReportBy)";

        using var cmd = new MySqlCommand(query, connection);

        cmd.Parameters.AddWithValue("@FlightNumber", report.FlightNumber);
        cmd.Parameters.AddWithValue("@Destination", report.Destination);
        cmd.Parameters.AddWithValue("@AircraftRegistration", report.AircraftRegistration);
        cmd.Parameters.AddWithValue("@Date", report.Date);
        cmd.Parameters.AddWithValue("@StartPeriod", report.StartPeriod);
        cmd.Parameters.AddWithValue("@EndPeriod", report.EndPeriod);
        cmd.Parameters.AddWithValue("@ScheduledTimeArrival", report.ScheduledTimeArrival);
        cmd.Parameters.AddWithValue("@ActualTimeArrival", report.ActualTimeArrival);
        cmd.Parameters.AddWithValue("@ScheduledTimeDeparture", report.ScheduledTimeDeparture);
        cmd.Parameters.AddWithValue("@ActualTimeDeparture", report.ActualTimeDeparture);
        cmd.Parameters.AddWithValue("@ArrivalAtGate", report.ArrivalAtGate);
        cmd.Parameters.AddWithValue("@Airborne", report.Airborne);
        cmd.Parameters.AddWithValue("@ParkingGate", report.ParkingGate);
        cmd.Parameters.AddWithValue("@FrontDoorAccessPosition", report.FrontDoorAccessPosition);
        cmd.Parameters.AddWithValue("@RampAccessPosition", report.RampAccessPosition);
        cmd.Parameters.AddWithValue("@BaggageMakeupPosition", report.BaggageMakeupPosition);
        cmd.Parameters.AddWithValue("@CateringPosition", report.CateringPosition);
        cmd.Parameters.AddWithValue("@BackDoorAccessPosition", report.BackDoorAccessPosition);
        cmd.Parameters.AddWithValue("@InboundWheelchairs", report.InboundWheelchairs);
        cmd.Parameters.AddWithValue("@OutboundWheelchairs", report.OutboundWheelchairs);
        cmd.Parameters.AddWithValue("@LiftChairs", report.LiftChairs);
        cmd.Parameters.AddWithValue("@Cleaners", report.Cleaners);
        cmd.Parameters.AddWithValue("@Fuelers", report.Fuelers);
        cmd.Parameters.AddWithValue("@IsCatering", report.IsCatering);
        cmd.Parameters.AddWithValue("@PodNumber", report.PodNumber);
        cmd.Parameters.AddWithValue("@PodOffload", report.PodOffload);
        cmd.Parameters.AddWithValue("@PodOnload", report.PodOnload);
        cmd.Parameters.AddWithValue("@LeftFrontSeal", report.LeftFrontSeal);
        cmd.Parameters.AddWithValue("@RightFrontSeal", report.RightFrontSeal);
        cmd.Parameters.AddWithValue("@BatterySeal", report.BatterySeal);
        cmd.Parameters.AddWithValue("@DryIce", report.DryIce);
        cmd.Parameters.AddWithValue("@LFwd", report.LFwd);
        cmd.Parameters.AddWithValue("@LFwdOverwingDoor", report.LFwdOverwingDoor);
        cmd.Parameters.AddWithValue("@LAftOverwingDoor", report.LAftOverwingDoor);
        cmd.Parameters.AddWithValue("@LAftDoor", report.LAftDoor);
        cmd.Parameters.AddWithValue("@RFwdDoor", report.RFwdDoor);
        cmd.Parameters.AddWithValue("@RFwdOverwingDoor", report.RFwdOverwingDoor);
        cmd.Parameters.AddWithValue("@RAftOverwingDoor", report.RAftOverwingDoor);
        cmd.Parameters.AddWithValue("@RAftDoor", report.RAftDoor);
        cmd.Parameters.AddWithValue("@FrontBaggageHoldDoor", report.FrontBaggageHoldDoor);
        cmd.Parameters.AddWithValue("@BackBaggageHoldDoor", report.BackBaggageHoldDoor);
        cmd.Parameters.AddWithValue("@BulkBaggageHoldDoorC5", report.BulkBaggageHoldDoorC5);
        cmd.Parameters.AddWithValue("@ElectricEquipmentAccess", report.ElectricEquipmentAccess);
        cmd.Parameters.AddWithValue("@GroundServiceCommunication", report.GroundServiceCommunication);
        cmd.Parameters.AddWithValue("@GroundCommunicationAir", report.GroundCommunicationAir);
        cmd.Parameters.AddWithValue("@AirExhaustL", report.AirExhaustL);
        cmd.Parameters.AddWithValue("@AirExhaustR", report.AirExhaustR);
        cmd.Parameters.AddWithValue("@FlightCanceled", report.FlightCanceled);
        cmd.Parameters.AddWithValue("@CancelledTime", report.CancelledTime);
        cmd.Parameters.AddWithValue("@CancelledParkingGate", report.CancelledParkingGate);
        cmd.Parameters.AddWithValue("@Comments", report.Comments);
        cmd.Parameters.AddWithValue("@ReportBy", report.ReportBy);

        return await cmd.ExecuteNonQueryAsync() > 0;
    }
}
