using BankRDMS.Core.Entities;
using BankRDMS.Data;
using BankRDMS.Repository.Abstract;
using Microsoft.EntityFrameworkCore;

namespace BankRDMS.Repository.Concrete;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
    }

    public Customer? GetByIdWithDetails(int id)
    {
        return _context.Customers
            .AsNoTracking()
            .Include(customer => customer.Accounts)
                .ThenInclude(account => account.AccountCards)
            .Include(customer => customer.CreditCards)
            .FirstOrDefault(customer => customer.Id == id);
    }

    public Task<Customer?> GetByIdForUpdateAsync(int id)
    {
        return _context.Customers
            .FromSqlInterpolated($"SELECT * FROM customers WHERE id = {id} FOR UPDATE")
            .SingleOrDefaultAsync();
    }

    public Task<Customer?> GetByIdentityNumberAsync(string identityNumber)
    {
        return _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(customer => customer.IdentityNumber == identityNumber);
    }

    public Task<bool> IdentityNumberExistsAsync(string identityNumber)
    {
        return _context.Customers
            .AnyAsync(customer => customer.IdentityNumber == identityNumber);
    }
}
