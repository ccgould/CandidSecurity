using CandidQVmMulti.Enumerators;
using CandidQVmMulti.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Networking;
using MySql.Data.MySqlClient;

namespace CandidQVmMulti.Services;

public class MySqlFlightNumberService
{
    private readonly string _connectionString;
    private readonly ApiSettings configuration;
    private readonly NetworkAccess connectivity = Connectivity.Current.NetworkAccess;

    public MySqlFlightNumberService(IConfiguration config)
    {
        configuration = config.GetRequiredSection("SqlSettings").Get<ApiSettings>();
        _connectionString = $"Server={configuration.Host};Port={configuration.Port};Database={configuration.Name};Uid={configuration.Username};Pwd={configuration.Password};";
        this.connectivity = connectivity;
    }

    public async Task<List<FlightNumber>> GetAllFlightNumbersAsync()
    {
        try
        {
            if (connectivity == NetworkAccess.Internet)
            {
                var flights = new List<FlightNumber>();

                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "SELECT id, number, addDate, airline_id, terminal_id FROM flight_numbers_tb";
                using var command = new MySqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    flights.Add(new FlightNumber
                    {
                        Id = reader.GetInt32(0),
                        Number = reader.GetString(1),
                        AddedDate = reader.GetInt32(2),
                        AirlineId = reader.GetInt32(3),
                        TerminalId = reader.GetInt32(4),
                    });
                }

                return flights;
            }
        }
        catch (Exception ex)
        {
            await App.AlertSvc.ShowAlertAsync("Error", ex.Message);
        }

        return new();
    }

    public async Task<bool> AddFlightNumberAsync(FlightNumber flight)
    {
        if (connectivity == NetworkAccess.Internet)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"INSERT INTO flight_numbers_tb 
                      (number, addDate, airline_id, terminal_id) 
                      VALUES (@Number, @AddDate, @AirlineId, @TerminalId)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Number", flight.Number);
            command.Parameters.AddWithValue("@AddDate", flight.AddedDate);
            command.Parameters.AddWithValue("@AirlineId", flight.AirlineId);
            command.Parameters.AddWithValue("@TerminalId", flight.TerminalId);

            var result = await command.ExecuteNonQueryAsync() > 0;
            flight.Id = (int)command.LastInsertedId;

            return result;
        }
        return false;
    }

    public async Task UpdateFlightNumberAsync(FlightNumber flight)
    {
        if (connectivity == NetworkAccess.Internet)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"UPDATE flight_numbers_tb SET 
                      number = @Number, 
                      addDate = @AddDate, 
                      airline_id = @AirlineId, 
                      terminal_id = @TerminalId 
                      WHERE id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", flight.Id);
            command.Parameters.AddWithValue("@Number", flight.Number);
            command.Parameters.AddWithValue("@AddDate", flight.AddedDate);
            command.Parameters.AddWithValue("@AirlineId", flight.AirlineId);
            command.Parameters.AddWithValue("@TerminalId", flight.TerminalId);

            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task DeleteFlightNumberAsync(int flightId)
    {
        if (connectivity == NetworkAccess.Internet)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "DELETE FROM flight_numbers_tb WHERE id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", flightId);

            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task<List<FlightNumber>> GetFlightNumbersByAirlineAndTerminalAsync(int airlineId, int terminalId)
    {
        if (connectivity == NetworkAccess.Internet)
        {
            var flights = new List<FlightNumber>();

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var terminal = (Terminals)terminalId;

            var query = string.Empty;

            if (terminal == Terminals.Bothe)
            {
                query = @"SELECT id, number, addDate, airline_id, terminal_id 
                  FROM flight_numbers_tb 
                  WHERE airline_id = @AirlineId";
            }
            else
            {
                query = @"SELECT id, number, addDate, airline_id, terminal_id 
                  FROM flight_numbers_tb 
                  WHERE airline_id = @AirlineId AND terminal_id = @TerminalId";
            }

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@AirlineId", airlineId);
            command.Parameters.AddWithValue("@TerminalId", terminalId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                flights.Add(new FlightNumber
                {
                    Id = reader.GetInt32(0),
                    Number = reader.GetString(1),
                    AddedDate = reader.GetInt64(2),
                    AirlineId = reader.GetInt32(3),
                    TerminalId = reader.GetInt32(4),
                });
            }

            return flights;
        }
        return new();
    }

}