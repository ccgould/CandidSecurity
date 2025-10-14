using CandidQVmMulti.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace CandidQVmMulti.Services;

public class MySqlAirlinesService
{
    private readonly string _connectionString;
    private readonly ApiSettings configuration;
    private readonly NetworkAccess connectivity = Connectivity.Current.NetworkAccess;

    public MySqlAirlinesService(IConfiguration config)
    {
        configuration = config.GetRequiredSection("SqlSettings").Get<ApiSettings>();
        _connectionString = $"Server={configuration.Host};Port={configuration.Port};Database={configuration.Name};Uid={configuration.Username};Pwd={configuration.Password};";
    }

    public async Task<List<Airline>> GetAllAsync()
    {
        try
        {
            if(connectivity == NetworkAccess.Internet)
            {
                var airlines = new List<Airline>();
                var flightNumbers = new List<FlightNumber>();

                await using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                // Step 1: Load Airlines
                var airlineCmd = new MySqlCommand("SELECT * FROM airlines_tb", connection);
                await using var airlineReader = await airlineCmd.ExecuteReaderAsync();
                while (await airlineReader.ReadAsync())
                {
                    airlines.Add(new Airline
                    {
                        Id = airlineReader.GetInt32("id"),
                        Name = airlineReader.GetString("name"),
                        Iata = airlineReader.GetString("iata"),
                        AddDate = airlineReader.GetInt64("add_date"),
                        Terminal = airlineReader.GetInt32("terminal"),
                        FlightNumbers = new() // Initialize
                    });
                }
                airlineReader.Close();

                // Step 2: Load FlightNumbers
                var flightCmd = new MySqlCommand("SELECT * FROM flight_numbers_tb", connection);
                await using var flightReader = await flightCmd.ExecuteReaderAsync();
                while (await flightReader.ReadAsync())
                {
                    flightNumbers.Add(new FlightNumber
                    {
                        Id = flightReader.GetInt32("id"),
                        AirlineId = flightReader.GetInt32("airline_id"),
                        TerminalId = flightReader.GetInt32("terminal_id"),
                        Number = flightReader.GetString("number"),
                        AddedDate = flightReader.GetInt64("addDate")
                    });
                }

                // Step 3: Map FlightNumbers to Airlines
                var airlineMap = airlines.ToDictionary(a => a.Id);
                foreach (var flight in flightNumbers)
                {
                    if (airlineMap.TryGetValue(flight.AirlineId, out var airline))
                    {
                        airline.FlightNumbers.Add(flight);
                    }
                }

                return airlines;
            }
        }
        catch (Exception ex)
        {
            await App.AlertSvc.ShowAlertAsync("Error", ex.Message);
        }
        return null;
    }

    public async Task<Airline> GetByIdAsync(int id)
    {
        if (connectivity == NetworkAccess.Internet)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new MySqlCommand("SELECT * FROM airlines_tb WHERE id = @id", connection);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Airline
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    Iata = reader.GetString("iata"),
                    AddDate = reader.GetInt64("add_date"),
                    Terminal = reader.GetInt32("Terminal"),
                };
            }
        }

        return new();
    }

    public async Task<bool> AddAsync(Airline item)
    {
        if (connectivity == NetworkAccess.Internet)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new MySqlCommand(@"
            INSERT INTO airlines_tb (name, iata, add_date, terminal)
            VALUES (@name, @iata, @add_date, @terminal)", connection);

            cmd.Parameters.AddWithValue("@name", item.Name);
            cmd.Parameters.AddWithValue("@iata", item.Iata);
            cmd.Parameters.AddWithValue("@add_date", item.AddDate);
            cmd.Parameters.AddWithValue("@terminal", item.Terminal);

            var result = await cmd.ExecuteNonQueryAsync() > 0;

            item.Id = (int)cmd.LastInsertedId;

            return result;
        }

        return false;
    }

    public async Task<bool> UpdateAsync(Airline item)
    {
        if (connectivity == NetworkAccess.Internet)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new MySqlCommand(@"
            UPDATE airlines_tb SET 
                name = @name, 
                iata = @iata, 
                add_date = @add_date,
                terminal = @terminal
            WHERE id = @id", connection);

            cmd.Parameters.AddWithValue("@name", item.Name);
            cmd.Parameters.AddWithValue("@iata", item.Iata);
            cmd.Parameters.AddWithValue("@add_date", item.AddDate);
            cmd.Parameters.AddWithValue("@id", item.Id);
            cmd.Parameters.AddWithValue("@terminal", item.Terminal);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        return false;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        if (connectivity == NetworkAccess.Internet)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new MySqlCommand("DELETE FROM airlines_tb WHERE id = @id", connection);
            cmd.Parameters.AddWithValue("@id", id);

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        return false;
    }

    public async Task<int> GetCountAsync()
    {
        if (connectivity == NetworkAccess.Internet)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand("SELECT COUNT(*) FROM airlines_tb", connection);
            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result);
        }
        return 0;
    }
}