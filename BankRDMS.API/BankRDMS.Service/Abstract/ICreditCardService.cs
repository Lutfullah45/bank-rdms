using BankRDMS.Core.Entities;

namespace BankRDMS.Service.Abstract;

public interface ICreditCardService
{
    Task CreateCreditCardAsync(int customerId, decimal requestedLimit);
    Task<List<CreditCard>> GetAllCreditCardsAsync();
    CreditCard? GetCreditCardById(int id);
    Task<List<CreditCard>> GetCreditCardsByCustomerIdAsync(int customerId);
}
