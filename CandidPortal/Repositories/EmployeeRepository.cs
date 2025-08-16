using CandidPortal.Configuration;
using CandidPortal.Models;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System.Collections.ObjectModel;
using System.Data;

namespace CandidPortal.Repositories;
public class EmployeeRepository
{
    private const string MYSQL_DATE_FORMAT = "yyyy-MM-dd";
    private const string MYSQL_TIME_FORMAT = "HH:mm:ss";
    private string connectionString;
    private AppSettings? settings => App.AppConfig.GetSection("AppSettings") as AppSettings;

    public EmployeeRepository()
    {

        SetConnectionString();
    }

    public void SetConnectionString()
    {
        connectionString = $"datasource={settings.MysqlDatasource};port={settings.MysqlPortNumber};username={settings.MySqlUsername};password={settings.MysqlPassword};database={settings.MysqlDatabaseName};";
    }

    private async Task<MySqlConnection> OpenConnection()
    {
        MySqlConnection? connection = null;
        try
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();

        }
        catch (Exception ex)
        {
            //await messageService.ShowErrorMessage("Mysql", "Failed to connect to mysql service. Please ensure login information is correct and servide is running. " + ex.Message);
            connection = null;
        }

        return connection;
    }

    public async Task<ObservableCollection<Employee>> GetEmployees()
    {
        MySqlConnection connection = await OpenConnection();

        if (connection is null) return null;
        try
        {
            ObservableCollection<Employee> returnThese = new ObservableCollection<Employee>();

            MySqlCommand command = new MySqlCommand("SELECT * FROM employees_tb", connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    await Task.Run(() =>
                    {
                        var employee = new Employee()
                        {
                            ID = reader.GetInt32("id"),
                            FirstName = reader.GetString("first_name"),
                            LastName = reader.GetString("last_name"),
                            BadgeID = reader.GetInt32("badge_id"),
                            EmployeeID = reader.GetInt32("employee_id"),
                        };

                        returnThese.Add(employee);
                    });
                }
            }

            return returnThese;
        }
        catch (Exception ex)
        {
            //await messageService.ShowErrorMessage("Error Retrieving Pending Order", ex.Message, ex.StackTrace, "524270c5-ec9d-4d65-ab2e-bf39d771484d");
        }
        finally
        {
            connection.Close();
        }
        return new();
    }
}
