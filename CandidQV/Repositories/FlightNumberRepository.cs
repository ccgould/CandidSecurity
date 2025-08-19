using CandidQV.Models.Items;
using SQLite;

namespace CandidQV.Repositories;
public class FlightNumberRepository
{
    private const string DB_NAME = "candidDB.db3";
    private readonly SQLiteAsyncConnection _connection;

    public FlightNumberRepository()
    {
        _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
        _connection.CreateTableAsync<FlightNumber>();
    }

    public async Task<List<FlightNumber>> GetFlightNumber()
    {
        return await _connection.Table<FlightNumber>().ToListAsync();
    }

    public async Task<FlightNumber> GetById(int id)
    {
        return await _connection.Table<FlightNumber>().Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<FlightNumber>> GetByAirlineId(int id)
    {
        return await _connection.Table<FlightNumber>().Where(x => x.AirlineId == id).ToListAsync();
    }

    public async Task Create(FlightNumber Airline)
    {
        await _connection.InsertAsync(Airline);
    }

    public async Task Update(FlightNumber Airline)
    {
        await _connection.UpdateAsync(Airline);
    }

    public async Task Delete(FlightNumber Airline)
    {
        await _connection.DeleteAsync(Airline);
    }

    public async Task<bool> DoesRecordExistAsync(string nameToFind, int airlineId)
    {
        var existingItem = await _connection.Table<FlightNumber>()
                                        .Where(i => i.Number == nameToFind && i.AirlineId == airlineId)
                                        .FirstOrDefaultAsync();

        return existingItem != null;
    }

}
