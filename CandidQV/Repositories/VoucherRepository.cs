using CandidQV.Models.Items;
using SQLite;
using System.Collections.ObjectModel;
using System.Text;

namespace CandidQV.Repositories;
public class VoucherRepository
{
    private const string DB_NAME = "candidDB.db3";
    private readonly EmployeeRepository employeeRepository;
    private readonly FlightNumberRepository flightNumberRepository;
    private readonly AirlineRepository airlineRepository;
    private SQLiteAsyncConnection _connection;

    public VoucherRepository(EmployeeRepository employeeRepository,FlightNumberRepository flightNumberRepository,AirlineRepository airlineRepository)
    {
        _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
        this.employeeRepository = employeeRepository;
        this.flightNumberRepository = flightNumberRepository;
        this.airlineRepository = airlineRepository;
    }

    public async Task<List<Voucher>> GetVouchersAsync()
    {
        await InitAsync();
        var vouchers = await _connection.Table<Voucher>().ToListAsync();
        var airlines = await _connection.Table<Airline>().ToListAsync();
        var flightNumbers = await _connection.Table<FlightNumber>().ToListAsync();

        var airlineMap = airlines.ToDictionary(a => a.Id);
        var flightMap = flightNumbers.ToDictionary(f => f.Id);

        foreach (var voucher in vouchers)
        {
            if (airlineMap.TryGetValue(voucher.AirlineId, out var airline))
                voucher.Airline = airline;

            if (flightMap.TryGetValue(voucher.FlightNumberId, out var flight))
                voucher.FlightNumber = flight;
        }

        return vouchers;
    }


    public async Task<List<Voucher>> GetFilteredVouchersAsync(string name, DateTime date)
    {

        await InitAsync();
        var dateS = DateOnly.FromDateTime(date.Date).ToString("ddd, dd MMM yyyy");
        var g = await GetVouchersAsync();
        
        //var query = await _connection.Table<Voucher>().Where(p => p.DateString.Equals(dateS));
        
        var query = g.Where(p => !string.IsNullOrWhiteSpace(p.DateString) && p.DateString.Equals(dateS));
        
        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(p => p.PassengerName.Contains(name));
        }

        return query.ToList();

        //return await GetVouchers();
    }


    public async Task<Voucher> GetByIdAsync(int id)
    {
        await InitAsync();
        return await _connection.Table<Voucher>().Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Voucher Voucher)
    {
        try
        {
            await InitAsync();
            await _connection.InsertAsync(Voucher);
        }
        catch(Exception ex)
        {
           await App.AlertSvc.ShowAlertAsync("Error",ex.Message);
        }
        finally
        {
            await _connection.CloseAsync();
        }
    }

    private async Task InitAsync()
    {
        await _connection.CreateTableAsync<Voucher>();
    }

    public async Task UpdateAsync(Voucher Voucher)
    {
        await InitAsync();
        await _connection.UpdateAsync(Voucher);
    }

    public async Task DeleteAsync(Voucher Voucher)
    {
        await InitAsync();
        await _connection.DeleteAsync(Voucher);
    }

    internal async Task ExportSelectedAsync(ObservableCollection<object> list)
    {
        var sb = new StringBuilder();

        foreach (Voucher voucher in list)
        {
            var airline = await airlineRepository.GetById(voucher?.AirlineId ?? 0);
            var flightNumber = await flightNumberRepository.GetByIdAsync(voucher?.FlightNumberId ?? 0);
            var employee = await employeeRepository.GetByIdAsync(voucher?.EmployeeID ?? 0);
            var terminal = voucher.IsUsDeparture ? TerminalType.US : TerminalType.Domestic;

            sb.Append($"Date: {voucher.Date.ToLongDateString()}");
            sb.Append(Environment.NewLine);
            sb.Append($"Passenger Name: {voucher?.PassengerName ?? "N/A"}");
            sb.Append(Environment.NewLine);
            sb.Append($"Flight: {airline?.IataCode ?? "N/A"} - {flightNumber?.Number ?? "N/A"}");
            sb.Append(Environment.NewLine);
            sb.Append($"Time: {voucher?.StartTime ?? "N/A"} - {voucher?.EndTime ?? "N/A"}");
            sb.Append(Environment.NewLine);
            sb.Append($"Guard: {employee?.FullName ?? "N/A"}");
            sb.Append(Environment.NewLine);
            sb.Append($"Terminal: {terminal}");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
        }

        // Ensure the operation is on the main UI thread
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Clipboard.Default.SetTextAsync(sb.ToString());
        });

        await MarkAsSentAsync(list);

        //Add notification
        await AppShell.DisplayToastAsync("Exported Successfully to Clipboard");
    }

    private async Task MarkAsSentAsync(ObservableCollection<object> list)
    {
        foreach (Voucher voucher in list)
        {
            voucher.IsSent = true;
            await UpdateAsync(voucher);
        }
    }

}