using CandidQV.Models.Items;
using SQLite;

namespace CandidQV.Repositories;
public class AirlineRepository
{
    private const string DB_NAME = "candidDB.db3";
    private readonly SQLiteAsyncConnection _connection;

    public AirlineRepository()
    {
        _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
        _connection.CreateTableAsync<Airline>();
    }

    public async Task<List<Airline>> GetAirlines()
    {
        return await _connection.Table<Airline>().ToListAsync();
    }

    public async Task<Airline> GetById(int id)
    {
        return await _connection.Table<Airline>().Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<int> Create(Airline Airline)
    {
        await _connection.InsertAsync(Airline);
        long lastInsertedId = await _connection.ExecuteScalarAsync<long>("SELECT last_insert_rowid()");
        return Convert.ToInt32(lastInsertedId);
    }

    public async Task Update(Airline Airline)
    {
        await _connection.UpdateAsync(Airline);
    }

    public async Task Delete(Airline Airline)
    {
        await _connection.DeleteAsync(Airline);
    }
}
