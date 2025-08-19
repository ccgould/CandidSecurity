using CandidQV.Models.Items;
using SQLite;

namespace CandidQV.Repositories;
public class Repository
{
    private const string DB_NAME = "candidDB.db3";
    private readonly SQLiteAsyncConnection _connection;

    public Repository()
    {
        _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
        _connection.CreateTableAsync<Employee>();
    }

    public async Task<List<Employee>> GetEmployees()
    {
        return await _connection.Table<Employee>().ToListAsync();
    }

    public async Task<Employee> GetById(int id)
    {
        return await _connection.Table<Employee>().Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task Create(Employee employee)
    {
        await _connection.InsertAsync(employee);
    }

    public async Task Update(Employee employee)
    {
        await _connection.UpdateAsync(employee);
    }

    public async Task Delete(Employee employee)
    {
        await _connection.DeleteAsync(employee);
    }
}
