using CandidQVmMulti.Models;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Storage;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.XlsIO;
using HorizontalAlignment = Syncfusion.DocIO.DLS.HorizontalAlignment;
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
                        picture.Height = 20;
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

public async Task ExportVouchersToPdfAsync(List<Voucher> vouchers, int vouchersPerPage, string logoFilePath)
{
    // Create a new Word document
    WordDocument document = new WordDocument();
    IWSection section = document.AddSection();

        //Styles
        document.AddParagraphStyle("14pointText").CharacterFormat.FontSize = 14;
        document.AddParagraphStyle("8pointText").CharacterFormat.FontSize = 8;

        // Setting document page margins.
        MarginsF pagemargins = new MarginsF();
    pagemargins.Bottom = 10;
    pagemargins.Top = 10;
    pagemargins.Left = 10;
    pagemargins.Right = 10;
    // Assigning document page margins to the current section.
    section.PageSetup.Margins = pagemargins;

        // Load logo from file
        FileStream logoStream = null;
    if (!string.IsNullOrEmpty(logoFilePath) && File.Exists(logoFilePath))
    {
        logoStream = new FileStream(logoFilePath, FileMode.Open, FileAccess.Read);
    }

        int count = 0;

    foreach (var v in vouchers)
        {
            // Create a table for the voucher
            IWTable table = section.AddTable();
            table.TableFormat.Borders.BorderType = BorderStyle.Single;

            CreateHeaderRow(logoStream, table);
            AddEmptyRow(table,$"No.: {v.Id:00000}");
            CreateRow(table, "Date:", 52.800003f, $"{new DateTime(v.Date):MMMM dd, yyyy}", "Terminal:", 59.5f, v.Terminal);
            CreateRow(table, "Airline:", 52.800003f, v.Airline, "Flight #:", 59.5f, v.FullFlightNumber);
            CreateSignatureRow(table, "Airline Agent Name:", 125.4f, v.Signature, "Officer Name:", 85.8f, v.Employee);
            CreateRow(table, "Passenger Name:", 99, v.PassengerName, string.Empty, 1, string.Empty);
            CreateRow(table, "Time passenger enters chair:", 148.8f, TimeOnly.FromTimeSpan(TimeSpan.FromTicks(v.StartTime)).ToShortTimeString(), "Time passenger exits chair:", 148.8f, TimeOnly.FromTimeSpan(TimeSpan.FromTicks(v.EndTime)).ToShortTimeString());
            AddEmptyRow(table);
            AddEmptyRow(table, string.Empty, BorderStyle.None, BorderStyle.Single, BorderStyle.None, BorderStyle.None);

            count++;
            if(count >= 3)
            {
                // Insert a page break
                IWParagraph pageBreakParagraph = section.AddParagraph();
                pageBreakParagraph.AppendBreak(BreakType.PageBreak);
                count = 1;
            }

        }


        // Convert to PDF
        DocIORenderer renderer = new DocIORenderer();
    PdfDocument pdfDocument = renderer.ConvertToPDF(document);

    using var stream = new MemoryStream();
    pdfDocument.Save(stream);
    stream.Position = 0;

    var result = await FileSaver.Default.SaveAsync("Vouchers.pdf", stream, CancellationToken.None);

    pdfDocument.Close(true);
    document.Close();
}

    private static void AddEmptyRow(IWTable table,string lastCellText = "", BorderStyle left = BorderStyle.None, BorderStyle top = BorderStyle.None, BorderStyle right = BorderStyle.None, BorderStyle bottom = BorderStyle.None)
    {
        WTableRow row = table.AddRow();
        var text = row.Cells[row.Cells.Count - 1].AddParagraph().AppendText(lastCellText);
        text.CharacterFormat.Bold = true;
        text.CharacterFormat.TextColor = Syncfusion.Drawing.Color.Red;

        RemoveBorders(row,left,top,right,bottom);
    }

    private static void CreateHeaderRow(FileStream logoStream, IWTable table)
    {
        //Adds the first row into table
        WTableRow row = table.AddRow();
        //Adds the first cell into first row 
        WTableCell cell = row.AddCell();

        if (logoStream != null)
        {
            IWPicture leftLogo = cell.AddParagraph().AppendPicture(logoStream);
            leftLogo.Height = 80;
            leftLogo.Width = 80;
        }
        //Specifies the cell width
        cell.Width = 90;

        //Adds the second cell into first row 
        WTableCell cell1 = row.AddCell();
        //Specifies the cell width
        cell1.Width = 395.3f;

        IWParagraph headerPara = cell1.AddParagraph();
        var headerText = headerPara.AppendText("CANDID SECURITY LTD.");
        headerPara.ApplyStyle(BuiltinStyle.Heading1);
        headerPara.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
        headerText.CharacterFormat.UnderlineStyle = UnderlineStyle.Single;

        IWParagraph headerPara1 = cell1.AddParagraph();
        headerPara1.AppendText("WHEELCHAIR VOUCHER");
        headerPara1.ApplyStyle("14pointText");
        headerPara1.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;

        IWParagraph headerPara2 = cell1.AddParagraph();
        headerPara2.AppendText("Suite #8 FML Plaza, Carmichael Road, Nassau, Bahamas \r\nTelephone: (242) 361-5499 (242) 361-5497 (242) 225-8384 (USA) 305-224-1809\r\nFax: (242) 361-5490\r\n");
        headerPara2.ApplyStyle("8pointText");
        headerPara2.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;

        //Adds the third cell into first row 
        WTableCell cell2 = row.AddCell();
        //Specifies the cell width
        cell2.Width = 90;
        if (logoStream != null)
        {
            IWPicture leftLogo = cell2.AddParagraph().AppendPicture(logoStream);
            leftLogo.Height = 80;
            leftLogo.Width = 80;
        }

        RemoveBorders(row);
    }

    private static void RemoveBorders(WTableRow row, BorderStyle left = BorderStyle.None, BorderStyle top = BorderStyle.None, BorderStyle right = BorderStyle.None, BorderStyle bottom = BorderStyle.None)
    {
        foreach (WTableCell cell in row.Cells)
        {
            cell.CellFormat.Borders.Left.BorderType = left;
            cell.CellFormat.Borders.Top.BorderType = top;
            cell.CellFormat.Borders.Right.BorderType = right;
            cell.CellFormat.Borders.Bottom.BorderType = bottom;
        }
    }

    private static void CreateRow(IWTable table, string cell1Text,float cell1Size, string cell2Text, string cell3Text,float cell3Size, string cell4Text)
    {
        //Adds the first row into table
        WTableRow row = table.AddRow();
        row.Cells.Clear();
        
        //Adds the first cell into first row 
        WTableCell cell1 = row.AddCell();
        cell1.CellFormat.Borders.BorderType = BorderStyle.None;
        WTableCell cell2 = row.AddCell();
        cell2.CellFormat.Borders.BorderType = BorderStyle.None;

        if(!string.IsNullOrWhiteSpace(cell2Text))
        {
            cell2.CellFormat.Borders.Bottom.BorderType = BorderStyle.Single;
        }
        WTableCell cell3 = row.AddCell();
        cell3.CellFormat.Borders.BorderType = BorderStyle.None;
        WTableCell cell4 = row.AddCell();
        cell4.CellFormat.Borders.BorderType = BorderStyle.None;

        if (!string.IsNullOrWhiteSpace(cell4Text))
        {
            cell4.CellFormat.Borders.Bottom.BorderType = BorderStyle.Single;
        }

        float tableSize = 575.3f;

        var cell1TextRange = cell1.AddParagraph().AppendText(cell1Text);       
        cell2.AddParagraph().AppendText(cell2Text);
        var cell3TextRange = cell3.AddParagraph().AppendText(cell3Text);
        cell4.AddParagraph().AppendText(cell4Text);

        var labelSum = cell1Size + cell3Size;
        var remainder = (tableSize - labelSum) / 2;

        //Specifies the cell width
        cell1.Width = cell1Size;
        cell3.Width = cell3Size;
        cell2.Width = remainder;
        cell4.Width = remainder;
    }

    private static void CreateSignatureRow(IWTable table, string cell1Text, float cell1Size, string cell2Text, string cell3Text, float cell3Size, string cell4Text)
    {
        if (!string.IsNullOrEmpty(cell2Text))
        {
            //Adds the first row into table
            WTableRow row = table.AddRow();
            row.Cells.Clear();

            //Adds the first cell into first row 
            WTableCell cell1 = row.AddCell();
            WTableCell cell2 = row.AddCell();
            WTableCell cell3 = row.AddCell();
            WTableCell cell4 = row.AddCell();

            float tableSize = 575.3f;

            var cell1TextRange = cell1.AddParagraph().AppendText(cell1Text);
            
            byte[] imageBytes = Convert.FromBase64String(cell2Text);
            MemoryStream imageStream = new MemoryStream(imageBytes);
            IWPicture signaturePic = cell2.AddParagraph().AppendPicture(imageStream);
            signaturePic.Height = 20;
            signaturePic.Width = 75;
           
            var cell3TextRange = cell3.AddParagraph().AppendText(cell3Text);
            cell4.AddParagraph().AppendText(cell4Text);

            var labelSum = cell1Size + cell3Size;
            var remainder = (tableSize - labelSum) / 2;

            //Specifies the cell width
            cell1.Width = cell1Size;
            cell3.Width = cell3Size;
            cell2.Width = remainder;
            cell4.Width = remainder;
        }
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
