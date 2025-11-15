using CandidQVmMulti.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Storage;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;
using Syncfusion.XlsIO;
using IApplication = Syncfusion.XlsIO.IApplication;

namespace CandidQVmMulti.Services;

public class ExportServices
{
    private readonly MySqlEmployeeService employeeService;

    public ExportServices(MySqlEmployeeService employeeService)
    {
        this.employeeService = employeeService;
    }
    public void ExportVouchersGroupedByAirline(List<Voucher> vouchers, Stream outputStream)
{
    using (ExcelEngine excelEngine = new ExcelEngine())
    {
        IApplication application = excelEngine.Excel;
        application.DefaultVersion = ExcelVersion.Excel2016;

        IWorkbook workbook = application.Workbooks.Create(0); // Start with empty workbook

        var grouped = vouchers.GroupBy(v => v.Airline);

        foreach (var group in grouped)
        {
            string sheetName = string.IsNullOrWhiteSpace(group.Key) ? "Unknown Airline" : group.Key;
            sheetName = sheetName.Length > 31 ? sheetName.Substring(0, 31) : sheetName;

            IWorksheet sheet = workbook.Worksheets.Create(sheetName);

            string[] headers = {
                "Date", "Passenger", "Employee",
                "Airline", "Flight No.", "Start Time", "End Time"
            };

            // Add headers
            for (int i = 0; i < headers.Length; i++)
            {
                sheet.Range[1, i + 1].Text = headers[i];
                sheet.Range[1, i + 1].CellStyle.Font.Bold = true;
                sheet.Range[1, i + 1].CellStyle.Color = Syncfusion.Drawing.Color.LightGray;
            }

            int row = 2;
            foreach (var v in group)
            {
                sheet.Range[row, 1].DateTime = new DateTime((long)v.Date);
                sheet.Range[row, 1].NumberFormat = "mmmm dd, yyyy";

                sheet.Range[row, 2].Text = v.PassengerName;
                sheet.Range[row, 3].Text = v.Employee;
                sheet.Range[row, 4].Text = v.Airline;
                sheet.Range[row, 5].Text = v.Flight;

                sheet.Range[row, 6].Text = TimeOnly.FromTimeSpan(TimeSpan.FromTicks(v.StartTime)).ToShortTimeString();
                sheet.Range[row, 6].NumberFormat = "HH:mm:ss";

                sheet.Range[row, 7].Text = TimeOnly.FromTimeSpan(TimeSpan.FromTicks(v.EndTime)).ToShortTimeString();
                sheet.Range[row, 7].NumberFormat = "HH:mm:ss";

                row++;
            }

            sheet.UsedRange.AutofitColumns();
        }

        // Save to stream
        workbook.SaveAs(outputStream);
    }
}

    public async Task ExportVouchersToPdfAsync(List<Voucher> vouchers)
    {
        using var templateStream = await FileSystem.OpenAppPackageFileAsync("Template.docx");
        WordDocument document = new WordDocument(templateStream, FormatType.Docx);

        var dataTable = new System.Data.DataTable("Voucher");
        dataTable.Columns.Add("Airline");
        dataTable.Columns.Add("PassengerName");
        dataTable.Columns.Add("Employee");
        dataTable.Columns.Add("Flight");
        dataTable.Columns.Add("Date");
        dataTable.Columns.Add("StartTime");
        dataTable.Columns.Add("EndTime");
        dataTable.Columns.Add("Terminal");
        dataTable.Columns.Add("Agent");
        dataTable.Columns.Add("Signature"); // Important for image merge

        foreach (var v in vouchers)
        {
            dataTable.Rows.Add(
                v.Airline,
                v.PassengerName,
                v.Employee,
                v.Flight,
                new DateTime((long)v.Date).ToString("MMMM dd, yyyy"),
                TimeOnly.FromTimeSpan(TimeSpan.FromTicks(v.StartTime)).ToShortTimeString(),
                TimeOnly.FromTimeSpan(TimeSpan.FromTicks(v.EndTime)).ToShortTimeString(),
                "NA",
                v.Employee,
                "Signature" // Placeholder
            );

            SaveSignatureToFileAsync(v.Signature, $"{v.PassengerName}_Signature.png");
        }

        // ✅ Hook image merge event BEFORE ExecuteGroup
        document.MailMerge.MergeImageField += (sender, args) =>
        {
            if (args.FieldName == "Signature")
            {
                var voucher = vouchers[args.RowIndex];
                if (voucher.Signature != null && voucher.Signature.Length > 0)
                {
                    // Convert Base64 string to byte array and save as a temp file
                    byte[] imageBytes = Convert.FromBase64String(voucher.Signature);
                    var tempFilePath = Path.GetTempFileName();
                    File.WriteAllBytes(tempFilePath, imageBytes);
                    var imageStream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read);

                    if (imageStream != null)
                    {
                        args.ImageStream = imageStream;
                        WPicture picture = args.Picture;
                        picture.Height = 40;
                        picture.Width = 75;
                    }
                }
            }
        };

        // Perform Mail Merge
        document.MailMerge.ExecuteGroup(dataTable);

        // Convert to PDF
        DocIORenderer renderer = new DocIORenderer();
        PdfDocument pdfDocument = renderer.ConvertToPDF(document);

        using var stream = new MemoryStream();
        pdfDocument.Save(stream);
        stream.Position = 0;

        var result = await FileSaver.Default.SaveAsync("Vouchers.pdf", stream, CancellationToken.None);

        if (result.IsSuccessful)
            await Toast.Make("Exported vouchers to PDF", ToastDuration.Short).Show();
        else
            await Toast.Make($"Failed: {result.Exception.Message}", ToastDuration.Long).Show();

        pdfDocument.Close(true);
        document.Close();

    }


    private void SaveSignatureToFileAsync(string signatureBytes, string filePath)
    {
        byte[] imageBytes = Convert.FromBase64String(signatureBytes);
        File.WriteAllBytes(filePath, imageBytes);
    }

    public async Task SaveSignatureImageAsync(byte[] signatureBytes, string fileName)
    {
        if (signatureBytes == null || signatureBytes.Length == 0)
            return;

        using var ms = new MemoryStream(signatureBytes);
        var result = await FileSaver.Default.SaveAsync(fileName, ms, CancellationToken.None);

        if (result.IsSuccessful)
            await Toast.Make($"Signature saved as {fileName}", ToastDuration.Short).Show();
        else
            await Toast.Make($"Failed: {result.Exception.Message}", ToastDuration.Long).Show();
    }


    private DateTime FromUnix(long unixMillis)
{
    return DateTimeOffset.FromUnixTimeMilliseconds(unixMillis).DateTime;
}
}
