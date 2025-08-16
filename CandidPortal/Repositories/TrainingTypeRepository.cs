using CandidPortal.Configuration;
using CandidPortal.Models;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System.Collections.ObjectModel;
using System.Data;

namespace CandidPortal.Repositories;
public class TrainingTypeRepository
{
    private const string MYSQL_DATE_FORMAT = "yyyy-MM-dd";
    private const string MYSQL_TIME_FORMAT = "HH:mm:ss";
    private string connectionString;
    private AppSettings? settings => App.AppConfig.GetSection("AppSettings") as AppSettings;

    public TrainingTypeRepository()
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

    public async Task<ObservableCollection<TrainingType>> GetTrainingTypes()
    {
        MySqlConnection connection = await OpenConnection();

        if (connection is null) return null;
        try
        {
            ObservableCollection<TrainingType> returnThese = new ObservableCollection<TrainingType>();

            MySqlCommand command = new MySqlCommand("SELECT * FROM training_type_tb", connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    await Task.Run(() =>
                    {
                        var trainingType = new TrainingType()
                        {
                            ID = reader.GetInt32("id"),
                            Name = reader.GetString("name"),
                            AnnualInterval = reader.GetDecimal("annual_interval"),
                        };

                        returnThese.Add(trainingType);
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

    public async Task<TrainingType> GetTrainingType(int id)
    {
        MySqlConnection connection = await OpenConnection();

        if (connection is null) return null;
        try
        {
            MySqlCommand command = new MySqlCommand("SELECT * FROM training_type_tb Where id = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var trainingType = new TrainingType()
                    {
                        ID = reader.GetInt32("id"),
                        Name = reader.GetString("name"),
                        AnnualInterval = reader.GetDecimal("annual_interval"),
                    };

                    return trainingType;
                }
                else
                {
                    return null;
                }
            }
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
