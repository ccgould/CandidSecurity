using CandidQVmMulti.Models;
using Google.Protobuf.WellKnownTypes;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidQVmMulti.Services;

public class ExportServices
{
    public void ExportVouchersGroupedByAirline(List<Voucher> vouchers, string filePath)
{
    Workbook workbook = new Workbook();

    var grouped = vouchers.GroupBy(v => v.Airline);

        workbook.Worksheets.Clear();


        foreach (var group in grouped)
    {
        string sheetName = string.IsNullOrWhiteSpace(group.Key) ? "Unknown Airline" : group.Key;
        Worksheet sheet = workbook.Worksheets.Add(sheetName.Length > 31 ? sheetName.Substring(0, 31) : sheetName);

        string[] headers = {
            "Date", "Passenger", "Employee",
            "Airline", "Flight No.", "Start Time", "End Time"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            sheet.Range[1, i + 1].Text = headers[i];
            sheet.Range[1, i + 1].Style.Font.IsBold = true;
            sheet.Range[1, i + 1].Style.Color = System.Drawing.Color.LightGray;
        }

        int row = 2;
        foreach (var v in group)
        {
            sheet.Range[row, 1].DateTimeValue = new DateTime((long)v.Date);
            sheet.Range[row, 1].Style.NumberFormat = "mmmm dd, yyyy";
            sheet.Range[row, 2].Text = v.PassengerName;
            sheet.Range[row, 3].Text = v.Employee;
            sheet.Range[row, 4].Text = v.Airline;
            sheet.Range[row, 5].Text = v.Flight;
            sheet.Range[row, 6].Text = TimeOnly.FromTimeSpan(TimeSpan.FromTicks(v.StartTime)).ToShortTimeString();
            sheet.Range[row, 6].Style.NumberFormat = "HH:mm:ss";
            sheet.Range[row, 7].Text = TimeOnly.FromTimeSpan(TimeSpan.FromTicks(v.EndTime)).ToShortTimeString();
            sheet.Range[row, 7].Style.NumberFormat = "HH:mm:ss";
            row++;
        }

        sheet.AllocatedRange.AutoFitColumns();
    }

    workbook.SaveToFile(filePath, ExcelVersion.Version2013);
}

    private DateTime FromUnix(long unixMillis)
{
    return DateTimeOffset.FromUnixTimeMilliseconds(unixMillis).DateTime;
}
}
