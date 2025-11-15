using CandidBritishAirways.Enumerator;
using CandidBritishAirways.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace CandidBritishAirways.Services
{
    public class MySqlGateService
    {
        private string _connectionString;
        private ApiSettings configuration;
        private readonly NetworkAccess connectivity = Connectivity.Current.NetworkAccess;


        public MySqlGateService(IConfiguration config)
        {
            var configuration = config.GetRequiredSection("SqlSettings").Get<ApiSettings>();
            _connectionString = $"Server={configuration.Host};Port={configuration.Port};Database={configuration.Name};Uid={configuration.Username};Pwd={configuration.Password};\r\n";
            this.configuration = configuration;
        }

        public async Task<List<Gate>> GetAllGatesAsync()
        {
            try
            {
                if (connectivity == NetworkAccess.Internet)
                {
                    var employees = new List<Gate>();

                    using var connection = new MySqlConnection(_connectionString);
                    await connection.OpenAsync();

                    var query = "SELECT id, name, terminal FROM gate_tb";
                    using var command = new MySqlCommand(query, connection);
                    using var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        employees.Add(new Gate
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Terminal = (Terminal)reader.GetInt32(2),
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

        internal async Task<Gate> GetGetAsync(int id)
        {
            try
            {
                if (connectivity == NetworkAccess.Internet)
                {
                    using var connection = new MySqlConnection(_connectionString);
                    await connection.OpenAsync();

                    var query = "SELECT * FROM gate_tb WHERE id = @Id";
                    using var command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", id);
                    using var reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        return new Gate
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Terminal = (Terminal)reader.GetInt32(2),
                        };
                    }

                }
            }
            catch (Exception ex)
            {
                //await App.AlertSvc.ShowAlertAsync("Error", ex.Message);
            }

            return null;
        }
    }
}
