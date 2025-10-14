using CandidQV.Models.Items;
using SQLite;

namespace CandidQV.Repositories;
public class EmployeeRepository
{
    private const string DB_NAME = "candidDB.db3";
    private readonly SQLiteAsyncConnection _connection;

    public EmployeeRepository()
    {
        _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
        _connection.CreateTableAsync<Employee>();
    }

    public async Task<List<Employee>> GetAllAsync()
    {
        return await _connection.Table<Employee>().ToListAsync();
    }

    public async Task<Employee> GetByIdAsync(int id)
    {
        return await _connection.Table<Employee>().Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Employee employee)
    {
        await _connection.InsertAsync(employee);
    }

    public async Task UpdateAsync(Employee employee)
    {
        await _connection.UpdateAsync(employee);
    }

    public async Task DeleteAsync(Employee employee)
    {
        await _connection.DeleteAsync(employee);
    }
}
