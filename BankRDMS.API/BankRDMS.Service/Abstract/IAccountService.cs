using BankRDMS.Core.Entities;

namespace BankApi.Services;

public interface IAccountService
{
    Task CreateAccountAsync(int customerId, string accountName);
    Task<List<Account>> GetAllAccountsAsync();
    Account? GetAccountById(int id);
    Task<List<Account>> GetAccountsByCustomerIdAsync(int customerId);
    Task<bool> UpdateAccountStatusAsync(int id, bool isActive);
}
