using BankRDMS.Core.Entities;

namespace BankRDMS.Repository.Abstract;

public interface ICustomerRepository
{
    Task AddAsync(Customer customer);
    Customer? GetByIdWithDetails(int id);
    Task<Customer?> GetByIdForUpdateAsync(int id);
    Task<Customer?> GetByIdentityNumberAsync(string identityNumber);
    Task<bool> IdentityNumberExistsAsync(string identityNumber);
}
