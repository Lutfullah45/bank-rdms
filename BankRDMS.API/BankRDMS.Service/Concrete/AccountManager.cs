using BankApi.Services;
using BankRDMS.Core.Entities;
using BankRDMS.Data;
using BankRDMS.Service.Abstract;
using Microsoft.EntityFrameworkCore;

namespace BankRDMS.Service.Concrete
{
    public class AccountManager : IAccountService
    {
        private readonly AppDbContext _context;

        public AccountManager(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAccountAsync(int customerId, string accountName)
        {
            Random rnd = new Random();

            var newAccount = new Account
            {
                CustomerId = customerId,
                AccountName = accountName,
                IsActive = true,
                Iban = "TR" + rnd.Next(10000000, 99999999).ToString() + rnd.Next(10000000, 99999999).ToString(),
                AccountNumber = rnd.Next(100000000, 999999999).ToString()
            };

            await _context.Accounts.AddAsync(newAccount);
            await _context.SaveChangesAsync(); 
        }

        public async Task<List<Account>> GetAllAccountsAsync()
        {
            return await _context.Accounts.ToListAsync();
        }

        public Account GetAccountById(int id)
        {
            return _context.Accounts
                           .Include(a => a.AccountCards)
                           .FirstOrDefault(a => a.Id == id);
        }

        public async Task<List<Account>> GetAccountsByCustomerIdAsync(int customerId)
        {
            return await _context.Accounts.Where(a => a.CustomerId == customerId).ToListAsync();
        }

        public bool UpdateAccountStatus(int id, bool isActive)
        {
            var account = _context.Accounts.FirstOrDefault(a => a.Id == id);

            if (account == null)
            {
                return false; 
            }

            account.IsActive = isActive; 

            _context.SaveChanges(); 

            return true; 
        }
    }
}