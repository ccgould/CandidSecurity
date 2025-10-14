using CandidQVmMulti.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace CandidQVmMulti.Services;

public class MySqlVoucherService
{
    private readonly string _connectionString;
    private readonly ApiSettings configuration;
    private readonly NetworkAccess connectivity = Connectivity.Current.NetworkAccess;

    public MySqlVoucherService(IConfiguration config)
    {
        var configuration = config.GetRequiredSection("SqlSettings").Get<ApiSettings>();
        _connectionString = $"Server={configuration.Host};Port={configuration.Port};Database={configuration.Name};Uid={configuration.Username};Pwd={configuration.Password};";
        this.configuration = configuration;
    }

    public async Task<List<Voucher>> GetAllVouchersAsync()
    {
        try
        {
            if (connectivity == NetworkAccess.Internet)
            {
                var vouchers = new List<Voucher>();

                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
        SELECT 
            v.id, v.passenger_name, v.employee_id, v.airline_id, v.flight_id, 
            v.start_time, v.end_time, v.date, v.status,
            a.name AS airline_name, a.iata AS airline_iata,
            e.name AS employee_name,
            f.number AS flight_number
        FROM vouchers_tb v
        LEFT JOIN airlines_tb a ON v.airline_id = a.id
        LEFT JOIN guards_tb e ON v.employee_id = e.id
        LEFT JOIN flight_numbers_tb f ON v.flight_id = f.id";

                using var command = new MySqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    vouchers.Add(new Voucher
                    {
                        Id = reader.GetInt32(0),
                        PassengerName = reader.GetString(1),
                        EmployeeID = reader.GetInt32(2),
                        AirlineID = reader.GetInt32(3),
                        FlightID = reader.GetInt32(4),
                        StartTime = reader.GetInt64(5),
                        EndTime = reader.GetInt64(6),
                        Date = reader.GetInt64(7),
                        Status = reader.GetInt32(8),
                        Airline = reader.IsDBNull(9) ? "Unknown Airline" : reader.GetString(9),
                        Iata = reader.IsDBNull(10) ? "--" : reader.GetString(10),
                        Employee = reader.IsDBNull(11) ? "Unassigned" : reader.GetString(11),
                        Flight = reader.IsDBNull(12) ? "N/A" : reader.GetString(12)
                    });
                }

                return vouchers;
            }
        }
        catch (Exception ex)
        {
            await App.AlertSvc.ShowAlertAsync("Error", ex.Message);
        }

        return new();
    }
    public async Task<bool> AddVoucherAsync(Voucher voucher)
    {
        if (connectivity == NetworkAccess.Internet)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"INSERT INTO vouchers_tb 
                      (passenger_name, employee_id, airline_id, flight_id, start_time, end_time, date, status) 
                      VALUES (@PassengerName, @EmployeeID, @AirlineID, @FlightID, @StartTime, @EndTime, @Date, @Status)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@PassengerName", voucher.PassengerName);
            command.Parameters.AddWithValue("@EmployeeID", voucher.EmployeeID);
            command.Parameters.AddWithValue("@AirlineID", voucher.AirlineID);
            command.Parameters.AddWithValue("@FlightID", voucher.FlightID);
            command.Parameters.AddWithValue("@StartTime", voucher.StartTime);
            command.Parameters.AddWithValue("@EndTime", voucher.EndTime);
            command.Parameters.AddWithValue("@Date", voucher.Date);
            command.Parameters.AddWithValue("@Status", voucher.Status);

            var result = await command.ExecuteNonQueryAsync() > 0;

            voucher.Id = (int)command.LastInsertedId;

            await Shell.Current.GoToAsync("..");

            return result;
        }
        return false;
    }

    public async Task UpdateVoucherAsync(Voucher voucher)
    {
        if (connectivity == NetworkAccess.Internet)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"UPDATE vouchers_tb SET 
                      passenger_name = @PassengerName, 
                      employee_id = @EmployeeID, 
                      airline_id = @AirlineID, 
                      flight_id = @FlightID, 
                      start_time = @StartTime, 
                      end_time = @EndTime, 
                      date = @Date, 
                      status = @Status
                      WHERE id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", voucher.Id);
            command.Parameters.AddWithValue("@PassengerName", voucher.PassengerName);
            command.Parameters.AddWithValue("@EmployeeID", voucher.EmployeeID);
            command.Parameters.AddWithValue("@AirlineID", voucher.AirlineID);
            command.Parameters.AddWithValue("@FlightID", voucher.FlightID);
            command.Parameters.AddWithValue("@StartTime", voucher.StartTime);
            command.Parameters.AddWithValue("@EndTime", voucher.EndTime);
            command.Parameters.AddWithValue("@Date", voucher.Date);
            command.Parameters.AddWithValue("@Status", voucher.Status);

            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task DeleteVoucherAsync(int voucherId)
    {
        if (connectivity == NetworkAccess.Internet)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "DELETE FROM vouchers_tb WHERE id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", voucherId);

            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task<int> GetCountAsync()
    {
        if (connectivity == NetworkAccess.Internet)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand("SELECT COUNT(*) FROM vouchers_tb", connection);
            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result);
        }

        return 0;
    }

    public async Task<List<Voucher>> GetTodayVouchersAsync()
    {
        var vouchers = new List<Voucher>();

        if (connectivity == NetworkAccess.Internet)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
        SELECT 
            v.id, v.passenger_name, v.employee_id, v.airline_id, v.flight_id, 
            v.start_time, v.end_time, v.date, v.status,
            a.name AS airline_name, a.iata AS airline_iata,
            e.name AS employee_name,
            f.number AS flight_number
        FROM vouchers_tb v
        LEFT JOIN airlines_tb a ON v.airline_id = a.id
        LEFT JOIN guards_tb e ON v.employee_id = e.id
        LEFT JOIN flight_numbers_tb f ON v.flight_id = f.id
        WHERE DATE(v.date) = CURDATE()";

            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                vouchers.Add(new Voucher
                {
                    Id = reader.GetInt32(0),
                    PassengerName = reader.GetString(1),
                    EmployeeID = reader.GetInt32(2),
                    AirlineID = reader.GetInt32(3),
                    FlightID = reader.GetInt32(4),
                    StartTime = reader.GetInt64(5),
                    EndTime = reader.GetInt64(6),
                    Date = reader.GetInt64(7),
                    Status = reader.GetInt32(8),
                    Airline = reader.IsDBNull(9) ? "Unknown Airline" : reader.GetString(9),
                    Iata = reader.IsDBNull(10) ? "--" : reader.GetString(10),
                    Employee = reader.IsDBNull(11) ? "Unassigned" : reader.GetString(11),
                    Flight = reader.IsDBNull(12) ? "N/A" : reader.GetString(12)
                });
            }
        }

        return vouchers;
    }

    public async Task<int> GetTodayVoucherCountAsync()
    {
        if (connectivity == NetworkAccess.Internet)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT COUNT(*) FROM vouchers_tb WHERE DATE(date) = CURDATE()";

            using var command = new MySqlCommand(query, connection);
            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result);
        }

        return 0;
    }

}