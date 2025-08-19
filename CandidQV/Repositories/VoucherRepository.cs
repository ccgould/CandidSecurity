using CandidQV.Models.Items;
using SQLite;

namespace CandidQV.Repositories;
public class VoucherRepository
{
    private const string DB_NAME = "candidDB.db3";
    private readonly SQLiteAsyncConnection _connection;

    public VoucherRepository()
    {
        _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
        _connection.CreateTableAsync<Voucher>();
    }

    public async Task<List<Voucher>> GetVouchers()
    {
        return await _connection.Table<Voucher>().ToListAsync();
    }

    public async Task<Voucher> GetById(int id)
    {
        return await _connection.Table<Voucher>().Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task Create(Voucher Voucher)
    {
        await _connection.InsertAsync(Voucher);
    }

    public async Task Update(Voucher Voucher)
    {
        await _connection.UpdateAsync(Voucher);
    }

    public async Task Delete(Voucher Voucher)
    {
        await _connection.DeleteAsync(Voucher);
    }
}
