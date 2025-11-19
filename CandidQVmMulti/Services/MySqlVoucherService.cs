using CandidQVmMulti.Enumerators;
using CandidQVmMulti.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace CandidQVmMulti.Services;

public class MySqlVoucherService
{
    private readonly string _connectionString;
    private readonly ApiSettings configuration;
    private NetworkAccess connectivity => Connectivity.Current.NetworkAccess;

    public MySqlVoucherService(IConfiguration config)
    {
        configuration = config.GetRequiredSection("SqlSettings").Get<ApiSettings>();
        _connectionString = $"Server={configuration.Host};Port={configuration.Port};Database={configuration.Name};Uid={configuration.Username};Pwd={configuration.Password};";
    }

    public async Task<List<Voucher>> GetAllVouchersAsync(CancellationToken token = default)
    {
        var vouchers = new List<Voucher>();

        try
        {
            if (connectivity == NetworkAccess.Internet)
            {
                await using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync(token).ConfigureAwait(false);

                var query = @"
                SELECT
                    v.id, v.passenger_name, v.employee_id, v.airline_id, v.flight_id, 
                    v.start_time, v.end_time, v.date, v.status, v.is_selected, v.signature_id,
                    a.name AS airline_name, a.iata AS airline_iata,
                    e.name AS employee_name,
                    f.number AS flight_number,
                    f.terminal_id,  -- Added this
                    s.Image AS signature_blob
                FROM vouchers_tb v
                LEFT JOIN airlines_tb a ON v.airline_id = a.id
                LEFT JOIN guards_tb e ON v.employee_id = e.id
                LEFT JOIN flight_numbers_tb f ON v.flight_id = f.id
                LEFT JOIN signature_tb s ON v.signature_id = s.id;";

                await using var command = new MySqlCommand(query, connection);
                await using var reader = await command.ExecuteReaderAsync(token).ConfigureAwait(false) as MySqlDataReader;

                while (await reader.ReadAsync(token).ConfigureAwait(false))
                {
                    vouchers.Add(new Voucher
                    {
                        Id = reader.GetInt32(0),
                        PassengerName = GetSafeString(reader, 1, "Unknown Passenger"),
                        EmployeeID = GetSafeInt(reader, 2),
                        AirlineID = GetSafeInt(reader, 3),
                        FlightID = GetSafeInt(reader, 4),
                        StartTime = reader.GetInt64(5),
                        EndTime = reader.GetInt64(6),
                        Date = reader.GetInt64(7),
                        Status = (VoucherStatus)reader.GetInt32(8),
                        IsSelected = GetSafeBool(reader, 9),
                        SignatureID = GetSafeInt(reader, 10),
                        Airline = GetSafeString(reader, 11, "Unknown Airline"),
                        Iata = GetSafeString(reader, 12, "--"),
                        Employee = GetSafeString(reader, 13, "Unassigned"),
                        Flight = GetSafeString(reader, 14, "N/A"),
                        TerminalID = GetSafeInt(reader, 15), // Added this
                        Signature = GetSafeString(reader, 16)
                    });
                }
            }
        }
        catch (Exception ex)
        {
            await App.AlertSvc.ShowAlertAsync("Error", ex.Message);
        }

        return vouchers;
    }

    /// <summary>
    /// Safely gets a string value from the reader.
    /// </summary>
    private static string GetSafeString(MySqlDataReader reader, int index, string defaultValue = "")
        => reader.IsDBNull(index) ? defaultValue : reader.GetString(index);

    /// <summary>
    /// Safely gets an int value from the reader.
    /// </summary>
    private static int GetSafeInt(MySqlDataReader reader, int index, int defaultValue = 0)
        => reader.IsDBNull(index) ? defaultValue : reader.GetInt32(index);

    /// <summary>
    /// Safely gets a bool value from the reader.
    /// </summary>
    private static bool GetSafeBool(MySqlDataReader reader, int index, bool defaultValue = false)
        => reader.IsDBNull(index) ? defaultValue : reader.GetBoolean(index);

    /// <summary>
    /// Safely gets a byte[] value from the reader.
    /// </summary>
    private static byte[] GetSafeBytes(MySqlDataReader reader, int index)
        => reader.IsDBNull(index) ? Array.Empty<byte>() : (byte[])reader.GetValue(index);

    public async Task<bool> AddVoucherAsync(Voucher voucher)
    {
        try
        {
            if (connectivity == NetworkAccess.Internet)
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"INSERT INTO vouchers_tb 
                              (passenger_name, employee_id, airline_id, flight_id, start_time, end_time, date, status, is_selected, signature_id) 
                              VALUES (@PassengerName, @EmployeeID, @AirlineID, @FlightID, @StartTime, @EndTime, @Date, @Status, @IsSelected, @SignatureID)";

                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@PassengerName", voucher.PassengerName);
                command.Parameters.AddWithValue("@EmployeeID", voucher.EmployeeID);
                command.Parameters.AddWithValue("@AirlineID", voucher.AirlineID);
                command.Parameters.AddWithValue("@FlightID", voucher.FlightID);
                command.Parameters.AddWithValue("@StartTime", voucher.StartTime);
                command.Parameters.AddWithValue("@EndTime", voucher.EndTime);
                command.Parameters.AddWithValue("@Date", voucher.Date);
                command.Parameters.AddWithValue("@Status", voucher.Status);
                command.Parameters.AddWithValue("@IsSelected", voucher.IsSelected);
                command.Parameters.AddWithValue("@SignatureID", voucher.SignatureID);

                var result = await command.ExecuteNonQueryAsync() > 0;
                voucher.Id = (int)command.LastInsertedId;

                await Shell.Current.GoToAsync("..");

                return result;
            }
        }
        catch (Exception ex)
        {
            await App.AlertSvc.ShowAlertAsync("Error", ex.Message);
        }

