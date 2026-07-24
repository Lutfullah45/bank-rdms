using BankRDMS.Core.Entities;

namespace BankRDMS.Service.Abstract;

public interface ICustomerService
{
    Task AddCustomerWithAccountAsync(Customer customer);
    List<Customer> GetAllCustomers();
    Customer? GetCustomerById(int id);
}
