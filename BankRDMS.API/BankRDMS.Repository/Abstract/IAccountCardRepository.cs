using BankRDMS.Core.Entities;

namespace BankRDMS.Repository.Abstract;

public interface IAccountCardRepository
{
    Task AddAsync(AccountCard accountCard);
    AccountCard? GetById(int id, int customerId);
    Task<AccountCard?> GetActiveByCardNumberAsync(string cardNumber, int customerId);
    Task<List<AccountCard>> GetAllAsync(int customerId);
    List<AccountCard> GetByAccountId(int accountId, int customerId);
    Task<bool> UpdateStatusAsync(int id, int customerId, bool isActive);
    Task<bool> CardNumberExistsAsync(string cardNumber);
}
