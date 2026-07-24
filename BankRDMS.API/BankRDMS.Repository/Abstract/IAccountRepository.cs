using BankRDMS.Core.Entities;

namespace BankRDMS.Repository.Abstract;

public interface IAccountRepository
{
    Task AddAsync(Account account);
    Task<List<Account>> GetAllByCustomerIdAsync(int customerId);
    Account? GetByIdWithCards(int id, int customerId);
    Task<Account?> GetByIdAsync(int id, int customerId);
    Task<Account?> GetByIdForUpdateAsync(int id, int customerId);
    Task<List<Account>> GetByCustomerIdAsync(int customerId);
    Task<bool> UpdateStatusAsync(int id, int customerId, bool isActive);
    Task<bool> AccountNumberExistsAsync(string accountNumber);
    Task<bool> IbanExistsAsync(string iban);
}
