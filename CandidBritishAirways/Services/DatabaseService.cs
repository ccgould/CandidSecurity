using CandidBritishAirways.Models;
using SQLite;

namespace CandidBritishAirways.Services;
public class DatabaseService
{
    private readonly SQLiteAsyncConnection _db;
    private readonly MySqlEmployeeService mySqlEmployeeService;

    public DatabaseService(MySqlEmployeeService mySqlEmployeeService)
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
        _db = new SQLiteAsyncConnection(dbPath);
        _db.CreateTableAsync<ReportModel>().Wait();
        this.mySqlEmployeeService = mySqlEmployeeService;
    }

    public Task<List<ReportModel>> GetReportsAsync() =>
        _db.Table<ReportModel>().ToListAsync();

    public async Task<List<ReportModel>> GetReportsWithNamesAsync()
    {
        var reports = await _db.Table<ReportModel>().ToListAsync();
        var employees = await mySqlEmployeeService.GetAllEmployeesAsync();

        // Map ReportBy to Employee Name
        foreach (var report in reports)
        {
            var employee = employees.FirstOrDefault(e => e.Id == report.ReportBy);
            report.ReportByName = employee?.Name ?? "Unknown";
        }

        return reports;
    }

    public Task<ReportModel> GetReportByIdAsync(int id) =>
        _db.Table<ReportModel>().Where(r => r.Id == id).FirstOrDefaultAsync();

    public Task<int> SaveReportAsync(ReportModel report) =>
        report.Id != 0 ? _db.UpdateAsync(report) : _db.InsertAsync(report);

    public Task<int> DeleteReportAsync(ReportModel report) =>
        _db.DeleteAsync(report);

    // ✅ Search by Flight Number
    public Task<List<ReportModel>> SearchByFlightNumberAsync(int flightNumber) =>
        _db.Table<ReportModel>().Where(r => r.FlightNumber == flightNumber).ToListAsync();

    // ✅ Filter by Date Range
    public Task<List<ReportModel>> FilterByDateRangeAsync(DateTime start, DateTime end) =>
        _db.Table<ReportModel>().Where(r => r.Date >= start && r.Date <= end).ToListAsync();

    // ✅ Filter by Destination
    public Task<List<ReportModel>> FilterByDestinationAsync(int destination) =>
        _db.Table<ReportModel>().Where(r => r.Destination == destination).ToListAsync();

    // ✅ Combined Search
    public Task<List<ReportModel>> SearchFlightAndDateAsync(int flightNumber, DateTime start, DateTime end) =>
        _db.Table<ReportModel>().Where(r => r.FlightNumber == flightNumber && r.Date >= start && r.Date <= end).ToListAsync();
}