        return false;
    }

    public async Task UpdateVoucherAsync(Voucher voucher)
    {
        try
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
                              status = @Status,
                              signature_id = @SignatureID
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
                //command.Parameters.AddWithValue("@IsSelected", voucher.IsSelected);
                command.Parameters.AddWithValue("@SignatureID", voucher.SignatureID);

                await command.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            await App.AlertSvc.ShowAlertAsync("Error", ex.Message);
        }
    }

    public async Task DeleteVoucherAsync(int voucherId)
    {
        try
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
        catch (Exception ex)
        {
            await App.AlertSvc.ShowAlertAsync("Error", ex.Message);
        }
    }

    public async Task<int> GetCountAsync()
    {
        try
        {
            if (connectivity == NetworkAccess.Internet)
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = new MySqlCommand("SELECT COUNT(*) FROM vouchers_tb", connection);
                var result = await command.ExecuteScalarAsync();

                return Convert.ToInt32(result);
            }
        }
        catch (Exception ex)
        {
            await App.AlertSvc.ShowAlertAsync("Error", ex.Message);
        }

        return 0;
    }

    public async Task<List<Voucher>> GetTodayVouchersAsync()
    {
        var vouchers = new List<Voucher>();

        try
        {
            if (connectivity == NetworkAccess.Internet)
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        v.id, v.passenger_name, v.employee_id, v.airline_id, v.flight_id, 
                        v.start_time, v.end_time, v.date, v.status, v.is_selected, v.signature_id,
                        a.name AS airline_name, a.iata AS airline_iata,
                        e.name AS employee_name,
                        f.number AS flight_number
                    FROM vouchers_tb v
                    LEFT JOIN airlines_tb a ON v.airline_id = a.id
                    LEFT JOIN guards_tb e ON v.employee_id = e.id
                    LEFT JOIN flight_numbers_tb f ON v.flight_id = f.id
                    WHERE DATE(FROM_UNIXTIME(v.date)) = CURDATE()";

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
                        Status = (VoucherStatus)reader.GetInt32(8),
                        IsSelected = reader.IsDBNull(9) ? false : reader.GetBoolean(9),
                        SignatureID = reader.IsDBNull(10) ? 0 : reader.GetInt32(10),
                        Airline = reader.IsDBNull(11) ? "Unknown Airline" : reader.GetString(11),
                        Iata = reader.IsDBNull(12) ? "--" : reader.GetString(12),
                        Employee = reader.IsDBNull(13) ? "Unassigned" : reader.GetString(13),
                        Flight = reader.IsDBNull(14) ? "N/A" : reader.GetString(14),
                    });
                }
            }
        }
        catch (Exception ex)
        {
            await App.AlertSvc.ShowAlertAsync("Error", ex.Message);
        }

        return vouchers;
    }

    public async Task<int> GetTodaysAssistanceCountAsync()
    {
        try
        {
            if (connectivity == NetworkAccess.Internet)
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();


                long todayStartTicks = DateTime.Today.Ticks;
                long tomorrowStartTicks = DateTime.Today.AddDays(1).Ticks;


                var query = "SELECT COUNT(*) FROM vouchers_tb WHERE date >= @startTicks AND date < @endTicks;";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@startTicks", todayStartTicks);
                command.Parameters.AddWithValue("@endTicks", tomorrowStartTicks);

                int count = Convert.ToInt32(await command.ExecuteScalarAsync());

                return count;
            }
        }
        catch (Exception ex)
        {
            await App.AlertSvc.ShowAlertAsync("Error", ex.Message);
        }

        return 0;
    }

    public async Task<int> AddSignatureAsync(string base64)
    {
        try
        {
            if (connectivity == NetworkAccess.Internet)
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = @"INSERT INTO signature_tb (image) VALUES (@Image)";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.Add("@Image", MySqlDbType.String).Value = base64;

                var result = await command.ExecuteNonQueryAsync();

                if (result > 0)
                {
                    return (int)command.LastInsertedId;
                }
            }
        }
        catch (Exception ex)
        {
            await App.AlertSvc.ShowAlertAsync("Error", ex.Message);
        }

        return 0;
    }

    internal async Task<int> GetUnsignedVouchersCountAsync()
    {
        try
        {
            if (connectivity == NetworkAccess.Internet)
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();


                long todayStartTicks = DateTime.Today.Ticks;
                long tomorrowStartTicks = DateTime.Today.AddDays(1).Ticks;


                var query = "SELECT COUNT(*) FROM vouchers_tb WHERE signature_id = 0";
                using var command = new MySqlCommand(query, connection);
                int count = Convert.ToInt32(await command.ExecuteScalarAsync());

                return count;
            }
        }
        catch (Exception ex)
        {
            await App.AlertSvc.ShowAlertAsync("Error", ex.Message);
        }
        return 0;
    }
}