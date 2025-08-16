using CandidPortal.Configuration;
using CandidPortal.Models;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System;
using System.Collections.ObjectModel;
using System.Data;

namespace CandidPortal.Repositories;
public class TrainingRecordRepository
{
    private const string MYSQL_DATE_FORMAT = "yyyy-MM-dd";
    private const string MYSQL_TIME_FORMAT = "HH:mm:ss";
    private readonly TrainingTypeRepository trainingTypeRepository;
    private string connectionString;
    private AppSettings? settings => App.AppConfig.GetSection("AppSettings") as AppSettings;

    public TrainingRecordRepository(TrainingTypeRepository trainingTypeRepository)
    {

        SetConnectionString();
        this.trainingTypeRepository = trainingTypeRepository;
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

    public async Task<ObservableCollection<TrainingRecord>> GetTrainingRecords()
    {
        MySqlConnection connection = await OpenConnection();

        if (connection is null) return null;
        try
        {
            ObservableCollection<TrainingRecord> returnThese = new ObservableCollection<TrainingRecord>();

            MySqlCommand command = new MySqlCommand("SELECT * FROM training_records_tb", connection);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    returnThese.Add(await CreateTrainingRecord(reader));
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

    private async Task<TrainingRecord> CreateTrainingRecord(System.Data.Common.DbDataReader reader)
    {
        var trainingRecord = new TrainingRecord()
        {
            ID = reader.GetInt32("id"),
            EmployeeID = reader.GetInt32("employee_id"),
            IssuedDate = DateOnly.FromDateTime(reader.GetDateTime("issued_date")),
            TrainingType = reader.GetInt32("training_type"),
            AnnualInternal = await GetTrainingInterval(reader.GetInt32("training_type")),
        };

        return trainingRecord;
    }

    private async Task<decimal> GetTrainingInterval(int id)
    {
        var f = await trainingTypeRepository.GetTrainingType(id);
        
        if(f is not null)
        {
            return f.AnnualInterval;
        }
        return -1;
    }

    public async Task<ObservableCollection<TrainingRecord>> GetTrainingRecords(int employee)
    {
        MySqlConnection connection = await OpenConnection();

        if (connection is null) return null;
        try
        {
            ObservableCollection<TrainingRecord> returnThese = new ObservableCollection<TrainingRecord>();

            MySqlCommand command = new MySqlCommand("SELECT * FROM training_records_tb Where id = @id", connection);

            command.Parameters.AddWithValue("@id", employee);

            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                   returnThese.Add(await CreateTrainingRecord(reader));
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

    internal async Task Save(TrainingRecord record)
    {
        
    }

    internal void AddNew(TrainingRecord record)
    {

    }
}
