using BankRDMS.Core.Entities;

namespace BankRDMS.Repository.Abstract;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction);
    Task<decimal> GetAccountBalanceAsync(int accountId);
    Task<decimal> GetCreditCardDebtAsync(int creditCardId);
    Task<List<Transaction>> GetAllWithDetailsAsync(int customerId);
    Transaction? GetByIdWithDetails(int id, int customerId);
}
