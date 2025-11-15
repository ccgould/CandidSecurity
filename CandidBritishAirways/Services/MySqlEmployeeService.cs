
using CandidBritishAirways.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Networking;
using MySql.Data.MySqlClient;
using System.Data;

namespace CandidBritishAirways.Services;


public class MySqlEmployeeService
{
    private readonly string _connectionString;
    private readonly ApiSettings configuration;
    private readonly NetworkAccess connectivity = Connectivity.Current.NetworkAccess;

    public MySqlEmployeeService(IConfiguration config)
    {
        var configuration = config.GetRequiredSection("SqlSettings").Get<ApiSettings>();
        _connectionString = $"Server={configuration.Host};Port={configuration.Port};Database={configuration.Name};Uid={configuration.Username};Pwd={configuration.Password};\r\n";
        this.configuration = configuration;
    }

    public async Task<List<Employee>> GetAllEmployeesAsync()
    {
        try
        {
            if (connectivity == NetworkAccess.Internet)
            {
                var employees = new List<Employee>();

                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "SELECT id, name, position, addDate, isActive FROM guards_tb";
                using var command = new MySqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    employees.Add(new Employee
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Position = reader.GetString(2),
                        AddedDate = reader.GetInt64(3),
                        IsActive = reader.GetBoolean(4),
                    });
                }

                return employees.OrderBy(x => x.Name).ToList();
            }
        }
        catch (Exception ex)
        {
            //await App.AlertSvc.ShowAlertAsync("Error", ex.Message);
        }

        return new();
    }

    public async Task<bool> AddEmployeeAsync(Employee employee)
    {
        if (connectivity == NetworkAccess.Internet)
        {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                var query = "INSERT INTO guards_tb (name, position,addDate,isActive) VALUES (@Name, @Position, @AddDate, @IsActive)";
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", employee.Name);
                command.Parameters.AddWithValue("@Position", employee.Position);
                command.Parameters.AddWithValue("@AddDate", employee.AddedDate);
                command.Parameters.AddWithValue("@IsActive", employee.IsActive);

                var result = await command.ExecuteNonQueryAsync() > 0;

                employee.Id = (int)command.LastInsertedId;

                return result;
        }
       

        return false;
    }

    public async Task UpdateEmployeeAsync(Employee employee)
    {
        if (connectivity == NetworkAccess.Internet)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "UPDATE guards_tb SET name = @Name, position = @Position, addDate = @AddDate, isActive = @IsActive WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", employee.Id);
            command.Parameters.AddWithValue("@Name", employee.Name);
            command.Parameters.AddWithValue("@Position", employee.Position);
            command.Parameters.AddWithValue("@AddDate", employee.AddedDate);
            command.Parameters.AddWithValue("@IsActive", employee.IsActive);

            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task DeleteEmployeeAsync(int employeeId)
    {
        if (connectivity == NetworkAccess.Internet)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "DELETE FROM guards_tb WHERE Id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", employeeId);

            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task<int> GetCountAsync()
    {
        if (connectivity == NetworkAccess.Internet)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand("SELECT COUNT(*) FROM guards_tb", connection);
            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result);
        }

        return 0;
    }

    public async Task<Employee> GetEmployeeAsync(int id)
    {
        if (connectivity == NetworkAccess.Internet)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new MySqlCommand("SELECT * FROM guards_tb WHERE Id=@Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = await command.ExecuteReaderAsync();


            while (await reader.ReadAsync())
            {
                return new Employee
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Position = reader.GetString(2),
                    AddedDate = reader.GetInt64(3),
                    IsActive = reader.GetBoolean(4),
                };
            }
        }

        return null;
    }
}
