using BankRDMS.Core.Entities;
using BankRDMS.Data;
using BankRDMS.Repository.Abstract;
using Microsoft.EntityFrameworkCore;

namespace BankRDMS.Repository.Concrete;

public class AccountRepository : IAccountRepository
{
    private readonly AppDbContext _context;

    public AccountRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Account account)
    {
        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();
    }

    public Task<List<Account>> GetAllByCustomerIdAsync(int customerId)
    {
        return _context.Accounts
            .AsNoTracking()
            .Where(account => account.CustomerId == customerId)
            .ToListAsync();
    }

    public Account? GetByIdWithCards(int id, int customerId)
    {
        return _context.Accounts
            .AsNoTracking()
            .Include(account => account.AccountCards)
            .FirstOrDefault(account => account.Id == id && account.CustomerId == customerId);
    }

    public Task<Account?> GetByIdAsync(int id, int customerId)
    {
        return _context.Accounts
            .FirstOrDefaultAsync(account => account.Id == id && account.CustomerId == customerId);
    }

    public Task<Account?> GetByIdForUpdateAsync(int id, int customerId)
    {
        return _context.Accounts
            .FromSqlInterpolated(
                $"SELECT * FROM accounts WHERE id = {id} AND customer_id = {customerId} FOR UPDATE")
            .SingleOrDefaultAsync();
    }

    public Task<List<Account>> GetByCustomerIdAsync(int customerId)
    {
        return _context.Accounts
            .AsNoTracking()
            .Where(account => account.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task<bool> UpdateStatusAsync(int id, int customerId, bool isActive)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(item => item.Id == id && item.CustomerId == customerId);

        if (account is null)
        {
            return false;
        }

        account.IsActive = isActive;
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<bool> AccountNumberExistsAsync(string accountNumber)
    {
        return _context.Accounts.AnyAsync(account => account.AccountNumber == accountNumber);
    }

    public Task<bool> IbanExistsAsync(string iban)
    {
        return _context.Accounts.AnyAsync(account => account.Iban == iban);
    }
}
