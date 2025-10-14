using CandidQV.Models.Items;
using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.Collections.ObjectModel;

namespace CandidQV.Repositories;
public partial class AirlineRepository : ObservableObject
{
    private const string DB_NAME = "candidDB.db3";
    private readonly SQLiteAsyncConnection _connection;

    public AirlineRepository()
    {
        _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
        _connection.CreateTableAsync<Airline>();
    }

    public async Task<List<Airline>> GetAllAsync()
    {   
        return await _connection.Table<Airline>().ToListAsync();
    }

    public async Task<Airline> GetById(int id)
    {
        return await _connection.Table<Airline>().Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<int> CreateAsync(Airline airline)
    {
        await _connection.InsertAsync(airline);
        long lastInsertedId = await _connection.ExecuteScalarAsync<long>("SELECT last_insert_rowid()");
        return Convert.ToInt32(lastInsertedId);
    }

    public async Task UpdateAsync(Airline Airline)
    {
        await _connection.UpdateAsync(Airline);
    }

    public async Task Delete(Airline airline)
    {
        await _connection.DeleteAsync(airline);
    }

    internal async Task<bool> DoesRecordExistAsync(string result)
    {
        var existingItem = await _connection.Table<Airline>()
                                .Where(i => i.Name.ToLower() == result.ToLower())
                                .FirstOrDefaultAsync();

        return existingItem != null;
    }
    internal async Task<bool> DoesIataRecordExistAsync(string result)
    {
        var existingItem = await _connection.Table<Airline>()
                                .Where(i => i.IataCode.ToLower() == result.ToLower())
                                .FirstOrDefaultAsync();

        return existingItem != null;
    }
}
