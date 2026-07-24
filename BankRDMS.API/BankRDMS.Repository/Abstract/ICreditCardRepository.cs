using BankRDMS.Core.Entities;

namespace BankRDMS.Repository.Abstract;

public interface ICreditCardRepository
{
    Task AddAsync(CreditCard creditCard);
    Task<List<CreditCard>> GetAllAsync(int customerId);
    CreditCard? GetById(int id, int customerId);
    Task<CreditCard?> GetActiveByCardNumberForUpdateAsync(string cardNumber, int customerId);
    Task<List<CreditCard>> GetByCustomerIdAsync(int customerId);
    Task<decimal> GetTotalActiveLimitAsync(int customerId);
    Task<bool> CardNumberExistsAsync(string cardNumber);
}
